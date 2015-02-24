using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Editor;

namespace NumberedBookmarks
{
    [Export(typeof(IGlyphFactoryProvider))]
    [Name("NumberedBookmarkGlyph")]
    [Order(After = "VsTextMarker")]
    [ContentType("text")]
    [TagType(typeof(NumberedBookmarkTag))]
    internal sealed class NumberedBookmarkGlyphFactoryProvider : IGlyphFactoryProvider
    {
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            return new NumberedBookmarkGlyphFactory();
        }
    }
}
