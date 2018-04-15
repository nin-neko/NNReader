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
using NNReader.Ordering;


namespace NNReader.Shells.ViewModels.HomeContents
{
    class HeaderViewModels
    {
        public HeaderViewModels(AboutViewModels aboutViewModels)
        {
            this.AboutViewModels = aboutViewModels;
        }

        public AboutViewModels AboutViewModels { get; }
    }
}
