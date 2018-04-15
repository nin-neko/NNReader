using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NNReader.Bookmarks;


namespace NNReader.Debugging
{
    class MoqBookmarkInfo : BaseLoadableBookmarkInfo
    {
        private static readonly Random random = new Random();
        private static TimeSpan GetRandomDelay(double max = 1) => TimeSpan.FromSeconds(max * random.NextDouble());

        public MoqBookmarkInfo(string ncode)
            : base(Guid.NewGuid(), ncode)
        {

        }

        public override async Task<bool> IsChapterLoadableAsync()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);
            return true;
        }

        public override async Task<bool> IsSummaryLoadableAsync()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);
            return true;
        }

        protected override async Task DoChapterDownloading()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);

            var chapters = Enumerable.Range(1, random.Next(1, 10)).Select(x => new MoqChapter(this.Ncode, x));
            foreach (var x in chapters)
            {
                delay = GetRandomDelay();
                await Task.Delay(delay);
                this.Add(x);
            }
        }

        protected override async Task DoChapterLoading()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);

            var chapters = Enumerable.Range(1, random.Next(1, 10)).Select(x => new MoqChapter(this.Ncode, x));
            foreach (var x in chapters)
            {
                delay = GetRandomDelay();
                await Task.Delay(delay);
                this.Add(x);
            }            
            
            //var tasks = this.Chapters.Select(x => x.LoadTitleIfCanAsync());
            //await Task.WhenAll(tasks);
        }

        protected override async Task DoSummaryDownloading()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);

            this.Title = $"Novel Title {this.Ncode}: {delay}";
            this.Writer = this.Ncode;
        }

        protected override async Task DoSummaryLoading()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);

            this.Title = $"Novel Title {this.Ncode}: {delay}";
            this.Writer = this.Ncode;

            //await this.LoadChapterIfCanAsync();
        }
    }
}
