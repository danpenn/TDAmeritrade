using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace TDProvider
{
    /// <summary>
    /// ServiceRequest is the structure required for requesting data
    /// </summary>
    public class ServiceRequest {
        /// <value>The TD Ameritrade service to access</value>
        [JsonPropertyName("service")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ServiceName Service {get; set;}

        /// <value>The command to issue</value>
        [JsonPropertyName("command")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CommandType Command {get; set;}

        /// <value>RequestID used for reference</value>
        [JsonPropertyName("requestid")]
        public int requestID {get; set;}

        /// <value>Customer’s account name or number</value>
        [JsonPropertyName("account")]
        public string Account {get; set;}

        /// <value>Client app’s assigned source ID</value>
        [JsonPropertyName("source")]
        public string Source {get; set;}

        /// <value>Parameters specific to the ServiceName and Command</value>
        [JsonPropertyName("parameters")]
        public ServiceRequestParameters Parameters {get; set;} 

        /// <summary>
        /// ToJSON converts the object into a JSON string which can then be submitted to the server
        /// </summary>
        /// <returns>A JSON string of the serialized ServiceRequest</returns>
        public string ToJSON() {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = false
            };
            return JsonSerializer.Serialize(this, options);
        }
    }

    /// <summary>
    /// ServiceRequestParameters contains parameters used for the Service Request
    /// </summary>
    public class ServiceRequestParameters {
        /// <value>Credentials in the form of a queryString. Applies to Login command only</value>
        public string credential {get; set;}

        /// <value>Access Token. Applies to Login command only</value>
        public string token {get; set;}

        /// <value>Version number. Use "1.0"</value>
        public string version {get; set;}

        /// <value>Keys are typically the ticker symbols</value>
        public string keys {get; set;}

        /// <value>Data fields to return in the response. See https://developer.tdameritrade.com/content/streaming-data#_Toc504640587 for more information</value>
        public string fields {get; set;}
    }

    /// <summary>
    /// ServiceRequests is the outermost container for multiple requests
    /// </summary>
    public class ServiceRequests    {
        /// <value>Requests is a list of request to issue</value>
        public List<ServiceRequest> requests { get; set; } 

        /// <summary>
        /// Creates a new ServiceRequests instance
        /// </summary>
        public ServiceRequests() {
            this.requests = new List<ServiceRequest>();
        }

        /// <summary>
        /// ToJSON converts the object into a JSON string which can then be submitted to the server
        /// </summary>
        /// <returns>A JSON string of the serialized ServiceRequests</returns>
        public string ToJSON() {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = false
            };
            string rawJSON = JsonSerializer.Serialize(this, options);
            return rawJSON.Replace("\u0026", "&");
        }
    } 
}