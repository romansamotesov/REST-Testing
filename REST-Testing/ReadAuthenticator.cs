using RestSharp;
using RestSharp.Authenticators;


namespace REST_Testing
{
    public sealed class ReadAuthenticator: AuthenticatorBase
    {
        private static ReadAuthenticator instance;

        readonly string _baseUrl;
        readonly string _clientId;
        readonly string _clientSecret;

        private ReadAuthenticator(string baseUrl, string clientId, string clientSecret) : base("")
        {
            _baseUrl = baseUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public static ReadAuthenticator getInstance()
        {
            if (instance == null)
                instance = new ReadAuthenticator("http://localhost:49000", "0oa157tvtugfFXEhU4x7", "X7eBCXqlFC7x-mjxG5H91IRv_Bqe1oq7ZwXNA8aq");
            return instance;
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
        {
            Token = string.IsNullOrEmpty(Token) ? await GetToken() : Token;
            return new HeaderParameter(KnownHeaders.Authorization, Token);
        }

        async Task<string> GetToken()
        {
            var options = new RestClientOptions(_baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(_clientId, _clientSecret),
            };
            using var client = new RestClient(options);

            var request = new RestRequest("oauth/token")
                .AddParameter("grant_type", "client_credentials")
                .AddParameter("scope", "read");
            var response = await client.PostAsync<AuthentificationResponse>(request);
            return $"{ response!.TokenType}{ response!.AccessToken}";
        }
    }
}
