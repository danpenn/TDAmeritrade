using System.Collections.Generic;

namespace TDProvider {
    /// <summary>
    /// Notify contains information about a Notify heartbeat
    /// </summary>
    public class Notify    {
        /// <value>Heartbeat is a heartbeat timestamp from the service so that you know a connection is still active</value>
        public string heartbeat { get; set; } 
    }

    /// <summary>
    /// StreamingNotifyResponse is a list of notifications
    /// </summary>
    public class StreamingNotifyResponse    {
        /// <value>The list of notify objects</value>
        public List<Notify> notify { get; set; } 
    }
}