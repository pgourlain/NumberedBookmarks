using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using System.Linq;
using System;
using System.Diagnostics;


namespace NumberedBookmarks
{
    internal class NumberedBookmarkTagger : ITagger<NumberedBookmarkTag>
    {
        private ITextBuffer m_buffer;

        internal NumberedBookmarkTagger(ITextBuffer buffer)
        {
            m_buffer = buffer;
            NumberedBookmarksManager manager = NumberedBookmarksManager.GetBookmarkManager(m_buffer);
            manager.BookmarksChanged += new EventHandler<NumberedBookmarksManagerEventArgs>(manager_BookmarksChanged);
        }

        void manager_BookmarksChanged(object sender, NumberedBookmarksManagerEventArgs e)
        {
            if (this.TagsChanged != null)
            {
                if (e.NewLine >= 0)
                {
                    var snapshotline = this.m_buffer.CurrentSnapshot.GetLineFromLineNumber(e.NewLine);
                    this.TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshotline.Start, snapshotline.End)));
                }
                if (e.OldLine >= 0)
                {
                    var snapshotline = this.m_buffer.CurrentSnapshot.GetLineFromLineNumber(e.OldLine);
                    this.TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshotline.Start, snapshotline.End)));
                }
            }
        }

        IEnumerable<ITagSpan<NumberedBookmarkTag>> ITagger<NumberedBookmarkTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            NumberedBookmarksManager manager = NumberedBookmarksManager.GetBookmarkManager(m_buffer);
            if (manager != null)
            {
                foreach (SnapshotSpan span in spans)
                {
                    //var line = span.Start.GetContainingLine().LineNumber;
                    if (manager.GetNumber(span.Start).Any())
                    {
                        yield return new TagSpan<NumberedBookmarkTag>(new SnapshotSpan(span.Start, 1), new NumberedBookmarkTag { Numbers = manager.GetNumber(span.Start).ToArray() });
                    }
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    }

    internal class NumberedBookmarkTag : IGlyphTag 
    {
        public int[] Numbers { get; set; }
    }

    [Export(typeof(ITaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(NumberedBookmarkTag))]
    class NumberedBookmarkTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return new NumberedBookmarkTagger(buffer) as ITagger<T>;
        }
    }
}
