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
                var dialogService = container.Resolve<IDialogService>();
                dialogService.ObserveProperty(x => x.IsOpen)
                    .ObserveOnUIDispatcher()
                    .Subscribe(x => this.IsInfoOpen.Value = x);

                var builder = container.Resolve<Ordering.IOrderBuilder>();
                await builder.From("LoadingBookmarkService").DispatchAsync();
            });

            this.InfoClickCommand.Subscribe(async () =>
            {
                var dialogService = container.Resolve<IDialogService>();
                dialogService.IsOpen = true;
            });
        }

        public AsyncReactiveCommand ContentRenderedCommand { get; } = new AsyncReactiveCommand();

        public ReactivePropertySlim<bool> IsInfoOpen { get; } = new ReactivePropertySlim<bool>();
        public AsyncReactiveCommand InfoClickCommand { get; } = new AsyncReactiveCommand();
    }
}
