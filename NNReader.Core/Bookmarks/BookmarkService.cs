using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public BookmarkService()
        {
            this.Add("1111aaaa", "さすおに", "sume");
            this.Add("fdasfadfasf", "あｆｓふぁ", "5ｆ4あ65");
            this.Bookmarks = new ReadOnlyObservableCollection<IBookmarkInfo>(this.bookmarks);
        }

        public ReadOnlyObservableCollection<IBookmarkInfo> Bookmarks { get; }

        private Guid selectedBookmarkId;
        public Guid SelectedBookmarkId
        {
            get => selectedBookmarkId;
            set => this.SetProperty(ref selectedBookmarkId, value);
        }

        protected void Add(string ncode, string title, string author)
        {
            bookmarks.Add(new BookmarkInfo(ncode, title, author));
        }
    }

    class NarouBookmarkService : BookmarkService, INarouBookmarkService
    {
        private const string BASE_API_URL = "https://api.syosetu.com/novelapi/api/?out=json&gzip=5";
        private static readonly UTF8Encoding encoding = new UTF8Encoding(false);
        private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler() { UseCookies = false, });

        public NarouBookmarkService()
        {
        }

        private async Task<byte[]> ReceiveBytesAsync(string url)
        {
            using (var stream = await httpClient.GetStreamAsync(url))
            using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress, false))
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

        private async Task<JArray> ReceiveJTokenAsync(string url)
        {
            var json = await this.ReceiveJsonAsync(url);
            return (JArray)JToken.Parse(json);
        }

        public async Task AddAsync(string ncode)
        {
            var url = $"{BASE_API_URL}&ncode={ncode.ToLower()}";
            try
            {
                var jtoken = await this.ReceiveJTokenAsync(url);

                var targetJToken = jtoken[1];

                var title = targetJToken["title"]?.ToString() ?? "";
                var author = targetJToken["author"]?.ToString() ?? "";
                var c = targetJToken["ncode"]?.ToString() ?? "";

                this.Add(c, title, author);
            }
            catch (Exception)
            {
                
            }            
        }
    }
}
