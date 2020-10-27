namespace TDProvider {
    /// <summary>
    /// ServiceRequestFactory is a helper class for creation of service requests against the TD Ameritrade service
    /// </summary>
    public static class ServiceRequestFactory {
        /// <summary>
        /// CreateLoginRequest creates a login request
        /// </summary>
        /// <param name="credentials">A Credentials object which was created during authentication</param>
        /// <returns>A ServiceRequst object that can be passed to the Streamer</returns>
        public static ServiceRequest CreateLoginRequest(Credentials credentials) {
            ServiceRequest request = new ServiceRequest {
                Service = ServiceName.ADMIN,
                Command = CommandType.LOGIN,
                requestID = 1,
                Account = credentials.UserID,
                Source = credentials.ApplicationID,
                Parameters = new ServiceRequestParameters {
                    token = credentials.Token,
                    version = "1.0",
                    credential = credentials.ToQuery()
                },
            };
            
            return request;
        }

        /// <summary>
        /// CreateNewsRequest creates a request for News. Please note that a News subscription with TD Ameritrade
        /// must first be established with your account.
        /// </summary>
        /// <param name="credentials">A Credentials object which was created during authentication</param>
        /// <returns>A ServiceRequst object that can be passed to the Streamer</returns>
        public static ServiceRequest CreateNewsRequest(Credentials credentials) {
            return new ServiceRequest {
                Service = ServiceName.NEWS_HEADLINE,
                Command = CommandType.SUBS,
                requestID = 2,
                Account = credentials.UserID,
                Source = credentials.ApplicationID,
                Parameters = new ServiceRequestParameters {
                    keys = "GOOG",
                    fields = "0,1,2,3,4"
                },
            };
        }

        /// <summary>
        /// CreateChartEquityRequest creates a request for Chart equity data
        /// </summary>
        /// <param name="credentials">A Credentials object which was created during authentication</param>
        /// <param name="symbols">An array of stock symbols to retrieve data for</param>
        /// <returns>A ServiceRequst object that can be passed to the Streamer</returns>
        public static ServiceRequest CreateChartEquityRequest(Credentials credentials, string[] symbols) {
            return new ServiceRequest {
                Service = ServiceName.CHART_EQUITY,
                Command = CommandType.SUBS,
                requestID = 3,
                Account = credentials.UserID,
                Source = credentials.ApplicationID,
                Parameters = new ServiceRequestParameters {
                    keys = string.Join(",", symbols),
                    fields = "0,1,2,3,4,5,6,7,8"
                },
            };
        }

        /// <summary>
        /// CreateTimesaleFuturesRequest creates a request for Timesale_Futures. 
        /// </summary>
        /// <param name="credentials">A Credentials object which was created during authentication</param>
        /// <param name="symbols">An array of stock symbols to retrieve data for</param>
        /// <returns>A ServiceRequst object that can be passed to the Streamer</returns>
        public static ServiceRequest CreateTimesaleFuturesRequest(Credentials credentials, string[] symbols) {
            return new ServiceRequest {
                Service = ServiceName.TIMESALE_FUTURES,
                Command = CommandType.SUBS,
                requestID = 4,
                Account = credentials.UserID,
                Source = credentials.ApplicationID,
                Parameters = new ServiceRequestParameters {
                    keys = string.Join(",", symbols),
                    fields = "0,1,2,3,4"
                },
            };
        }

        /// <summary>
        /// CreateTimesaleEquityRequest creates a request for Chart_Timesales. Valid fields must be from 0-8
        /// </summary>
        /// <param name="credentials">A Credentials object which was created during authentication</param>
        /// <param name="symbols">An array of stock symbols to retrieve data for</param>
        /// <returns>A ServiceRequst object that can be passed to the Streamer</returns>
        public static ServiceRequest CreateTimesaleEquityRequest(Credentials credentials, string[] symbols) {
            return new ServiceRequest {
                Service = ServiceName.TIMESALE_EQUITY,
                Command = CommandType.SUBS,
                requestID = 5,
                Account = credentials.UserID,
                Source = credentials.ApplicationID,
                Parameters = new ServiceRequestParameters {
                    keys = string.Join(",", symbols),
                    fields = "0,1,2,3,4"
                },
            };
        }
    }
}