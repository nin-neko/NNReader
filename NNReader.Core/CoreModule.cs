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

            containerRegistry.RegisterSingleton<Diagnostics.IComponentService, Diagnostics.ComponentService>();

            containerRegistry.RegisterSingleton<IOrderDispatcher, OrderDispatcher>();
            containerRegistry.RegisterSingleton<IOrderBuilder, OrderBuilder>();

            containerRegistry.Register<IOrder, LoadingBookmarkService>(nameof(LoadingBookmarkService));
            containerRegistry.Register<IOrder, LoadingBookmarkSummary>(nameof(LoadingBookmarkSummary));
            containerRegistry.Register<IOrder, LoadingBookmarkChapter>(nameof(LoadingBookmarkChapter));
            containerRegistry.Register<IOrder, LoadingChapterTitle>(nameof(LoadingChapterTitle));
            containerRegistry.Register<IOrder, LoadingChapterContent>(nameof(LoadingChapterContent));
            containerRegistry.Register<IOrder, RequestingBookmark>(nameof(RequestingBookmark));
            containerRegistry.Register<IOrder, RequestingChapter>(nameof(RequestingChapter));
            containerRegistry.Register<IOrder, DownloadingBookmarkInfo>(nameof(DownloadingBookmarkInfo));
            containerRegistry.Register<IOrder, UpdatingBookmarkChapter>(nameof(UpdatingBookmarkChapter));

            var service = new NarouBookmarkService();
            containerRegistry.RegisterInstance<ILoadableBookmarkService>(service);
            containerRegistry.RegisterInstance<BaseBookmarkService>(service);

            //containerRegistry.RegisterInstance<ILoadableBookmarkService>(Debugging.MoqBookmarkService.Default);
            //containerRegistry.RegisterInstance<BaseBookmarkService>(Debugging.MoqBookmarkService.Default);

            containerRegistry.RegisterSingleton<Shells.IDialogService, Shells.DialogService>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}
