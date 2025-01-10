using MenulioPocMvc.Models.Apis;

namespace MenulioPocMvc.CustomerApi.Interfaces
{
    public interface IApiCalls
    {
        Task<BaseResponse> Get(string uri, ContentType contentType);
        Task<BaseResponse> Put(string uri, ContentType contentType, string body);
        Task<BaseResponse> Post(string uri, ContentType contentType, string body);
        Task<BaseResponse> Delete(string uri, ContentType contentType, string body);
    }
}
