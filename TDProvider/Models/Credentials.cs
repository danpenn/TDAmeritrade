using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace TDProvider
{
    /// <summary>
    /// Credentials contains the information necessary to authenticate
    /// </summary>
    public class Credentials {
        /// <value>User ID</value>
        [JsonPropertyName("userid")]
        public string UserID {get; set;}

        /// <value>Token</value>
        [JsonPropertyName("token")]
        public string Token {get; set;}

        /// <value>Company Name</value>
        [JsonPropertyName("company")]
        public string Company {get; set;}

        /// <value>Segment</value>
        [JsonPropertyName("segment")]
        public string Segment {get; set;}

        /// <value>Cd Domain</value>
        [JsonPropertyName("cddomain")]
        public string CdDomain {get; set;}

        /// <value>User Group</value>
        [JsonPropertyName("usergroup")]
        public string UserGroup {get; set;}

        /// <value>Access Level</value>
        [JsonPropertyName("accesslevel")]
        public string AccessLevel {get; set;}

        /// <value>Authorized</value>
        [JsonPropertyName("authorized")]
        public string Authorized {get; set;}

        /// <value>Timestamp</value>
        [JsonPropertyName("timestamp")]
        public long Timestamp {get; set;}

        /// <value>Application ID</value>
        [JsonPropertyName("appid")]
        public string ApplicationID {get; set;}

        /// <value>ACL</value>
        [JsonPropertyName("acl")]
        public string ACL {get; set;}

        /// <summary>
        /// ToJSON converts the object into a JSON string which can then be submitted to the server
        /// </summary>
        /// <returns>An instance of the Credentials object</returns>
        public string ToJSON() {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// ToQuery is a helper function to create a querystring for authentication
        /// </summary>
        /// <returns>A querystring containing all values of the class</returns>
        public string ToQuery()
        {
            string result = "";
            result += "userid=" + HttpUtility.UrlEncode(this.UserID) + "&";
            result += "token=" + HttpUtility.UrlEncode(this.Token) + "&";
            result += "company=" + HttpUtility.UrlEncode(this.Company) + "&";
            result += "segment=" + HttpUtility.UrlEncode(this.Segment) + "&";
            result += "cddomain=" + HttpUtility.UrlEncode(this.CdDomain) + "&";
            result += "usergroup=" + HttpUtility.UrlEncode(this.UserGroup) + "&";
            result += "accesslevel=" + HttpUtility.UrlEncode(this.AccessLevel) + "&";
            result += "authorized=Y&";
            result += "timestamp=" + this.Timestamp.ToString() + "&";
            result += "appid=" + HttpUtility.UrlEncode(this.ApplicationID) + "&";
            result += "acl=" + HttpUtility.UrlEncode(this.ACL);

            return result;
        }
    }
}