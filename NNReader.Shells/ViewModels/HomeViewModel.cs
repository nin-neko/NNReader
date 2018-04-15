using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;

using Prism.Ioc;

using Reactive.Bindings;
using Reactive.Bindings.Binding;
using Reactive.Bindings.Extensions;

using MaterialDesignThemes.Wpf.Transitions;

using NNReader.Bookmarks;
using NNReader.Ordering;


namespace NNReader.Shells.ViewModels
{
    class HomeViewModel
    {
        public HomeViewModel(NewBookmarkDialogViewModel newBookmarkDialogViewModel, IOrderBuilder orderBuilder, ILoadableBookmarkService loadableBookmarkService)
        {
            this.NewBookmarkDialogViewModel = newBookmarkDialogViewModel;
            this.Bookmarks = loadableBookmarkService.Bookmarks.ToReadOnlyReactiveCollection(x => new BookmarkInfoViewModel(orderBuilder, x), UIDispatcherScheduler.Default);

            this.IsLoading = loadableBookmarkService.ObserveProperty(x => x.Status)
                .Select(x => x == BookmarkServiceStatus.BookmarkInfoLoading || x == BookmarkServiceStatus.BookmarkInfoDownloading)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim();
        }

        public NewBookmarkDialogViewModel NewBookmarkDialogViewModel { get; }
        public ReadOnlyReactiveCollection<BookmarkInfoViewModel> Bookmarks { get; }

        public ReadOnlyReactivePropertySlim<bool> IsLoading { get; }
    }

    class BookmarkInfoViewModel : BaseViewModel
    {
        public BookmarkInfoViewModel(IOrderBuilder orderBuilder, IBookmarkInfo bookmarkInfo)
        {
            this.LoadedCommand = new AsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    await orderBuilder.From("LoadingBookmarkSummary")
                        .With("Id", bookmarkInfo.Id)
                        .DispatchAsync();
                })
                .AddTo(this.CompositeDisposable);

            this.Title = bookmarkInfo.ObserveProperty(x => x.Title)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(this.CompositeDisposable);

            this.Writer = bookmarkInfo.ObserveProperty(x => x.Writer)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(this.CompositeDisposable);

            this.ChapterCount = bookmarkInfo.Chapters.ObserveProperty(x => x.Count)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(this.CompositeDisposable);

            this.ReadCommand = new AsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    Transitioner.MoveNextCommand.Execute(null, null);
                    await orderBuilder.From("RequestingBookmark")
                        .With("Id", bookmarkInfo.Id)
                        .DispatchAsync();
                })
                .AddTo(this.CompositeDisposable);
        }

        public AsyncReactiveCommand LoadedCommand { get; }

        public ReadOnlyReactivePropertySlim<string> Title { get; }
        public ReadOnlyReactivePropertySlim<string> Writer { get; }
        public ReadOnlyReactivePropertySlim<int> ChapterCount { get; }
        public AsyncReactiveCommand ReadCommand { get; }
    }

    class NewBookmarkDialogViewModel
    {
        public NewBookmarkDialogViewModel(IOrderBuilder orderBuilder, ILoadableBookmarkService loadableBookmarkService)
        {
            this.Ncode = new ReactiveProperty<string>("")
                .SetValidateNotifyError(x => string.IsNullOrWhiteSpace(x) ? "Empty" : null)
                .SetValidateNotifyError(x => loadableBookmarkService.Bookmarks.Any(b => b.Ncode == x) ? "Already has !" : null);

            this.AcceptCommand = this.Ncode.ObserveHasErrors
                .Select(x => !x)
                .ObserveOnUIDispatcher()
                .ToAsyncReactiveCommand();

            this.AcceptCommand.Subscribe(async () =>
            {
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
                await orderBuilder.From("DownloadingBookmarkInfo")
                    .With("Ncode", this.Ncode.Value)
                    .DispatchAsync();
                this.Ncode.Value = "";
            });

            this.CancelCommand.Subscribe(async () =>
            {
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, null);
                this.Ncode.Value = "";
            });
        }

        public ReactiveProperty<string> Ncode { get; }

        public AsyncReactiveCommand AcceptCommand { get; }
        public AsyncReactiveCommand CancelCommand { get; } = new AsyncReactiveCommand();
    }
}
