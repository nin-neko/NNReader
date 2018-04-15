using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NNReader.Bookmarks;


namespace NNReader.Serialization
{
    class BookmarkInfoSerializer : BaseSerializer, ISerializer
    {
        private readonly BaseBookmarkInfo bookmarkInfo;
        private readonly JObject serialized = new JObject();

        public BookmarkInfoSerializer(BaseBookmarkInfo bookmarkInfo)
        {
            this.bookmarkInfo = bookmarkInfo;
        }

        public BookmarkInfoSerializer WithNcode()
        {
            serialized[nameof(IBookmarkInfo.Ncode)] = bookmarkInfo.Ncode;
            return this;
        }

        public BookmarkInfoSerializer WithTitle()
        {
            serialized[nameof(IBookmarkInfo.Title)] = bookmarkInfo.Title;
            return this;
        }

        public BookmarkInfoSerializer WithWriter()
        {
            serialized[nameof(IBookmarkInfo.Writer)] = bookmarkInfo.Writer;
            return this;
        }

        public BookmarkInfoSerializer WithBookmarkedDate()
        {
            serialized[nameof(IBookmarkInfo.BookmarkedDate)] = bookmarkInfo.BookmarkedDate;
            return this;
        }

        public BookmarkInfoSerializer WithSummary() => this.WithNcode().WithTitle().WithWriter().WithBookmarkedDate();

        public BookmarkInfoSerializer WithChapters()
        {
            serialized[nameof(IBookmarkInfo.Chapters)] = bookmarkInfo.Chapters.Count;
            return this;
        }

        public BookmarkInfoSerializer WithAll() => this.WithSummary().WithChapters();

        public async Task SaveAsync()
        {
            var fileName = $"{bookmarkInfo.Ncode}.json";
            var locator = IO.Locator.Default;
            var di = Directory.CreateDirectory($"{bookmarkInfo.Ncode}");
            var filePath = Path.Combine(locator.BootstrapperDirectory, di.Name, fileName);

            await this.WriteAsync(filePath, serialized.ToString());
        }
    }

    class BookmarkInfoDeserializer : BaseDeserializer
    {
        private readonly string ncode;

        public BookmarkInfoDeserializer(string ncode)
        {
            this.ncode = ncode;
        }

        private string BuildFilePath()
        {
            var fileName = $"{ncode}.json";
            var locator = IO.Locator.Default;
            var filePath = Path.Combine(locator.BootstrapperDirectory, ncode, fileName);
            return filePath;
        }

        public async Task<bool> LoadableSummaryAsync()
        {
            var filePath = this.BuildFilePath();

            if (!File.Exists(filePath)) return false;

            try
            {
                var jtoken = await ReadJTokenAsync(filePath);
                var title = jtoken[nameof(IBookmarkInfo.Title)];
                var writer = jtoken[nameof(IBookmarkInfo.Writer)];
                var bookmarkedDate = jtoken[nameof(IBookmarkInfo.BookmarkedDate)];
                if (title == null || writer == null || bookmarkedDate == null) return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<(string Title, string Writer, DateTimeOffset BookmarkedDate)> GetSummaryAsync()
        {
            var filePath = this.BuildFilePath();

            var jtoken = await ReadJTokenAsync(filePath);
            var title = jtoken[nameof(IBookmarkInfo.Title)].ToString();
            var writer = jtoken[nameof(IBookmarkInfo.Writer)].ToString();
            var bookmarkedDate = jtoken[nameof(IBookmarkInfo.BookmarkedDate)].ToObject<DateTimeOffset>();

            return (title, writer, bookmarkedDate);
        }

        public async Task<bool> LoadableChapterAsync()
        {
            var filePath = this.BuildFilePath();

            if (!File.Exists(filePath)) return false;

            try
            {
                var jtoken = await ReadJTokenAsync(filePath);
                var title = jtoken[nameof(IBookmarkInfo.Title)];
                var writer = jtoken[nameof(IBookmarkInfo.Writer)];
                var bookmarkedDate = jtoken[nameof(IBookmarkInfo.BookmarkedDate)];
                var chapters = jtoken[nameof(IBookmarkInfo.Chapters)];
                if (title == null || writer == null || bookmarkedDate == null || chapters == null) return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<int> GetChapterCountAsync()
        {
            var filePath = this.BuildFilePath();

            var jtoken = await ReadJTokenAsync(filePath);
            var chapters = jtoken[nameof(IBookmarkInfo.Chapters)].ToObject<int>();

            return chapters;
        }
    }
}
