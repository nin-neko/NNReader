using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace NNReader.Bookmarks
{
    public interface IChapter : INotifyPropertyChanged
    {
        Guid Id { get; }
        string Ncode { get; }
        int Index { get; }
        string Title { get; }
        string Content { get; }
        ChapterStatus Status { get; }
    }

    public interface ILoadableChapter : IChapter
    {
        Task<bool> IsTitleLoadableAsync();
        Task<bool> IsContentLoadableAsync();

        Task LoadTitleAsync();
        Task DownloadTitleAsync();

        Task LoadContentAsync();
        Task DownloadContentAsync();

        event EventHandler TitleLoaded;
        event EventHandler TitleDownloaded;

        event EventHandler ContentLoaded;
        event EventHandler ContentDownloaded;
    }

    public static class ILoadableChapterExtensions
    {
        public static async Task<bool> LoadTitleIfCanAsync(this ILoadableChapter self)
        {
            if (self.Status == ChapterStatus.TitleLoaded) return false;
            if (!await self.IsTitleLoadableAsync()) return false;
            await self.LoadTitleAsync();
            return true;
        }

        public static async Task<bool> LoadContentIfCanAsync(this ILoadableChapter self)
        {
            if (self.Status == ChapterStatus.ContentLoaded) return false;
            if (!await self.IsContentLoadableAsync()) return false;
            await self.LoadContentAsync();
            return true;
        }
    }
}
