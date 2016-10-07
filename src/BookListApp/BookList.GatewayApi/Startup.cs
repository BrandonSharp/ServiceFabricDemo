using System.Web.Http;
using Owin;
using BookList.GatewayApi.App_Start;
using Swashbuckle.Application;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookList.GatewayApi {
    public static class Startup {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder) {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.MapHttpAttributeRoutes();

            FormatterConfig.ConfigureFormatters(config.Formatters);

            config
                .EnableSwagger(c => {
                    c.RootUrl(req => GetRootUrl(req));
                    c.SingleApiVersion("v1", "Library Gateway API");
                })
                .EnableSwaggerUi();


            appBuilder.UseWebApi(config);
            config.EnsureInitialized();
        }

        /// <summary>
        /// GetRootUrl is useful for appropriately mapping DNS names for applications deployed in Azure behind the load balancer.
        /// Forwarded / X-Forwarded-* headers are inspected, and the url is remapped accordingly.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string GetRootUrl(HttpRequestMessage request) {
            string scheme = null;
            string host = null;

            // Is there a "Forwarded" header?
            string forwarded = GetHeaderValue(request, "Forwarded");
            if (forwarded != null) {
                string[] parts = forwarded.Replace(" ", "").Split(';');

                foreach (var part in parts) {
                    if (part != null && part.StartsWith("host=", StringComparison.OrdinalIgnoreCase)) {
                        host = part.Substring("host=".Length);
                        if (host == string.Empty) host = null;
                    }
                    if (part != null && part.StartsWith("proto=", StringComparison.OrdinalIgnoreCase)) {
                        scheme = part.Substring("proto=".Length);
                        if (scheme == string.Empty) scheme = null;
                    }
                }
            }

            // Fallback to non-standard "X-Forwarded-Host"/"Port"
            string xForwardedHost = GetHeaderValue(request, "X-Forwarded-Host");
            if (host == null && xForwardedHost != null) {
                host = xForwardedHost;

                // in case there was no port in the host
                string xForwardedPort = GetHeaderValue(request, "X-Forwarded-Port");
                if (host.IndexOf(':') < 0 && xForwardedPort != null) {
                    host += ":" + xForwardedPort;
                }
            }

            // take the current URI if there was no header
            scheme = scheme ?? GetHeaderValue(request, "X-Forwarded-Proto") ?? request.RequestUri.Scheme;
            host = host ?? (request.RequestUri.Host + ":" + request.RequestUri.Port.ToString());

            // relative path (Swashbuckle doesn't allow a trailing '/')
            string path = GetHeaderValue(request, "X-Forwarded-PathBase") ?? request.GetRequestContext().VirtualPathRoot.ToString();
            path = "/" + path.TrimStart('/');
            path = path.TrimEnd('/');

            return $"{scheme}://{host}{path}";
        }

        private static string GetHeaderValue(HttpRequestMessage request, string headerName) {
            IEnumerable<string> list;
            return request.Headers.TryGetValues(headerName, out list) ? list.FirstOrDefault() : null;
        }

    }
}
