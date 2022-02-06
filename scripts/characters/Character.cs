using FaffLatest.scripts.constants;
using FaffLatest.scripts.input;
using Godot;
using System;

namespace FaffLatest.scripts.characters
{
	public class Character : Spatial
	{
		public Character()
		{
		}

		[Export]
		public CharacterStats Stats;

		public override void _Ready()
		{
			AddToGroup("characters");
		}

		public void SetPosition(Vector3 position)
		{
			GetNode<KinematicBody>("KinematicBody").Transform = new Transform(Transform.basis, position);
		}
	}
}

