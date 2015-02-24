using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;

namespace NumberedBookmarks
{
    class Bookmark
    {
        public ITextBuffer Buffer { get; set; }
        public int Line { get; set; }
        public int KeyNumber { get; set; }
        public SnapshotPoint Point { get; set; }
    }

    class NumberedBookmarksGlobalManager
    {
        static ConcurrentDictionary<int, Bookmark> _dico = new ConcurrentDictionary<int, Bookmark>();

        public static Bookmark ToogleBookmark(ConcurrentDictionary<int, Bookmark> dico, ITextBuffer buffer, SnapshotPoint point, int number)
        {
            if (dico == null)
                dico = _dico;

            int line = point.GetContainingLine().LineNumber;
            Bookmark oldBmk = null;
            if (dico.TryRemove(number, out oldBmk))
            {
                var newBmk = new Bookmark { Line = line, KeyNumber = number, Point = point, Buffer = buffer };
                if (oldBmk.Line == newBmk.Line)
                {
                    return null;
                }
                else
                {
                    dico.TryAdd(number, newBmk);
                    return oldBmk;
                }
            }
            else
            {
                oldBmk = new Bookmark { Line = line, KeyNumber = number, Point = point, Buffer = buffer };
                dico.TryAdd(number, oldBmk);
            }
            return null;
        }

        public static void GotoBookmark(ConcurrentDictionary<int, Bookmark> dico, int keyNumber)
        {
            if (dico == null)
                dico = _dico;
            Bookmark bmk;
            ITextBuffer buffer;
            IWpfTextView wpfView;
            if (dico.TryGetValue(keyNumber, out bmk))
            {
                var dte = NumberedBookmarksPackage.Instance.DTE;
                for (int i = 0; i < dte.Documents.Count; i++)
                {
                    var doc = dte.Documents.Item(i + 1);
                    if (DocumentUtilities.GetTextBufferAndView(dte, doc, out buffer, out wpfView))
                    {
                        if (buffer == bmk.Buffer)
                        {
                            doc.Activate();
                            wpfView.Caret.MoveTo(bmk.Point);
                            wpfView.DisplayTextLineContainingBufferPosition(bmk.Point, wpfView.ViewportHeight / 2, ViewRelativePosition.Top);
                        }
                    }
                }
            }
            else
            {
                SetStatusText(string.Format("the bookmark '{0}' has no been pinned in document or document has been closed, press ctrl-shift-{0} to assign location", keyNumber));
            }
        }

        private static void SetStatusText(string text)
        {
            var status = (IVsStatusbar)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsStatusbar));
            status.SetText(text);
        }

        public static bool IsAcrossDocuments
        {
            get
            {
                return NumberedBookmarksPackage.Instance.IsAcrossDocuments;
            }
        }


        public static int GetNumber(ConcurrentDictionary<int, Bookmark> dico, ITextBuffer buffer, int line)
        {
            if (dico == null)
                dico = _dico;
            foreach (var item in dico.Values)
            {
                if (item.Buffer == buffer)
                {
                    if (item.Line == line)
                        return item.KeyNumber;
                }
            }
            return -1;
        }


        internal static void Clear(ConcurrentDictionary<int, Bookmark> dico)
        {
            if (dico == null)
            {
                dico = _dico;
            }
            var bmks = dico.Values.ToArray();
            dico.Clear();
            foreach (var bmk in bmks)
            {
                NumberedBookmarksManager.GetBookmarkManager(bmk.Buffer).DoBookmarksChanged(-1, bmk.Line);
            }
        }
    }
}
