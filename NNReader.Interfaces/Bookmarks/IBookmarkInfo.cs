using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NNReader.Bookmarks
{
    public interface IBookmarkInfo
    {
        Guid Id { get; }
        string Ncode { get; }
        string Title { get; }
        string Author { get; }
        DateTime BookmarkedDate { get; }
    }
}
