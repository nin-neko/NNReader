using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NNReader.Bookmarks
{
    public interface INovel
    {
        Guid Id { get; }
        int Index { get; }
        string Title { get; }
        string Content { get; }
    }
}
