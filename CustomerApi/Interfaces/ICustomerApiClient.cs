using MenulioPocMvc.Models.Apis;

namespace MenulioPocMvc.CustomerApi.Interfaces
{
    public interface ICustomerApiClient
    {
        Task<BaseResponse> SendRequestAsync(HttpMethod method, string uri, ContentType contentType, string? body = null);
    }
}