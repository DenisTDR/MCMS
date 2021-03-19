using System.Collections.Generic;

namespace MCMS.Base.Middlewares
{
    public class ReverseProxyMiddlewareOptions
    {
        public Dictionary<string, string> ProxyRules { get; set; }
    }
}