using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System.Windows;
using SoftwareSigning;
using NavigationModule;
using SigningKeyManagment;
using Infrastructure;
namespace SigningTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            var catalog = moduleCatalog;
            catalog.AddModule(typeof(StartupModule));
            catalog.AddModule(typeof(Navigation));
            catalog.AddModule(typeof(InfrastructureModule));
            catalog.AddModule(typeof(KeyManagementModule));
            catalog.AddModule(typeof(SoftwareSigningModule));
            catalog.AddModule(typeof(TracingModule.TracingModule));
        }
    }
}
