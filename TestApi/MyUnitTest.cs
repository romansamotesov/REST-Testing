using RestSharp;
using REST_Testing;
using System.Net;
using System.Threading.Tasks.Dataflow;


namespace TestApi
{
    public class MyUnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetZipCodes_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "23456",
                "ABCDE",
            };

            var options = new RestClientOptions()
            {
                Authenticator = ReadAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:49000/zip-codes");
            var response = client.GetAsync(request).Result;
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code is not 200");
            var receivedZipcodes = ZipCode.ZipCodesToList(response.Content);
            Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "list of zipCodes is not equal");
        }

        [Test]
        public void PostZipCodes_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "23456",
                "ABCDE",
                "12333",
                "12344"
            };

            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:49000/zip-codes/expand");;
            var zipcodes = new ZipCode(new List<string>() { "12333", "12344" });
            request.AddJsonBody(zipcodes.Body);
            var response = client.Post(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
            var receivedZipcodes = ZipCode.ZipCodesToList(response.Content);
            Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "lists of zipCodes are not equal");
        }

        [Test]
        public void PostDuplicateZipCodes_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "23456",
                "ABCDE",
                "12355"
            };

            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:49000/zip-codes/expand"); ;
            var zipcodes = new ZipCode(new List<string>() { "12355", "12355" });
            request.AddJsonBody(zipcodes.Body);
            var response = client.Post(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
            var receivedZipcodes = ZipCode.ZipCodesToList(response.Content);
            Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "lists of zipCodes are not equal");
        }

        [Test]
        public void PostAlreadyUsedZipCodes_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "23456",
                "ABCDE",
                "12366"
            };

            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:49000/zip-codes/expand"); ;
            var zipcodes = new ZipCode(new List<string>() { "12366", "12345" });
            request.AddJsonBody(zipcodes.Body);
            var response = client.Post(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
            var receivedZipcodes = ZipCode.ZipCodesToList(response.Content);
            Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "lists of zipCodes are not equal");
        }
    }
}