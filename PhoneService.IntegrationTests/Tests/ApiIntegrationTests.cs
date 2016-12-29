using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Newtonsoft.Json;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace Proekspert.PhoneServiceTask.IntegrationTests
{
    public class ApiIntegrationTests
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests()
        {
            string testExecutablePath = PlatformServices.Default.Application.ApplicationBasePath;
            string testProjectRelativePath = @"..\..\..\..\PhoneServiceHost";
            string contentRoot = Path.Combine(testExecutablePath, testProjectRelativePath);

            var server = new TestServer(
                new WebHostBuilder()
                    .UseContentRoot(contentRoot)
                    .UseStartup<TestStartup>()
            );
            
            _client = server.CreateClient();
        }

        [Fact(DisplayName = "Data API integration test")]
        public async Task DataTest()
        {
            string dataPath = "/data";
            HttpResponseMessage response = await _client.GetAsync(dataPath);
            Assert.True(response.IsSuccessStatusCode);
            
            string responseString = await response.Content.ReadAsStringAsync();
            dynamic responseData = JsonConvert.DeserializeObject(responseString);
            string responseDataNumber = (string)responseData.state.number;

            Assert.Equal(TestStartup.Number, responseDataNumber);
        }
    }
}