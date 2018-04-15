using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO.Compression;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Prism.Mvvm;
using Prism.Ioc;

using NNReader.Ordering;


namespace NNReader.Bookmarks
{
    class RequestingBookmark : BaseOrder
    {
        public static readonly string IdContext = nameof(IdContext);

        public override async Task InvokeAsync()
        {
            var id = (Guid)this.Contexts[IdContext];
            var bookmarkService = this.Container.Resolve<BaseBookmarkService>();

            bookmarkService.RequestBookmark(id);
        }
    }
}
