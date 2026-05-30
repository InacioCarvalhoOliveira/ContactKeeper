using ContactKeeper.Models.Test;
using Microsoft.Extensions.Configuration;using System.Net;

namespace ContactKeeper.Test
{
    [TestFixture]
    [NonParallelizable]
    public class UnitTest1
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

        [Test]
        public async Task HealthDataBaseEndpoint_ReturnsOk()
        {
            var response = await _client.GetAsync("/Health/HealthDataBase");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Does.Contain("Connection available")
                .Or.Contain("service isn't avaliable"));
        }
    }
}