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

using AngleSharp.Parser.Html;

using Prism.Mvvm;


namespace NNReader.Net
{
    class NarouClient : BaseNarouClient
    {
        public NarouClient()
        {
        }

        public async Task<bool> ExistNcodeAsync(string ncode)
        {
            var url = new NarouApiUrlBuilder().WithLimit(1).WithNCode(ncode).ToString();
            var jtoken = await this.ReceiveJTokenAsync(url);

            var targetJTokens = jtoken.Where(x =>
            {
                var ncodeToken = x["ncode"];
                if (ncodeToken == null) return false;
                return ncodeToken.ToString().ToLower() == ncode.ToLower();
            })
            .ToArray();

            if (!targetJTokens.Any() || targetJTokens.Length > 1) return false;

            var targetJToken = targetJTokens[0];

            var actualNcode = targetJToken["ncode"]?.ToString()?.ToLower() ?? "";

            return actualNcode == ncode;
        }

        public async Task<(string Title, string Writer)> GetSummaryAsync(string ncode)
        {
            var url = new NarouUrlBuilder().WithNCode(ncode).ToString();
            var text = await this.ReceiveAsync(url);

            var parser = new HtmlParser();
            using (var html = await parser.ParseAsync(text))
            {
                var title = html.GetElementsByClassName("novel_title").First();
                var writer = html.GetElementsByClassName("novel_writername").First().FirstElementChild;

                return (title.TextContent, writer.TextContent);
            }
        }

        public async Task<int> GetChapterCountAsync(string ncode)
        {
            var url = new NarouUrlBuilder().WithNCode(ncode).ToString();
            var text = await this.ReceiveAsync(url);

            var parser = new HtmlParser();
            using (var html = await parser.ParseAsync(text))
            {
                return html.QuerySelectorAll("dd > a").Length;
            }
        }

        public async Task<string> GetChapterTitleAsync(string ncode, int index)
        {
            var url = new NarouUrlBuilder().WithNCode(ncode).WithIndex(index).ToString();
            var text = await this.ReceiveAsync(url);

            var parser = new HtmlParser();
            using (var html = await parser.ParseAsync(text))
            {
                var title = html.GetElementsByClassName("novel_subtitle").First();
                return title.TextContent;
            }
        }

        public async Task<string> GetChapterContentAsync(string ncode, int index)
        {
            var url = new NarouUrlBuilder().WithNCode(ncode).WithIndex(index).ToString();
            var text = await this.ReceiveAsync(url);

            var parser = new HtmlParser();
            using (var html = await parser.ParseAsync(text))
            {
                var contentBuilder = new StringBuilder();
                foreach (var e in html.GetElementsByClassName("novel_view"))
                {
                    contentBuilder.AppendLine(e.TextContent);
                }
                return contentBuilder.ToString();
            }
        }
    }
}
