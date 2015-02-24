using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;

namespace NumberedBookmarks
{
    internal class NumberedBookmarkGlyphFactory : IGlyphFactory
    {
        const double m_glyphSize = 16.0;

        #region IGlyphFactory Members

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            // Ensure we can draw a glyph for this marker.
            if (tag == null || !(tag is NumberedBookmarkTag))
            {
                return null;
            }

            Border b = new Border();
            b.Background = Brushes.LightBlue;
            b.CornerRadius = new CornerRadius(4);
            b.BorderThickness = new Thickness(1.0);
            b.BorderBrush = Brushes.DarkBlue;
            b.Width = 12; 
            b.Height = m_glyphSize;
            b.Child = new TextBlock() { Text = (tag as NumberedBookmarkTag).Number.ToString(), HorizontalAlignment= HorizontalAlignment.Center };
            return b;
        }

        #endregion
    }
}
