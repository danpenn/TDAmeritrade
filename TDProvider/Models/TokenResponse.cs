namespace TDProvider {
    /// <summary>
    /// TokenResponse is the response returned from a request to create or update tokens
    /// </summary>
    internal class TokenResponse    {
        /// <value>Access token used for subsequent requests to the TD Ameritrade service</value>
        public string access_token { get; set; } 

        /// <value>Refresh token used for requesting new access tokens</value>
        public string refresh_token { get; set; } 

        /// <value>Scope</value>
        public string scope { get; set; } 

        /// <value>Duration of time the access token is valid</value>
        public int expires_in { get; set; } 

        /// <value>The type of token</value>
        public string token_type { get; set; } 

        /// <value>Duration of time the refresh token is valid</value>
        public string refresh_token_expires_in { get; set; }
    }
}