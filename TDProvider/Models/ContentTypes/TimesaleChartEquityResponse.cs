using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace TDProvider {
    /// <summary>
    /// TimesaleChartEquityResponse contains the response returned from the TD Ameritrade service for a Chart Equity request
    /// </summary>
    public class TimesaleChartEquityResponse
    {
        /// <value>A list of ChartEquityServiceInfo objects</value>
        public List<ChartEquityServiceInfo> data { get; set; } 
    }

    /// <summary>
    /// ChartEquityServiceInfo contains summary information for a Time Sale event
    /// </summary>
    public class ChartEquityServiceInfo {
        /// <value>service is the name of the service</value>
        public string service { get; set; } 

        /// <value>timestamp is the Unix timestamp (milliseconds) of the event</value>
        public object timestamp { get; set; } 

        /// <value>command is the command</value>
        public string command { get; set; } 

        /// <value>content contains the detailed information from a Chart Equity request</value>
        public List<ChartEquityContent> content { get; set; } 
    }

    /// <summary>
    /// ChartEquityContent contains the detailed information from a Chart Equity request
    /// </summary>
    public class ChartEquityContent {
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

        /// <value>Opening price for the minute</value>
        [JsonPropertyName("1")]
        public object OpenPrice { get; set; } 

        /// <value>Highest price for the minute</value>
        [JsonPropertyName("2")]
        public double HighPrice { get; set; } 

        /// <value>Chart’s lowest price for the minute</value>
        [JsonPropertyName("3")]
        public double LowPrice { get; set; } 

        /// <value>Closing price for the minute</value>
        [JsonPropertyName("4")]
        public double ClosePrice { get; set; } 

        /// <value>Total volume for the minute</value>
        [JsonPropertyName("5")]
        public double TotalVolume { get; set; } 

        /// <value>	Identifies the candle minute</value>
        [JsonPropertyName("6")]
        public long Sequence { get; set; } 

        /// <value>Milliseconds since Epoch</value>
        [JsonPropertyName("7")]
        public long ChartTime { get; set; } 

        /// <value>Day of chart</value>
        [JsonPropertyName("8")]
        public int ChartDay { get; set; } 
    }
}