using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.IO.Compression;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Prism.Mvvm;


namespace NNReader.Bookmarks
{
    abstract class BaseBookmarkService : BindableBase, IBookmarkService
    {
        private readonly ObservableCollection<ILoadableBookmarkInfo> bookmarks = new ObservableCollection<ILoadableBookmarkInfo>();

        protected BaseBookmarkService()
        {
            this.Bookmarks = new ReadOnlyObservableCollection<ILoadableBookmarkInfo>(this.bookmarks);
            this.Status = BookmarkServiceStatus.Created;
        }

        public ReadOnlyObservableCollection<ILoadableBookmarkInfo> Bookmarks { get; }

        private BookmarkServiceStatus status;
        public BookmarkServiceStatus Status
        {
            get => status;
            protected set => this.SetProperty(ref status, value);
        }

        protected void Add(ILoadableBookmarkInfo bookmarkInfo) => bookmarks.Add(bookmarkInfo);

        public void RequestBookmark(IBookmarkInfo bookmarkInfo) => this.BookmarkRequested?.Invoke(this, new BookmarkRequestEventArgs(bookmarkInfo));

        public void RequestBookmark(Guid bookmarkInfoId)
        {
            if (bookmarkInfoId == Guid.Empty)
            {
                this.RequestBookmark(null);
                return;
            }
            var requested = this.Bookmarks.Single(x => x.Id == bookmarkInfoId);
            this.RequestBookmark(requested);
        }

        public void RequestChapter(IChapter chapter) => this.ChapterRequested?.Invoke(this, new ChapterRequestEventArgs(chapter));

        public void RequestChapter(Guid chapterId)
        {
            if (chapterId == Guid.Empty)
            {
                this.RequestChapter(null);
                return;
            }
            var requested = this.Bookmarks.SelectMany(x => x.Chapters).Single(x => x.Id == chapterId);
            this.RequestChapter(requested);
        }

        public event EventHandler<BookmarkRequestEventArgs> BookmarkRequested;
        public event EventHandler<ChapterRequestEventArgs> ChapterRequested;
    }

    abstract class BaseLoadableBookmarkService : BaseBookmarkService, ILoadableBookmarkService
    {
        public BaseLoadableBookmarkService()
        {
        }

        public abstract Task<bool> IsLoadableAsync();

        protected virtual async Task OnLoadedAsync() { }
        protected abstract Task DoLoadingAsync();

        public async Task LoadAsync()
        {
            this.Status = BookmarkServiceStatus.BookmarkInfoLoading;
            try
            {
                await this.DoLoadingAsync();
                this.Status = BookmarkServiceStatus.Loaded;
                await this.OnLoadedAsync();
                this.Loaded?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                this.LoadingFailed?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
            }
        }

        protected virtual async Task OnDownloadedAsync(string ncode) { }
        protected abstract Task DoDownloadingAsync(string ncode);

        public async Task DownloadAsync(string ncode)
        {
            this.Status = BookmarkServiceStatus.BookmarkInfoDownloading;
            try
            {
                await this.DoDownloadingAsync(ncode);
                this.Status = BookmarkServiceStatus.Loaded;
                await this.OnDownloadedAsync(ncode);
                this.Downloaded?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                this.DownloadingFailed?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
            }
        }

        public event EventHandler Loaded;
        public event EventHandler LoadingFailed;
        public event EventHandler Downloaded;
        public event EventHandler DownloadingFailed;
    }
}
