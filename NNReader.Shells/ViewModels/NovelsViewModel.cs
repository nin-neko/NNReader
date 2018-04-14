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
    class NovelsViewModel
    {
        public NovelsViewModel(IContainerProvider container, IOrderBuilder orderBuilder, INarouBookmarkService bookmarkService)
        {
            this.IsEmpty = bookmarkService.ObserveProperty(x => x.SelectedBookmarkId)
                .Select(x => x == Guid.Empty)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim();

            this.IsAnySelected = this.IsEmpty.Select(x => !x)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim();

            this.Chapters = bookmarkService.Chapters.ToReadOnlyReactiveCollection(x => new ChapterViewModel(x), scheduler: UIDispatcherScheduler.Default);

            IDisposable contentSubscribing = default;
            bookmarkService.ObserveProperty(x => x.SelectedNovelId)
                .ObserveOnUIDispatcher()
                .Subscribe(x =>
                {
                    contentSubscribing?.Dispose();
                    if (x == Guid.Empty) return;

                    var selected = bookmarkService.Chapters.Single(n => n.Id == x);

                    contentSubscribing = selected.ObserveProperty(xx => xx.Content)
                        .ObserveOnUIDispatcher()
                        .Subscribe(xx => this.NovelContent.Value = xx);
                });

            this.SelectionChangedCommand.Subscribe(async e =>
            {
                var selectred = e.AddedItems.Cast<ChapterViewModel>().FirstOrDefault();
                await orderBuilder.From("ChangingNovelSelection")
                    .With("Id", selectred?.Id ?? Guid.Empty)
                    .DispatchAsync();
            });
        }

        public ReadOnlyReactivePropertySlim<bool> IsEmpty { get; }
        public ReadOnlyReactivePropertySlim<bool> IsAnySelected { get; }

        public ReadOnlyReactiveCollection<ChapterViewModel> Chapters { get; }
        public AsyncReactiveCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; } = new AsyncReactiveCommand<SelectionChangedEventArgs>();

        public ReactivePropertySlim<string> NovelContent { get; } = new ReactivePropertySlim<string>("");
    }

    class ChapterViewModel : IDisposable
    {
        private readonly System.Reactive.Disposables.CompositeDisposable disposables = new System.Reactive.Disposables.CompositeDisposable();

        public ChapterViewModel(IChapter chapter)
        {
            this.Id = chapter.Id;

            this.Title = chapter.ObserveProperty(x => x.Title)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposables);

            this.Status = chapter.ObserveProperty(x => x.Status)
                .Select(x => x.ToString())
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposables);
        }

        public Guid Id { get; }

        public ReadOnlyReactivePropertySlim<string> Title { get; }
        public ReadOnlyReactivePropertySlim<string> Status { get; }

        public void Dispose() => disposables.Dispose();
    }
}
