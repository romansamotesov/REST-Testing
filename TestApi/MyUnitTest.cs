using RestSharp;
using REST_Testing;
using System.Net;
using System.Threading.Tasks.Dataflow;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;
using Allure.Net.Commons;
using System.Text;
using Newtonsoft.Json;


namespace TestApi
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("ApiTests")]
    public class MyUnitTests
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [OneTimeSetUp]
        public void Setup()
        {
            Logger.Info("Test execution started");//test
        }

        [Test]
        [AllureTag("ZipCodes")]
        [AllureOwner("Roma")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureSubSuite("Get")]
        [AllureIssue("Issue Status Code is not 200")]
        public void GetZipCodes_Test()
        {
            var expectedZipCodes = new List<string>()//test githubactions
            {
                "12345",
                "23456",
                "ABCDE",
            };

            AllureApi.Step("Send Request");
            var options = new RestClientOptions()
            {
                Authenticator = ReadAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:49000/zip-codes");
            Logger.Info(request.Resource, "Request");
            var response = client.GetAsync(request).Result;
            Logger.Info(response.Content, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code is not 200");
                var receivedZipcodes = ZipCode.ZipCodesToList(response.Content);
                Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "list of zipCodes is not equal");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("ZipCodes")]
        [AllureOwner("Roma")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureSubSuite("Post")]
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

            AllureApi.Step("Send Request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:49000/zip-codes/expand");
            var zipcodes = new ZipCode(new List<string>() { "12333", "12344" });
            request.AddJsonBody(zipcodes.Body);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(zipcodes.Body), "json");
            Logger.Info(request.Resource, "Request");
            var response = client.Post(request);
            Logger.Info(response.Content, "Response");

            AllureApi.Step("Check Response");
            try 
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
                var receivedZipcodes = ZipCode.ZipCodesToList(response.Content);
                Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "lists of zipCodes are not equal");
            }
            catch (Exception ex) 
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("ZipCodes")]
        [AllureOwner("Roma")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureSubSuite("Post")]
        [AllureIssue("Issue Duplicate ZipCode is created")]
        public void PostDuplicateZipCodes_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "ABCDE",
                "12333",
                "12344",
                "12355"
            };

            AllureApi.Step("Send Request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:49000/zip-codes/expand"); 
            var zipcodes = new ZipCode(new List<string>() { "12355", "12355" });
            request.AddJsonBody(zipcodes.Body);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(zipcodes.Body), "json");
            Logger.Info(request.Resource, "Request");
            var response = client.Post(request);
            Logger.Info(response.Content, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
                var receivedZipcodes = ZipCode.ZipCodesToList(response.Content);
                Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "lists of zipCodes are not equal");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("ZipCodes")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureIssue("Issue Already used ZipCode is created")]
        [AllureSeverity(SeverityLevel.normal)]
        public void PostAlreadyUsedZipCodes_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "23456",
                "ABCDE",
                "12366"
            };

            AllureApi.Step("Send Request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:49000/zip-codes/expand"); 
            var zipcodes = new ZipCode(new List<string>() { "12366", "12345" });
            request.AddJsonBody(zipcodes.Body);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(zipcodes.Body), "json");
            Logger.Info(request.Resource, "Request");
            var response = client.Post(request);
            Logger.Info(response.Content, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
                var receivedZipcodes = ZipCode.ZipCodesToList(response.Content);
                Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "lists of zipCodes are not equal");//bug
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.critical)]
        public void PostCreateUser_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "ABCDE"
            };

            AllureApi.Step("Send Request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var createUserRequest = new RestRequest("http://localhost:49000/users");
            var user = new User(20, "TestName", Enums.Sex.FEMALE, "23456");
            createUserRequest.AddJsonBody(user);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(user)), "json");
            Logger.Info(createUserRequest.Resource, "Request");
            var createUserResponse = writeClient.Post(createUserRequest);
            Logger.Info(createUserResponse.Content, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(createUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user exist");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try
            {
                Assert.That(getUserResponse.Any(u => u.Name.Equals(user.Name)), "User is not created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that used zip code is removed from the list");
            var getZipCodesRequest = new RestRequest("http://localhost:49000/zip-codes");
            var getZipCodesResponse = readClient.GetAsync(getZipCodesRequest).Result;
            var receivedZipcodes = ZipCode.ZipCodesToList(getZipCodesResponse.Content);
            try
            {
                Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "list of zipCodes is not equal");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.normal)]
        public void PostRequiredFieldsCreateUser_Test()
        {
            AllureApi.Step("Send Request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var createUserRequest = new RestRequest("http://localhost:49000/users");
            var user = new User("TestNameRF", Enums.Sex.FEMALE);
            createUserRequest.AddJsonBody(user);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(user)), "json");
            Logger.Info(createUserRequest.Resource, "Request");
            var createUserResponse = writeClient.Post(createUserRequest);
            Logger.Info(createUserResponse.Content, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(createUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is exist");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try 
            { 
                Assert.That(getUserResponse.Any(u => u.Name.Equals(user.Name)), "User is not created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.normal)]
        public void PostNotAvailableZipCodeCreateUser_UserIsNotCreated_Test()
        {
            AllureApi.Step("Send Request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var createUserRequest = new RestRequest("http://localhost:49000/users");
            var user = new User(20, "TestNameZC", Enums.Sex.FEMALE, "111");
            createUserRequest.AddJsonBody(user);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(user)), "json");
            Logger.Info(createUserRequest.Resource, "Request");

            AllureApi.Step("Check Response");
            try
            {
                var exception = Assert.Throws<HttpRequestException>(() => writeClient.Post(createUserRequest));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.FailedDependency), "Status Code is not 424");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;

            AllureApi.Step("Check that user is not exist");
            try
            {
                Assert.That(getUserResponse.Any(u => u.Name.Equals(user.Name)), Is.False, "User is created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureIssue("Issue Status Code is not 400")]
        public void PostDuplicateCreateUser_UserIsNotCreated_Test()
        {
            AllureApi.Step("Send request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var createUserRequest = new RestRequest("http://localhost:49000/users");
            var user = new User(20, "TestName", Enums.Sex.FEMALE, "23456"); ;
            createUserRequest.AddJsonBody(user);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(user)), "json");
            Logger.Info(createUserRequest.Resource, "Request");

            AllureApi.Step("Check Response");
            try
            {
                var exception = Assert.Throws<HttpRequestException>(() => writeClient.Post(createUserRequest));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Status Code is not 400");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is not exist");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try
            {
                Assert.That(getUserResponse.Any(u => u.Name.Equals(user.Name)), Is.False, "User is created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Get")]
        [AllureSeverity(SeverityLevel.critical)]
        public void GetUsers_Test()
        {
            var users = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            AllureApi.Step("Create users");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var createUserRequest = new RestRequest("http://localhost:49000/users");
            createUserRequest.AddJsonBody(users[0]);
            var createUserResponse = writeClient.Post(createUserRequest);
            createUserRequest = new RestRequest("http://localhost:49000/users");
            createUserRequest.AddJsonBody(users[1]);
            createUserResponse = writeClient.Post(createUserRequest);
            createUserRequest = new RestRequest("http://localhost:49000/users");
            createUserRequest.AddJsonBody(users[2]);
            createUserResponse = writeClient.Post(createUserRequest);

            AllureApi.Step("Check that users are exist");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try
            {
                Assert.That(getUserResponse.All(x => users.Any(y => y.Name == x.Name)), "Users are not correspond");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Get")]
        [AllureSeverity(SeverityLevel.normal)]
        public void GetUsersByOlderAge_Test()
        {
            var expectedUsers = new List<User>()
            {
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            AllureApi.Step("Send request");
            var options = new RestClientOptions()
            {
                Authenticator = ReadAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            getUserRequest.AddParameter("olderThan", 29);
            Logger.Info(getUserRequest.Resource, "Request");
            var getUserResponse = client.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Logger.Info(getUserResponse.ToString(), "Response");

            AllureApi.Step("Check response");
            try
            {
                Assert.That(expectedUsers.All(x => getUserResponse.Any(y => y.Name == x.Name)), "Users filtered by age are not correspond");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Get")]
        [AllureSeverity(SeverityLevel.normal)]
        public void GetUsersByYoungerAge_Test()
        {
            var expectedUsers = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
            };

            AllureApi.Step("Send request");
            var options = new RestClientOptions()
            {
                Authenticator = ReadAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            getUserRequest.AddParameter("youngerThan", 39);
            Logger.Info(getUserRequest.Resource, "Request");
            var getUserResponse = client.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Logger.Info(getUserResponse.ToString(), "Response");

            AllureApi.Step("Check response");
            try
            {
                Assert.That(expectedUsers.All(x => getUserResponse.Any(y => y.Name == x.Name)), "Users filtered by age are not correspond");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Get")]
        [AllureSeverity(SeverityLevel.normal)]
        public void GetUsersBySex_Test()
        {
            var expectedUsers = new List<User>()
            {
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            AllureApi.Step("Send request");
            var options = new RestClientOptions()
            {
                Authenticator = ReadAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            getUserRequest.AddParameter("sex", "FEMALE");
            Logger.Info(getUserRequest.Resource, "Request");
            var getUserResponse = client.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Logger.Info(getUserResponse.ToString(), "Response");

            AllureApi.Step("Check response");
            try
            {
                Assert.That(expectedUsers.All(x => getUserResponse.Any(y => y.Name == x.Name)), "Users filtered by age are not correspond");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Put")]
        [AllureSeverity(SeverityLevel.critical)]
        public void UpdateUser_Test()
        {
            var userToChange = new User(30, "TestName30", Enums.Sex.FEMALE, "12333");
            var userNewValues = new User(30, "TestName31", Enums.Sex.FEMALE, "12333");
            var updateUserBody = new UpdateUserRequest()
            {
                UserToChange = userToChange,
                UserNewValues = userNewValues
            };

            AllureApi.Step("Create user");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var createUserRequest = new RestRequest("http://localhost:49000/users");
            createUserRequest.AddJsonBody(userToChange);
            var createUserResponse = writeClient.Post(createUserRequest);

            AllureApi.Step("Send response");
            var putUserRequest = new RestRequest("http://localhost:49000/users");
            putUserRequest.AddJsonBody(updateUserBody);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(updateUserBody)), "json");
            Logger.Info(putUserRequest.Resource, "Request");
            var putUserResponse = writeClient.PutAsync(putUserRequest).Result;
            Logger.Info(putUserResponse.ToString(), "Response");

            AllureApi.Step("Check response");
            try
            {
                Assert.That(putUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code is not 200");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is updated");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try
            {
                Assert.That(getUserResponse.Any(u => u.Name.Equals(userNewValues.Name)), "User is not updated");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Put")]
        [AllureSeverity(SeverityLevel.normal)]
        public void UpdateUserWithIndalidZipCode_UserIsNotUpdated_Test()
        {
            var userToChange = new User(30, "TestName31", Enums.Sex.FEMALE, "12333");
            var userNewValues = new User(30, "TestName32", Enums.Sex.FEMALE, "123333");
            var updateUserBody = new UpdateUserRequest()
            {
                UserToChange = userToChange,
                UserNewValues = userNewValues
            };
            var expectedMessage = "One or more errors occurred. (Request failed with status code FailedDependency)";

            AllureApi.Step("Send request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var putUserRequest = new RestRequest("http://localhost:49000/users");
            putUserRequest.AddJsonBody(updateUserBody);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(updateUserBody)), "json");
            Logger.Info(putUserRequest.Resource, "Request");

            AllureApi.Step("Check that exception was thrown");
            try
            {
                var exception = Assert.Throws<AggregateException>(() => writeClient.PutAsync(putUserRequest).Result.ToString());
                Assert.That(exception.Message, Is.EqualTo(expectedMessage), "Status Code is not 424");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is not updated");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try
            {
                Assert.That(getUserResponse.Any(u => u.Name.Equals(userNewValues.Name)), Is.False, "User is updated");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Put")]
        [AllureSeverity(SeverityLevel.normal)]
        public void UpdateUserWithNotFilledReqiuredFields_UserIsNotUpdated_Test()
        {
            var userToChange = new User(30, "TestName31", Enums.Sex.FEMALE, "12333");
            var userNewValues = new User("TestName33", Enums.Sex.FEMALE);
            userNewValues.Name = null;
            var updateUserBody = new UpdateUserRequest()
            {
                UserToChange = userToChange,
                UserNewValues = userNewValues
            };
            var expectedMessage = "One or more errors occurred. (Request failed with status code Conflict)";

            AllureApi.Step("Send request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var putUserRequest = new RestRequest("http://localhost:49000/users");
            putUserRequest.AddJsonBody(updateUserBody);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(updateUserBody)), "json");
            Logger.Info(putUserRequest.Resource, "Request");

            AllureApi.Step("Check that exception was thrown");
            try
            {
                var exception = Assert.Throws<AggregateException>(() => writeClient.PutAsync(putUserRequest).Result.ToString());
                Assert.That(exception.Message, Is.EqualTo(expectedMessage), "Status Code is not 409");//
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is not updated");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try
            {
                Assert.That(getUserResponse.Any(u => u.Name.Equals(userNewValues.Name)), Is.False, "User is updated");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Delete")]
        [AllureSeverity(SeverityLevel.critical)]
        public void DeleteUser_Test()
        {
            var userToDelete = new User(20, "TestName20", Enums.Sex.MALE, "12345");
            var expectedZipCode = "12345";

            AllureApi.Step("Send request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var deleteUserRequest = new RestRequest("http://localhost:49000/users");
            deleteUserRequest.AddJsonBody(userToDelete);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(userToDelete)), "json");
            Logger.Info(deleteUserRequest.Resource, "Request");
            var deleteUserResponse = writeClient.DeleteAsync(deleteUserRequest).Result;

            AllureApi.Step("Check response");
            try
            {
                Assert.That(deleteUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), "Status code is not 204");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is deleted");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Logger.Info(getUserResponse.ToString(), "Response");
            try 
            { 
                Assert.That(getUserResponse.Any(u => u.Name.Equals(userToDelete.Name)), Is.False, "User is not deleted"); 
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that zipcode is available");
            var getZipCodesRequest = new RestRequest("http://localhost:49000/zip-codes");
            var getZipCodesresponse = readClient.GetAsync(getZipCodesRequest).Result;
            var receivedZipcodes = ZipCode.ZipCodesToList(getZipCodesresponse.Content);
            try
            {
                Assert.That(receivedZipcodes.Any(z => z.Equals(expectedZipCode)), Is.True, "Zipcode not exist");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Delete")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureIssue("Issue User is not deleted")]
        public void DeleteUserWithRequiredFields_UserIsDeleted_Test()
        {
            var userToDelete = new User("TestName40", Enums.Sex.FEMALE);
            var expectedZipCode = "12345";

            AllureApi.Step("Send request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var deleteUserRequest = new RestRequest("http://localhost:49000/users");
            deleteUserRequest.AddJsonBody(userToDelete);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(userToDelete)), "json");
            Logger.Info(deleteUserRequest.Resource, "Request");
            var deleteUserResponse = writeClient.DeleteAsync(deleteUserRequest).Result;
            Logger.Info(deleteUserResponse.Content, "Response");

            AllureApi.Step("Check response");
            try
            {
                Assert.That(deleteUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), "Status code is not 204");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is deleted");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try
            {
                Assert.That(getUserResponse.Any(u => u.Name.Equals(userToDelete.Name)), Is.False, "User is not deleted");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that zipcode is available");
            var getZipCodesRequest = new RestRequest("http://localhost:49000/zip-codes");
            var getZipCodesresponse = readClient.GetAsync(getZipCodesRequest).Result;
            var receivedZipcodes = ZipCode.ZipCodesToList(getZipCodesresponse.Content);
            try
            {
                Assert.That(receivedZipcodes.Any(z => z.Equals(expectedZipCode)), Is.True, "Zipcode not exist");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Delete")]
        [AllureSeverity(SeverityLevel.normal)]
        public void DeleteUserWithEmptyRequiredField_UserIsNotDeleted_Test()
        {
            var userToDelete = new User(20, "TestName40", Enums.Sex.FEMALE, "ABCDE");
            userToDelete.Name = null;
            var expectedErrorMessage = "One or more errors occurred. (Request failed with status code Conflict)";

            AllureApi.Step("Send request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var deleteUserRequest = new RestRequest("http://localhost:49000/users");
            Logger.Info(deleteUserRequest.Resource, "Request");
            deleteUserRequest.AddJsonBody(userToDelete);
            AllureApi.AddAttachment("request", "application/json", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(userToDelete)), "json");

            AllureApi.Step("Check that exception is thrown");
            try
            {
                var exception = Assert.Throws<AggregateException>(() => writeClient.DeleteAsync(deleteUserRequest).Result.ToString());
                Assert.That(exception.Message, Is.EqualTo(expectedErrorMessage), "Status Code is not 409");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that user is not deleted");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try
            {
                Assert.That(getUserResponse.Any(u => u.Name.Equals("TestName40")), Is.True, "User is deleted deleted");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.critical)]
        public void UploadUsers_Test()
        {
            var expectedUsers = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            AllureApi.Step("Send request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var uploadUsersRequest = new RestRequest("http://localhost:49000/users/upload");
            uploadUsersRequest.AddFile("file", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\\MyJson.json");
            AllureApi.AddAttachment("request", "application/json", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\\MyJson.json");
            Logger.Info(uploadUsersRequest.Resource, "Request");
            var uploadUsersResponse = writeClient.Post(uploadUsersRequest);
            Logger.Info(uploadUsersResponse.Content, "Response");

            AllureApi.Step("Check Response");
            try
            {
                Assert.That(uploadUsersResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that users are created");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try
            {
                Assert.That(getUserResponse.All(x => expectedUsers.Any(y => y.Name == x.Name)), "Users are not correspond");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureIssue("Issue Status code is not 424")]
        public void UploadUsersWithIncorrectZipCodes_UsersAreNotUploadedTest()
        {
            var expectedUsers = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12344"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            AllureApi.Step("Send Request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var uploadUsersRequest = new RestRequest("http://localhost:49000/users/upload");
            uploadUsersRequest.AddFile("file", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\\MyIncorrectJson.json");
            AllureApi.AddAttachment("request", "application/json", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\\MyIncorrectJson.json");
            Logger.Info(uploadUsersRequest.Resource, "Request");

            AllureApi.Step("Check that error was thrown");
            try
            {
                var exception = Assert.Throws<HttpRequestException>(() => writeClient.Post(uploadUsersRequest));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.FailedDependency), "Status Code is not 424");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that users are not uploaded");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try
            {
                Assert.That(getUserResponse.Any(x => expectedUsers.Any(y => y.Name == x.Name)), Is.False, "Users are uploaded");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }
        }

        [Test]
        [AllureTag("Users")]
        [AllureOwner("Roma")]
        [AllureSubSuite("Post")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureIssue("Issue Status code is not 409")]
        public void UploadUsersWithMissedRequiredFields_UsersAreNotUploadedTest()
        {
            var expectedUsers = new List<User>()
            {
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            AllureApi.Step("Send Request");
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var uploadUsersRequest = new RestRequest("http://localhost:49000/users/upload");
            uploadUsersRequest.AddFile("file", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\\MyMissedRequiredFieldsJson.json");
            AllureApi.AddAttachment("request", "application/json", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\\MyMissedRequiredFieldsJson.json");
            Logger.Info(uploadUsersRequest.Resource, "Request");

            AllureApi.Step("Check that error was thrown");
            try
            {
                var exception = Assert.Throws<HttpRequestException>(() => writeClient.Post(uploadUsersRequest));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict), "Status Code is not 409");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is occurred");
                throw new Exception();
            }

            AllureApi.Step("Check that users are not uploaded");
            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            try
            {
                Assert.That(getUserResponse.Any(x => expectedUsers.Any(y => y.Name == x.Name)), Is.True, "Users are uploaded");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception is  occurred");
                throw new Exception();
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Logger.Info("Test execution finished");
        }
    }
}