using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Prism.Modularity;
using Prism.Ioc;
using Prism.Regions;


namespace NNReader.Shells
{
    public class ShellModule : IModule
    {
        public ShellModule()
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var shell = (Views.Shell)Application.Current.MainWindow;
            containerRegistry.RegisterInstance<Window>(shell);
            containerRegistry.RegisterInstance(shell);
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var rm = containerProvider.Resolve<IRegionManager>();
            rm.RegisterViewWithRegion(nameof(Views.ShellContent), typeof(Views.ShellContent));
            rm.RegisterViewWithRegion(nameof(Views.HomeContents.Header), typeof(Views.HomeContents.Header));
            rm.RegisterViewWithRegion(nameof(Views.HomeContents.About), typeof(Views.HomeContents.About));
            rm.RegisterViewWithRegion(nameof(Views.Home), typeof(Views.Home));
            rm.RegisterViewWithRegion(nameof(Views.ReadingRoom), typeof(Views.ReadingRoom));
            rm.RegisterViewWithRegion(nameof(Views.ReadingRooms), typeof(Views.ReadingRooms));
        }
    }
}
