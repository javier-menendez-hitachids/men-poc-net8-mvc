namespace MenulioPocMvc
{
    public class Configuration(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public string BaseUri => _configuration["CustomerApi:BaseUri"] ?? string.Empty;
        public string SubscriptionKey => _configuration["CustomerApi:SubscriptionKey"] ?? string.Empty;
        public string InstrumentationKey => _configuration["ApplicationInsights:InstrumentationKey"] ?? string.Empty;
        //public string UserAgent => _configuration["Telemetry:Reference"];
        //public string SignalRFunctionAppUrl => _configuration["SignalRService:FunctionAppUrl"];
        //public string SignalRSubscriptionKey => _configuration["SignalRService:SubscriptionKey"];
        //public List<string> SuspendedStatusList => _configuration.GetSection("SuspendedStatus").Get<List<string>>() ?? new List<string>();
    }
}
