using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NNReader.Bookmarks;


namespace NNReader.Debugging
{
    class MoqChapter : BaseLoadableChapter
    {
        private static readonly Random random = new Random();
        private static TimeSpan GetRandomDelay(double max = 1) => TimeSpan.FromSeconds(max * random.NextDouble());

        public MoqChapter(string ncode, int index)
            : base(Guid.NewGuid(), ncode, index)
        {

        }

        public override async Task<bool> IsContentLoadableAsync()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);
            return true;
        }

        public override async Task<bool> IsTitleLoadableAsync()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);
            return true;
        }

        protected override async Task DoContentDownloading()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);

            this.Content = $"This is content...: {delay}";
        }

        protected override async Task DoContentLoading()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);

            this.Content = $"This is content...: {delay}";
        }

        protected override async Task DoTitleDownloading()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);

            this.Title = $"Chapter {this.Index}: {delay}";
        }

        protected override async Task DoTitleLoading()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);

            this.Title = $"Chapter {this.Index}: {delay}";

            //await this.LoadContentIfCanAsync();
        }
    }
}
