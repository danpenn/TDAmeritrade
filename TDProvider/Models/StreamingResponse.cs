using System.Collections.Generic;

namespace TDProvider
{
    /// <summary>
    /// Content is the content of a response
    /// </summary>
    public class Content    {
        /// <value></value>
        public int code { get; set; } 

        /// <value></value>
        public string msg { get; set; } 
        
    }

    /// <summary>
    /// Response contains the full details for a response from the service
    /// </summary>
    public class Response    {
        /// <value>Service name</value>
        public string service { get; set; } 

        /// <value>Request ID for reference</value>
        public string requestid { get; set; } 

        /// <value>The issued command</value>
        public string command { get; set; } 

        /// <value>Timestamp of response</value>
        public long timestamp { get; set; } 

        /// <value>Detailed information about the response</value>
        public Content content { get; set; } 
    }

    /// <summary>
    /// StreamingResponse contains a list of responses from the service
    /// </summary>
    public class StreamingResponse    {
        /// <value>The list of Responses</value>
        public List<Response> response { get; set; } 
    }
}