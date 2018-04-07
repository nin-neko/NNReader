using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Modularity;
using Prism.Ioc;

using NNReader.Ordering;
using NNReader.Bookmarks;


namespace NNReader
{
    public class CoreModule : IModule
    {
        public CoreModule()
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IOrderDispatcher, OrderDispatcher>();
            containerRegistry.Register<IOrder, ChangingSelectionOrder>(nameof(ChangingSelectionOrder));

            containerRegistry.RegisterSingleton<INarouBookmarkService, NarouBookmarkService>();
            containerRegistry.RegisterSingleton<NarouBookmarkService>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}
