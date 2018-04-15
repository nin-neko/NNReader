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

using Prism.Mvvm;


namespace NNReader.Net
{
    class BaseNarouClient
    {
        private const int DECOMPRESS_BUFFER_SIZE = 1024;
        private static readonly UTF8Encoding encoding = new UTF8Encoding(false);
        private static readonly HttpClient httpApiClient = new HttpClient();
        private static readonly HttpClient httpTextClient = new HttpClient();

        public BaseNarouClient()
        {
        }

        private static async Task<byte[]> DecompressAsync(Stream stream)
        {
            using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                var receivedBytes = new List<byte>();
                var buffer = new byte[DECOMPRESS_BUFFER_SIZE];
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

        private static async Task<byte[]> ReceiveBytesAsync(string url)
        {
            using (var stream = await httpApiClient.GetStreamAsync(url))
            {
                return await DecompressAsync(stream);
            }
        }

        private async Task<string> ReceiveJsonAsync(string url)
        {
            var bin = await ReceiveBytesAsync(url);
            return encoding.GetString(bin);
        }

        public async Task<JArray> ReceiveJTokenAsync(string url)
        {
            var json = await this.ReceiveJsonAsync(url);
            return (JArray)JToken.Parse(json);
        }

        public async Task<string> ReceiveAsync(string url)
        {
            return await httpTextClient.GetStringAsync(url);
        }
    }
}
