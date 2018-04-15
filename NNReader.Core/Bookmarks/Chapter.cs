using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Mvvm;


namespace NNReader.Bookmarks
{
    abstract class BaseChapter : BindableBase, IChapter
    {
        protected BaseChapter(Guid id, string ncode, int index)
        {
            if (index <= 0) throw new ArgumentOutOfRangeException(nameof(index));

            this.Id = id;
            this.Ncode = ncode;
            this.Index = index;
            this.Title = "";
            this.Content = "";
            this.Status = ChapterStatus.Created;
        }

        public Guid Id { get; }
        public string Ncode { get; }
        public int Index { get; }

        private string title;
        public string Title
        {
            get => title;
            protected set => this.SetProperty(ref title, value);
        }

        private string content;
        public string Content
        {
            get => content;
            protected set => this.SetProperty(ref content, value);
        }

        private ChapterStatus status;
        public ChapterStatus Status
        {
            get => status;
            protected set => this.SetProperty(ref status, value);
        }
    }

    abstract class BaseLoadableChapter : BaseChapter, ILoadableChapter
    {
        protected BaseLoadableChapter(Guid id, string ncode, int index)
            : base(id, ncode, index)
        {
        }

        public abstract Task<bool> IsTitleLoadableAsync();
        public abstract Task<bool> IsContentLoadableAsync();

        protected abstract Task DoTitleLoading();
        protected virtual async Task OnTitleLoadedAsync() { }

        public async Task LoadTitleAsync()
        {
            this.Status = ChapterStatus.TitleLoading;
            try
            {
                await this.DoTitleLoading();
                this.Status = ChapterStatus.TitleLoaded;
                await this.OnTitleLoadedAsync();
                this.TitleLoaded?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
            }
        }
        
        protected abstract Task DoTitleDownloading();
        protected virtual async Task OnTitleDownloadedAsync() { }

        public async Task DownloadTitleAsync()
        {
            this.Status = ChapterStatus.TitleDownloading;
            try
            {
                await this.DoTitleDownloading();
                this.Status = ChapterStatus.TitleLoaded;
                await this.OnTitleDownloadedAsync();
                this.TitleDownloaded?.Invoke(this, EventArgs.Empty);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
            }
        }
        
        protected abstract Task DoContentLoading();
        protected virtual async Task OnContentLoadedAsync() { }

        public async Task LoadContentAsync()
        {
            this.Status = ChapterStatus.ContentLoading;
            try
            {
                await this.DoContentLoading();
                this.Status = ChapterStatus.ContentLoaded;
                await this.OnContentLoadedAsync();
                this.ContentLoaded?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
            }
        }

        protected abstract Task DoContentDownloading();
        protected virtual async Task OnContentDownloadedAsync() { }

        public async Task DownloadContentAsync()
        {
            this.Status = ChapterStatus.ContentDownloading;
            try
            {
                await this.DoContentDownloading();
                this.Status = ChapterStatus.ContentLoaded;
                await this.OnContentDownloadedAsync();
                this.ContentDownloaded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
            }
        }

        public event EventHandler TitleLoaded;
        public event EventHandler TitleDownloaded;

        public event EventHandler ContentLoaded;
        public event EventHandler ContentDownloaded;
    }
}
