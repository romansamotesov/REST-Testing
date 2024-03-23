using RestSharp;
using REST_Testing;


namespace TestApi
{
    public class MyUnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Authentification()
        {

            var options = new RestClientOptions()
            {
                Authenticator = ReadAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:49000/zip-codes");
            var result = client.GetAsync(request).Result;
        }
    }
}