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
        public BookmarksViewModel(IContainerProvider container, IOrderDispatcher orderDispatcher, INarouBookmarkService bookmarkService)
        {
            this.BookmarkInfos = bookmarkService.Bookmarks.ToReadOnlyReactiveCollection();

            this.Add.Subscribe(async () =>
            {
            });

            this.SelectionChangedCommand.Subscribe(async e =>
            {
                var selectred = e.AddedItems.Cast<IBookmarkInfo>().FirstOrDefault();
                if (selectred == null) return;
                var o = container.Resolve<IOrder>("ChangingSelectionOrder");
                await orderDispatcher.DispatchAsync(o.With("Id", selectred.Id));
            });
        }

        public AsyncReactiveCommand Add { get; } = new AsyncReactiveCommand();

        public ReadOnlyReactiveCollection<IBookmarkInfo> BookmarkInfos { get; }
        public AsyncReactiveCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; } = new AsyncReactiveCommand<SelectionChangedEventArgs>();
    }
}
