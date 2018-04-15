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
    abstract class BaseSerializer
    {
        private static readonly UTF8Encoding encoding = new UTF8Encoding(false);
        private static readonly ExecutionDataflowBlockOptions option = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, };
        private static readonly ActionBlock<(string, string)> writingAction = new ActionBlock<(string Path, string Data)>(async x => await WriteUnsafe(x.Path, x.Data), option);

        public BaseSerializer()
        {
        }

        private static async Task WriteUnsafe(string filePath, byte[] data)
        {
            using (var stream = File.OpenWrite(filePath))
            {
                await stream.WriteAsync(data, 0, data.Length);
            }
        }

        private static async Task WriteUnsafe(string filePath, string data) => await WriteUnsafe(filePath, encoding.GetBytes(data));

        protected async Task WriteAsync(string filePath, string data)
        {
            await writingAction.SendAsync((filePath, data));
        }
    }

    class NovelSerializer : ISerializer
    {
        private readonly BaseChapter novel;

        public NovelSerializer(BaseChapter novel)
        {
            this.novel = novel;
        }

        internal JToken ToJToken()
        {
            return new JObject()
            {
                [nameof(IChapter.Ncode)] = novel.Ncode,
                [nameof(IChapter.Index)] = novel.Index,
                [nameof(IChapter.Title)] = novel.Title,
                [nameof(IChapter.Content)] = novel.Content,
            };
        }

        internal string ToJson() => this.ToJToken().ToString();

        public async Task SaveAsync()
        {
            var fileName = $"{novel.Ncode}-{novel.Index}.json";
            var di = Directory.CreateDirectory($"{novel.Ncode}");
            var filePath = Path.Combine(di.Name, fileName);
            var encoding = new UTF8Encoding(false);
            using (var stream = File.OpenWrite(filePath))
            {
                var json = this.ToJson();
                var bin = encoding.GetBytes(json);
                await stream.WriteAsync(bin, 0, bin.Length);
            }
        }

        public async Task Load()
        {
        }
    }

    class NovelsSerializer : ISerializer
    {
        private readonly NovelSerializer[] novelSerializers;

        public NovelsSerializer(IEnumerable<BaseChapter> novels)
        {
            this.novelSerializers = novels.Select(x => new NovelSerializer(x)).ToArray();
        }

        internal JToken ToJToken()
        {
            return new JArray(novelSerializers.Select(x => x.ToJToken()));
        }

        internal string ToJson() => this.ToJToken().ToString();

        public async Task SaveAsync()
        {
            var tasks = novelSerializers.Select(x => x.SaveAsync()).ToArray();
            await Task.WhenAll(tasks);
        }
    }

}
