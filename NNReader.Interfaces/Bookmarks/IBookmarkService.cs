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
    }

    public interface INarouBookmarkService : IBookmarkService
    {
        Task AddAsync(string ncode);
    }
}
