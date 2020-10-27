using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace TDProvider {
    partial class Streamer {
        // Member variables
        Credentials credentials;
        ClientWebSocket socket;
        List<ServiceRequest> pendingRequests;
        bool streamingLoginSuccessful;
        bool captureData;
        StreamWriter captureDataStreamWriter;
        DateTime captureUntilTime;
        string captureDataFilename;

        /// <summary>The OnDataReceived event handler recieves notifications when new data is available
        /// <example>For example:
        /// <code>
        ///    Streamer streamer = new Streamer(auth.StreamingCredentials);
        ///    streamer.DataReceived += dataReceived;
        ///    static void dataReceived(object sender, DataReceivedEventArgs e) {
        ///        Console.WriteLine(e.JSON)
        ///    }
        /// </code>
        /// </example>
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>The DebugDataReceived event handler recieves notifications when new debug logging information is available
        /// <example>For example:
        /// <code>
        ///    Streamer streamer = new Streamer(auth.StreamingCredentials);
        ///    streamer.DataReceived += DebugDataReceived;
        ///    static void dataReceived(object sender, DataReceivedEventArgs e) {
        ///        Console.WriteLine(e.Message)
        ///    }
        /// </code>
        /// </example>
        /// </summary>
        public event EventHandler<DebugDataReceivedEventArgs> DebugDataReceived;

        /// <summary>The RequestResponseReceived event handler recieves notifications in response to command requests
        /// <example>For example:
        /// <code>
        ///    Streamer streamer = new Streamer(auth.StreamingCredentials);
        ///    streamer.RequestResponseDataReceived += requestResponseReceived;
        ///    static void requestResponseReceived(object sender, DataReceivedEventArgs e) {
        ///        Console.WriteLine(e.Message)
        ///    }
        /// </code>
        /// </example>
        /// </summary>
        public event EventHandler<RequestResponseReceivedEventArgs> RequestResponseReceived;

        /// <summary>Streamer creates a new Streamer object. The Streamer is responsible for requesting and receiving data
        /// <example>For example:
        /// <code>
        ///    Streamer streamer = new Streamer(auth.StreamingCredentials);
        /// </code>
        /// </example>
        /// <param name="credentials">The Credentials obtained via Authentication</param>
        /// </summary>
        public Streamer(Credentials credentials) {
            this.credentials = credentials;
            this.pendingRequests = new List<ServiceRequest>();
        }

        /// <summary>SendRequest submits a request for data to be streamed
        /// <example>For example:
        /// <code>
        ///    Streamer streamer = new Streamer(auth.StreamingCredentials);
        ///    var symbols = new string[] {"AAPL,MSFT"};
        ///    var fields = new int[] {0,1,2,3,4};
        ///    streamer.SendRequest(ServiceRequestFactory.CreateTimesaleEquityRequest(auth.StreamingCredentials, symbols, fields));
        /// </code>
        /// </example>
        /// <param name="request">The request to be streamed.  Use the ServiceRequestFactory to easily manufacture requests.</param>
        /// </summary>
        public void SendRequest(ServiceRequest request) {
            if (this.socket.State != WebSocketState.Open) {
                throw new Exception("Cannot send a request to a closed websocket connection");
            }

            lock(this.pendingRequests) {
                this.pendingRequests.Add(request);
            }
        }

        /// <summary>StartDataStreaming initializes communication with the TD Ameritrade websocket service and authenticates with it
        /// <example>For example:
        /// <code>
        ///    Authentication auth = new Authentication();
        ///    await auth.RefreshAccessToken();
        ///    await auth.GetUserPrincipals();
        ///    Streamer streamer = new Streamer(auth.StreamingCredentials);
        ///    streamer.StartDataStreaming(auth.UserPrincipal.streamerInfo.streamerSocketUrl, cts.Token)
        /// </code>
        /// </example>
        /// <param name="streamerSocketUrl">The websocket url. This value is obtained from UserPrincipals which is obtained during authentication</param>
        /// <param name="cancellationToken">The CancellationToken that is used to cancel the asynchronous streaming task</param>
        /// </summary>
        public async Task StartDataStreaming(string streamerSocketUrl, CancellationToken cancellationToken) {
            // Was cancellation already requested?
            if (cancellationToken.IsCancellationRequested)
            {
                Log(LogType.Error, $"Task was cancelled before it got started.");
                return;
            }

            string url = "wss://" + streamerSocketUrl + "/ws";
            bool loginSent = false;

            using (socket = new ClientWebSocket()) {
                await socket.ConnectAsync(new Uri(url), CancellationToken.None);
                do {
                    try
                    {
                        // Login if this is the first time we are here
                        if (!loginSent) {
                            Log(LogType.Info, "Sending Login Request...");
                            var loginRequest = ServiceRequestFactory.CreateLoginRequest(this.credentials);
                            string requestsJSON = loginRequest.ToJSON().Replace("\\u0026", "&");
                            await Send(socket, requestsJSON);
                            loginSent = true;
                            await HandleMessages(cancellationToken);
                        }
                        
                        // Process any pending requests
                        if (this.streamingLoginSuccessful && this.pendingRequests.Count > 0) {
                            for (int x = this.pendingRequests.Count - 1; x >= 0; x--) {
                                string requestJSON = this.pendingRequests[x].ToJSON();
                                Log(LogType.Info, string.Format("Sending requests for data. JSON={0}", requestJSON));
                                await Send(socket, requestJSON);
                                await HandleMessages(cancellationToken);
                                lock(this.pendingRequests) {
                                    this.pendingRequests.RemoveAt(x);
                                }
                            }
                        }

                        await HandleMessages(cancellationToken);

                        // Check to see if we've received a cancellation request
                        if (cancellationToken.IsCancellationRequested)
                        {
                            Log(LogType.Info, "Exiting data streaming - A cancellation was received.");
                            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(LogType.Error, $"Failed to send or receive data - {ex.Message}");
                        break;
                    }
                } while (true);
            }
        }

        /// <summary>
        /// StopDataStream halts all streaming operations
        /// </summary>
        /// <param name="cancellationToken">The CancellationToken that is used to cancel the asynchronous streaming task</param>
        /// <returns></returns>
        public async Task StopDataStreaming(CancellationToken cancellationToken) {
            if (this.socket.State == WebSocketState.Open) {
                await this.socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken);
            }
        }

        private async Task Send(ClientWebSocket socket, string data) =>
            await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);

        private async Task HandleMessages(CancellationToken ct) {
            try {
                using (var ms = new MemoryStream()) {
                    while (socket.State == WebSocketState.Open) {
                        WebSocketReceiveResult result;
                        do {
                            var messageBuffer = WebSocket.CreateClientBuffer(1024, 16);
                            result = await socket.ReceiveAsync(messageBuffer, CancellationToken.None);
                            ms.Write(messageBuffer.Array, messageBuffer.Offset, result.Count);
                        }
                        while (!result.EndOfMessage);

                        if (result.MessageType == WebSocketMessageType.Close) {
                            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", ct);
                            return;
                        } else if (result.MessageType == WebSocketMessageType.Text) {
                            var msgString = Encoding.UTF8.GetString(ms.ToArray());
                            Log(LogType.Info, $"Response Received: JSON={msgString}");

                            // Response type = 'response'
                            if (msgString.Contains("{\"response\":")) {
                                processResponse(msgString);
                                break;
                            }

                            // Response type = 'data'
                            if (msgString.Contains("{\"data\":")) {
                                processDataResponse(msgString);
                                break;
                            }

                            // Response type = 'notify'
                            if (msgString.Contains("{\"notify\":")) {
                                processNotifyResponse(msgString);
                                break;
                            }
                        }
                        ms.Seek(0, SeekOrigin.Begin);
                        ms.Position = 0;
                    }
                }
            } catch (InvalidOperationException ex) {
                Log(LogType.Error, $"[WS] Tried to receive message while already reading one - {ex.Message}");
            }
        }

        private void processResponse(string responseData) {
            StreamingResponse response = JsonSerializer.Deserialize<StreamingResponse>(responseData);
            foreach (Response item in response.response) {
                if (item.command == "LOGIN") {
                    if (item.content.code == 0) {
                        Log(LogType.Info, $"{item.service} | {item.command}: Login Succeeded ({item.content.msg})");
                        this.streamingLoginSuccessful = true;
                    } else {
                        Log(LogType.Warning, $"{item.service} | {item.command}: Login Failed ({item.content.msg})");
                    }
                } else {
                    Log(LogType.Info, $"{item.service} | {item.command}: Info = {item.content.msg}");
                }
                OnRequestResponseReceived(item);
            }
        }

        private void processNotifyResponse(string responseData) {
            StreamingNotifyResponse response = JsonSerializer.Deserialize<StreamingNotifyResponse>(responseData);
            foreach(Notify notify in response.notify) {
                try {
                    var seconds = long.Parse(notify.heartbeat);
                    var time = DateTimeOffset.FromUnixTimeMilliseconds(seconds);
                    Log(LogType.Info, $"Heartbeat: {time.ToLocalTime().ToString()}");
                }
                catch (Exception) {
                    Log(LogType.Info, $"Heartbeat: {notify.heartbeat}");
                }
            }
        }

        private void processDataResponse(string responseData) {
            // First we deserialize to the base StreamingDataResponse so that we can obtain the service name
            // which we will then use to further deserialze the response to the correct type
            StreamingDataResponse streamingDataResponse = JsonSerializer.Deserialize<StreamingDataResponse>(responseData);
            foreach (StreamingDataServiceInfo serviceInfo in streamingDataResponse.data) {
                OnDataReceived(serviceInfo.service, serviceInfo.ToJSON());

                if (this.captureData) {
                    writeDataForCapture(responseData);
                }
            }
        }

        /// <summary>
        /// OnDataReceived initiates a DataReceived event
        /// </summary>
        /// <param name="type">The content type of the data received. This value can be used to determine the type of object
        /// the JSON payload can be deserialized to
        /// </param>
        /// <param name="JSON">The JSON payload of the data received</param>
        protected virtual void OnDataReceived(string type, string JSON)
        {
            var e = new DataReceivedEventArgs(type, JSON);
            DataReceived?.Invoke(this, e);
        }

        /// <summary>
        /// Log initiates a DebugDataReceived event
        /// </summary>
        /// <param name="type">The type of the log information. Values are Info, Warning and Error</param>
        /// <param name="message">The information that was logged</param>
        protected virtual void Log(LogType type, string message) {
            var e = new DebugDataReceivedEventArgs(type, message);
            DebugDataReceived?.Invoke(this, e);
        }

        /// <summary>
        /// OnRequestResponseReceived initiates a RequestResponseReceived event
        /// </summary>
        /// <param name="response">A Response object containing the response from the service</param>
        protected virtual void OnRequestResponseReceived(Response response) {
            var e = new RequestResponseReceivedEventArgs(response);
            RequestResponseReceived?.Invoke(this, e);
        }
    }
}