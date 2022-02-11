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

        private bool isDisposing = false;

        private Vector2 targetDestination;

        public LabelWithTimeToLive(float secondsToLive)
        {
            SecondsToLive = secondsToLive;
        }

        public LabelWithTimeToLive()
        {
        }

        public override void _Ready()
        {
            base._Ready();
            targetDestination = new Vector2(this.RectPosition.x, this.RectPosition.y - 100);
            RectScale = new Vector2(3.0f, 3.0f);
        }

        public override void _Process(float delta)
        {
            secondsAlive += delta;

            CheckExpired();

            if(!isDisposing && !RectPosition.IsEqualApprox(targetDestination))
            {
                var direction = (targetDestination - RectPosition).Normalized();
                var velocity = direction * delta;
                var target = RectPosition + velocity;

                var result = RectPosition.LinearInterpolate(target, 0.5f);
                RectPosition = result;
                RectScale *= 0.9f;
            }
        }

        private void CheckExpired()
        {
            if (secondsAlive > secondsToLive)
            {
                if (isDisposing)
                {
                    GetParent().RemoveChild(this);
                    Dispose();
                    return;
                }

                QueueFree();
                isDisposing = true;
            }
        }
    }
}
