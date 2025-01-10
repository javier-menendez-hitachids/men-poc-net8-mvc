namespace MenulioPocMvc.CustomerApi
{
    using System.Net.Http.Headers;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using System.Text.Json;
    using MenulioPocMvc.CustomerApi.Interfaces;
    using MenulioPocMvc.Models.Apis;

    public class HttpRequestClient : IHttpRequestClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpRequestClient> _logger;

        public HttpRequestClient(HttpClient httpClient, ILogger<HttpRequestClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<BaseResponse> SendRequestAsync(HttpMethod method, string uri, ContentType contentType, string? body = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, uri);
                SetContentType(request, contentType, body);

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                var baseResponse = new BaseResponse
                {
                    StatusCode = response.StatusCode,
                    Response = response,
                    ResponseBody = responseBody,
                    ResponseHeaders = response.Headers.ToDictionary(h => h.Key, h => h.Value.First())
                };
                
                LogResponse(baseResponse, uri, method, contentType);

                return baseResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending {Method} request to {Uri}", method, uri);
                throw;
            }
        }

        public void AddDefaultRequestHeaders(string name, string? value)
        {
            _httpClient.DefaultRequestHeaders.Add(name, value);
        }
        public void AddAcceptDefaultRequestHeaders(MediaTypeWithQualityHeaderValue value)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(value);
        }

        private void SetContentType(HttpRequestMessage request, ContentType contentType, string? body)
        {
            var mediaType = contentType switch
            {
                ContentType.Form => "application/x-www-form-urlencoded",
                ContentType.Text => "text/plain",
                ContentType.Xml => "application/xml",
                ContentType.Json => "application/json",
                _ => "application/json"
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            if (!string.IsNullOrEmpty(body))
            {
                request.Content = new StringContent(body, Encoding.UTF8, mediaType);
            }
        }

        private void LogResponse(BaseResponse response, string uri, HttpMethod method, ContentType contentType)
        {
            var logLevel = response.StatusCode < System.Net.HttpStatusCode.BadRequest ? LogLevel.Information : LogLevel.Warning;

            _logger.Log(logLevel, "API Call: {Method} {Uri} - Status: {StatusCode}, ContentType: {ContentType}, ResponseLength: {Length}",
                method, uri, (int)response.StatusCode, contentType, response.ResponseBody?.Length ?? 0);
        }
    }

    public record ApiResponse
    {
        public System.Net.HttpStatusCode StatusCode { get; init; }
        public Dictionary<string, string> Headers { get; init; } = new();
        public string? Body { get; init; }
    }

    public enum ContentType
    {
        Form,
        Text,
        Xml,
        Json
    }
}
