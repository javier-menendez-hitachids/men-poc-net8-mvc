using MenulioPocMvc.Models.Apis;
using System.Net.Http.Headers;

namespace MenulioPocMvc.CustomerApi.Interfaces
{
    public interface IHttpRequestClient
    {
        Task<BaseResponse> SendRequestAsync(HttpMethod method, string uri, ContentType contentType, string? body = null);
        void AddDefaultRequestHeaders(string name, string? value);
        void AddAcceptDefaultRequestHeaders(MediaTypeWithQualityHeaderValue value);
    }
}