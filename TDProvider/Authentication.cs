using System.Net.Http;
using System.Web;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Net.Http.Headers;
using System;

namespace TDProvider
{
    // IMPORTANT: TD Ameritrade used two-factor authentication to login. This means that you must login manually for the time using instructions 
    // from here:  https://developer.tdameritrade.com/content/simple-auth-local-apps
    // Two-factor authorization means its a two-step process: (1) You login as normal and they will send you a 6-digit code (2) You login again with
    // the code (this is the 2nd factor).
    // Once you do this you will receive an access token and a refresh token.  The access token is used for every subsequent call you make to the API
    // for retrieving data. The access token only lasts for 30 minutes so it needs to be refreshed frequently. The application will handle the refresh
    // automatically.  The refresh token is used to generate a new access token. The refresh token lasts for 90 days which means every 90 days you will
    // need to login again (using two-factor auth).
    class Authentication {
        const string PRIVATE_DATA_FILENAME = "privateData.json";
        const string AUTH_URL = "https://auth.tdameritrade.com/auth?response_type=code&redirect_uri={{REDIRECT_URI}}&client_id={{CONSUMER_KEY}}%40AMER.OAUTHAP";
                                // https://auth.tdameritrade.com/auth?response_type=code&redirect_uri=http://localhost&client_id=HCROUMFTUJJCZILICD925NBCGAYBXTDB%40AMER.OAUTHAP
                                // url:  https://auth.tdameritrade.com/oauth?client_id=HCROUMFTUJJCZILICD925NBCGAYBXTDB%40AMER.OAUTHAP&response_type=code&redirect_uri=http%3A%2F%2Flocalhost
                                // QueryString:  client_id=HCROUMFTUJJCZILICD925NBCGAYBXTDB%40AMER.OAUTHAP&response_type=code&redirect_uri=http%3A%2F%2Flocalhost
                                // Form Data:  _csrf=e08ac889-8a0a-4b9b-8ab0-ad381688c0d9&s_scope=PlaceTrades+AccountAccess+MoveMoney&s_client-desc=FlowBar&s_client-name=HCROUMFTUJJCZILICD925NBCGAYBXTDB%40AMER.OAUTHAP&fp_fp2DeviceId=4e218777fa2272231bbc50708c6d86d8&fp_browser=mozilla%2F5.0+%28macintosh%3B+intel+mac+os+x+10_15_7%29+applewebkit%2F537.36+%28khtml%2C+like+gecko%29+chrome%2F86.0.4240.80+safari%2F537.36%7C5.0+%28Macintosh%3B+Intel+Mac+OS+X+10_15_7%29+AppleWebKit%2F537.36+%28KHTML%2C+like+Gecko%29+Chrome%2F86.0.4240.80+Safari%2F537.36%7CMacIntel&fp_screen=24%7C2560%7C1440%7C1337&fp_timezone=-7&fp_language=lang%3Den-US%7Csyslang%3D%7Cuserlang%3D&fp_java=0&fp_cookie=1&fp_cfp=%7B%22navigator%22%3A%7B%22properties%22%3A%7B%22vendorSub%22%3A%22%22%2C%22productSub%22%3A%2220030107%22%2C%22vendor%22%3A%22Google+Inc.%22%2C%22maxTouchPoints%22%3A%220%22%2C%22doNotTrack%22%3A%22NULL%22%2C%22hardwareConcurrency%22%3A%2212%22%2C%22cookieEnabled%22%3A%22true%22%2C%22appCodeName%22%3A%22Mozilla%22%2C%22appName%22%3A%22Netscape%22%2C%22appVersion%22%3A%225.0+%28Macintosh%3B+Intel+Mac+OS+X+10_15_7%29+AppleWebKit%2F537.36+%28KHTML%2C+like+Gecko%29+Chrome%2F86.0.4240.80+Safari%2F537.36%22%2C%22platform%22%3A%22MacIntel%22%2C%22product%22%3A%22Gecko%22%2C%22userAgent%22%3A%22Mozilla%2F5.0+%28Macintosh%3B+Intel+Mac+OS+X+10_15_7%29+AppleWebKit%2F537.36+%28KHTML%2C+like+Gecko%29+Chrome%2F86.0.4240.80+Safari%2F537.36%22%2C%22language%22%3A%22en-US%22%2C%22onLine%22%3A%22true%22%2C%22deviceMemory%22%3A%228%22%7D%2C%22propertiesMD5%22%3A%2284004913931c17c27ade7880079eb5d4%22%2C%22navorderMD5%22%3A%22a6985aaf35e7c9924f0c1ef2fb019da6%22%2C%22navmethods%22%3A%22getBattery%7CgetGamepads%7CjavaEnabled%7CsendBeacon%7Cvibrate%7CregisterProtocolHandler%7CunregisterProtocolHandler%7CgetUserMedia%7CrequestMIDIAccess%7CrequestMediaKeySystemAccess%7CwebkitGetUserMedia%7CgetInstalledRelatedApps%7CclearAppBadge%7CsetAppBadge%22%2C%22navmethMD5%22%3A%22df59fa4ea10b884b8d5ccaa0b07197ee%22%7D%2C%22screen%22%3A%7B%22properties%22%3A%7B%22availWidth%22%3A%222560%22%2C%22availHeight%22%3A%221337%22%2C%22width%22%3A%222560%22%2C%22height%22%3A%221440%22%2C%22colorDepth%22%3A%2224%22%2C%22pixelDepth%22%3A%2224%22%2C%22availLeft%22%3A%220%22%2C%22availTop%22%3A%2223%22%7D%2C%22propertiesMD5%22%3A%224ec5471bd9d77b72e8a3abaa9b667b10%22%2C%22tamper%22%3Afalse%7D%2C%22timezone%22%3A%22-8%7C-7%22%2C%22canvas%22%3A%226e1fb87b228e4f79341213f957e2afb4%22%2C%22java%22%3Afalse%2C%22localstorage%22%3Atrue%2C%22sessionstorage%22%3Atrue%2C%22indexedDB%22%3Atrue%2C%22plugins%22%3A%7B%22names%22%3A%22Chrome+PDF+Plugin%7CChrome+PDF+Viewer%7CNative+Client%7Capplication%2Fpdf%7Capplication%2Fvxg_media_player%7CPortable+Document+Format%7CNative+Client+Executable%7CPortable+Native+Client+Executable%22%2C%22namesMD5%22%3A%22f0cf143767aed3c46dead84000749168%22%7D%2C%22fonts%22%3A%2216dae2b959ac3c264bc2a1949fe41c71%22%2C%22mathroutines%22%3A%2211013.232920103324%7C-1.4214488238747245%22%2C%22md5%22%3A%22b0fffaf00b06fb33e23a95b8363f2377%22%2C%22latency%22%3A84%2C%22form%22%3A%7B%22fields%22%3A%22su_username%7Csu_password%7Crememberuserid%7Cauthorize%7Creject%22%7D%2C%22clkhz%22%3A200003%7D&lang=en-us&su_username=Pennogt9&su_password=101Dubred%21&authorize=Log+in&signed=7f3ad4ae99e37f98ae022394c61e919b49749cdd

                                // redirect url:  https://localhost/?code=UvQeNExgxzRu8YNr8CuZY4UCNFRNabe0DVR9xAL%2BG4BKMHbV15uNFHvkGpjMV8IeL6S9iIeXR73T54WjeuZNNoAdft5ZWEyD5Bcph5A3G51pp1v7Jj%2FudkEEAhG%2BOg9B5zHKujdxPWPrKYIr1spghLGnBVRhNqLbWGw4%2Fk7hC551h%2BHJlu9ShmwTCOvbBzY8cURrv%2Fw8%2BjBpEiIVwlwKHqJqAQvrFy1mgmOuyesMfduRqC7nflBCiC%2B8RgEAR9unMN4V2SQRsn7c48pQIFfJfdcsUmgexrRmUXoUGaLog%2F3Cs4NBAS02o3eG0B5O7q5iYMckKrL32Qk%2BGChlfX%2BKqXDElkNc0QIBxfA9dJs%2BN3FEgskXpQmukyTp6KRoPRBamoeSuMeyJz40TKLh24OxFksC0DGYPEF8SJxQVZOFsZy8SygNoOVsPj239FJ100MQuG4LYrgoVi%2FJHHvlUF%2BYR60U2qunQnABBJEc9pLvGPvVXm%2FBcPYBdLcaFnkap4QPYJxxJYOvf4GNlgNp0AKqyhbhXFvt2j6%2FJgMq2t8OZBrqYs3l8NSspiWz86KG6mXlimAhsXOQ9DYQd%2Bp5tIVeWFWJqJmb9dVE%2B8aYIJfG3lUTTRSA74SuFpgh2xANXqmVCBjrCDxMjYDT5BObIAFJuIIVABFBBl%2B%2FsstTMZSMh6%2BOtbY2lg85clIKEhQVbcvDGkdnMC9Nbg3b%2FW1rKgPSpnx3iSLzP%2FVyH%2Fsy%2BlPjbITRSxnJLGCjthYP3hWcLVRUiY4pizOgetn23gtNCAcK%2B1vn8q9TdKqPPyVF7ZBAx54FdNt%2B7edjpZpuqgdHYOteEcl1mMs99X5sPBXQyvRgLLmKammbv8fKTHgg0H%2FiN6qLtrlLipUvkq9zBot2x86nq971l0WLSkA%3D212FD3x19z9sWBHDJACbC00B75E
                                

        const string TOKEN_URL = "https://api.tdameritrade.com/v1/oauth2/token";
        const string USER_PRINCIPALS_URL = "https://api.tdameritrade.com/v1/userprincipals?fields=streamerConnectionInfo,streamerSubscriptionKeys";
    
        static readonly HttpClient client = new HttpClient();

        PrivateData privateData;
        private UserPrincipalResponse userPrincipal;

        public UserPrincipalResponse UserPrincipal { 
            get { return this.userPrincipal; }
        }

        public Credentials StreamingCredentials {
            get {
                var creds = new Credentials();

                creds.Token = userPrincipal.streamerInfo.token;
                creds.UserGroup = userPrincipal.streamerInfo.userGroup;
                creds.AccessLevel = userPrincipal.streamerInfo.accessLevel;
                creds.ApplicationID = userPrincipal.streamerInfo.appId;
                creds.ACL = userPrincipal.streamerInfo.acl;

                var tokenDate = DateTime.Parse(userPrincipal.streamerInfo.tokenTimestamp);
                creds.Timestamp = new DateTimeOffset(tokenDate).ToUnixTimeMilliseconds();

                if (userPrincipal.accounts.Count > 0) {
                    creds.UserID = userPrincipal.accounts[0].accountId;
                    creds.Company = userPrincipal.accounts[0].company;
                    creds.Segment = userPrincipal.accounts[0].segment;
                    creds.CdDomain = userPrincipal.accounts[0].accountCdDomainId;
                }

                return creds;
            }
        }

        public Authentication() {
            // Load any saved settings
            this.privateData = loadPrivateData();
            this.userPrincipal = new UserPrincipalResponse();
        }

        public async Task RefreshAccessToken() {
            if (!isValidPrivateData()) {
                return;
            }

            try {
                TokenRequest tokenRequest = new TokenRequest{
                    grant_type = "refresh_token",
                    refresh_token = privateData.RefreshToken,
                    client_id = privateData.ConsumerKey
                };

                Debug.WriteLine("Refresh Request Body: " + tokenRequest.ToFormUrlEncodedForTokenRefresh());
                HttpContent content = new StringContent(tokenRequest.ToFormUrlEncodedForTokenRefresh(), Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.PostAsync(TOKEN_URL, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                TokenResponse tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseBody);

                this.privateData.AccessToken = tokenResponse.access_token;
                Debug.WriteLine("Retrieved new AccessToken: " + this.privateData.AccessToken);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.privateData.AccessToken);

                // Save the information so we can use it in the future
                savePrivateData();
            }
            catch(HttpRequestException e)
            {
                Debug.WriteLine("\nException Caught RefreshToken()!");	
                Debug.WriteLine("Message :{0} ",e.Message);
            }
        }

        public async Task GetUserPrincipals() {
            if (!isValidPrivateData()) {
                return;
            }

            try {
                HttpResponseMessage response = await client.GetAsync(USER_PRINCIPALS_URL);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                this.userPrincipal = JsonSerializer.Deserialize<UserPrincipalResponse>(responseBody);
            }
            catch(HttpRequestException e)
            {
                Debug.WriteLine("\nException Caught GetUserPrincipals()!");	
                Debug.WriteLine("Message :{0} ",e.Message);
            }
        }

        // loadPrivateData loads private data (i.e., username, password, etc.) from a file
        private PrivateData loadPrivateData() {
            if (File.Exists(PRIVATE_DATA_FILENAME)) {
                string rawJSON = File.ReadAllText(PRIVATE_DATA_FILENAME);
                return new PrivateData(rawJSON);
            }

            // Create a default private data file that can be manually edited
            privateData = new PrivateData();
            savePrivateData();

            return new PrivateData();
        }

        // savePrivateData saves private data (i.e., username, password, etc.) to a file
        private void savePrivateData() {
            File.WriteAllText(PRIVATE_DATA_FILENAME, privateData.ToJSON());
        }

        private bool isValidPrivateData()  {
            if (privateData.AccessToken == string.Empty && privateData.RefreshToken == string.Empty) {
                Debug.WriteLine("No private data exists. You will need to authenticate first in order access the TD Ameritrade service.");
                return false;
            }

            return true;
        }
    }
}