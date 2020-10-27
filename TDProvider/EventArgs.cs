using System;

namespace TDProvider {
    /// <summary>DataReceivedEventArgs contains information about financial data and
    /// is the payload returned from a DataReceived event</summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <value>The type of content data</value>
        public string Type { get; set; }

        /// <value>The JSON payload of the content data</value>
        /// 
        public string JSON { get; set; }

        /// <summary>
        /// DataReceivedEventArgs creates a new instance of the DataReceivedEventArgs object
        /// </summary>
        /// <param name="type">The type of content data</param>
        /// <param name="JSON">The JSON payload of the content data</param>
        public DataReceivedEventArgs(string type, string JSON) {
            this.Type = type;
            this.JSON = JSON;
        }
    }

    /// <summary>DebugDataReceivedEventArgs contains information about debug logging information and
    /// is the payload returned from a DebugDataReceived event</summary>
    public class DebugDataReceivedEventArgs : EventArgs
    {
        /// <value>The type of the log information. Values are Info, Warning and Error</value>
        public LogType Type { get; set; }

        /// <value>The information that was logged</value>
        public string Message { get; set; }

        /// <summary>
        /// DataReceivedEventArgs creates a new instance of the DebugDataReceivedEventArgs object
        /// </summary>
        /// <param name="type">The type of the log information. Values are Info, Warning and Error</param>
        /// <param name="message">The information that was logged</param>
        public DebugDataReceivedEventArgs(LogType type, string message) {
            this.Type = type;
            this.Message = $"{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} {type,7}: {message}";
        }
    }

    /// <summary>RequestResponseReceivedEventArgs contains information about the response received from
    /// the issuance of a command. This information is helpful to confirm that a request was successfully
    /// received</summary>
    public class RequestResponseReceivedEventArgs : EventArgs
    {
        /// <value>The response received from the service</value>
        public Response Response { get; set; }

        /// <summary>
        /// RequestResponseReceivedEventArgs creates a new instance of the ResponseReceivedEventArgs object
        /// </summary>
        /// <param name="response">A Response object</param>

        public RequestResponseReceivedEventArgs(Response response) {
            this.Response = response;
        }
    }
}