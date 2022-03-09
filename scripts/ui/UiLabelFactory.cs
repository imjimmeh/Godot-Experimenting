using FaffLatest.scripts.movement;
using FaffLatest.scripts.shared;
using Godot;

namespace FaffLatest.scripts.ui{
    public static class UiLabelFactory
    {
        private static Camera camera;
        private static FontManager fonts;
        private static Node uiParent;

        public static void Initialise(Node uiParent, FontManager fontManager, Camera camera)
        {
            SetFontManager(fontManager);
            SetReferences(uiParent, camera);
        }

        private static void SetReferences(Node uiParent, Camera camera)
        {
            UiLabelFactory.camera = camera;
            UiLabelFactory.uiParent = uiParent;
        }

        public static void SetFontManager(FontManager fontManager) => fonts = fontManager;

        public static (Control parent, Label label) GenerateUiLabel(string text, 
                                                FontValues values, 
                                                Vector2 position,
                                                Vector3? centerOnWorldPosition = null, 
                                                Vector2? movementVector = null,
                                                float? timeToLive = null)
        {
            Label label = values.GenerateLabel(text, position);        
            Control finalParent = label;

            if(centerOnWorldPosition.HasValue)
            {
                finalParent = label.CenteredOnWorldPosition(position, centerOnWorldPosition.Value);
            }

            if(movementVector.HasValue)
            {
                label.MoveTowards(movementVector.Value);
            }

            if(timeToLive.HasValue)
            {
                finalParent.SetExpiresInSeconds(timeToLive.Value);
            }

            uiParent.AddChildDeferred(finalParent);
            return (finalParent, label);
        }

        private static Label GenerateLabel(this FontValues values, string text, Vector2 position)
        {
            var label = new Label();
            label.RectPosition = new Vector2(-999, -999);

            var fontToUse = fonts.GetFont(values);
            label.AddFontOverride("font", fontToUse);
            label.Text = text;
            label.RectPosition = position;

            return label;
        }

        private static Label MoveTowards(this Label label, Vector2 movementVector)
        {
            var mover = new MovingUiControl();
            mover.GetAndSetParent(label);
            mover.SetMovementVector(movementVector);

            label.AddChildDeferred(mover);
            
            return label;
        }

        private static Control CenteredOnWorldPosition(this Label label, Vector2 position, Vector3 worldPosition)
        {
            var parent = new Control();
            
            parent.RectPosition = position;
            label.RectPosition = Vector2.Zero;

            var worldCentered = new WorldCenteredControl();
            worldCentered.Initialise(parent, worldPosition, camera);
                
            parent.AddChildDeferred(worldCentered);
            parent.AddChildDeferred(label);

            return parent;
        }

        private static void AddChildDeferred(this Node parent, Node child)
        {
            parent.CallDeferred("add_child", child);
        }

        private static void SetExpiresInSeconds(this Control parent, float timeToLiveInSeconds)
        {
            var timeToLiveNode = new TimeToLiveNode();
            timeToLiveNode.SetSecondsToLive(timeToLiveInSeconds);

            parent.AddChildDeferred(timeToLiveNode);
        }
    }
}