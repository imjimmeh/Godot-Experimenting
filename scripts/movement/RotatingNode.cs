using FaffLatest.scripts.characters;
using Godot;

namespace FaffLatest.scripts.movement
{
    public class RotatingNode : Node
    {
        private Spatial parent;

        public override void _Ready()
        {
            base._Ready();
            parent = GetParent<Spatial>();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            var rotated = parent.Transform.Rotated(new Vector3(0, 1, 0), 90);
            parent.Transform = parent.Transform.InterpolateWith(rotated, 0.15f);
        }
    }
}