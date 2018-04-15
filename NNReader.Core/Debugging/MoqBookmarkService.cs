using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NNReader.Bookmarks;


namespace NNReader.Debugging
{
    class MoqBookmarkService : BaseLoadableBookmarkService
    {
        private static readonly Random random = new Random();
        private static TimeSpan GetRandomDelay(double max = 1) => TimeSpan.FromSeconds(max * random.NextDouble());

        public static MoqBookmarkService Default { get; } = new MoqBookmarkService();

        public MoqBookmarkService()
        {
            
        }

        public override async Task<bool> IsLoadableAsync()
        {
            var delay = GetRandomDelay();
            await Task.Delay(delay);
            return true;
        }

        protected override async Task DoDownloadingAsync(string ncode)
        {
            var delay = GetRandomDelay(10);
            await Task.Delay(delay);

            var b = new MoqBookmarkInfo(ncode);
            this.Add(b);
        }

        protected override async Task DoLoadingAsync()
        {
            var delay = GetRandomDelay(2);
            await Task.Delay(delay);

            var b1 = new MoqBookmarkInfo("111");
            this.Add(b1);

            delay = GetRandomDelay(2);
            await Task.Delay(delay);

            var b2 = new MoqBookmarkInfo("222");
            this.Add(b2);
        }
    }
}
