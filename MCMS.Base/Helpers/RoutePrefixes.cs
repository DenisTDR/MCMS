namespace MCMS.Base.Helpers
{
    public static class RoutePrefixes
    {
        private static string _routePrefix;

        public static string RoutePrefix
        {
            get
            {
                if (_routePrefix != null) return _routePrefix;
                _routePrefix = Env.Get("ROUTE_PREFIX");
                if (string.IsNullOrEmpty(_routePrefix)) _routePrefix = "/";

                return _routePrefix;
            }
        }

        private static string _adminRoutePrefix;

        public static string AdminRoutePrefix
        {
            get
            {
                if (_adminRoutePrefix != null) return _adminRoutePrefix;
                _adminRoutePrefix = Env.Get("ADMIN_ROUTE_PREFIX");
                if (string.IsNullOrEmpty(_adminRoutePrefix)) _adminRoutePrefix = "/";

                return _adminRoutePrefix;
            }
        }

        public static bool IsAdminRoutePrefixSet => AdminRoutePrefix != "/";

        private static string _adminRouteBasePath;

        public static string AdminRouteBasePath => _adminRouteBasePath ??=
            "~/" + (IsAdminRoutePrefixSet ? AdminRoutePrefix[1..] : "");

        public static string AdminApiRouteBasePath => AdminRouteBasePath + "api/";

        // Ensure route prefixes Env vars are correctly set
        public static void CheckRoutePrefixVars()
        {
            var routePrefix = Env.Get("ROUTE_PREFIX");
            if (!string.IsNullOrEmpty(routePrefix) && (!routePrefix.StartsWith("/") || !routePrefix.EndsWith("/")))
            {
                Utils.DieWith("Env var 'ROUTE_PREFIX' should start with a / (slash) and end with a / (slash).");
            }

            var adminRoutePrefix = Env.Get("ADMIN_ROUTE_PREFIX");
            if (!string.IsNullOrEmpty(adminRoutePrefix) &&
                (!adminRoutePrefix.StartsWith("/") || !adminRoutePrefix.EndsWith("/")))
            {
                Utils.DieWith("Env var 'ADMIN_ROUTE_PREFIX' should start with a / (slash) and end with a / (slash).");
            }
        }
    }
}