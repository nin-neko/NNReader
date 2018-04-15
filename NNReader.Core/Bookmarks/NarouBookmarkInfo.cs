using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Prism.Mvvm;

using NNReader.Net;
using NNReader.Serialization;


namespace NNReader.Bookmarks
{
    sealed class NarouBookmarkInfo : BaseLoadableBookmarkInfo
    {
        private NarouBookmarkInfo(Guid id, string ncode)
            : base(id, ncode)
        {
        }

        public NarouBookmarkInfo(string ncode)
            : this(Guid.NewGuid(), ncode)
        {
        }

        public override async Task<bool> IsSummaryLoadableAsync()
        {
            if (this.Status != BookmarkInfoStatus.Created) return false;
            return await new BookmarkInfoDeserializer(this.Ncode).LoadableSummaryAsync();
        }

        public override async Task<bool> IsChapterLoadableAsync()
        {
            if (this.Status == BookmarkInfoStatus.ChapterLoading || this.Status == BookmarkInfoStatus.ChapterDownloading) return false;
            if (this.Status == BookmarkInfoStatus.ChapterLoaded || this.Status == BookmarkInfoStatus.AllChapterLoaded) return false;
            return await new BookmarkInfoDeserializer(this.Ncode).LoadableChapterAsync();
        }

        protected override async Task DoSummaryLoading()
        {
            var deserializer = new BookmarkInfoDeserializer(this.Ncode);
            (this.Title, this.Writer) = await deserializer.GetSummaryAsync();
        }

        protected override async Task DoSummaryDownloading()
        {
            var narou = new NarouClient();
            (this.Title, this.Writer) = await narou.GetSummaryAsync(this.Ncode);
        }

        protected override async Task OnSummaryDownloadedAsync()
        {
            await new BookmarkInfoSerializer(this)
                .WithSummary()
                .SaveAsync();
        }

        protected override async Task DoChapterLoading()
        {
            var deserializer = new BookmarkInfoDeserializer(this.Ncode);
            var chapters = await deserializer.GetChapterCountAsync();

            for (int i = 0; i < chapters; i++)
            {
                var c = new NarouChapter(this.Ncode, i + 1);
                this.Add(c);
            }
        }

        protected override async Task DoChapterDownloading()
        {
            var narou = new NarouClient();
            var count = await narou.GetChapterCountAsync(this.Ncode);

            for (int i = 0; i < count; i++)
            {
                var chapter = new NarouChapter(this.Ncode, i + 1);
                this.Add(chapter);
            }
        }

        protected override async Task OnChapterDownloadedAsync()
        {
            await new BookmarkInfoSerializer(this)
                .WithAll()
                .SaveAsync();
        }
    }
}
