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
            var request = new RestRequest("http://localhost:49000/zip-codes/expand");
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
            var request = new RestRequest("http://localhost:49000/zip-codes/expand"); 
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
            var request = new RestRequest("http://localhost:49000/zip-codes/expand"); 
            var zipcodes = new ZipCode(new List<string>() { "12366", "12345" });
            request.AddJsonBody(zipcodes.Body);
            var response = client.Post(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
            var receivedZipcodes = ZipCode.ZipCodesToList(response.Content);
            Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "lists of zipCodes are not equal");
        }

        [Test]
        public void PostCreateUser_Test()
        {
            var expectedZipCodes = new List<string>()
            {
                "12345",
                "ABCDE"
            };

            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var createUserRequest = new RestRequest("http://localhost:49000/users");
            var user = new User(20, "TestName", Enums.Sex.FEMALE, "23456");
            createUserRequest.AddJsonBody(user);
            var createUserResponse = writeClient.Post(createUserRequest);
            Assert.That(createUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");

            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(getUserResponse.Any(u => u.Name.Equals(user.Name)), "User is not created");

            var getZipCodesRequest = new RestRequest("http://localhost:49000/zip-codes");
            var getZipCodesResponse = readClient.GetAsync(getZipCodesRequest).Result;
            Assert.That(getZipCodesResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code is not 200");
            var receivedZipcodes = ZipCode.ZipCodesToList(getZipCodesResponse.Content);
            Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCodes), "list of zipCodes is not equal");
        }

        [Test]
        public void PostRequiredFieldsCreateUser_Test()
        {
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var createUserRequest = new RestRequest("http://localhost:49000/users");
            var user = new User("TestNameRF", Enums.Sex.FEMALE);
            createUserRequest.AddJsonBody(user);
            var createUserResponse = writeClient.Post(createUserRequest);
            Assert.That(createUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");

            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(getUserResponse.Any(u => u.Name.Equals(user.Name)), "User is not created");
        }

        [Test]
        public void PostNotAvailableZipCodeCreateUser_UserIsNotCreated_Test()
        {
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var createUserRequest = new RestRequest("http://localhost:49000/users");
            var user = new User(20, "TestName", Enums.Sex.FEMALE, "111");
            createUserRequest.AddJsonBody(user);
            var exception = Assert.Throws<HttpRequestException>(() => writeClient.Post(createUserRequest));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.FailedDependency), "Status Code is not 424");

            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(getUserResponse.Any(u => u.Name.Equals(user.Name)), Is.False, "User is created");
        }

        [Test]
        public void PostDuplicateCreateUser_UserIsNotCreated_Test()
        {
            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var createUserRequest = new RestRequest("http://localhost:49000/users");
            var user = new User(20, "TestName", Enums.Sex.FEMALE, "23456"); ;
            createUserRequest.AddJsonBody(user);
            var createUserResponse = writeClient.Post(createUserRequest);
            Assert.That(createUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status Code is not 201");
            var exception = Assert.Throws<HttpRequestException>(() => writeClient.Post(createUserRequest));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Status Code is not 400");

            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(getUserResponse.Any(u => u.Name.Equals(user.Name)), Is.False, "User is created");
        }

        [Test]
        public void GetUsers_Test()
        {
            var users = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

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

            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(getUserResponse.All(x => users.Any(y => y.Name == x.Name)), "Users are not correspond");
        }

        [Test]
        public void GetUsersByOlderAge_Test()
        {
            var expectedUsers = new List<User>()
            {
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            var options = new RestClientOptions()
            {
                Authenticator = ReadAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            getUserRequest.AddParameter("olderThan", 29);
            var getUserResponse = client.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(expectedUsers.All(x => getUserResponse.Any(y => y.Name == x.Name)), "Users filtered by age are not correspond");
        }

        [Test]
        public void GetUsersByYoungerAge_Test()
        {
            var expectedUsers = new List<User>()
            {
                new User(20, "TestName20", Enums.Sex.MALE, "12345"),
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
            };

            var options = new RestClientOptions()
            {
                Authenticator = ReadAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            getUserRequest.AddParameter("youngerThan", 39);
            var getUserResponse = client.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(expectedUsers.All(x => getUserResponse.Any(y => y.Name == x.Name)), "Users filtered by age are not correspond");
        }

        [Test]
        public void GetUsersBySex_Test()
        {
            var expectedUsers = new List<User>()
            {
                new User(30, "TestName30", Enums.Sex.FEMALE, "23456"),
                new User(40, "TestName40", Enums.Sex.FEMALE, "ABCDE"),
            };

            var options = new RestClientOptions()
            {
                Authenticator = ReadAuthenticator.getInstance(),
            };
            var client = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            getUserRequest.AddParameter("sex", "FEMALE");
            var getUserResponse = client.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(expectedUsers.All(x => getUserResponse.Any(y => y.Name == x.Name)), "Users filtered by age are not correspond");
        }


        [Test]
        public void UpdateUser_Test()
        {
            var userToChange = new User(30, "TestName30", Enums.Sex.FEMALE, "23456");
            var userNewValues = new User(30, "TestName31", Enums.Sex.FEMALE, "23456");
            var updateUserBody = new UpdateUserRequest()
            {
                UserToChange = userToChange,
                UserNewValues = userNewValues
            };

            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var putUserRequest = new RestRequest("http://localhost:49000/users");
            putUserRequest.AddJsonBody(updateUserBody);
            var putUserResponse = writeClient.PutAsync(putUserRequest).Result;
            Assert.That(putUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code is not 200");

            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(getUserResponse.Any(u => u.Name.Equals(userNewValues.Name)), "User is not updated");
        }

        [Test]
        public void UpdateUserWithIndalidZipCode_UserIsNotUpdated_Test()
        {
            var userToChange = new User(30, "TestName30", Enums.Sex.FEMALE, "23456");
            var userNewValues = new User(30, "TestName31", Enums.Sex.FEMALE, "23466");
            var updateUserBody = new UpdateUserRequest()
            {
                UserToChange = userToChange,
                UserNewValues = userNewValues
            };
            var expectedErrorMessage = "One or more errors occurred. (Request failed with status code FailedDependency)";

            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var putUserRequest = new RestRequest("http://localhost:49000/users");
            putUserRequest.AddJsonBody(updateUserBody);
            try
            {
                var putUserResponse = writeClient.PutAsync(putUserRequest).Result;
            }
            catch (Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo(expectedErrorMessage), "Status Code is not 424");
            }

            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(getUserResponse.Any(u => u.Name.Equals(userNewValues.Name)), Is.False, "User is updated");
        }

        [Test]
        public void UpdateUserWithNotFilledReqiuredFields_UserIsNotUpdated_Test()
        {
            var userToChange = new User(30, "TestName30", Enums.Sex.FEMALE, "23456");
            var userNewValues = new User("TestName33", Enums.Sex.FEMALE);
            userNewValues.Name = null;
            var updateUserBody = new UpdateUserRequest()
            {
                UserToChange = userToChange,
                UserNewValues = userNewValues
            };
            var expectedErrorMessage = "One or more errors occurred. (Request failed with status code Conflict)";

            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var putUserRequest = new RestRequest("http://localhost:49000/users");
            putUserRequest.AddJsonBody(updateUserBody);
            try
            {
                var putUserResponse = writeClient.PutAsync(putUserRequest).Result;
            }
            catch (Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo(expectedErrorMessage), "Status Code is not 409");
            }

            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(getUserResponse.Any(u => u.Name.Equals(userNewValues.Name)), Is.False, "User is updated");
        }


        [Test]
        public void UpdateUserWithNotFilledReqiuredFields_UserIsNotUpdated_Test()
        {
            var userToChange = new User(30, "TestName30", Enums.Sex.FEMALE, "23456");
            var userNewValues = new User("TestName33", Enums.Sex.FEMALE);
            userNewValues.Name = null;
            var updateUserBody = new UpdateUserRequest()
            {
                UserToChange = userToChange,
                UserNewValues = userNewValues
            };
            var expectedErrorMessage = "One or more errors occurred. (Request failed with status code Conflict)";

            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var putUserRequest = new RestRequest("http://localhost:49000/users");
            putUserRequest.AddJsonBody(updateUserBody);
            try
            {
                var putUserResponse = writeClient.PutAsync(putUserRequest).Result;
            }
            catch (Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo(expectedErrorMessage), "Status Code is not 409");
            }

            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(getUserResponse.Any(u => u.Name.Equals(userNewValues.Name)), Is.False, "User is updated");
        }

        [Test]
        public void DeleteUser_Test()
        {
            var userToDelete = new User(20, "TestName20", Enums.Sex.MALE, "12345");
            var expectedZipCode = new List<string>()
            {
                "12345",
            };

            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var deleteUserRequest = new RestRequest("http://localhost:49000/users");
            deleteUserRequest.AddJsonBody(userToDelete);
            var deleteUserResponse = writeClient.DeleteAsync(deleteUserRequest).Result;
            Assert.That(deleteUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), "Status code is not 204");

            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(getUserResponse.Any(u => u.Name.Equals(userToDelete.Name)), Is.False, "User is not deleted");

            var getZipCodesRequest = new RestRequest("http://localhost:49000/zip-codes");
            var getZipCodesresponse = readClient.GetAsync(getZipCodesRequest).Result;
            var receivedZipcodes = ZipCode.ZipCodesToList(getZipCodesresponse.Content);
            Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCode), "Zipcode is not match");
        }

        [Test]
        public void DeleteUserWithRequiredFields_UserIsDeleted_Test()
        {
            var userToDelete = new User("TestName20", Enums.Sex.MALE);
            var expectedZipCode = new List<string>()
            {
                "12345",
            };

            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var deleteUserRequest = new RestRequest("http://localhost:49000/users");
            deleteUserRequest.AddJsonBody(userToDelete);
            var deleteUserResponse = writeClient.DeleteAsync(deleteUserRequest).Result;
            Assert.That(deleteUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), "Status code is not 204");

            options.Authenticator = ReadAuthenticator.getInstance();
            var readClient = new RestClient(options);
            var getUserRequest = new RestRequest("http://localhost:49000/users");
            var getUserResponse = readClient.GetAsync<List<UserResponse>>(getUserRequest).Result;
            Assert.That(getUserResponse.Any(u => u.Name.Equals(userToDelete.Name)), Is.False, "User is not deleted");

            var getZipCodesRequest = new RestRequest("http://localhost:49000/zip-codes");
            var getZipCodesresponse = readClient.GetAsync(getZipCodesRequest).Result;
            var receivedZipcodes = ZipCode.ZipCodesToList(getZipCodesresponse.Content);
            Assert.That(receivedZipcodes, Is.EqualTo(expectedZipCode), "Zipcode is not match");
        }

        [Test]
        public void DeleteUserWithEmptyRequiredField_UserIsDeleted_Test()
        {
            var userToDelete = new User(20, "TestName20", Enums.Sex.MALE, "12345");
            userToDelete.Name = null;
            var expectedErrorMessage = "One or more errors occurred. (Request failed with status code Conflict)";

            var options = new RestClientOptions()
            {
                Authenticator = WriteAuthenticator.getInstance(),
            };
            var writeClient = new RestClient(options);
            var deleteUserRequest = new RestRequest("http://localhost:49000/users");
            deleteUserRequest.AddJsonBody(userToDelete);
            try
            {
                var deleteUserResponse = writeClient.DeleteAsync(deleteUserRequest).Result;
            }
            catch (Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo(expectedErrorMessage), "Status Code is not 409");
            }
        }
    }
}