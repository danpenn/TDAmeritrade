using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace TDProvider {

    /// <summary>
    /// TimesaleDataResponse contains the response returned from the TD Ameritrade service from a Time Sale event
    /// </summary>
    public class TimesaleDataResponse    {
        /// <value>A list of TimeSaleServiceInfo objects</value>
        public List<TimesaleServiceInfo> data { get; set; } 
    }

    // {"data":[{"service":"TIMESALE_EQUITY", "timestamp":1603563278419,"command":"SUBS","content":[{"seq":243616,"key":"MSFT","1":1603497598931,"2":217.0,"3":5.0,"4":78437},{"seq":897259,"key":"AAPL","1":1603497598930,"2":115.09,"3":50.0,"4":285030}]}]}
    // {"service":"TIMESALE_EQUITY","timestamp":1603563278419,"command":"SUBS","content":[{"seq":243616,"key":"MSFT","1":1603497598931,"2":217.0,"3":5.0,"4":78437},{"seq":897259,"key":"AAPL","1":1603497598930,"2":115.09,"3":50.0,"4":285030}]}

    /// <summary>
    /// TimesaleServiceInfo contains summary information for a Time Sale event
    /// </summary>
    public class TimesaleServiceInfo {
        /// <value>service is the name of the service</value>
        public string service { get; set; } 

        /// <value>timestamp is the Unix timestamp (milliseconds) of the event</value>
        public object timestamp { get; set; } 

        /// <value>command is the command</value>
        public string command { get; set; } 

        /// <value>content contains the detailed information from a Time Sale event</value>
        public List<TimesaleContent> content { get; set; } 
    }

    /// <summary>
    /// TimesaleContent contains the detailed information from a Time Sale event
    /// </summary>
    public class TimesaleContent {
        /// <value>seq is the sequence number</value>
        public int seq { get; set; } 

        /// <value>key is the ticker symbol</value>
        public string key { get; set; } 

        /// <value>Ticker symbol</value>
        [JsonPropertyName("0")]
        public string Symbol { 
            get {
                return this.key;
            }
         } 

        /// <value>Trade time of the last trade in milliseconds since epoch</value>
        [JsonPropertyName("1")]
        public object TradeTime { get; set; } 

        /// <value>Price at which the last trade was matched</value>
        [JsonPropertyName("2")]
        public double LastPrice { get; set; } 

        /// <value>Number of shares traded with last trade</value>
        [JsonPropertyName("3")]
        public double LastSize { get; set; } 

        /// <value>Number of shares for bid</value>
        [JsonPropertyName("4")]
        public int LastSequence { get; set; } 
    }
}