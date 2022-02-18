using System.Collections.Generic;
using Godot;

namespace FaffLatest.scripts.ui
{
    public class FontManager : Reference
    {
        private Dictionary<FontValues, DynamicFont> generatedFonts;

        private DynamicFontData baseFont;

        public FontManager()
        {
        }

        public FontManager(DynamicFontData baseFont)
        {
            generatedFonts = new Dictionary<FontValues, DynamicFont>(10);
            this.baseFont = baseFont;
        }

        public DynamicFont GetFont(FontValues fontValues)
        {
            if (generatedFonts.TryGetValue(fontValues, out DynamicFont existingFont))
                return existingFont;

            return GenerateNewFont(fontValues);
        }

        private DynamicFont GenerateNewFont(FontValues values)
        {
            var newFont = new DynamicFont();

            newFont.FontData = baseFont;
            newFont.Size = values.FontSize;
            newFont.OutlineColor = values.OutlineColor;
            newFont.OutlineSize = values.OutlineSize;

            generatedFonts.Add(values, newFont);

            return newFont;
        }
    }
}