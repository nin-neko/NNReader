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
    abstract class BookmarkService : BindableBase, IBookmarkService
    {
        private readonly ObservableCollection<IBookmarkInfo> bookmarks = new ObservableCollection<IBookmarkInfo>();
        private readonly ObservableCollection<IChapter> chapters = new ObservableCollection<IChapter>();

        protected BookmarkService()
        {
            this.Bookmarks = new ReadOnlyObservableCollection<IBookmarkInfo>(this.bookmarks);
            this.Chapters = new ReadOnlyObservableCollection<IChapter>(this.chapters);
            this.Status = BookmarkServiceStatus.Created;
        }

        public ReadOnlyObservableCollection<IBookmarkInfo> Bookmarks { get; }

        private Guid selectedBookmarkId;
        public Guid SelectedBookmarkId
        {
            get => selectedBookmarkId;
            private set => this.SetProperty(ref selectedBookmarkId, value);
        }

        public ReadOnlyObservableCollection<IChapter> Chapters { get; }

        private Guid selectedNovelId;
        public Guid SelectedNovelId
        {
            get => selectedNovelId;
            private set => this.SetProperty(ref selectedNovelId, value);
        }

        private BookmarkServiceStatus status;
        public BookmarkServiceStatus Status
        {
            get => status;
            protected set => this.SetProperty(ref status, value);
        }

        protected void Add(IBookmarkInfo bookmarkInfo) => bookmarks.Add(bookmarkInfo);

        protected virtual void OnChangingBookmarkSelection(IBookmarkInfo lastBookmarkInfo, IBookmarkInfo nextBookmarkInfo)
        {
        }

        protected virtual void OnChangedBookmarkSelection(IBookmarkInfo lastBookmarkInfo, IBookmarkInfo nextBookmarkInfo)
        {
        }

        public void ChangeBookmarkSelection(Guid id)
        {
            if (this.SelectedBookmarkId == id) return;

            var nextBookmark = this.Bookmarks.SingleOrDefault(x => x.Id == id);
            var lastBookmarkInfo = this.Bookmarks.SingleOrDefault(x => x.Id == this.SelectedBookmarkId);

            this.OnChangingBookmarkSelection(lastBookmarkInfo, nextBookmark);

            void OnAddChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action != NotifyCollectionChangedAction.Add) return;
                foreach (var x in e.NewItems.Cast<IChapter>())
                {
                    chapters.Add(x);
                }
            }

            void OnRemoveChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action != NotifyCollectionChangedAction.Remove) return;
                foreach (var x in e.OldItems.Cast<IChapter>())
                {
                    chapters.Remove(x);
                }
            }

            void OnClearChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action != NotifyCollectionChangedAction.Reset) return;
                chapters.Clear();
            }

            this.SelectedNovelId = Guid.Empty;
            if (lastBookmarkInfo != null)
            {
                ((INotifyCollectionChanged)lastBookmarkInfo.Chapters).CollectionChanged -= OnAddChanged;
                ((INotifyCollectionChanged)lastBookmarkInfo.Chapters).CollectionChanged -= OnRemoveChanged;
                ((INotifyCollectionChanged)lastBookmarkInfo.Chapters).CollectionChanged -= OnClearChanged;
            }
            chapters.Clear();

            this.SelectedBookmarkId = id;
            if (nextBookmark != null)
            {
                ((INotifyCollectionChanged)nextBookmark.Chapters).CollectionChanged += OnAddChanged;
                ((INotifyCollectionChanged)nextBookmark.Chapters).CollectionChanged += OnRemoveChanged;
                ((INotifyCollectionChanged)nextBookmark.Chapters).CollectionChanged += OnClearChanged;
                chapters.AddRange(nextBookmark.Chapters);
            }

            this.OnChangedBookmarkSelection(lastBookmarkInfo, nextBookmark);
        }

        protected virtual void OnChangingChapterSelection(IChapter lastNovel, IChapter nextNovel)
        {
        }

        protected virtual void OnChangedChapterSelection(IChapter lastNovel, IChapter nextNovel)
        {
        }

        public void ChangeChapterSelection(Guid id)
        {
            if (this.SelectedNovelId == id) return;

            var lastNovel = this.Chapters.SingleOrDefault(x => x.Id == this.SelectedNovelId);
            var nextNovel = this.Chapters.SingleOrDefault(x => x.Id == id);

            this.OnChangingChapterSelection(lastNovel, nextNovel);
            this.SelectedNovelId = id;
            this.OnChangedChapterSelection(lastNovel, nextNovel);
        }
    }
}
