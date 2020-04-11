using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CangguEvents.IntegrationTests.Utils
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient httpClient, string url,
            object jsonValue, CancellationToken cancellationToken = default)
        {
            var content = new StringContent(JsonConvert.SerializeObject(jsonValue), Encoding.UTF8, "application/json");
            return httpClient.PostAsync(url, content, cancellationToken);
        }
    }
}