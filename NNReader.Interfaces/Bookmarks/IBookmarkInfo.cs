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
        string Author { get; }
        DateTime BookmarkedDate { get; }

        ReadOnlyObservableCollection<INovel> Novels { get; }
    }
}
