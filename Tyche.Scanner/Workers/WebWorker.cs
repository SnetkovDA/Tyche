using System;
using System.Net.Http;
using System.Text.Json;

namespace Tyche.Scanner.Workers
{
    public class WebWorker : IDisposable
    {
        private readonly HttpClient _httpClient;

        public WebWorker(string host, bool isHttps)
        {
            _httpClient = new();
            _httpClient.BaseAddress = new Uri($"{(isHttps ? "https" : "http")}://{host}");
        }

        public bool SendRequest(HttpMethod method, string address, string content)
        {
            System.Diagnostics.Debug.WriteLine($"Send {method} request to address: {address}, with content: {content}");
            var request = new HttpRequestMessage(method, address)
            {
                Content = new StringContent(content)
            };
            var responce = _httpClient.Send(request);
            return responce.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public void Dispose() => _httpClient.Dispose();
    }
}
