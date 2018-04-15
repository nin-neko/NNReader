using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NNReader.Bookmarks;


namespace NNReader.Serialization
{
    class ChapterSerializer : BaseSerializer, ISerializer
    {
        private readonly IChapter chapter;
        private readonly JObject serialized = new JObject();

        public ChapterSerializer(IChapter chapter)
        {
            this.chapter = chapter;
        }

        public ChapterSerializer WithTitle()
        {
            serialized[nameof(IChapter.Title)] = chapter.Title;
            return this;
        }

        public ChapterSerializer WithContent()
        {
            serialized[nameof(IChapter.Content)] = chapter.Content;
            return this;
        }

        public ChapterSerializer WithAll() => this.WithTitle().WithContent();

        public async Task SaveAsync()
        {
            var fileName = $"{chapter.Ncode}-{chapter.Index}.json";
            var locator = IO.Locator.Default;
            var filePath = Path.Combine(locator.BootstrapperDirectory, chapter.Ncode, fileName);

            await this.WriteAsync(filePath, serialized.ToString());
        }
    }

    class ChapterDeserializer : BaseDeserializer
    {
        private readonly string ncode;
        private readonly int index;

        public ChapterDeserializer(string ncode, int index)
        {
            this.ncode = ncode;
            this.index = index;
        }

        private string BuildFilePath()
        {
            var fileName = $"{ncode}-{index}.json";
            var locator = IO.Locator.Default;
            var filePath = Path.Combine(locator.BootstrapperDirectory, ncode, fileName);
            return filePath;
        }

        public async Task<bool> LoadableTitleAsync()
        {
            var filePath = this.BuildFilePath();

            if (!File.Exists(filePath)) return false;

            try
            {
                var jtoken = await ReadJTokenAsync(filePath);
                var title = jtoken[nameof(IChapter.Title)];
                if (title == null) return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<string> GetTitleAsync()
        {
            var filePath = this.BuildFilePath();

            var jtoken = await ReadJTokenAsync(filePath);
            var title = jtoken[nameof(IChapter.Title)].ToString();

            return title;
        }

        public async Task<bool> LoadableContentAsync()
        {
            var filePath = this.BuildFilePath();

            if (!File.Exists(filePath)) return false;

            try
            {
                var jtoken = await ReadJTokenAsync(filePath);
                var title = jtoken[nameof(IChapter.Title)];
                var content = jtoken[nameof(IChapter.Content)];
                if (title == null || content == null) return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<string> GetContentAsync()
        {
            var filePath = this.BuildFilePath();

            var jtoken = await ReadJTokenAsync(filePath);
            var content = jtoken[nameof(IChapter.Content)].ToString();

            return content;
        }
    }
}
