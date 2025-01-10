namespace MenulioPocMvc.Session
{
    public class RuntimeConstants
    {
        /// <summary>
        /// The name of the custom state cookie used to identify end users.
        /// </summary>
        public const string CustomStateCookieName = "SSCN";

        /// <summary>
        /// The name of a cookie containing an additional/analytics user identifier.
        /// </summary>
        public const string AnalyticsIdCookieName = "_ga";

        private static bool _debug = false;

        static RuntimeConstants()
        {
#if DEBUG
            _debug = true;
#endif
        }

        /// <summary>
        /// If the application is in debug compilation, we can allow insecure cookies.
        /// </summary>
        public static bool RequireSecureCookies { get { return _debug == false; } }

        /// <summary>
        /// Could be useful elsewhere instead of lots of debug switches.
        /// </summary>
        public static bool IsDebugCompilation { get { return _debug; } }

        /// <summary>
        /// The session state lifetime (in distributed cache).
        /// </summary>
        public static readonly TimeSpan CustomStateLifetime = TimeSpan.FromHours(1);
    }
}
