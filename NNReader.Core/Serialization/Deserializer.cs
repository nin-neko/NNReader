using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NNReader.Bookmarks;


namespace NNReader.Serialization
{
    abstract class BaseDeserializer
    {
        protected static UTF8Encoding Encoding = new UTF8Encoding(false);

        protected static async Task<byte[]> ReadBinaryAsync(string filePath)
        {
            var bin = new List<byte>();
            var buffer = new byte[1024];
            using (var stream = File.OpenRead(filePath))
            {
                while (true)
                {
                    var actual = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (actual == 0) break;
                    if (actual == buffer.Length)
                    {
                        bin.AddRange(buffer);
                    }
                    else
                    {
                        bin.AddRange(buffer.Take(actual));
                    }
                }
                return bin.ToArray();
            }
        }

        protected static async Task<string> ReadStringAsync(string filePath)
        {
            var bin = await ReadBinaryAsync(filePath);
            return Encoding.GetString(bin);
        }

        protected static async Task<JToken> ReadJTokenAsync(string filePath)
        {
            var json = await ReadStringAsync(filePath);
            return JToken.Parse(json);
        }
    }

    abstract class BaseDeserializer<T> : IDeserializer<T>
    {
        protected static UTF8Encoding Encoding = new UTF8Encoding(false);

        protected static async Task<byte[]> ReadBinaryAsync(string filePath)
        {
            var bin = new List<byte>();
            var buffer = new byte[1024];
            using (var stream = File.OpenRead(filePath))
            {
                while (true)
                {
                    var actual = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (actual == 0) break;
                    if (actual == buffer.Length)
                    {
                        bin.AddRange(buffer);
                    }
                    else
                    {
                        bin.AddRange(buffer.Take(actual));
                    }
                }
                return bin.ToArray();
            }
        }

        protected static async Task<string> ReadStringAsync(string filePath)
        {
            var bin = await ReadBinaryAsync(filePath);
            return Encoding.GetString(bin);
        }

        protected static async Task<JToken> ReadJTokenAsync(string filePath)
        {
            var json = await ReadStringAsync(filePath);
            return JToken.Parse(json);
        }

        public abstract Task LoadAsync(T target);
    }

    //class NovelDeserializer
    //{
    //    protected static UTF8Encoding Encoding = new UTF8Encoding(false);

    //    protected static async Task<byte[]> ReadBinaryAsync(string filePath)
    //    {
    //        var bin = new List<byte>();
    //        var buffer = new byte[1024];
    //        using (var stream = File.OpenRead(filePath))
    //        {
    //            while (true)
    //            {
    //                var actual = await stream.ReadAsync(buffer, 0, buffer.Length);
    //                if (actual == 0) break;
    //                if (actual == buffer.Length)
    //                {
    //                    bin.AddRange(buffer);
    //                }
    //                else
    //                {
    //                    bin.AddRange(buffer.Take(actual));
    //                }
    //            }
    //            return bin.ToArray();
    //        }
    //    }

    //    protected static async Task<string> ReadStringAsync(string filePath)
    //    {
    //        var bin = await ReadBinaryAsync(filePath);
    //        return Encoding.GetString(bin);
    //    }

    //    protected static async Task<JToken> ReadJTokenAsync(string filePath)
    //    {
    //        var json = await ReadStringAsync(filePath);
    //        return JToken.Parse(json);
    //    }

    //    public static async Task<BaseChapter> Load(string ncode, int index)
    //    {
    //        var fileName = $"{ncode}-{index}.json";
    //        var filePath = Path.Combine(ncode, fileName);

    //        var jtoken = await ReadJTokenAsync(filePath);

    //        var dncode = jtoken[nameof(IChapter.Ncode)].ToString();
    //        var dtitle = jtoken[nameof(IChapter.Title)].ToString();
    //        var dcontent = jtoken[nameof(IChapter.Content)].ToString();

    //        var novel = new NarouChapter(dncode, index)
    //        {
    //            //Content = dcontent,
    //        };
    //        return novel;
    //    }
    //}

    //class NovelsDeserializer : NovelDeserializer
    //{
    //    public static async Task<BaseChapter[]> Load(string ncode)
    //    {
    //        var fileDir = ncode;
    //        var fileCount = Directory.GetFiles(fileDir).Length;
    //        var novelFileCount = fileCount - 1;

    //        var novels = new List<BaseChapter>();
    //        for (int i = 0; i < novelFileCount; i++)
    //        {
    //            var novel = await NovelDeserializer.Load(ncode, i);
    //            novels.Add(novel);
    //        }

    //        return novels.ToArray();
    //    }
    //}
}
