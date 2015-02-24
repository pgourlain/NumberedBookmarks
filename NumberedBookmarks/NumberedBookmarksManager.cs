using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using System.Collections.Concurrent;

namespace NumberedBookmarks
{
    class NumberedBookmarksManagerEventArgs : EventArgs
    {
        public int NewLine { get; set; }
        public int OldLine { get; set; }
    }

    /// <summary>
    /// provide bookmark manager (one per document)
    /// </summary>
    class NumberedBookmarksManager
    {
        ConcurrentDictionary<int, Bookmark> _dico = null;

        internal void DoBookmarksChanged(int newLine, int oldLine)
        {
            if (BookmarksChanged != null)
            {
                BookmarksChanged(this, new NumberedBookmarksManagerEventArgs { NewLine = newLine, OldLine = oldLine });
            }
        }

        public IEnumerable<int> GetNumber(int line)
        {
            if (NumberedBookmarksGlobalManager.IsAcrossDocuments)
            {
                return NumberedBookmarksGlobalManager.GetNumber(null, this.buffer, line);
            }
            else
            {
                if (_dico != null)
                {
                    return NumberedBookmarksGlobalManager.GetNumber(_dico, buffer, line);
                }
            }
            return new int[0];
        }

        public event EventHandler<NumberedBookmarksManagerEventArgs> BookmarksChanged;
        private ITextBuffer buffer;

        public NumberedBookmarksManager(ITextBuffer buffer)
        {
            this.buffer = buffer;
        }

        #region static
        public static NumberedBookmarksManager GetBookmarkManager(ITextBuffer buffer)
        {
            if (buffer != null)
            {
                return buffer.Properties.GetOrCreateSingletonProperty<NumberedBookmarksManager>(() =>  new NumberedBookmarksManager(buffer));
            }
            return null;
        }

        public static NumberedBookmarksManager GetBookmarkManager(ITextView view)
        {
            if (view != null)
            {
                return GetBookmarkManager(view.TextBuffer);
            }
            throw new NullReferenceException("GetBookmarkManager(view) with a null view");
        }
        #endregion

        internal void ToogleBookmark(Microsoft.VisualStudio.Text.Editor.CaretPosition caretPosition, int number)
        {
            int oldLine = -1;
            var point = caretPosition.BufferPosition;
            var line = point.GetContainingLine().LineNumber;
            ConcurrentDictionary<int, Bookmark> currentDico = null;
 
            if (NumberedBookmarksGlobalManager.IsAcrossDocuments)
            {
            }
            else
            {
                if (_dico == null)
                {
                    _dico = new ConcurrentDictionary<int, Bookmark>();
                }
                currentDico = _dico;
            }
            var oldBmk = NumberedBookmarksGlobalManager.ToogleBookmark(currentDico, this.buffer, point, number);
            if (oldBmk != null)
            {
                GetBookmarkManager(oldBmk.Buffer).DoBookmarksChanged(-1, oldBmk.Line);
            }

            DoBookmarksChanged(line, oldLine);
        }

        internal void GotoBookmark(IWpfTextView view, int keyNumber)
        {
            if (NumberedBookmarksGlobalManager.IsAcrossDocuments)
            {
                NumberedBookmarksGlobalManager.GotoBookmark(null, keyNumber);
            }
            else
            {
                if (_dico == null)
                    return;
                NumberedBookmarksGlobalManager.GotoBookmark(_dico, keyNumber);
            }
        }

        internal void Clear()
        {
            if (_dico != null)
            {
                NumberedBookmarksGlobalManager.Clear(_dico);
            }
        }
    }
}
