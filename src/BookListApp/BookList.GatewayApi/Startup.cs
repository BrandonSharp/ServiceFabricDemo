using System.Web.Http;
using Owin;
using BookList.GatewayApi.App_Start;

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

            appBuilder.UseWebApi(config);
            config.EnsureInitialized();
        }
    }
}
