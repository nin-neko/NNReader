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

        ReadOnlyObservableCollection<IChapter> Chapters { get; }
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
}
