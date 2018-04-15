using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows.Controls;

using Prism.Ioc;

using Reactive.Bindings;
using Reactive.Bindings.Binding;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;

using MaterialDesignThemes.Wpf.Transitions;

using NNReader.Bookmarks;
using NNReader.Ordering;


namespace NNReader.Shells.ViewModels
{
    class ReadingRoomViewModel : BaseViewModel
    {
        public ReadingRoomViewModel(IOrderBuilder orderBuilder, ILoadableBookmarkService loadableBookmarkService, IBookmarkInfo bookmarkInfo)
        {
            this.Loading = bookmarkInfo.ObserveProperty(x => x.Status)
                .Select(x => x == BookmarkInfoStatus.SummaryLoading || x == BookmarkInfoStatus.SummaryDownloading || x == BookmarkInfoStatus.ChapterLoading || x == BookmarkInfoStatus.ChapterDownloading)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(this.CompositeDisposable);

            this.BackCommand = new AsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    Transitioner.MovePreviousCommand.Execute(null, null);
                    await orderBuilder.From("RequestingChapter")
                        .With("Id", Guid.Empty)
                        .Next("RequestingBookmark")
                        .With("Id", Guid.Empty)
                        .DispatchAsync();
                })
                .AddTo(this.CompositeDisposable);

            this.Chapters = bookmarkInfo.Chapters.ToReadOnlyReactiveCollection(x => new ChapterViewModel(orderBuilder, loadableBookmarkService, x), UIDispatcherScheduler.Default)
                .AddTo(this.CompositeDisposable);

            this.SelectionChangedCommand = new AsyncReactiveCommand<SelectionChangedEventArgs>()
                .WithSubscribe(async e =>
                {
                    var selectred = e.AddedItems.Cast<ChapterViewModel>().FirstOrDefault();
                    await orderBuilder.From("RequestingChapter")
                        .With("Id", selectred?.Id ?? Guid.Empty)
                        .DispatchAsync();
                })
                .AddTo(this.CompositeDisposable);

            this.IsSelected = Observable.FromEventPattern<BookmarkRequestEventArgs>(h => loadableBookmarkService.BookmarkRequested += h, h => loadableBookmarkService.BookmarkRequested -= h)
                .Select(x => x.EventArgs.Bookmark != null && x.EventArgs.Bookmark.Id == bookmarkInfo.Id)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(this.CompositeDisposable);

            this.ChapterTitle = Observable.FromEventPattern<ChapterRequestEventArgs>(h => loadableBookmarkService.ChapterRequested += h, h => loadableBookmarkService.ChapterRequested -= h)
                .Select(x => x.EventArgs.Chapter?.Title ?? "")
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(this.CompositeDisposable);

            //this.ChapterContent = new ReactivePropertySlim<string>("").AddTo(this.CompositeDisposable);

            this.ChapterContent = Observable.FromEventPattern<ChapterRequestEventArgs>(h => loadableBookmarkService.ChapterRequested += h, h => loadableBookmarkService.ChapterRequested -= h)
                .SelectMany(x =>
                {
                    var chapter = x.EventArgs.Chapter;
                    if(chapter == null || bookmarkInfo.Chapters.All(c => c.Id != chapter.Id)) return Observable.Return("");
                    return chapter.ObserveProperty(c => c.Content);
                })
                .ObserveOnUIDispatcher()
                //.Subscribe(x => this.ChapterContent.Value = x)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(this.CompositeDisposable);

            this.IsSelected.Where(x => x).Take(1).SelectMany(x =>
            {
                return orderBuilder.From("LoadingBookmarkChapter")
                    .With("Id", bookmarkInfo.Id)
                    .DispatchAsync()
                    .ToObservable();
            })
            .Subscribe()
            .AddTo(this.CompositeDisposable);
        }

        public ReadOnlyReactivePropertySlim<bool> IsSelected { get; }

        public ReadOnlyReactivePropertySlim<bool> Loading { get; }

        public AsyncReactiveCommand BackCommand { get; }

        public ReadOnlyReactiveCollection<ChapterViewModel> Chapters { get; }
        public AsyncReactiveCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; }

        public ReadOnlyReactivePropertySlim<string> ChapterTitle { get; }
        public ReadOnlyReactivePropertySlim<string> ChapterContent { get; }
        //public ReactivePropertySlim<string> ChapterContent { get; }
    }

    class ChapterViewModel : BaseViewModel
    {
        private bool isLoaded;

        public ChapterViewModel(IOrderBuilder orderBuilder, ILoadableBookmarkService loadableBookmarkService, IChapter chapter)
        {
            this.Id = chapter.Id;

            this.LoadedCommand = new AsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    if (isLoaded) return;
                    isLoaded = true;
                    await orderBuilder.From("LoadingChapterTitle")
                       .With("Id", chapter.Id)
                       .DispatchAsync();
                })
                .AddTo(this.CompositeDisposable);

            this.IsSelected = new ReactivePropertySlim<bool>().AddTo(this.CompositeDisposable);

            Observable.FromEventPattern<ChapterRequestEventArgs>(h => loadableBookmarkService.ChapterRequested += h, h => loadableBookmarkService.ChapterRequested -= h)
                .Select(x => x.EventArgs.Chapter != null && x.EventArgs.Chapter.Id == chapter.Id)
                .ObserveOnUIDispatcher()
                .Subscribe(x => this.IsSelected.Value = x)
                .AddTo(this.CompositeDisposable);

            this.Title = chapter.ObserveProperty(x => x.Title)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(this.CompositeDisposable);

            this.Status = chapter.ObserveProperty(x => x.Status)
                .Select(x => x.ToString())
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(this.CompositeDisposable);

            this.IsSelected.Where(x => x).Take(1).SelectMany(x =>
            {
                return orderBuilder.From("LoadingChapterContent")
                    .With("Id", chapter.Id)
                    .DispatchAsync()
                    .ToObservable();
            })
            .Subscribe()
            .AddTo(this.CompositeDisposable);
        }

        public Guid Id { get; }

        public AsyncReactiveCommand LoadedCommand { get; }

        public ReactivePropertySlim<bool> IsSelected { get; }

        public ReadOnlyReactivePropertySlim<string> Title { get; }
        public ReadOnlyReactivePropertySlim<string> Status { get; }
    }
}
