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

using NNReader.Net;
using NNReader.Serialization;


namespace NNReader.Bookmarks
{
    sealed class NarouBookmarkService : BaseLoadableBookmarkService
    {
        public NarouBookmarkService()
        {
        }

        private static IEnumerable<string> FindPersistedFolders()
        {
            var locator = IO.Locator.Default;
            var licenseFolderPath = Path.Combine(locator.BootstrapperDirectory, "License");
            return Directory.EnumerateDirectories(locator.BootstrapperDirectory)
                .Where(x => x != licenseFolderPath);
        }

        public override async Task<bool> IsLoadableAsync()
        {
            return FindPersistedFolders()
                //.Where()// todo: regex check
                .Any();
        }

        protected override async Task DoDownloadingAsync(string ncode)
        {
            var narou = new NarouClient();
            if (!await narou.ExistNcodeAsync(ncode)) return;

            var bookmarkInfo = new NarouBookmarkInfo(ncode);
            this.Add(bookmarkInfo);
        }

        protected override async Task DoLoadingAsync()
        {
            foreach (var x in FindPersistedFolders())
            {
                var ncode = Path.GetFileName(x);
                var bookmark = new NarouBookmarkInfo(ncode);
                this.Add(bookmark);
            }
        }

        protected override async Task OnDownloadedAsync(string ncode)
        {
            //await new Serialization.BookmarkInfoSerializer(bookmarkInfo).SaveAsync();
        }
    }
}
