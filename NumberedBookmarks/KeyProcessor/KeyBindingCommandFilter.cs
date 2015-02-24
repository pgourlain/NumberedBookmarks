//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.VisualStudio.OLE.Interop;
//using Microsoft.VisualStudio.Text.Editor;
//using Microsoft.VisualStudio;
//using System.ComponentModel.Composition;
//using Microsoft.VisualStudio.Utilities;
//using Microsoft.VisualStudio.Editor;
//using Microsoft.VisualStudio.TextManager.Interop;

//namespace NumberedBookmarks
//{
//    internal class KeyBindingCommandFilter : IOleCommandTarget
//    {
//        private IWpfTextView m_textView;
//        internal IOleCommandTarget m_nextTarget;
//        internal bool m_added;
//        internal bool m_adorned;


//        public KeyBindingCommandFilter(IWpfTextView textView)
//        {
//            m_textView = textView;
//            m_adorned = false;
//        }

//        #region IOleCommandTarget Members

//        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
//        {
//            if (pguidCmdGroup == GuidList.guidNumberedBookmarksCmdSet)
//            {
//                System.Diagnostics.Trace.WriteLine(string.Format("KeyBindingCommandFilter.Exec({0},{1},{2})", pguidCmdGroup, nCmdID, nCmdexecopt));
//                return 0;
//            }
//            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
//            //if (m_adorned == false)
//            //{
//            //    char typedChar = char.MinValue;

//            //    if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
//            //    {
//            //        typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
//            //        if (typedChar.Equals('+'))
//            //        {
//            //            new PurpleCornerBox(m_textView);
//            //            m_adorned = true;
//            //        }
//            //    }
//            //}
//            //return m_nextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
//        }

//        public int QueryStatus(ref Guid pguidCmdGroup, uint nCmdID, OLECMD[] prgCmds, IntPtr pCmdText)
//        {
//            if (pguidCmdGroup == GuidList.guidNumberedBookmarksCmdSet)
//            {
//                System.Diagnostics.Trace.WriteLine(string.Format("KeyBindingCommandFilter.QueryStatus({0},{1})", pguidCmdGroup, nCmdID));
//                return (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDF_SUPPORTED;
//            }
//            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
//            //System.Diagnostics.Trace.WriteLine(string.Format("QueryStatus({0}, {1})", pguidCmdGroup, nCmdID));
//            //return m_nextTarget.QueryStatus(ref pguidCmdGroup, nCmdID, prgCmds, pCmdText);
//        }

//        #endregion
//    }

//    //[Export(typeof(IVsTextViewCreationListener))]
//    //[ContentType("text")]
//    //[Name("NumberedBookmarks"),
//    //Order(Before=WpfTextViewKeyboardFilterName.KeyboardFilterOrderingName),
//    //TextViewRole(PredefinedTextViewRoles.Editable)]
//    internal class KeyBindingCommandFilterProvider : IVsTextViewCreationListener
//    {
//        [Import(typeof(IVsEditorAdaptersFactoryService))]
//        internal IVsEditorAdaptersFactoryService editorFactory = null;


//        public void VsTextViewCreated(IVsTextView textViewAdapter)
//        {
//            IWpfTextView textView = editorFactory.GetWpfTextView(textViewAdapter);
//            if (textView == null)
//                return;

//            AddCommandFilter(textViewAdapter, new KeyBindingCommandFilter(textView));
//        }

//        void AddCommandFilter(IVsTextView viewAdapter, KeyBindingCommandFilter commandFilter)
//        {
//            if (commandFilter.m_added == false)
//            {
//                //get the view adapter from the editor factory
//                IOleCommandTarget next;
//                int hr = viewAdapter.AddCommandFilter(commandFilter, out next);

//                if (hr == VSConstants.S_OK)
//                {
//                    commandFilter.m_added = true;
//                    //you'll need the next target for Exec and QueryStatus
//                    if (next != null)
//                        commandFilter.m_nextTarget = next;
//                }
//            }
//        }


//    }
//}
