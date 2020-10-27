using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace TDProvider {
    class TokenRequest {
        public string grant_type { get; set; } 
        public string refresh_token { get; set; } 
        public string access_type { get; set; } 
        public string code { get; set; } 
        public string client_id { get; set; } 
        public string redirect_url { get; set; } 

        // ToJSON converts the object into a JSON string which can then be submitted to the server
        public string ToJSON() {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = false
            };
            return JsonSerializer.Serialize(this, options);
        }

        public string ToFormUrlEncodedForTokenRefresh() {
            string result = "grant_type=refresh_token&";
            result += "refresh_token=" + HttpUtility.UrlEncode(this.refresh_token) + "&";
            result += "client_id=" + HttpUtility.UrlEncode(this.client_id);

            return result;
        }
    }
}