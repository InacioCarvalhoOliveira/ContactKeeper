using ContactKeeper.Models.Test;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NUnit.Framework;


[assembly: LevelOfParallelism(1)]

namespace ContactKeeper.Test
{
    [TestFixture]
    [NonParallelizable]
    public class UserEndpointsTests
    {
        private HttpClient _client = null!;
        private EnvironmentUrls? _urls;

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.TestDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            _urls = configuration
                .GetSection("EnvironmentUrls")
                .Get<EnvironmentUrls>();

            Assert.That(_urls, Is.Not.Null);
            Assert.That(_urls!.Devops, Is.Not.Null.And.Not.Empty);

            _client = new HttpClient
            {
                BaseAddress = new Uri(_urls.Devops)
            };
        }

        [TearDown]
        public void TearDown()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            _client?.Dispose();
        }

        private async Task<string> LoginAsAdminAsync()
        {
            var payload = new
            {
                username = "inacio",
                role = "admin",
                password = "12345678910"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/User/login", content);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);

            var token = doc.RootElement.GetProperty("token").GetString();

            Assert.That(token, Is.Not.Null.And.Not.Empty);
            return token!;
        }

        private async Task AuthorizeAsAdminAsync()
        {
            var token = await LoginAsAdminAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        [Test]
        public async Task ValidLogin_ReturnsOkAndToken()
        {
            var payload = new
            {
                username = "inacio",
                password = "12345678910",
                role = "admin"
            };

            var response = await _client.PostAsync(
                "/User/login",
                new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Does.Contain("token"));
            Assert.That(content, Does.Contain("user"));
        }

        [Test]
        public async Task InvalidLogin_ReturnsNotFound()
        {
            var payload = new
            {
                username = "inacio",
                role = "admin",
                password = "senha-errada"
            };

            var response = await _client.PostAsync(
                "/User/login",
                new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task UserWithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/User");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetUser_AsAdmin_ReturnsOk()
        {
            await AuthorizeAsAdminAsync();

            var response = await _client.GetAsync("/User");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Does.StartWith("["));
        }

        [Test]
        public async Task InvalidUserId_ReturnsNotFound()
        {
            await AuthorizeAsAdminAsync();

            var response = await _client.GetAsync("/User/999999");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }


        [Test]
        public async Task InvalidUserPayload_ReturnsBadRequest()
        {
            var payload = new
            {
                username = "",
                password = "",
                role = "",
                email = ""
            };

            var response = await _client.PostAsync(
                "/User",
                new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}