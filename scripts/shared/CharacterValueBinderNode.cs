using System;
using FaffLatest.scripts.characters;
using Godot;

namespace FaffLatest.scripts.shared
{
    public class CharacterValueBinderNode : Node
    {
        protected internal Character parent;
        
        private Action<Character> setValueAction;

        public override void _Ready()
        {
            base._Ready();
        }

        public void SetParentAndAction(Character parent, Action<Character> action)
        {
            this.parent = parent;
            setValueAction = action;
        }

        public void _On_Value_Change()
        {
            setValueAction(parent);
        }

        public void UpdateValues() => setValueAction(parent);
    }
}
