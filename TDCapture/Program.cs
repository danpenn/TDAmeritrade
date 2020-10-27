using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using TDProvider;
using System.Collections.Generic;

namespace TDAmeritrade
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // We first need to authenticate and obtain a new access token. Then we need to get User Principals which
            // contains additional information we need for accessing resources such as streaming data
            Authentication auth = new Authentication();
            await auth.RefreshAccessToken();
            await auth.GetUserPrincipals();

            // Create an instance of the StreamingProvider which will be used for processing the streaming data
            Streamer streamer = new Streamer(auth.StreamingCredentials);

            // Register to recieve data events. Any data events we receive will be sent to the dataReceived() function.
            streamer.DataReceived += dataReceived;

            // Register to receive debug logging data. This is only necessary when you are making changes to the application
            // and want to make sure there are no errors.  Comment out the following line to hide the debug log information.
            streamer.DebugDataReceived += DebugLogDataReceived;

            // register to receive confirmation from requests being sent.
            streamer.RequestResponseReceived += RequestResponseReceived;

            // Enable data capture recording for 60 seconds
            //streamer.StartCapturingData("datacapture.txt", 60);

            // Example code to play back previously captured data at 1-second intervals
            // streamer.PlaybackCapturedData("datacapture.txt", 3000);
            // await Task.Delay(90000);
            // return;

            // Create a cancellation token which will be used to cancel the asynchrnous websocket later
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Closing streaming connection and exiting...");
                cts.Cancel();
            };

            Console.WriteLine("Press ESC to Exit");

            var tasks = new List<Task> {
                streamer.StartDataStreaming(auth.UserPrincipal.streamerInfo.streamerSocketUrl, cts.Token)
            };

            Thread.Sleep(2000); // Sleep for 2 seconds before we issue any commands

            // TIMESALE_EQUITY
            var symbols = new string[] {"AAPL,MSFT"};
            streamer.SendRequest(ServiceRequestFactory.CreateTimesaleEquityRequest(auth.StreamingCredentials, symbols));

            // TIMESALE_FUTURES
            symbols = new string[] {"/ES"};
            streamer.SendRequest(ServiceRequestFactory.CreateTimesaleFuturesRequest(auth.StreamingCredentials, symbols));

            await Task.WhenAll(tasks);
            Console.WriteLine("All done, exiting application!");
        }

        static void dataReceived(object sender, DataReceivedEventArgs e) {
            switch (e.Type) {
                case "TIMESALE_FUTURES":
                case "TIMESALE_EQUITY":
                try {
                    TimesaleServiceInfo serviceInfo = JsonSerializer.Deserialize<TimesaleServiceInfo>(e.JSON);
                    showTimesaleContent(serviceInfo);
                }
                catch (Exception ex) {
                    Console.WriteLine("Failed to deserialize TIMESALE JSON to object. {0} - {1}", ex.Message, e.JSON); 
                }
                    break;
                default:
                    Console.WriteLine("Uknonwn type: " + e.Type);
                    break;
            }
        }

        static void DebugLogDataReceived(object sender, DebugDataReceivedEventArgs e) {
            Console.WriteLine(e.Message);
        }

        static void RequestResponseReceived(object sender, RequestResponseReceivedEventArgs e) {
            Console.WriteLine($"Request Response --> {e.Response.service}/{e.Response.command}: {e.Response.content.msg} ({e.Response.content.code})");
        }

        static void showTimesaleContent(TimesaleServiceInfo serviceInfo) {
            if (serviceInfo.content == null) {
                Console.WriteLine("ERROR! The content was 'null'.");
                return;
            }

            foreach (TimesaleContent content in serviceInfo.content) {
                Console.WriteLine(serviceInfo.service + ": Symbol=" + content.Symbol + ", Price=" + content.LastPrice + ", Size=" + content.LastSize);
            }
        }
    }
}



// {
//   "access_token": "UMzHkYdfn3bd7LJKR5XQsHppIfCuNDTSkhWPdNTHdWRw0qWM/F7B5+378jSIoqruUNl3ZrCNLsQ8j1kRg48/90B2S4joMmIFd++ynqSJ/y38jUapx2zofzfELDx4g7bHeeMFFoDjM0/od4Kdc7K6YUgDkdAnqXdQijSCStVpdmLn6barNJTLMH2O7RQXN0acmV7x7YcqfPCX53xWrqQTH6ESAIlwSeOUUkIF5Z6cSq+aXMJhpqkP6HYtxcFPrkUHVdQOm0pLqHscVU4zv3eCQmY3D6clDbYAvI6+GcLpibK07sGo2X0Btc1VSKy7W5SK115TRXw6PTFlOo/c9n7SwS0BQuiIqnviixBBw+RywrR/XEogdqZDRO0phUM2kuAgL6+fwOMRgcOuItzc/3+1czVGFuUvl8Sb936UfJeLoCC2HhwnGjueVQvjr/iLkIF2GQGEOZkjfreOImnPinPylZMF4SXXkIKy7MLzadi9bb457w/HAPjeZC3jZqyFnLfACg4uzxQul5M9266jzc+Ae8Yf2F0y+/WwvMbm2ngM2vDSVt6WB+nHYS979xhszQO/FzUJFY3Ni/CHbxhnV2YoOQ58VJM5AQWelto/y100MQuG4LYrgoVi/JHHvlMDdNlq03pHsQsNfEw/Ty75StjqjCROZ/Qyk3xrCQYbv24oBAvr5d+qoB35h1U3rHmm/r6w4Ko/RqK0lrBBlNJdLGp2Wd385QWgSVvK8/o1oP+pzyvfddtqBtX3uU4IevHg2vHl1CxMMBzVKu/gLCwBdw1eU0ibQB/OkSCkBrFpkQj1ZvTDBLGIeemjdy4vSIawjqJ6tiHdrj5ijtrixHPEZ6To3OSJixM5b+FKxUVcpLV5dWsb/3aZjC2ps7z8dcr4XKUXxovUi5JVCy+On86rqusjl/NQEyd1Tygk9OATkqXyz4pUlStsPmjKM6rysgMk5dAXuevaX8QKMe+Zdq6aAvh7g5yG+p4+vDHlK6/GDv6I2zoJALfnHYRCzCtJBvIcGB3wOzXHGoYUlfZVr0O/Y2pSxcZJSxjHSE0fyiTiVOKy69Yfjfu59Mzz7aznLosJ+K92C8dAJtvzMFxB4KKFMnCWd+fwCu73hl5iwmVSrtTVXcXCSzcka47bTGX2Qxi+PMPnaqm6Tj5OniWOJOPykLnIkTaZuGLzGQMsZ2RY5ydlRUyiYX+cCez7GZmbyy5JAbrXP8e72igBu2uF9cDZbOWHzs9azy4DYWli212FD3x19z9sWBHDJACbC00B75E",
//   "refresh_token": "eAMegdE//ptbiW2FC5jvSD6vimadWSTVtT6hxHLBmNkXCqZguVk0/zGWCn8rm0QfQwtI95G+wsus0LPkzBsThhcZFKhkbMrHtZ3B6uaVYh847DfT8PpYx4yKMtz4o6jAcpIQ6u4+Za+CAHnQLLMeNi4lmuAkUSsGHABIUaP+NYswmDnADBx8sOtmJrqGYcNKvw2rXFojvWTKVuANFAyInLSpnXXvdP4xtNcpma0cg4QXCXwPTQj+u68NWRmtZ2JjGC5eehwyaZYWhh3FjkPML4BLMc+hzN552DUo95axIeLG8tADkOswDpW1EvGh0c+/PF9EVnJfa9o3FvaACZsq7HeKhugP8cAziFLJm2gTLvlsZXtyUditDldPfIB+bDldu4FSbgreP3HN5VvxuLLKtRDjBtP132AoFm9pVnGZHkQOO6CJR6f3RyqRgOw100MQuG4LYrgoVi/JHHvlvc0R0lx+fibHPfCYLTWUP3g5VcYodLmmiqzpMb1+LhxeITsacQzN93g/DVJ3cPWqdnFjMHumQLUL5IPESf1g+i+tcX7ZsdpPPuTxYrRFYeoCEWOr3ps3xMZba8nMV3igGdzPyIFSOAHEYdZ1w3nIgDGI6ISL8Rh/tm6xaZP9Pi+nUTP8yVdjojVCC4kcCZzuFSUDGoh6BeMT39DeRp8UqE21r5z89Conw8zB84k6X1tF04k/Uiz67PuveXbvEhL49ww8/w7rLlJXwS2LTbrTzeZYRaFEDtLKfnXW/3tIHEoqU2SX7/s02DRn+c0JtWINUgX+SFa+QXGRxJkTbO9wnDFgsEM9u9nNgCgGfxLY6ftfp0ZMiC0Y4NbgM00FA+UMb/nVtQFWrEd+aBqF7xO/SEhqN3Ij22RtyYfyV8/o0k4fYTFG+r8YO+cSd3Q=212FD3x19z9sWBHDJACbC00B75E",
//   "scope": "PlaceTrades AccountAccess MoveMoney",
//   "expires_in": 1800,
//   "refresh_token_expires_in": 7776000,
//   "token_type": "Bearer"
// }

