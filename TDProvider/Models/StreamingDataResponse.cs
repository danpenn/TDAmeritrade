using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TDProvider {

    /// <summary>
    /// StreamingDataServiceInfo contains service info for a data response
    /// </summary>
    public class StreamingDataServiceInfo    {
        /// <value>Service Name</value>
        public string service { get; set; } 

        /// <value>Timestamp</value>
        public long timestamp { get; set; } 

        /// <value>Command issued</value>
        public string command { get; set; } 

        /// <value>Detailed content information</value>
        public List<object> content { get; set; }

        /// <summary>
        /// Converts the StreamingDataServiceInfo to a serialized JSON string
        /// </summary>
        /// <returns>JSON string</returns>
        public string ToJSON() {
            return JsonSerializer.Serialize(this);
        }
    }

    /// <summary>
    /// StreamingDataResponse is the parent object for a data response received from the service
    /// </summary>
    public class StreamingDataResponse    {
        /// <value>List of streaming data service info responses</value>
        public List<StreamingDataServiceInfo> data { get; set; } 
    }
}