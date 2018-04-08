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
    class DownloadingBookmarkInfo : BaseOrder
    {
        public static readonly string NcodeContext = nameof(NcodeContext);

        public override async Task InvokeAsync()
        {
            var ncode = (string)this.Contexts[NcodeContext];
            var bookmarkService = this.Container.Resolve<NarouBookmarkService>();

            await bookmarkService.DownloadAsync(ncode);
        }
    }
}
