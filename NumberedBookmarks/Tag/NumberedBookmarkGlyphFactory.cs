using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using System.Linq;
using System.Windows.Media.Effects;

namespace NumberedBookmarks
{
    internal class NumberedBookmarkGlyphFactory : IGlyphFactory
    {
        const double m_glyphSize = 16.0;

        #region IGlyphFactory Members

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            NumberedBookmarkTag nTag = tag as NumberedBookmarkTag;
            // Ensure we can draw a glyph for this marker.
            if (nTag == null)
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
            b.Child = new TextBlock() { Text = nTag.Numbers.First().ToString(), HorizontalAlignment= HorizontalAlignment.Center };
            //if more than one number on same line, add a shadow
            if (nTag.Numbers.Length > 1)
            {
                var effect = new DropShadowEffect();
                effect.BlurRadius = 3;
                effect.Direction = 0;
                effect.ShadowDepth = 3;
                effect.Color = Colors.LightBlue;
                b.Effect = effect;
                b.ToolTip = string.Format("There is several bookmarks at this line : {0}", string.Join("..", nTag.Numbers.Select(x => x.ToString())));
            }
            return b;
        }

        #endregion
    }
}
