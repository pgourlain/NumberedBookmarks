using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Runtime.InteropServices;
using EnvDTE;

namespace PierrickGourlain.FolderOutputPath.OptionsPage
{
    [System.Runtime.InteropServices.Guid("194B148F-A2F4-4063-9D85-4FBA5FB81A70")]
    [ClassInterface(ClassInterfaceType.AutoDual), ComVisible(true)]    
    public class NumberedBookmarksOptions : DialogPage
    {
        private NumberedBookmarksOptionsModel _model = new NumberedBookmarksOptionsModel();

        protected override void OnApply(DialogPage.PageApplyEventArgs e)
        {
            base.OnApply(e);
            if (OnCommit != null)
            {
                OnCommit(this, EventArgs.Empty);
            }
        }

        public override void LoadSettingsFromXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsReader reader)
        {
            base.LoadSettingsFromXml(reader);
        }

        public override void SaveSettingsToXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsWriter writer)
        {
            base.SaveSettingsToXml(writer);
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();
        }

        public override object AutomationObject
        {
            get
            {
                return _model;
            }
        }

        public NumberedBookmarksOptionsModel Options
        {
            get
            {
                return _model;
            }
        }

        public event EventHandler OnCommit;

    }


    [Serializable]
    public class NumberedBookmarksOptionsModel
    {
        [Category("General")]
        [Description("if this option is set to true, you can use numbered bookmarks across all documents")]
        [DisplayName("Use bookmarks across all documents")]
        [DefaultValue(false)]
        public bool UseBoomarkAcrossDocument { get; set; }

    }
}
