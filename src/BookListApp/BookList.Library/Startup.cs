using BookList;
using BookList.Library.App_Start;
using Microsoft.ServiceFabric.Data;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookList.Library {
    public class Startup : IOwinAppBuilder {
        private readonly IReliableStateManager stateManager;

        public Startup(IReliableStateManager stateManager) {
            this.stateManager = stateManager;
        }

        /// <summary>
        /// Configures the app builder using Web API.
        /// </summary>
        /// <param name="appBuilder"></param>
        public void Configuration(IAppBuilder appBuilder) {
            HttpConfiguration config = new HttpConfiguration();

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.MapHttpAttributeRoutes();

            FormatterConfig.ConfigureFormatters(config.Formatters);
            UnityConfig.RegisterComponents(config, this.stateManager);

            appBuilder.UseWebApi(config);
            config.EnsureInitialized();
        }
    }
}
