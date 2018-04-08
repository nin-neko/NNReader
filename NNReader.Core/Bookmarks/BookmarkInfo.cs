using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Mvvm;


namespace NNReader.Bookmarks
{
    class BookmarkInfo : BindableBase, IBookmarkInfo
    {
        private readonly ObservableCollection<INovel> novels = new ObservableCollection<INovel>();

        protected BookmarkInfo(Guid id, string ncode, string title, string author)
        {
            this.Id = id;
            this.Ncode = ncode;
            this.Title = title;
            this.Author = author;
            this.BookmarkedDate = DateTime.Now;

            this.Novels = new ReadOnlyObservableCollection<INovel>(this.novels);
        }

        public Guid Id { get; }
        public string Ncode { get; }
        public string Title { get; }
        public string Author { get; }
        public DateTime BookmarkedDate { get; }

        public ReadOnlyObservableCollection<INovel> Novels { get; }

        protected void Add(INovel novel) => novels.Add(novel);
        protected NarouNovel Add(string title)
        {
            var nextIndex = this.Novels.Count;
            var novel = new NarouNovel(this.Ncode, nextIndex, title);
            this.Add(novel);
            return novel;
        }
    }

    class NarouBookmarkInfo : BookmarkInfo
    {
        public NarouBookmarkInfo(Guid id, string ncode, string title, string author)
            : base(id, ncode, title, author)
        {
        }

        public NarouBookmarkInfo(string ncode, string title, string author)
            : this(Guid.NewGuid(), ncode, title, author)
        {
        }

        public bool Loaded { get; private set; }

        private bool downloading;
        public bool Downloading
        {
            get => downloading;
            private set => this.SetProperty(ref downloading, value);
        }

        public async Task DownloadAsync()
        {
            this.Loaded = false;
            this.Downloading = true;

            var url = $"{Narou.BASE_URL}{this.Ncode.ToLower()}/";
            try
            {
                var narou = new Narou();
                var text = await narou.ReceiveAsync(url);
                var parser = new AngleSharp.Parser.Html.HtmlParser();
                using (var html = await parser.ParseAsync(text))
                {
                    foreach (var e in html.QuerySelectorAll("dd > a"))
                    {
                        var novel = this.Add(e.TextContent);
                        //await novel.Download();
                    }
                }
                this.Loaded = true;
            }
            finally
            {
                this.Downloading = false;
            }
        }

        public async Task DownloadAllNovelContentAsync()
        {
            this.Downloading = true;

            try
            {
                var tasks = this.Novels.Cast<NarouNovel>().Select(x => x.DownloadAsync()).ToArray();
                await Task.WhenAll(tasks);
            }
            finally
            {
                this.Downloading = false;
            }
        }
    }
}
