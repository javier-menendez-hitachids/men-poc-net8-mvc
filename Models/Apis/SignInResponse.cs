namespace MenulioPocMvc.Models.Apis
{
    public class SignInResponse
    {
        public HttpResponseMessage Response { get; set; }
        public string AuthToken { get; set; }
        public bool NeedsRepermission { get; set; }
        public bool NeedsPrivilege { get; set; }

        public string Error { get; set; }

        public string ErrorDescription { get; set; }

        public class SuccessfulLogin
        {
            public string access_token { get; set; }
        }

        public class FailLogin
        {
            public string error { get; set; }
            public string error_description { get; set; }
        }
    }
}
