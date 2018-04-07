using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Prism.Modularity;
using Prism.Ioc;


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
        }
    }
}
