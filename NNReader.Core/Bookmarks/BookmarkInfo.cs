using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NNReader.Bookmarks
{
    class BookmarkInfo : IBookmarkInfo
    {
        private BookmarkInfo(Guid id, string ncode, string title, string author)
        {
            this.Id = id;
            this.Ncode = ncode;
            this.Title = title;
            this.Author = author;
            this.BookmarkedDate = DateTime.Now;
        }

        public BookmarkInfo(string ncode, string title, string author)
            : this(Guid.NewGuid(), ncode, title, author)
        {
        }

        public Guid Id { get; }
        public string Ncode { get; }
        public string Title { get; }
        public string Author { get; }
        public DateTime BookmarkedDate { get; }
    }
}
