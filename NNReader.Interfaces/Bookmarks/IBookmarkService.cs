using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace NNReader.Bookmarks
{
    public interface IBookmarkService : INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<ILoadableBookmarkInfo> Bookmarks { get; }

        BookmarkServiceStatus Status { get; }

        event EventHandler<BookmarkRequestEventArgs> BookmarkRequested;
        event EventHandler<ChapterRequestEventArgs> ChapterRequested;
    }

    public class BookmarkRequestEventArgs : EventArgs
    {
        public BookmarkRequestEventArgs(IBookmarkInfo bookmarkInfo)
            => this.Bookmark = bookmarkInfo;

        public IBookmarkInfo Bookmark { get; }
    }

    public class ChapterRequestEventArgs : EventArgs
    {
        public ChapterRequestEventArgs(IChapter chapter)
            => this.Chapter = chapter;

        public IChapter Chapter { get; }
    }

    public interface ILoadableBookmarkService : IBookmarkService
    {
        Task<bool> IsLoadableAsync();

        Task LoadAsync();
        Task DownloadAsync(string ncode);
        
        event EventHandler Loaded;
        event EventHandler LoadingFailed;
        event EventHandler Downloaded;
        event EventHandler DownloadingFailed;
    }

    public static class ILoadableBookmarkServiceExtensions
    {
        public static async Task LoadIfCanAsync(this ILoadableBookmarkService self)
        {
            if (self.Status == BookmarkServiceStatus.Loaded) return;
            if (!await self.IsLoadableAsync()) return;
            await self.LoadAsync();
        }
    }
}
