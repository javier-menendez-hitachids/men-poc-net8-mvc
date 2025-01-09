using System.Net;

namespace MenulioPocMvc.Models.Apis
{
    public class BaseResponse
    {
        public Guid ResponseId { get; private set; }
        public HttpResponseMessage Response { get; set; } = new HttpResponseMessage();
        public string ResponseBody { get; set; } = string.Empty;
        public IDictionary<string, string> ResponseHeaders { get; set; } = new Dictionary<string, string>();
        public HttpStatusCode StatusCode { get; internal set; }

        public BaseResponse()
        {
            ResponseId = Guid.NewGuid();
        }
    }
}
