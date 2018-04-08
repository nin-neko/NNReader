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

            this.Novels = bookmarkService.Novels.ToReadOnlyReactiveCollection(scheduler: UIDispatcherScheduler.Default);

            bookmarkService.ObserveProperty(x => x.SelectedNovelId)
                .Select(x =>
                {
                    if (x == Guid.Empty) return "";
                    var selected = bookmarkService.Novels.Single(n => n.Id == x);
                    return selected.Content;
                })
                .ObserveOnUIDispatcher()
                .Subscribe(x =>
                {
                    this.NovelContent.Value = x;
                });

            this.SelectionChangedCommand.Subscribe(async e =>
            {
                var selectred = e.AddedItems.Cast<INovel>().FirstOrDefault();
                await orderBuilder.From("ChangingNovelSelection")
                    .With("Id", selectred?.Id ?? Guid.Empty)
                    .DispatchAsync();
            });
        }

        public ReadOnlyReactivePropertySlim<bool> IsEmpty { get; }
        public ReadOnlyReactivePropertySlim<bool> IsAnySelected { get; }

        public ReadOnlyReactiveCollection<INovel> Novels { get; }
        public AsyncReactiveCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; } = new AsyncReactiveCommand<SelectionChangedEventArgs>();

        public ReactivePropertySlim<string> NovelContent { get; } = new ReactivePropertySlim<string>("");
    }
}
