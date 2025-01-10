using MenulioPocMvc.Telemetry.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.IdentityModel.Abstractions;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;

namespace MenulioPocMvc.Telemetry
{
    /// <summary>
    /// Utility class that contains logic for special telemetry events.
    /// </summary>
    public class TelemetryHelper : ITelemetryHelper
    {
        private readonly DateTime StartTime = DateTime.UtcNow;

        private readonly Lazy<TelemetryMetadata> _metadata;

        private readonly Lazy<TelemetryClient> client;

        private readonly ConcurrentQueue<string> uniqueEvents;

        private static Func<string> _actualSessionIdFunc = null;

        private static Func<string> _analyticsIdFunc = null;

        private static Func<string> _authCustomerIdFunc = null;

        // NOTE: this limit is intended to be not so big to harm performance but useful enough to prevent throttling on the same unique keys.
        private const int MaxTrackedEvents = 2000;

        private Lazy<ITelemetryHelper> _current;

        public TelemetryHelper(IConfiguration configuration)
        {
            this.uniqueEvents = new ConcurrentQueue<string>();
            this.client = new Lazy<TelemetryClient>(
                () =>
                {
                    var key = configuration["ApplicationInsights:InstrumentationKey"] ?? string.Empty;
                    if (string.IsNullOrEmpty(key))
                    {
                        throw new NotSupportedException(
                            @"Instrumentation key not specified: /AppSettings/" + TelemetryMetadata.AppSettingKey);
                    }

                    var clt = new TelemetryClient() { InstrumentationKey = key };
                    return clt;
                });
            _metadata = new Lazy<TelemetryMetadata>(TelemetryMetadata.Create(configuration));
            _current = new Lazy<ITelemetryHelper>(() => new TelemetryHelper(configuration));
        }

        public static void ConfigureSessionProperties(
            Func<string> sessionStateIdFunc,
            Func<string> analyticsIdFunc,
            Func<string> authCustomerIdFunc)
        {
            _actualSessionIdFunc = sessionStateIdFunc;
            _analyticsIdFunc = analyticsIdFunc;
            _authCustomerIdFunc = authCustomerIdFunc;
        }

        //static TelemetryHelper(IConfiguration configuration)
        //{
        //    metadata = new Lazy<TelemetryMetadata>(TelemetryMetadata.Create);
        //    current = new Lazy<ITelemetryHelper>(() => new TelemetryHelper(configuration));
        //}

        //public static ITelemetryHelper Current
        //{
        //    get { return current.Value; }
        //}

        /// <summary>
        /// We can identify application lifetime scope using an instance id.
        /// NOTE: the data points provide further capability for aggregation and product level data analysis.
        /// </summary>
        /// <returns>The common properties dictionary.</returns>
        private Dictionary<string, string> GetCommonProperties(string scope, Dictionary<string, string> properties)
        {
            var data = _metadata.Value;
            var runMinutes = DateTime.UtcNow.Subtract(StartTime).TotalMinutes.ToString(CultureInfo.InvariantCulture);

            var stateId = SafelyExecPropertyGetter(_actualSessionIdFunc);
            var analyticsId = SafelyExecPropertyGetter(_analyticsIdFunc);
            var customerId = SafelyExecPropertyGetter(_authCustomerIdFunc);

            var result = new Dictionary<string, string>() {
                { "Scope", scope },
                { "Reference", data.Reference },
                { "LifetimeId", data.LifetimeId },
                { "MachineName", data.MachineName },
                { "RunMinutes", runMinutes },
                { "SiteName", data.SiteName },
                { "SlotName", data.SlotName },
                { "Version", data.Version },
                { "StateId", stateId },
                { "AnalyticsId", analyticsId },
                { "CustomerId", customerId } };

            if (properties != null)
            {
                properties.ToList().ForEach(p => result.Add(p.Key, p.Value));
            }

            // NOTE: if a source is not already present, we can assume it was direct.
            if (result.ContainsKey("Source") == false)
            {
                result.Add("Source", "Direct");
            }

            return result;
        }

        /// <summary>
        /// Captures elapsed time of a task with common properties.
        /// </summary>
        /// <param name="name">The name of the item being tracked.</param>
        /// <param name="action">The action being tracked.</param>
        public void TrackElapsedTime(string name, Action action, Dictionary<string, string> properties)
        {
            var props = this.GetCommonProperties(TelemetryScopes.Event, properties);
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            action.Invoke();
            stopwatch.Stop();

            this.client.Value.TrackMetric(name, stopwatch.ElapsedMilliseconds, props);
        }

        /// <summary>
        /// Captures occurrence of an event with common properties.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="metrics">The metrics.</param>
        public void TrackEvent(string name, Dictionary<string, string> properties, Dictionary<string, double> metrics = null)
        {
            var props = this.GetCommonProperties(TelemetryScopes.Event, properties);
            this.client.Value.TrackEvent(name, props, metrics);
        }

        /// <summary>
        /// Captures occurrence of an event with common properties.
        /// NOTE: uses a simple in memory bag to determine whether the event is already captured to 
        /// minimise duplicates and excessive/throttled event capture.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="dataKey">The dataKey to use relating to the uniqueness of this event.</param>
        /// <param name="properties">The properties of this event.</param>
        public void TrackUnique(string name, string dataKey, Dictionary<string, string> properties)
        {
            var internalKey = string.Format("{0}|{1}", name, dataKey);
            if (uniqueEvents.Contains(internalKey))
            {
                return;
            }

            uniqueEvents.Enqueue(internalKey);
            var props = this.GetCommonProperties(TelemetryScopes.Event, properties);
            this.client.Value.TrackEvent(name, props, null);

            // NOTE: if we end up with lots of items, we better start trimming the oldest ones.
            if (uniqueEvents.Count > MaxTrackedEvents)
            {
                string item;
                uniqueEvents.TryDequeue(out item);
            }
        }

        /// <summary>
        /// Ensures any unsent metrics are dispatched. 
        /// </summary>
        public void Flush()
        {
            if (this.client.IsValueCreated)
            {
                this.client.Value.Flush();
            }
        }

        /// <summary>
        /// Captures exception with common properties.
        /// </summary>
        /// <param name="exception">The exception to capture.</param>
        public void TrackException(Exception exception, Dictionary<string, string> properties)
        {
            var props = this.GetCommonProperties(TelemetryScopes.Exception, properties
                    ?? new Dictionary<string, string>() { { "Source", "Unhandled" }, { "Level", "Error" } });

            this.client.Value.TrackException(exception, props);
        }

        /// <summary>
        /// Captures generic error messages with common properties.
        /// NOTE: we can simply cast the severity level type since it is a 1:1 long integer mapping.
        /// </summary>
        /// <param name="message">The trace message.</param>
        public void TrackTrace(string message, TelemetrySeverity severity, Dictionary<string, string> properties)
        {
            var props = this.GetCommonProperties(TelemetryScopes.Error, properties);
            this.client.Value.TrackTrace(message, (SeverityLevel)severity, props);
        }

        /// <summary>
        /// Provides a safety net around any configured property getters, ASP.NET pipeline might not be ready for example. 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private string SafelyExecPropertyGetter(Func<string> func)
        {
            if (func == null)
            {
                return TelemetryMetadata.ValueNotConfigured;
            }

            string result;
            try
            {
                result = func.Invoke();
            }
            catch (Exception ex)
            {
                // NOTE: we don't want recursion here so just include the exception type and carry on.
                return string.Format("{0}:{1}", TelemetryMetadata.ValueException, ex.GetType().FullName);
            }

            return string.IsNullOrWhiteSpace(result) ? TelemetryMetadata.ValueUnspecified : result;
        }
    }
}
