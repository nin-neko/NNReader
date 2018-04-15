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
    class LoadingChapterTitle : BaseOrder
    {
        public static readonly string IdContext = nameof(IdContext);

        public override async Task InvokeAsync()
        {
            var id = (Guid)this.Contexts[IdContext];
            var bookmarkService = this.Container.Resolve<ILoadableBookmarkService>();

            var chapter = bookmarkService.Bookmarks.SelectMany(x => x.Chapters).Single(x => x.Id == id);

            if (chapter.Status == ChapterStatus.TitleLoaded) return;

            var can = await chapter.LoadTitleIfCanAsync();
            if (!can) await chapter.DownloadTitleAsync();
        }
    }
}
