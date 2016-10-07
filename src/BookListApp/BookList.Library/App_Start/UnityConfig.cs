using BookList.Library.Controllers;
using Microsoft.Practices.Unity;
using Microsoft.ServiceFabric.Data;
using System.Web.Http;
using Unity.WebApi;

namespace BookList.Library
{
    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config, IReliableStateManager stateManager) {
            UnityContainer container = new UnityContainer();

            container.RegisterType<BookController>(
                new TransientLifetimeManager(),
                new InjectionConstructor(stateManager));

            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}