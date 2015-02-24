using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;

namespace NumberedBookmarks
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class MyTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import(typeof(IVsEditorAdaptersFactoryService))]
        internal IVsEditorAdaptersFactoryService editorFactory = null;

        

        class BookmarkReceiver
        {
            IWpfTextView wpfTextView;
            public BookmarkReceiver(IWpfTextView wpfTextView)
            {
                this.wpfTextView = wpfTextView;
                wpfTextView.Closed += new EventHandler(wpfTextView_Closed);
                NumberedBookmarksPackage.Instance.OnGotoBookmark += new BookmarkDelegate(Instance_OnGotoBookmark);
                NumberedBookmarksPackage.Instance.OnSetBookmark += new BookmarkDelegate(Instance_OnSetBookmark);
            }

            void wpfTextView_Closed(object sender, EventArgs e)
            {
                NumberedBookmarksPackage.Instance.OnGotoBookmark -= new BookmarkDelegate(Instance_OnGotoBookmark);
                NumberedBookmarksPackage.Instance.OnSetBookmark -= new BookmarkDelegate(Instance_OnSetBookmark);
            }

            void Instance_OnSetBookmark(int bookmarkId)
            {
                try
                {
                    if (this.wpfTextView.VisualElement == null || !this.wpfTextView.VisualElement.IsFocused)
                    {
                        System.Diagnostics.Debug.WriteLine("view has not focus");
                        return;
                    }
                    var manager = NumberedBookmarksManager.GetBookmarkManager(this.wpfTextView);
                    manager.ToogleBookmark(this.wpfTextView.Caret.Position, bookmarkId);
                }
                catch (Exception ex)
                {
                    NumberedBookmarksPackage.Instance.WriteError(ex);
                }
            }

            void Instance_OnGotoBookmark(int bookmarkId)
            {
                try
                {
                    if (this.wpfTextView.VisualElement == null || !this.wpfTextView.VisualElement.IsFocused)
                    {
                        System.Diagnostics.Debug.WriteLine("view has not focus");
                        return;
                    }
                    var manager = NumberedBookmarksManager.GetBookmarkManager(this.wpfTextView);
                    manager.GotoBookmark(this.wpfTextView, bookmarkId);
                }
                catch (Exception ex)
                {
                    NumberedBookmarksPackage.Instance.WriteError(ex);
                }
            }
        }
        #region IVsTextViewCreationListener Members

        public void VsTextViewCreated(Microsoft.VisualStudio.TextManager.Interop.IVsTextView textViewAdapter)
        {
            IWpfTextView textView = editorFactory.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;
            new BookmarkReceiver(textView);
        }

        #endregion
    }

}
