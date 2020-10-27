using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace TDProvider {
    class PrivateData {

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConsumerKey { get; set; }

        public PrivateData() {

        }

        public PrivateData(string JSON) {
            try {
                var privateData = JsonSerializer.Deserialize<PrivateData>(JSON);
                this.AccessToken = privateData.AccessToken;
                this.RefreshToken = privateData.RefreshToken;
                this.Username = privateData.Username;
                this.Password = privateData.Password;
                this.ConsumerKey = privateData.ConsumerKey;
            }
            catch {
                Debug.WriteLine("Failed to convert the JSON to a PrivateData object.");
            }
        }

        // ToJSON converts the object into a JSON string which can then be submitted to the server
        public string ToJSON() {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false
            };
            return JsonSerializer.Serialize(this, options);
        }

    }
}