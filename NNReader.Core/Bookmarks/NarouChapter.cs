using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Prism.Mvvm;

using NNReader.Net;
using NNReader.Serialization;


namespace NNReader.Bookmarks
{
    sealed class NarouChapter : BaseLoadableChapter
    {
        private NarouChapter(Guid id, string ncode, int index)
            : base(id, ncode, index)
        {
        }

        public NarouChapter(string ncode, int index)
            : this(Guid.NewGuid(), ncode, index)
        {
        }

        public override async Task<bool> IsTitleLoadableAsync()
        {
            if (this.Status != ChapterStatus.Created) return false;
            return await new ChapterDeserializer(this.Ncode, this.Index).LoadableTitleAsync();
        }

        public override async Task<bool> IsContentLoadableAsync()
        {
            if (this.Status == ChapterStatus.ContentLoading || this.Status == ChapterStatus.ContentDownloading) return false;
            if (this.Status == ChapterStatus.ContentLoaded) return false;
            return await new ChapterDeserializer(this.Ncode, this.Index).LoadableContentAsync();
        }

        protected override async Task DoTitleLoading()
        {
            var deserializer = new ChapterDeserializer(this.Ncode, this.Index);
            this.Title = await deserializer.GetTitleAsync();
        }

        protected override async Task DoTitleDownloading()
        {
            var narou = new NarouClient();
            this.Title = await narou.GetChapterTitleAsync(this.Ncode, this.Index);
        }

        protected override async Task OnTitleDownloadedAsync()
        {
            await new ChapterSerializer(this).WithTitle().SaveAsync();
        }

        protected override async Task DoContentLoading()
        {
            var deserializer = new ChapterDeserializer(this.Ncode, this.Index);
            this.Content = await deserializer.GetContentAsync();
        }

        protected override async Task DoContentDownloading()
        {
            var narou = new NarouClient();
            this.Content = await narou.GetChapterContentAsync(this.Ncode, this.Index);
        }

        protected override async Task OnContentDownloadedAsync()
        {
            await new ChapterSerializer(this).WithAll().SaveAsync();
        }
    }
}
