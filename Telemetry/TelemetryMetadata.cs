namespace MenulioPocMvc.Telemetry
{
    /// <summary>
    /// A data object containing application level metadata for telemetry purposes.
    /// </summary>
    public class TelemetryMetadata
    {
        public const string AppSettingReference = "Telemetry.Reference";

        public const string AppSettingKey = "ApplicationInsights.InstrumentationKey";

        public const string ValueUnspecified = "N/A";

        public const string ValueAnonymous = "ANON";

        public const string ValueException = "EX";

        public const string ValueNotConfigured = "N/C";

        private const string EnvironmentDeploymentSite = "WEBSITE_SITE_NAME";

        private const string EnvironmentDeploymentSlot = "WEBSITE_SLOT_NAME";

        public string LifetimeId { get; private set; }

        public string Version { get; private set; }

        public string MachineName { get; private set; }

        public string Reference { get; private set; }

        public string SiteName { get; private set; }

        public string SlotName { get; private set; }

        /// <summary>
        /// Creates and instance and populates it with application runtime values.
        /// </summary>
        /// <returns></returns>
        public static TelemetryMetadata Create(IConfiguration configuration)
        {
            return new TelemetryMetadata
            {
                LifetimeId = Guid.NewGuid().ToString(),
                Version = typeof(TelemetryHelper).Assembly.GetName().Version.ToString(),
                MachineName = Environment.MachineName,
                Reference = configuration["ApplicationInsights:Reference"] ?? ValueUnspecified,
                SiteName = Environment.GetEnvironmentVariable(EnvironmentDeploymentSite) ?? ValueUnspecified,
                SlotName = Environment.GetEnvironmentVariable(EnvironmentDeploymentSlot) ?? ValueUnspecified
            };
        }
    }
}
