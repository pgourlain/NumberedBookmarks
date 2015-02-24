using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text.Editor;

namespace NumberedBookmarks
{
    class DocumentUtilities
    {

        public static bool GetTextBufferAndView(EnvDTE.DTE dte, EnvDTE.Document document, out ITextBuffer buffer, out IWpfTextView wpfView)
        {
            buffer = null;
            wpfView = null;

            using (ServiceProvider serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte))
            {

                var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
                var editorAdapterFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();

                IVsUIHierarchy uiHierarchy;
                uint itemID;
                IVsWindowFrame windowFrame;
                if (VsShellUtilities.IsDocumentOpen(
                  serviceProvider,
                  document.FullName,
                  Guid.Empty,
                  out uiHierarchy,
                  out itemID,
                  out windowFrame))
                {
                    IVsTextView view = VsShellUtilities.GetTextView(windowFrame);
                    IVsTextLines lines;
                    if (view.GetBuffer(out lines) == 0)
                    {
                        var buf = lines as IVsTextBuffer;
                        if (buf != null)
                        {
                            buffer = editorAdapterFactoryService.GetDataBuffer(buf);
                            wpfView = editorAdapterFactoryService.GetWpfTextView(view);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static ITextBuffer GetTextBuffer(ServiceProvider serviceProvider, IVsEditorAdaptersFactoryService editorAdapterFactoryService,  string fileName)
        {

            IVsUIHierarchy uiHierarchy;
            uint itemID;
            IVsWindowFrame windowFrame;
            if (VsShellUtilities.IsDocumentOpen(
              serviceProvider,
              fileName,
              Guid.Empty,
              out uiHierarchy,
              out itemID,
              out windowFrame))
            {
                IVsTextView view = VsShellUtilities.GetTextView(windowFrame);
                IVsTextLines lines;
                if (view.GetBuffer(out lines) == 0)
                {
                    var buffer = lines as IVsTextBuffer;
                    if (buffer != null)
                        return editorAdapterFactoryService.GetDataBuffer(buffer);
                }
            }

            return null;
        }

        public static IVsTextView GetTextView(EnvDTE.DTE dte, EnvDTE.Document document)
        {
            using (ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte))
            {

                IVsUIHierarchy uiHierarchy;
                uint itemID;
                IVsWindowFrame windowFrame;

                VsShellUtilities.IsDocumentOpen(sp, document.FullName,
                                                Guid.Empty, out uiHierarchy,
                                                out itemID, out windowFrame);

                IVsTextView textView = VsShellUtilities.GetTextView(windowFrame);
                return textView;
            }
        }

        private static bool InternalGetFilePath(ITextBuffer textBuffer, out string filePath)
        {
            ITextDocument doc;
            filePath = null;
            if (textBuffer.Properties.TryGetProperty<ITextDocument>(typeof(ITextDocument), out doc))
            {
                filePath = doc.FilePath;
                return true;
            }
            return false;
        }

        internal static string GetFilePath(ITextBuffer buffer)
        {
            string filePath;
            if (InternalGetFilePath(buffer, out filePath))
            {
                return filePath;
            }
            return string.Empty;
        }

        internal static IEnumerable<ITextBuffer> OpenedBuffers(EnvDTE.DTE dte)
        {
            using (ServiceProvider serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte))
            {

                var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
                var editorAdapterFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();

                List<ITextBuffer> result = new List<ITextBuffer>();
                for (int i = 0; i < dte.Documents.Count; i++)
                {
                    var doc = dte.Documents.Item(i + 1);
                    var buffer = GetTextBuffer(serviceProvider, editorAdapterFactoryService, doc.FullName);
                    if (buffer != null)
                        result.Add(buffer);
                }
                return result;
            }
        }
    }
}
