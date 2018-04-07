using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Binding;
using Reactive.Bindings.Extensions;

using NNReader.Bookmarks;


namespace NNReader.Shells.ViewModels
{
    class NovelsViewModel
    {
        public NovelsViewModel(INarouBookmarkService bookmarkService)
        {
            this.IsEmpty = bookmarkService.ObserveProperty(x => x.SelectedBookmarkId)
                .Select(x => x == Guid.Empty)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim();

            this.IsAnySelected = this.IsEmpty.Select(x => !x)
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactivePropertySlim();
        }

        public ReadOnlyReactivePropertySlim<bool> IsEmpty { get; }
        public ReadOnlyReactivePropertySlim<bool> IsAnySelected { get; }

        public ReadOnlyReactiveCollection<IBookmarkInfo> BookmarkInfos { get; }
    }
}
