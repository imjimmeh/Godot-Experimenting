using Godot;

namespace FaffLatest.scripts.ui
{
    public class IconWithText : TextureRect
    {
        private const string LABEL_NODE_NAME = "Text";

        private Label text;

        [Export]
        public DynamicFont FontToUse {get; private set;}

        [Export]
        public int FontSize {get; private set;} = 16;

        [Export]
        public Vector2 IconSize {get; private set;} = new Vector2(20, 20);

        [Export]
        public Color FontColour {get; private set;} = Colors.White;

        public override void _Ready()
        {
            text = GetNode<Label>(LABEL_NODE_NAME);

            if(FontToUse != null)
            {
                FontToUse.Size = FontSize;
                text.AddFontOverride("font", FontToUse);
            }

            text.AddColorOverride("font_color", FontColour);    
            this.RectSize = IconSize;
            base._Ready();
        }

        public void SetLabelText(string newText) => text.Text = newText;

        public void SetToolTip(string tooltip) => HintTooltip = tooltip;
    }
}