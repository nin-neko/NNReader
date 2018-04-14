using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Windows.Controls;

using Prism.Ioc;

using Reactive.Bindings;
using Reactive.Bindings.Binding;
using Reactive.Bindings.Extensions;

using NNReader.Ordering;
using NNReader.Bookmarks;


namespace NNReader.Shells.ViewModels
{
    class BookmarksViewModel
    {
        public BookmarksViewModel(IContainerProvider container, IOrderBuilder orderBuilder, INarouBookmarkService bookmarkService)
        {
            this.NewNovelViewModel = container.Resolve<NewNovelViewModel>();

            this.BookmarkInfos = bookmarkService.Bookmarks.ToReadOnlyReactiveCollection(scheduler: UIDispatcherScheduler.Default);

            this.Add.Subscribe(async () =>
            {
            });

            this.SelectionChangedCommand.Subscribe(async e =>
            {
                var selectred = e.AddedItems.Cast<IBookmarkInfo>().FirstOrDefault();

                await orderBuilder.From("ChangingSelection")
                    .With("Id", selectred?.Id ?? Guid.Empty)
                    .DispatchAsync();
            });
        }

        public NewNovelViewModel NewNovelViewModel { get; }

        public AsyncReactiveCommand Add { get; } = new AsyncReactiveCommand();

        public ReadOnlyReactiveCollection<IBookmarkInfo> BookmarkInfos { get; }
        public AsyncReactiveCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; } = new AsyncReactiveCommand<SelectionChangedEventArgs>();
    }

    class NewNovelViewModel
    {
        public NewNovelViewModel(IContainerProvider container, IOrderBuilder orderBuilder, INarouBookmarkService bookmarkService)
        {
            this.DownloadCommand = bookmarkService.ObserveProperty(x => x.Status)
                .Select(x => x != BookmarkServiceStatus.BookmarkInfoDownloading && x != BookmarkServiceStatus.BookmarkInfoLoading)
                .ObserveOnUIDispatcher()
                .ToAsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    await orderBuilder.From("DownloadingBookmarkInfo")
                        .With("Ncode", this.Ncode.Value)
                        .DispatchAsync();
                });

            this.Downloading = bookmarkService.ObserveProperty(x => x.Status)
                .Select(x => x == BookmarkServiceStatus.BookmarkInfoDownloading)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim();

            var downloaded = Observable.FromEventPattern(h => bookmarkService.Downloaded += h, h => bookmarkService.Downloaded -= h);
            var failed = Observable.FromEventPattern(h => bookmarkService.DownloadingFailed += h, h => bookmarkService.DownloadingFailed -= h);

            new[] { downloaded, failed }
                .Merge()
                .ObserveOnUIDispatcher()
                .Subscribe(_ => this.Ncode.Value = "");

            downloaded
                .ObserveOnUIDispatcher()
                .Subscribe(_ => this.IsOpen.Value = false);
        }

        public ReactivePropertySlim<string> Ncode { get; } = new ReactivePropertySlim<string>("");
        public AsyncReactiveCommand DownloadCommand { get; }

        public ReadOnlyReactivePropertySlim<bool> Downloading { get; }

        public ReactivePropertySlim<bool> IsOpen { get; } = new ReactivePropertySlim<bool>();
    }
}
