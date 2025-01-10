namespace MenulioPocMvc.Telemetry.Interfaces
{
    public interface ITelemetryHelper
    {
        void TrackElapsedTime(string name, Action action, Dictionary<string, string> properties);

        void TrackEvent(string name, Dictionary<string, string> properties, Dictionary<string, double> metrics = null);

        void TrackException(Exception exception, Dictionary<string, string> properties);

        void TrackTrace(string message, TelemetrySeverity severity, Dictionary<string, string> properties);

        void TrackUnique(string name, string dataKey, Dictionary<string, string> properties);

        void Flush();
    }
}
