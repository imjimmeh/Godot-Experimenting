using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.shared
{
	public class TimeToLiveNode : Node
	{
		[Export]
		public float SecondsToLive { get; private set; } = 5;


		private float secondsAlive = 0.0f;
		private bool isDisposing = false;

		public void SetSecondsToLive(float seconds)
		{
			SecondsToLive = seconds;
		}
		
		public override void _Process(float delta)
		{
			base._Process(delta);

			if (isDisposing)
				return;

			secondsAlive += delta;
			CheckExpired();
		}

		private void CheckExpired()
		{
			if (isDisposing)
				return;

			if (secondsAlive > SecondsToLive)
			{
				var parent = GetParent();
				parent.CallDeferred("queue_free");
				isDisposing = true;
			}
		}
	}
}
