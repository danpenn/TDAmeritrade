# TD Ameritrade

TD Ameritrade is a C# implementation of a websocket client for accessing TD Ameritrade Streaming Data.  It supports .Net Core and was written using Visual Studio Code.

This project consists of two components:
- **TDProvider**: TDProvider is a class which manages all communication with TD Ameritrade including authentication and data streaming.
- **TDCapture**: TDCapture is a sample command line application that demonstrates the usage pattern for TDProvider.

## TDCapture Initial Usage Instructions:
Please do the following steps to configure access to TDProvider. Once this is accomplished, the refresh token will be cached to a file and will last for 90 days.
1. You must first authenticate using TD Ameritrades two-factor auth. The initial implementation has not streamlined the authentication process so you must follow these instructions to obtain a refresh token.  https://developer.tdameritrade.com/content/simple-auth-local-apps. Once you have the information from this step you can proceed to the next.

2. Run the TDCapture application and it will generate a file called **privateData.json**. Edit this file in a text editor using your account information as well as the information obtained from the step above. The 'privateData.json' file looks like the following:

```json
{
        "AccessToken":"access_token from Step #1 above",
        "RefreshToken":"efresh_token from Step #1 above",  
        "Username":"username",                   
        "Password":"password",                    
        "ConsumerKey":"my_consumer_key from your account" 
}
```

## TDProvider Usage Instructions:
1. Step #1 is to authenticate and receive a new access token.  All authentication is handled within the "Authentication" class.

```
// We first need to authenticate and obtain a new access token. Then we need to get User Principals which
// contains additional information we need for accessing resources such as streaming data
Authentication auth = new Authentication();
await auth.RefreshAccessToken();
await auth.GetUserPrincipals();
```

2. Create an instance of the **Streamer** class. This class is responsible for all of the heavy lifting.

```
Streamer streamer = new Streamer(auth.StreamingCredentials);
```

3. Register to receive events. You will need to register for the **DataReceived** event as this is how you will get notified of new data.  Optionally, register for **DebugDataRecieved** to get debug information which can be helpful when troubleshooting problems or to see how the underlying flow of data looks like. Finally, you can register for **RequestResponseReceived** events if you want to see confirmation messages after a Service Request is submitted.

```
// Register to recieve data events. Any data events we receive will be sent to the dataReceived() function.
streamer.DataReceived += dataReceived;

// Register to receive debug logging data. This is only necessary when you are making changes to the application
// and want to make sure there are no errors.  Comment out the following line to hide the debug log information.
streamer.DebugDataReceived += DebugLogDataReceived;

// register to receive confirmation from requests being sent.
streamer.RequestResponseReceived += RequestResponseReceived;
```

4. Start streaming data. Starting the data stream will login to the streaming service and then wait for a Service Request for data to be submitted.

```
// Create a cancellation token which will be used to cancel the asynchrnous websocket later
var cancellationTokenSource = new CancellationTokenSource();

streamer.StartDataStreaming(auth.UserPrincipal.streamerInfo.streamerSocketUrl, cancellationTokenSource.Token)
```

5. The TD Ameritrade Streaming Data service uses ongoing websocket communication to send requests for data as well as receive the requested data. To request data or conduct an action you must create a **Service Request**. For example, you will create a Service Requests to request data such as stock quotes.  A helper class called "ServiceRequestFactory" contains several different methods for creating various Service Requests.  The following code generates the Service Request for getting a stream of stock quotes for Apple and Microsoft.

```
var symbols = new string[] {"AAPL,MSFT"};
streamer.SendRequest(ServiceRequestFactory.CreateTimesaleEquityRequest(auth.StreamingCredentials, symbols));
```

6. When you are done monitoring the data then shut everything down.

```
streamer.StopDataStreaming(cancellationTokenSource.Token)

// Alternatively, you can also call cancellationTokenSource.Cancel();
```

**NOTE:** Please see the source code in TDCapture (Program.cs) for a working example of this code as it also includes additional usage regarding managing asynchronous tasks.

## Other Features
Since streaming data is only available at certains times of day, we added the ability to capture data to a file and then be able to "replay" the data at a later time. This is quite helpful if you are developing an application and prefer to use test data rather than live streams.

To capture data to a file add the following code. This will save the data to file called "datacapture.txt" and record 60 seconds of data.

```
streamer.StartCapturingData("datacapture.txt", 60); 
```

To playback data from a file, run the following code.

```
// Add code here to authenticate, create a Streamer instance and register for events
// DO NOT call streamer.StartDataStreaming() as this will cause your test data to be played over the top of live data (unless you really want to do this)

// Playback the captured data with a constant rate of a 1-second delay between each item
streamer.PlaybackCapturedData("datacapture.txt", 1000)

// Alternatively, you can set the duration to '0' which will utilize the actual timestamps in the captured file to determine the rate at which data is sent
streamer.PlaybackCapturedData("datacapture.txt", 0)
```

## Future Work
1. Implement a full initial authentication path that accepts username/password as well as a secondary authentication code.
2. Add support for more TD Ameritrade Service Request types and their associated data models.