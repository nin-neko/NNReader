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
        ReadOnlyObservableCollection<IBookmarkInfo> Bookmarks { get; }

        Guid SelectedBookmarkId { get; }

        ReadOnlyObservableCollection<IChapter> Chapters { get; }

        Guid SelectedNovelId { get; }

        BookmarkServiceStatus Status { get; }
    }

    public interface INarouBookmarkService : IBookmarkService
    {
        Task LoadAsync();
        Task DownloadAsync(string ncode);
        
        event EventHandler Loaded;
        event EventHandler Downloaded;
        event EventHandler DownloadingFailed;
    }
}
