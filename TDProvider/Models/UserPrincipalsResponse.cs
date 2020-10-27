
using System.Collections.Generic;

namespace TDProvider
{
    /// <summary>
    /// StreamerInfo contains information about the Streaming connection
    /// </summary>
    public class StreamerInfo    {
        /// <value>Binary URL</value>
        public string streamerBinaryUrl { get; set; } 

        /// <value>Websocket URL</value>
        public string streamerSocketUrl { get; set; } 

        /// <value>Token</value>
        public string token { get; set; } 

        /// <value>Timestamp Token was created</value>
        public string tokenTimestamp { get; set; } 

        /// <value>UserGroup</value>
        public string userGroup { get; set; } 

        /// <value>Access Level</value>
        public string accessLevel { get; set; } 

        /// <value>ACL</value>
        public string acl { get; set; } 

        /// <value>Application ID</value>
        public string appId { get; set; } 
    }

    /// <summary>
    /// Quotes contains information about whether exchanges are delayed
    /// </summary>
    public class Quotes    {
        /// <value>Whether NYSE is delayed</value>
        public bool isNyseDelayed { get; set; } 

        /// <value>Whether Nasdaq is delayed</value>
        public bool isNasdaqDelayed { get; set; } 

        /// <value>Whether Opra is delayed</value>
        public bool isOpraDelayed { get; set; } 

        /// <value>Whether Amex is delayed</value>
        public bool isAmexDelayed { get; set; } 

        /// <value>Whether Cme is delayed</value>
        public bool isCmeDelayed { get; set; } 

        /// <value>Whether Ice is delayed</value>
        public bool isIceDelayed { get; set; } 

        /// <value>Whether Forex is delayed</value>
        public bool isForexDelayed { get; set; } 
    }

    /// <summary>
    /// Key is a subscription
    /// </summary>
    public class Key    {
        /// <value>Subscription Key</value>
        public string key { get; set; } 
    }

    /// <summary>
    /// StreamerSubscriptionKeys is a list of subscription keys
    /// </summary>
    public class StreamerSubscriptionKeys    {
        /// <value>List of subscription keys</value>
        public List<Key> keys { get; set; } 
    }

    /// <summary>
    /// Preferences
    /// </summary>
    public class Preferences    {
        /// <value>Express Trading</value>
        public bool expressTrading { get; set; } 

        /// <value>Direct Options Routing</value>
        public bool directOptionsRouting { get; set; } 

        /// <value>Direct Equity Routing</value>
        public bool directEquityRouting { get; set; } 

        /// <value>Default Equity Order Leg Instructions</value>
        public string defaultEquityOrderLegInstruction { get; set; } 

        /// <value>Default Equity Order Type</value>
        public string defaultEquityOrderType { get; set; } 

        /// <value>Default Equity Order Price Link Type</value>
        public string defaultEquityOrderPriceLinkType { get; set; } 

        /// <value>Default Equity Order Information</value>
        public string defaultEquityOrderDuration { get; set; } 

        /// <value>Default Equity Order Market Session</value>
        public string defaultEquityOrderMarketSession { get; set; } 

        /// <value>Default Equity Quantity</value>
        public int defaultEquityQuantity { get; set; } 

        /// <value>Mutual Fund Tax Lot Method</value>
        public string mutualFundTaxLotMethod { get; set; } 

        /// <value>Option Tax Lot Method</value>
        public string optionTaxLotMethod { get; set; } 

        /// <value>Equity Tax Lot Method</value>
        public string equityTaxLotMethod { get; set; } 

        /// <value>Default Advanced Tool Launch</value>
        public string defaultAdvancedToolLaunch { get; set; } 

        /// <value>Authentication Token Timeout</value>
        public string authTokenTimeout { get; set; } 
    }

    /// <summary>
    /// Authorizations
    /// </summary>
    public class Authorizations    {
        /// <value></value>
        public bool apex { get; set; } 

        /// <value></value>
        public bool levelTwoQuotes { get; set; } 

        /// <value></value>
        public bool stockTrading { get; set; } 

        /// <value></value>
        public bool marginTrading { get; set; } 

        /// <value></value>
        public bool streamingNews { get; set; } 

        /// <value></value>
        public string optionTradingLevel { get; set; } 

        /// <value></value>
        public bool streamerAccess { get; set; } 

        /// <value></value>
        public bool advancedMargin { get; set; } 

        /// <value></value>
        public bool scottradeAccount { get; set; } 
    }

    /// <summary>
    /// Account Information
    /// </summary>
    public class Account    {
        /// <value></value>
        public string accountId { get; set; } 

        /// <value></value>
        public string description { get; set; } 

        /// <value></value>
        public string displayName { get; set; } 

        /// <value></value>
        public string accountCdDomainId { get; set; } 

        /// <value></value>
        public string company { get; set; } 

        /// <value></value>
        public string segment { get; set; } 

        /// <value></value>
        public string surrogateIds { get; set; } 

        /// <value></value>
        public Preferences preferences { get; set; } 

        /// <value></value>
        public string acl { get; set; } 

        /// <value></value>
        public Authorizations authorizations { get; set; } 
    }

    /// <summary>
    /// User Principal Response 
    /// </summary>
    public class UserPrincipalResponse    {
        /// <value></value>
        public string authToken { get; set; } 

        /// <value></value>
        public string userId { get; set; } 

        /// <value></value>
        public string userCdDomainId { get; set; } 

        /// <value></value>
        public string primaryAccountId { get; set; } 

        /// <value></value>
        public string lastLoginTime { get; set; } 

        /// <value></value>
        public string tokenExpirationTime { get; set; } 

        /// <value></value>
        public string loginTime { get; set; } 

        /// <value></value>
        public string accessLevel { get; set; } 

        /// <value></value>
        public bool stalePassword { get; set; } 

        /// <value></value>
        public StreamerInfo streamerInfo { get; set; } 

        /// <value></value>
        public string professionalStatus { get; set; } 

        /// <value></value>
        public Quotes quotes { get; set; } 

        /// <value></value>
        public StreamerSubscriptionKeys streamerSubscriptionKeys { get; set; } 

        /// <value></value>
        public List<Account> accounts { get; set; } 
    }
}