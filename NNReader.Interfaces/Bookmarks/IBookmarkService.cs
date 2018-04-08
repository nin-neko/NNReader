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

        ReadOnlyObservableCollection<INovel> Novels { get; }

        Guid SelectedNovelId { get; }
    }

    public interface INarouBookmarkService : IBookmarkService
    {
        bool Downloading { get; }

        Task DownloadAsync(string ncode);

        event EventHandler Downloaded;
        event EventHandler Failed;
    }
}
