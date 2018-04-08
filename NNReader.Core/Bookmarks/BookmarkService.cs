using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO.Compression;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Prism.Mvvm;


namespace NNReader.Bookmarks
{
    class BookmarkService : BindableBase, IBookmarkService
    {
        private readonly ObservableCollection<IBookmarkInfo> bookmarks = new ObservableCollection<IBookmarkInfo>();
        private readonly ObservableCollection<INovel> novels = new ObservableCollection<INovel>();

        public BookmarkService()
        {
            this.Bookmarks = new ReadOnlyObservableCollection<IBookmarkInfo>(this.bookmarks);
            this.Novels = new ReadOnlyObservableCollection<INovel>(this.novels);
        }

        public ReadOnlyObservableCollection<IBookmarkInfo> Bookmarks { get; }

        private Guid selectedBookmarkId;
        public Guid SelectedBookmarkId
        {
            get => selectedBookmarkId;
            private set => this.SetProperty(ref selectedBookmarkId, value);
        }

        public ReadOnlyObservableCollection<INovel> Novels { get; }

        private Guid selectedNovelId;
        public Guid SelectedNovelId
        {
            get => selectedNovelId;
            private set => this.SetProperty(ref selectedNovelId, value);
        }

        protected void Add(IBookmarkInfo bookmarkInfo) => bookmarks.Add(bookmarkInfo);

        protected NarouBookmarkInfo Add(string ncode, string title, string author)
        {
            var bookmark = new NarouBookmarkInfo(ncode, title, author);
            this.Add(bookmark);
            return bookmark;
        }

        protected virtual void OnChangingBookmarkSelection(IBookmarkInfo lastBookmarkInfo, IBookmarkInfo nextBookmarkInfo)
        {
        }

        protected virtual void OnChangedBookmarkSelection(IBookmarkInfo lastBookmarkInfo, IBookmarkInfo nextBookmarkInfo)
        {
        }

        public void ChangeBookmarkSelection(Guid id)
        {
            if (this.SelectedBookmarkId == id) return;

            var nextBookmark = this.Bookmarks.Single(x => x.Id == id);
            var lastBookmarkInfo = this.Bookmarks.SingleOrDefault(x => x.Id == this.SelectedBookmarkId);

            this.OnChangingBookmarkSelection(lastBookmarkInfo, nextBookmark);

            void OnAddChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action != NotifyCollectionChangedAction.Add) return;
                foreach (var x in e.NewItems.Cast<INovel>())
                {
                    novels.Add(x);
                }
            }

            void OnRemoveChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action != NotifyCollectionChangedAction.Remove) return;
                foreach (var x in e.OldItems.Cast<INovel>())
                {
                    novels.Remove(x);
                }
            }

            void OnClearChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action != NotifyCollectionChangedAction.Reset) return;
                novels.Clear();
            }

            this.SelectedNovelId = Guid.Empty;
            ((INotifyCollectionChanged)nextBookmark.Novels).CollectionChanged -= OnAddChanged;
            ((INotifyCollectionChanged)nextBookmark.Novels).CollectionChanged -= OnRemoveChanged;
            ((INotifyCollectionChanged)nextBookmark.Novels).CollectionChanged -= OnClearChanged;
            novels.Clear();

            this.SelectedBookmarkId = id;
            ((INotifyCollectionChanged)nextBookmark.Novels).CollectionChanged += OnAddChanged;
            ((INotifyCollectionChanged)nextBookmark.Novels).CollectionChanged += OnRemoveChanged;
            ((INotifyCollectionChanged)nextBookmark.Novels).CollectionChanged += OnClearChanged;
            novels.AddRange(nextBookmark.Novels);

            this.OnChangedBookmarkSelection(lastBookmarkInfo, nextBookmark);
        }

        public void ChangeNovelSelection(Guid id)
        {
            this.SelectedNovelId = id;
        }
    }

    class NarouBookmarkService : BookmarkService, INarouBookmarkService
    {
        public NarouBookmarkService()
        {
        }

        public bool Loaded { get; private set; }

        private bool downloading;
        public bool Downloading
        {
            get => downloading;
            private set => this.SetProperty(ref downloading, value);
        }

        protected override void OnChangedBookmarkSelection(IBookmarkInfo lastBookmarkInfo, IBookmarkInfo nextBookmarkInfo)
        {
            if (nextBookmarkInfo != null && nextBookmarkInfo is NarouBookmarkInfo bookmarkInfo)
            {
                Task.Run(async () => await bookmarkInfo.DownloadAsync());
            }
        }

        public async Task DownloadAsync(string ncode)
        {
            this.Loaded = false;
            this.Downloading = true;

            var url = $"{Narou.BASE_API_URL}&ncode={ncode.ToLower()}";
            try
            {
                var narou = new Narou();
                var jtoken = await narou.ReceiveJTokenAsync(url);

                var targetJTokens = jtoken.Where(x =>
                {
                    var ncodeToken = x["ncode"];
                    if (ncodeToken == null) return false;
                    return ncodeToken.ToString().ToLower() == ncode;
                })
                .ToArray();

                if (!targetJTokens.Any() || targetJTokens.Length > 1) return;

                var targetJToken = targetJTokens[0];

                var title = targetJToken["title"]?.ToString() ?? "";
                var author = targetJToken["writer"]?.ToString() ?? "";
                var c = targetJToken["ncode"]?.ToString() ?? "";

                var bookmark = this.Add(c, title, author);
                this.Loaded = true;

                this.Downloaded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception)
            {
                this.Failed?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                this.Downloading = false;
            }
        }

        public event EventHandler Downloaded;
        public event EventHandler Failed;
    }

    class Narou : BindableBase
    {
        public static readonly string BASE_API_URL = "https://api.syosetu.com/novelapi/api/?out=json&gzip=5";
        public static readonly string BASE_URL = "https://ncode.syosetu.com/";
        private static readonly UTF8Encoding encoding = new UTF8Encoding(false);
        private static readonly HttpClient httpApiClient = new HttpClient();
        private static readonly HttpClient httpTextClient = new HttpClient();

        public Narou()
        {

        }

        private bool downloading;
        public bool Downloading
        {
            get => downloading;
            private set => this.SetProperty(ref downloading, value);
        }

        private async Task<byte[]> ReceiveBytesAsync(string url)
        {
            using (var stream = await httpApiClient.GetStreamAsync(url))
            using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                var receivedBytes = new List<byte>();
                var buffer = new byte[1024];
                while (true)
                {
                    var actual = await gzipStream.ReadAsync(buffer, 0, buffer.Length);
                    if (actual == 0) break;
                    if (actual == buffer.Length)
                    {
                        receivedBytes.AddRange(buffer);
                    }
                    else
                    {
                        receivedBytes.AddRange(buffer.Take(actual));
                    }
                }
                return receivedBytes.ToArray();
            }
        }

        private async Task<string> ReceiveJsonAsync(string url)
        {
            var bin = await this.ReceiveBytesAsync(url);
            return encoding.GetString(bin);
        }

        public async Task<JArray> ReceiveJTokenAsync(string url)
        {
            var json = await this.ReceiveJsonAsync(url);
            return (JArray)JToken.Parse(json);
        }

        public async Task<string> ReceiveAsync(string url)
        {
            return await httpTextClient.GetStringAsync(url);
        }
    }
}
