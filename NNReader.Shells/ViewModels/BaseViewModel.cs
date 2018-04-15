using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;

using Prism.Ioc;

using Reactive.Bindings;
using Reactive.Bindings.Binding;
using Reactive.Bindings.Extensions;

using NNReader.Bookmarks;


namespace NNReader.Shells.ViewModels
{
    abstract class BaseViewModel : IDisposable
    {
        protected CompositeDisposable CompositeDisposable { get; } = new CompositeDisposable();

        public void Dispose() => this.CompositeDisposable.Dispose();
    }
}
