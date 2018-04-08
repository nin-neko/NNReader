using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Mvvm;


namespace NNReader.Bookmarks
{
    class Novel : BindableBase, INovel
    {
        protected Novel(Guid id, int index, string title, string content)
        {
            this.Id = id;
            this.Index = index;
            this.Title = title;
            this.Content = content;
        }

        public Guid Id { get; }
        public int Index { get; }
        public string Title { get; }

        private string content;
        public string Content
        {
            get => content;
            protected set => this.SetProperty(ref content, value);
        }
    }

    class NarouNovel : Novel
    {
        private NarouNovel(Guid id, int index, string title, string content)
            : base(id, index, title, content)
        {
        }

        public NarouNovel(string ncode, int index, string title)
            : this(Guid.NewGuid(), index, title, "")
        {
            this.Ncode = ncode;
        }

        public string Ncode { get; }

        private bool downloading;
        public bool Downloading
        {
            get => downloading;
            private set => this.SetProperty(ref downloading, value);
        }

        public async Task DownloadAsync()
        {
            this.Downloading = true;

            var url = $"{Narou.BASE_URL}{this.Ncode.ToLower()}/{this.Index + 1}/";
            try
            {
                var narou = new Narou();
                var text = await narou.ReceiveAsync(url);
                var parser = new AngleSharp.Parser.Html.HtmlParser();
                using (var html = await parser.ParseAsync(text))
                {
                    foreach (var e in html.GetElementsByClassName("novel_view"))
                    {
                        this.Content = e.TextContent;
                    }
                }
            }
            finally
            {
                this.Downloading = false;
            }
        }
    }
}
