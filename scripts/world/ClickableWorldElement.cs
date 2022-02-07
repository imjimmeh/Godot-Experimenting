using System;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.input;
using Godot;

namespace FaffLatest.scripts.world
{
    public class ClickableWorldElement : StaticBody
    {
        [Signal]
        public delegate void _World_ClickedOn(StaticBody world, InputEventMouseButton mouseButtonEvent, Vector3 position);

        public override void _Ready()
        {
            base._Ready();
            GetNode<InputManager>("/root/Root/Systems/InputManager").ConnectWorldClickedOnSignal(this);
        }

        public override void _InputEvent(Godot.Object camera, InputEvent inputEvent, Vector3 position, Vector3 normal, int shapeIdx)
        {
            if (inputEvent is InputEventMouseButton mouseButtonEvent)
            {
                //GD.Print("World clicked on - emitting signal");
                EmitSignal(SignalNames.World.CLICKED_ON, this, mouseButtonEvent, position);
            }
        }


    }
}
