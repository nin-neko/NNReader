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

using NNReader.Bookmarks;


namespace NNReader.Shells.ViewModels
{
    class ShellViewModel
    {
        public ShellViewModel(IContainerProvider container)
        {
            this.ContentRenderedCommand.Subscribe(async () =>
            {
                var builder = container.Resolve<Ordering.IOrderBuilder>();
                await builder.From("LoadingBookmarkService").DispatchAsync();
            });
        }

        public AsyncReactiveCommand ContentRenderedCommand { get; } = new AsyncReactiveCommand();
    }
}
