using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace NNReader.Bookmarks
{
    public enum BookmarkServiceStatus
    {
        Created,
        BookmarkInfoLoading,
        BookmarkInfoDownloading,
        Loaded,
    }

    public enum BookmarkInfoStatus
    {
        Created,
        SummaryLoading,
        SummaryDownloading,
        SummaryLoaded,
        ChapterLoading,
        ChapterDownloading,
        ChapterLoaded,
        AllChapterLoaded,
    }

    public enum ChapterStatus
    {
        Created,
        TitleLoading,
        TitleDownloading,
        TitleLoaded,
        ContentLoading,
        ContentDownloading,
        ContentLoaded,
    }
}
