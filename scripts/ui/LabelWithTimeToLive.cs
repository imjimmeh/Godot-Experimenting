using System;
using Godot;

namespace FaffLatest.scripts.ui
{
    public class LabelWithTimeToLive : Godot.Label
    {
        private float secondsToLive = 5;

        [Export]
        public float SecondsToLive { get => secondsToLive; private set => secondsToLive = value; } 

        private float secondsAlive = 0.0f;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _Process(float delta)
        {
            secondsAlive += delta;

            if (secondsAlive > secondsToLive)
            {
                QueueFree();
            }
        }

    }
}
