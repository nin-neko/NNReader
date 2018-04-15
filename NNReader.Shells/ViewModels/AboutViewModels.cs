using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Binding;
using Reactive.Bindings.Extensions;

using NNReader.Bookmarks;
using NNReader.Ordering;
using NNReader.Diagnostics;


namespace NNReader.Shells.ViewModels
{
    class AboutViewModel
    {
        public AboutViewModel(IComponentService componentService, IDialogService dialogService)
        {
            this.Version = componentService.Version;
            this.Components = componentService.Components.Select(x => new ComponentViewModel(x)).ToArray();

            this.CloseCommand.Subscribe(() =>
            {
                dialogService.IsOpen = false;
            });
        }

        public string Version { get; }
        public ComponentViewModel[] Components { get; }

        public ReactiveCommand CloseCommand { get; } = new ReactiveCommand();
    }

    class ComponentViewModel
    {
        public ComponentViewModel(IComponent component)
        {
            this.Name = component.Name;
            this.ProjectUrl = component.ProjectUrl;
            this.License = component.License;
            this.LicenseUrl = component.LicenseUrl;

            this.LinkClickCommand.Subscribe(x =>
            {
                Process.Start(x.AbsoluteUri);
            });
        }

        public string Name { get; }
        public string ProjectUrl { get; }
        public string License { get; }
        public string LicenseUrl { get; }

        public ReactiveCommand<Uri> LinkClickCommand { get; } = new ReactiveCommand<Uri>();
    }
}
