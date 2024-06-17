using Infrastructure.Certificates;
using Prism.Ioc;
using Prism.Modularity;
using Unity;

namespace Infrastructure
{
    public class InfrastructureModule : IModule
    {
        

        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IUnmanagedCertificates, UnmanagedCertificates>();
            containerRegistry.RegisterSingleton<PackageProcessing>();
            containerRegistry.RegisterSingleton<SecurityProcessing>();
            containerRegistry.RegisterSingleton<SignerCertificateMapping>();
            containerRegistry.RegisterSingleton<ConfigurationAccess>();
            containerRegistry.RegisterSingleton<Infrastructure.HSM.HSM>();
            containerRegistry.RegisterSingleton<StorePasswordSafe>();
        }
    }
}
