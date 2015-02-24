using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Text;
using PierrickGourlain.FolderOutputPath.OptionsPage;

namespace NumberedBookmarks
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    [ProvideOptionPage(typeof(NumberedBookmarksOptions), Constants.OptionPageCategoryName, Constants.OptionPageName, 110, 113, true)]
    [ProvideProfile(typeof(NumberedBookmarksOptions), Constants.OptionPageCategoryName, Constants.OptionPageName, 110, 113, true)]
    //[ProvideKeyBindingTable
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.SolutionExists)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.NoSolution)]

    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.2", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidNumberedBookmarksPkgString)]
    public sealed class NumberedBookmarksPackage : Package
    {
        /// <summary>
        /// provides current instance of this package
        /// </summary>
        /// <returns></returns>
        internal static NumberedBookmarksPackage Instance { get; private set; }
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public NumberedBookmarksPackage()
        {
            Instance = this;
            //Trace.WriteLine(CultureInfo.CurrentCulture, "Entering constructor for: NumberedBookmarksPackage");
        }

        //uint pdwCookie;

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            //Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            IVsRegisterPriorityCommandTarget cmdTarget = GetService(typeof(SVsRegisterPriorityCommandTarget)) as IVsRegisterPriorityCommandTarget;
            //var id = new CommandID(GuidList.guidNumberedBookmarksCmdSet, 0x101);
            //var cmd = new KeyBindingCommandFilter(null);
            //cmdTarget.RegisterPriorityCommandTarget(0, cmd, out pdwCookie);
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                //var oldTarget = mcs.ParentTarget;
                //mcs.ParentTarget = new MyParentOleCommandTarget() { m_nextTarget = oldTarget };
                RegisterCommand(mcs, PkgCmdIDList.cmdOne, (sender, e) => { SetBookmark(1); });
                RegisterCommand(mcs, PkgCmdIDList.cmdTwo, (sender, e) => { SetBookmark(2); });
                RegisterCommand(mcs, PkgCmdIDList.cmdThree, (sender, e) => { SetBookmark(3); });
                RegisterCommand(mcs, PkgCmdIDList.cmdFour, (sender, e) => { SetBookmark(4); });
                RegisterCommand(mcs, PkgCmdIDList.cmdFive, (sender, e) => { SetBookmark(5); });
                RegisterCommand(mcs, PkgCmdIDList.cmdSix, (sender, e) => { SetBookmark(6); });
                RegisterCommand(mcs, PkgCmdIDList.cmdSeven, (sender, e) => { SetBookmark(7); });
                RegisterCommand(mcs, PkgCmdIDList.cmdEight, (sender, e) => { SetBookmark(8); });
                RegisterCommand(mcs, PkgCmdIDList.cmdNine, (sender, e) => { SetBookmark(9); });
                //CommandID menuCommandID = new CommandID(GuidList.guidFirstLookCmdSet,
                //  (int)PkgCmdIDList.cmdOne);
                //MenuCommand menuItem = new MenuCommand(CommandCallback, menuCommandID);
                //mcs.AddCommand(menuItem);
                RegisterCommand(mcs, PkgCmdIDList.cmdGotoOne, (sender, e) => { GotoBookmark(1); });
                RegisterCommand(mcs, PkgCmdIDList.cmdGotoTwo, (sender, e) => { GotoBookmark(2); });
                RegisterCommand(mcs, PkgCmdIDList.cmdGotoThree, (sender, e) => { GotoBookmark(3); });
                RegisterCommand(mcs, PkgCmdIDList.cmdGotoFour, (sender, e) => { GotoBookmark(4); });
                RegisterCommand(mcs, PkgCmdIDList.cmdGotoFive, (sender, e) => { GotoBookmark(5); });
                RegisterCommand(mcs, PkgCmdIDList.cmdGotoSix, (sender, e) => { GotoBookmark(6); });
                RegisterCommand(mcs, PkgCmdIDList.cmdGotoSeven, (sender, e) => { GotoBookmark(7); });
                RegisterCommand(mcs, PkgCmdIDList.cmdGotoEight, (sender, e) => { GotoBookmark(8); });
                RegisterCommand(mcs, PkgCmdIDList.cmdGotoNine, (sender, e) => { GotoBookmark(9); });
            }
            this.IsAcrossDocuments = GeneralPage.Options.UseBoomarkAcrossDocument;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        internal event BookmarkDelegate OnGotoBookmark;

        private void GotoBookmark(int bookmarkId)
        {
            if (OnGotoBookmark != null)
            {
                OnGotoBookmark(bookmarkId);
            }
        }

        internal event BookmarkDelegate OnSetBookmark;

        private void SetBookmark(int bookmarkId)
        {
            if (OnSetBookmark != null)
            {
                OnSetBookmark(bookmarkId);
            }
        }

        private OleMenuCommand RegisterCommand(OleMenuCommandService mcs, uint id, EventHandler callback)
        {
            if (mcs == null) return null;
            var menuCommandID = new CommandID(GuidList.guidNumberedBookmarksCmdSet, (int)id);            
            var menuItem = new OleMenuCommand(callback, menuCommandID); 
            mcs.AddCommand(menuItem);
            return menuItem;
        }

        private void OutputCommandString(string text)
        {
            // --- Build the string to write on the debugger and Output window. 
            var outputText = new StringBuilder();
            outputText.AppendFormat("NumberedBookmarksPackage: {0} ", text);
            // --- Get a reference to IVsOutputWindow. 
            var outputWindow = GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            if (outputWindow == null) return;

            // --- Get the window pane for the general output. 
            var guidGeneral = Microsoft.VisualStudio.VSConstants.GUID_OutWindowDebugPane;
            IVsOutputWindowPane windowPane;
            if (Microsoft.VisualStudio.ErrorHandler.Failed(
              outputWindow.GetPane(ref guidGeneral, out windowPane)))
            {
                return;
            }
            // --- As the last step, write to the output window pane 
            windowPane.OutputString(outputText.ToString());
            windowPane.Activate();
        } 

        #endregion

        NumberedBookmarksOptions generalPage;
        public NumberedBookmarksOptions GeneralPage
        {
            get
            {
                if (this.generalPage == null)
                {
                    this.generalPage = base.GetDialogPage(typeof(NumberedBookmarksOptions)) as NumberedBookmarksOptions;
                    this.generalPage.LoadSettingsFromStorage();
                    this.generalPage.OnCommit += new EventHandler(generalPage_OnCommit);
                }
                return this.generalPage;
            }
        }

        void generalPage_OnCommit(object sender, EventArgs e)
        {
            NumberedBookmarksOptions options = sender as NumberedBookmarksOptions;
            if (options.Options.UseBoomarkAcrossDocument != this.IsAcrossDocuments)
            {
                var oldValue = this.IsAcrossDocuments;
                this.IsAcrossDocuments = options.Options.UseBoomarkAcrossDocument;
                if (oldValue)
                {
                    NumberedBookmarksGlobalManager.Clear(null);
                }
                else
                {
                    //todo on every documents
                    foreach (var buffer in DocumentUtilities.OpenedBuffers(this.DTE))
	                {
                        NumberedBookmarksManager.GetBookmarkManager(buffer).Clear();
	                } 
                }
            }
        }

        public bool IsAcrossDocuments
        {
            get ;
            private set;
        }

        public EnvDTE.DTE DTE
        {
            get
            {
                return GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            }
        }

        public void WriteError(Exception ex)
        {
            IVsOutputWindow outWindow;
            outWindow = (IVsOutputWindow)GetService(typeof(SVsOutputWindow));
            //Guid generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane; // P.S. There's also the GUID_OutWindowDebugPane available.
            Guid generalPaneGuid = VSConstants.GUID_OutWindowDebugPane;
            IVsOutputWindowPane generalPane;
            outWindow.GetPane(ref generalPaneGuid, out generalPane);

            if (generalPage != null)
            {
                generalPane.OutputString(string.Format("NumberredBookmarks exception : {0}", ex));
                generalPane.Activate(); // Brings this pane into view
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(string.Format("NumberredBookmarks exception : {0}", ex));
            }
        }
    }

    internal delegate void BookmarkDelegate(int bookmarkId);
}
