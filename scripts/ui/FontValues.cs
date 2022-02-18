using Godot;

namespace FaffLatest.scripts.ui
{
    public struct FontValues
    {
        public FontValues(Color fontColour, int fontSize, Color outlineColor, int outlineSize)
        {
            FontColour = fontColour;
            FontSize = fontSize;
            OutlineColor = outlineColor;
            OutlineSize = outlineSize;
        }

        public Color FontColour { get; set; }
        public int FontSize {get; set;}

        public Color OutlineColor{get; set;}

        public int OutlineSize{get; set;}
    }
}