using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Unity;

using Prism.Ioc;

using NNReader.Shells.Views;
using Prism.Modularity;

namespace NNReader
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App
    {
        public App()
        {

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var container = (IContainerExtension<IUnityContainer>)containerRegistry;

            containerRegistry.RegisterInstance<IContainerProvider>(container);
        }

        protected override Window CreateShell() => this.Container.Resolve<Shell>();

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            moduleCatalog.AddModule<Core.CoreModule>();
            moduleCatalog.AddModule<Shells.ShellModule>(dependsOn: nameof(Core.CoreModule));
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
        }
    }
}
