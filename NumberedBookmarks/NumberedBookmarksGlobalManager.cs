using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Globalization;

namespace NumberedBookmarks
{
    class Bookmark
    {
        public ITextBuffer Buffer { get; set; }
        public int Line { get; set; }
        public int KeyNumber { get; set; }
        //public SnapshotPoint Point { get; set; }
        //public ITextVersion Version { get; set; }
        public ITrackingPoint TrackingPoint { get; set; }
    }

    /// <summary>
    /// global BookmarkManager that manage bookmark across all documents if configuration is specified
    /// </summary>
    class NumberedBookmarksGlobalManager
    {
        static ConcurrentDictionary<int, Bookmark> _dico = new ConcurrentDictionary<int, Bookmark>();

        public static Bookmark ToogleBookmark(ConcurrentDictionary<int, Bookmark> dico, ITextBuffer buffer, SnapshotPoint point, int number)
        {
            if (dico == null)
                dico = _dico;

            int line = point.GetContainingLine().LineNumber;

            var trackingPoint = buffer.CurrentSnapshot.CreateTrackingPoint(point.Position, PointTrackingMode.Negative);
            Bookmark oldBmk = null;
            if (dico.TryRemove(number, out oldBmk))
            {

                var newBmk = new Bookmark { Line = line, KeyNumber = number, Buffer = buffer, TrackingPoint = trackingPoint };
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
                oldBmk = new Bookmark { Line = line, KeyNumber = number, Buffer = buffer, TrackingPoint = trackingPoint };
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
                            try
                            {
                                var pos = bmk.TrackingPoint.GetPosition(buffer.CurrentSnapshot);
                                var newSnap = new SnapshotPoint(buffer.CurrentSnapshot, pos);
                                wpfView.Caret.MoveTo(newSnap);
                                wpfView.DisplayTextLineContainingBufferPosition(newSnap, wpfView.ViewportHeight / 2, ViewRelativePosition.Top);
                            }
                            catch(Exception ex)
                            {
                                //send exception into task list window
                                var errorProvider = new ErrorListProvider(NumberedBookmarksPackage.Instance);
                                var t = new ErrorTask(ex);
                                t.Line = bmk.Line;
                                t.Column = 1;
                                t.Document = doc.Name;
                                t.Text = string.Format(CultureInfo.InvariantCulture, "Unable to go to bookmark #{0} : {1}", keyNumber, ex);
                                errorProvider.Tasks.Add(t);
                            }
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

        /// <summary>
        /// return numbers for a line in document (textBuffer)
        /// </summary>
        /// <param name="dico"></param>
        /// <param name="buffer"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetNumber(ConcurrentDictionary<int, Bookmark> dico, ITextBuffer buffer, int line)
        {
            if (dico == null)
                dico = _dico;
            foreach (var item in dico.Values)
            {
                if (item.Buffer == buffer)
                {
                    if (item.Line == line)
                        yield return item.KeyNumber;
                }
            }
            yield break;
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
