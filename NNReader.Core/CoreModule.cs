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
            containerRegistry.RegisterInstance(IO.Locator.Default);
            containerRegistry.RegisterInstance<IO.ILocator>(IO.Locator.Default);

            containerRegistry.RegisterSingleton<IOrderDispatcher, OrderDispatcher>();
            containerRegistry.RegisterSingleton<IOrderBuilder, OrderBuilder>();

            containerRegistry.Register<IOrder, ChangingSelection>(nameof(ChangingSelection));
            containerRegistry.Register<IOrder, ChangingNovelSelection>(nameof(ChangingNovelSelection));
            containerRegistry.Register<IOrder, DownloadingBookmarkInfo>(nameof(DownloadingBookmarkInfo));

            containerRegistry.RegisterSingleton<INarouBookmarkService, NarouBookmarkService>();
            containerRegistry.RegisterSingleton<NarouBookmarkService>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}
