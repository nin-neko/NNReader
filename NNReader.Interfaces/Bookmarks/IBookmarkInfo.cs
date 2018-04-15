using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace NNReader.Bookmarks
{
    public interface IBookmarkInfo : INotifyPropertyChanged
    {
        Guid Id { get; }
        string Ncode { get; }
        string Title { get; }
        string Writer { get; }
        DateTimeOffset BookmarkedDate { get; }
        BookmarkInfoStatus Status { get; }

        ReadOnlyObservableCollection<ILoadableChapter> Chapters { get; }
    }

    public interface ILoadableBookmarkInfo : IBookmarkInfo
    {
        Task<bool> IsSummaryLoadableAsync();
        Task<bool> IsChapterLoadableAsync();

        Task LoadSummaryAsync();
        Task DownloadSummaryAsync();

        Task LoadChapterAsync();
        Task DownloadChapterAsync();

        event EventHandler SummaryLoaded;
        event EventHandler SummaryDownloaded;

        event EventHandler ChapterLoaded;
        event EventHandler ChapterDownloaded;
    }

    public static class ILoadableBookmarkInfoExtensions
    {
        public static async Task<bool> LoadSummaryIfCanAsync(this ILoadableBookmarkInfo self)
        {
            if (!await self.IsSummaryLoadableAsync()) return false;
            await self.LoadSummaryAsync();
            return true;
        }

        public static async Task<bool> LoadChapterIfCanAsync(this ILoadableBookmarkInfo self)
        {
            if (!await self.IsChapterLoadableAsync()) return false;
            await self.LoadChapterAsync();
            return true;
        }
    }
}
