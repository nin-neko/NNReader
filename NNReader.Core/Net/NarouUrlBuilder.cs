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
    class NarouApiUrlBuilder
    {
        private static readonly string BASE_API_URL = "https://api.syosetu.com/novelapi/api/?out=json&gzip=5";

        private readonly Dictionary<string, string> attributes = new Dictionary<string, string>();

        public NarouApiUrlBuilder()
        {
        }

        public NarouApiUrlBuilder WithLimit(int limit)
        {
            attributes["lim"] = $"&lim={limit}";
            return this;
        }

        public NarouApiUrlBuilder WithNCode(string ncode)
        {
            attributes["ncode"] = $"&ncode={ncode.ToLower()}";
            return this;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(BASE_API_URL);
            foreach (var x in attributes)
            {
                builder.Append(x.Value);
            }
            return builder.ToString();
        }
    }

    class NarouUrlBuilder
    {
        private static readonly string BASE_URL = "https://ncode.syosetu.com/";
        private static readonly string NCODE_KEY = "ncode";
        private static readonly string INDEX_KEY = "index";

        private readonly Dictionary<string, string> attributes = new Dictionary<string, string>();

        public NarouUrlBuilder()
        {
        }

        public NarouUrlBuilder WithIndex(int index)
        {
            attributes[INDEX_KEY] = $"{index}/";
            return this;
        }

        public NarouUrlBuilder WithNCode(string ncode)
        {
            attributes[NCODE_KEY] = $"{ncode.ToLower()}/";
            return this;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(BASE_URL);

            if (attributes.ContainsKey(NCODE_KEY))
            {
                builder.Append(attributes[NCODE_KEY]);
                if (attributes.ContainsKey(INDEX_KEY))
                {
                    builder.Append(attributes[INDEX_KEY]);
                }
            }

            return builder.ToString();
        }
    }
}
