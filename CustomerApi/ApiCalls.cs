using MenulioPocMvc.Models.Apis;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using MenulioPocMvc.CustomerApi.Interfaces;
using MenulioPocMvc.Telemetry;
using MenulioPocMvc.Telemetry.Interfaces;

namespace MenulioPocMvc.CustomerApi
{
    public sealed class ApiCalls : IApiCalls
    {
        private readonly IHttpRequestClient _client;
        private readonly ITelemetryHelper _telemetryHelper;
        private readonly IConfiguration _config;

        private readonly string _subscriptionKey;

        public ApiCalls(IHttpRequestClient client, ITelemetryHelper telemetryHelper, IConfiguration config)
        {
            _client = client;
            _telemetryHelper = telemetryHelper;
            _config = config;
            _subscriptionKey = _config["CustomerApi:SubscriptionKey"] ?? string.Empty;
            _client.AddDefaultRequestHeaders("Ocp-Apim-Subscription-Key", _subscriptionKey);
        }

        private IDictionary<string, string> GetResponseHeaders(WebHeaderCollection headers)
        {
            var result = new Dictionary<string, string>();
            if (headers == null)
            {
                return result;
            }

            var keys = headers.AllKeys;
            for (var index = 0; index < keys.Length; index++)
            {
                result[keys[index]] = headers[index];
            }

            return result;
        }

        private IDictionary<string, string> GetResponseHeaders(HttpResponseHeaders headers)
        {
            var result = new Dictionary<string, string>();
            if (headers == null)
            {
                return result;
            }

            foreach (var header in headers)
            {
                result[header.Key] = header.Value.First();
            }

            return result;
        }

        private void CaptureExceptionTelemetry(Exception e, BaseResponse response, string uri, HttpMethod method, ContentType contentType)
        {
            // NOTE: capture everything we can about this error, including the response code where applicable.
            var props = GetApiProperties(response, uri, method, contentType);
            _telemetryHelper.TrackException(e, props);
            _telemetryHelper.TrackEvent("DIAPI:Error", props);
        }

        private void CaptureFaultTelemetry(BaseResponse response, string uri, HttpMethod method, ContentType contentType)
        {
            // NOTE: we now track every request/response, volumes are relatively low and throttle protected with the likes of recapture/af tokens.
            var code = (int)response.Response.StatusCode;
            var props = GetApiProperties(response, uri, method, contentType);
            if (code >= 200 && code < 300)
            {
                _telemetryHelper.TrackEvent("DIAPI:Ok", props);
                return;     
            }

            _telemetryHelper.TrackEvent("DIAPI:Fault", props);
        }

        private Dictionary<string, string> GetApiProperties(BaseResponse response, string uri, HttpMethod method,
            ContentType contentType)
        {
            // NOTE: capture some details about this error, including the response code.
            var length = response.ResponseBody != null ? response.ResponseBody.Length : 0;
            var code = (int)response.Response.StatusCode;
            var correlationId = this.GetHeaderValue(response, "X-CorrelationId");
            var apiVersion = this.GetHeaderValue(response, "X-IMGroupApiVersion");

            var props = new Dictionary<string, string>()
                {
                    { "Source", "APICalls.cs" },
                    { "EventCategory", "DIAPI" },
                    { "RequestUri", uri },
                    { "RequestMethod", method.ToString() },
                    { "RequestFormat", contentType.ToString() },
                    { "ResponseLength", length.ToString()},
                    { "ResponseCode", code.ToString() },
                    { "ResponseId", response.ResponseId.ToString("N") },
                    { "DICorrelationId", correlationId },
                    { "DIApiVersion", apiVersion },
                };

            return props;
        }

        private string GetHeaderValue(BaseResponse response, string key)
        {
            if (response.ResponseHeaders.ContainsKey(key))
            {
                return response.ResponseHeaders[key] ?? "Unspecified";
            }

            return "Unspecified";
        }

        public async Task<BaseResponse> Get(string uri, ContentType contentType, string subscriptionKey)
        {
            return await ActionRequest(uri, HttpMethod.Get, contentType, subscriptionKey);
        }

        public async Task<BaseResponse> Get(string uri, ContentType contentType)
        {
            return await ActionRequest(uri, HttpMethod.Get, contentType, _subscriptionKey);
        }

        public async Task<BaseResponse> Put(string uri, ContentType contentType, string body)
        {
            return await ActionRequest(uri, HttpMethod.Put, contentType, _subscriptionKey, body);
        }

        public async Task<BaseResponse> Post(string uri, ContentType contentType, string body)
        {
            return await ActionRequest(uri, HttpMethod.Post, contentType, _subscriptionKey, body);
        }

        public async Task<BaseResponse> Delete(string uri, ContentType contentType, string body)
        {
            return await ActionRequest(uri, HttpMethod.Delete, contentType, _subscriptionKey, body);
        }

        private async Task<BaseResponse> ActionRequest(string uri, HttpMethod method, ContentType contentType, string subscriptionKey, string body = "")
        {
            var apiResponse = new BaseResponse()
            {
                Response = new HttpResponseMessage()
            };

            HttpStatusCode responseStatus = HttpStatusCode.ExpectationFailed;

            try
            {
                var content = new StringContent(body, Encoding.UTF8, "application/xml");
                switch (contentType)
                {
                    case ContentType.Form:
                        _client.AddAcceptDefaultRequestHeaders(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                        content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                        break;
                    case ContentType.Text:
                        _client.AddAcceptDefaultRequestHeaders(new MediaTypeWithQualityHeaderValue("application/text"));
                        content = new StringContent(body, Encoding.UTF8, "application/text");
                        break;
                    case ContentType.Xml:
                        _client.AddAcceptDefaultRequestHeaders(new MediaTypeWithQualityHeaderValue("application/xml"));
                        content = new StringContent(body, Encoding.UTF8, "application/xml");
                        break;
                    case ContentType.Json:
                        _client.AddAcceptDefaultRequestHeaders(new MediaTypeWithQualityHeaderValue("application/json"));
                        content = new StringContent(body, Encoding.UTF8, "application/json");
                        break;
                    default:
                        _client.AddAcceptDefaultRequestHeaders(new MediaTypeWithQualityHeaderValue("application/xml"));
                        content = new StringContent(body, Encoding.UTF8, "application/xml");
                        break;
                }
                    
                var response = await _client.SendRequestAsync(method, uri, contentType, body);

                responseStatus = response.StatusCode;

                this.CaptureFaultTelemetry(apiResponse, uri, method, contentType);

                return response;
            }
            catch (WebException e)
            {
                apiResponse.Response.StatusCode = responseStatus;
                apiResponse.ResponseHeaders = this.GetResponseHeaders(e.Response?.Headers);
                if (e.Response is HttpWebResponse httpResp)
                {
                    // NOTE: response codes are more reliably extracted from the web exception.
                    apiResponse.Response.StatusCode = httpResp.StatusCode;
                }

                this.CaptureExceptionTelemetry(e, apiResponse, uri, method, contentType);
            }

            return apiResponse;
        }

    }
}
