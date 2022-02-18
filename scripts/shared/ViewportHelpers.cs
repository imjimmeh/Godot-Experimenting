using Godot;

namespace FaffLatest.scripts.shared{
    public static class ViewportHelpers
    {
        public static Vector2 GetCenterOfScreen(this Viewport viewport)
        {
            var viewportSize = viewport.GetVisibleRect().Size;

            return new Vector2(viewportSize.x * 0.5f, viewportSize.y * 0.5f);
        }

        public static Vector2 GetCenterOfScreen(this Node node)
            => node.GetViewport().GetCenterOfScreen();
    }
}