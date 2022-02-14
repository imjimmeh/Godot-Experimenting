using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.shared
{
    public class ShrinkingNode : Node
    {
        [Export]
        public float FloatingScale { get; private set; } = 0.95f;

        private Control parent;

        public Control Parent { get => GetAndSetParent(); private set => parent = value; }

        public Control GetAndSetParent()
        {
            if (parent == null)
                parent = GetParent<Control>();

            return parent;
        }

        public override void _Process(float delta)
        {
            if (Parent == null)
                return;

            Parent.RectScale *= FloatingScale;
            base._Process(delta);
        }
    }
}
