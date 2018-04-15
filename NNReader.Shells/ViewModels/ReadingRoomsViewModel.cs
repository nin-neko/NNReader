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

using MaterialDesignThemes.Wpf.Transitions;

using NNReader.Bookmarks;
using NNReader.Ordering;


namespace NNReader.Shells.ViewModels
{
    class ReadingRoomsViewModel
    {
        public ReadingRoomsViewModel(IOrderBuilder orderBuilder, ILoadableBookmarkService loadableBookmarkService)
        {
            this.Bookmarks = loadableBookmarkService.Bookmarks.ToReadOnlyReactiveCollection(x => new ReadingRoomViewModel(orderBuilder, loadableBookmarkService, x));
        }

        public ReadOnlyReactiveCollection<ReadingRoomViewModel> Bookmarks { get; }
    }
}
