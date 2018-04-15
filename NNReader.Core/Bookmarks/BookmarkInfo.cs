using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Mvvm;


namespace NNReader.Bookmarks
{
    abstract class BaseBookmarkInfo : BindableBase, IBookmarkInfo
    {
        private readonly ObservableCollection<ILoadableChapter> chapters = new ObservableCollection<ILoadableChapter>();

        protected BaseBookmarkInfo(Guid id, string ncode)
        {
            this.Id = id;
            this.Ncode = ncode;
            this.Title = "";
            this.Writer = "";
            this.BookmarkedDate = DateTimeOffset.Now;
            this.Status = BookmarkInfoStatus.Created;

            this.Chapters = new ReadOnlyObservableCollection<ILoadableChapter>(this.chapters);
        }

        public Guid Id { get; }
        public string Ncode { get; }

        private string title;
        public string Title
        {
            get => title;
            protected set => this.SetProperty(ref title, value);
        }

        private string writer;
        public string Writer
        {
            get => writer;
            protected set => this.SetProperty(ref writer, value);
        }

        private DateTimeOffset bookmarkedDate;
        public DateTimeOffset BookmarkedDate
        {
            get => bookmarkedDate;
            protected set => this.SetProperty(ref bookmarkedDate, value);
        }

        private BookmarkInfoStatus status;
        public BookmarkInfoStatus Status
        {
            get => status;
            protected set => this.SetProperty(ref status, value);
        }

        public ReadOnlyObservableCollection<ILoadableChapter> Chapters { get; }

        protected void Add(ILoadableChapter novel) => chapters.Add(novel);
    }

    abstract class BaseLoadableBookmarkInfo : BaseBookmarkInfo, ILoadableBookmarkInfo
    {
        protected BaseLoadableBookmarkInfo(Guid id, string ncode)
            : base(id, ncode)
        {
        }

        public abstract Task<bool> IsSummaryLoadableAsync();
        public abstract Task<bool> IsChapterLoadableAsync();

        protected virtual async Task OnSummaryLoadedAsync() { }
        protected abstract Task DoSummaryLoading();

        public async Task LoadSummaryAsync()
        {
            this.Status = BookmarkInfoStatus.SummaryLoading;
            try
            {
                await this.DoSummaryLoading();
                this.Status = BookmarkInfoStatus.SummaryLoaded;
                await this.OnSummaryLoadedAsync();
                this.SummaryLoaded?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
            }
        }

        protected virtual async Task OnSummaryDownloadedAsync() { }
        protected abstract Task DoSummaryDownloading();

        public async Task DownloadSummaryAsync()
        {
            this.Status = BookmarkInfoStatus.SummaryDownloading;
            try
            {
                await this.DoSummaryDownloading();
                this.Status = BookmarkInfoStatus.SummaryLoaded;
                await this.OnSummaryDownloadedAsync();
                this.SummaryDownloaded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
            }
        }

        protected virtual async Task OnChapterLoadedAsync() { }
        protected abstract Task DoChapterLoading();

        public async Task LoadChapterAsync()
        {
            this.Status = BookmarkInfoStatus.ChapterLoading;
            try
            {
                await this.DoChapterLoading();
                this.Status = BookmarkInfoStatus.ChapterLoaded;
                await this.OnChapterLoadedAsync();
                this.ChapterLoaded?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                
            }
        }

        protected virtual async Task OnChapterDownloadedAsync() { }
        protected abstract Task DoChapterDownloading();

        public async Task DownloadChapterAsync()
        {
            this.Status = BookmarkInfoStatus.ChapterDownloading;
            try
            {
                await this.DoChapterDownloading();
                this.Status = BookmarkInfoStatus.ChapterLoaded;
                await this.OnChapterDownloadedAsync();
                this.ChapterDownloaded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {

            }
        }

        public event EventHandler SummaryLoaded;
        public event EventHandler SummaryDownloaded;

        public event EventHandler ChapterLoaded;
        public event EventHandler ChapterDownloaded;
    }
}
