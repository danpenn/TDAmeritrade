

namespace TDProvider {
    /// <summary>
    /// ServiceName is the name of the streaming data service.  See 'Data Service Table' in 
    /// https://developer.tdameritrade.com/content/streaming-data#_Toc504640587 for more information
    /// </summary>
    public enum ServiceName {

        /// <summary>Admin requests: LOGIN, LOGOUT</summary>
        ADMIN, 

        /// <summary>Account Activity Notifications</summary>
        ACCT_ACTIVITY, 

        /// <summary>Actives for NASDAQ</summary>
        ACTIVES_NASDAQ, 

        /// <summary>Actives for NYSE</summary>
        ACTIVES_NYSE, 

        /// <summary>Actives for Options</summary>
        ACTIVES_OPTIONS, 

        /// <summary>Actives for OTCBB</summary>
        ACTIVES_OTCBB,

        /// <summary>Chart candle for Equity and Index</summary>
        CHART_EQUITY, 

        /// <summary>Chart candle for Futures and Futures OPtions</summary>
        CHART_FUTURES, 

        /// <summary>Chart candle for OPtions</summary>
        CHART_OPTIONS, 

        /// <summary>Chart history for Futures</summary>
        CHART_HISTORY_FUTURES, 

        /// <summary>Level 1 Equity</summary>
        QUOTE, 

        /// <summary>Level 1 Options</summary>
        OPTION, 

        /// <summary>Level 1 Equity Futures</summary>
        LEVELONE_FUTURES, 

        /// <summary>Level 1 Forex</summary>
        LEVELONE_FOREX, 

        /// <summary>Level 1 Futures Options</summary>
        LEVELONE_FUTURES_OPTIONS,

        /// <summary>News headline</summary>
        NEWS_HEADLINE, 

        /// <summary>Time and sale for Equity</summary>
        TIMESALE_EQUITY, 

        /// <summary>Time and sale for Forex</summary>
        TIMESALE_FOREX, 

        /// <summary>Time and sale for Futures and Futures Options</summary>
        TIMESALE_FUTURES, 

        /// <summary>Time and sale for Options</summary>
        TIMESALE_OPTIONS
    }

    /// <summary>
    /// CommandType is the command that corresponds to a ServiceName
    /// </summary>
    public enum CommandType {
        /// <summary>Login to the streaming service</summary>
        LOGIN,

        /// <summary>Logout from the streaming service</summary>
        LOGOUT,

        /// <summary>Quality of Service</summary>
        QOS,

        /// <summary>Subscribe to a data stream</summary>
        SUBS,

        /// <summary>Add a ticker symbol to an existing data stream</summary>
        ADD, 

        /// <summary>Unsubscribe from a data stream</summary>
        UNSUBS, 

        /// <summary>View???</summary>
        VIEW
    }

    /// <summary>LogType is the category of debug logging info the log message applies to</summary>
    public enum LogType {
        /// <summary>Warning signifies the related log data is a Warning</summary>
        Warning,

        /// <summary>Error signifies the related log data is an error</summary>
        Error,

        /// <summary>Info signifies the related log data is for Informational purposes</summary>
        Info
    }
}