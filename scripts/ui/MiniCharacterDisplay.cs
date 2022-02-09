using FaffLatest.scripts.characters;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.ui
{
	public class MiniCharacterDisplay : Control
	{
		private TextureRect characterFaceIcon;

		private Character character;

		[Export]
		public float MaxWidth { get; set; } = 64;

		public MiniCharacterDisplay(Character character)
		{
			this.character = character;
		}

		public MiniCharacterDisplay()
		{
		}

		public override void _Ready()
		{
			base._Ready();
			characterFaceIcon = GetNode<TextureRect>("Icon");
		}

		public void SetCharacter(Character character)
		{
			this.character = character;
		}

		public override void _Process(float delta)
		{
			base._Process(delta);

			if(characterFaceIcon.Texture == null && character?.Stats != null && character.Stats.FaceIcon != null)
			{
				characterFaceIcon.Texture = character.Stats.FaceIcon;

				if (TextureIsTooLarge())
				{
					ResizeTexture();
				}
			}
		}

		private void ResizeTexture()
		{
			var div = MaxWidth / characterFaceIcon.Texture.GetWidth();
			RectScale = new Vector2(div, div);
		}

		private bool TextureIsTooLarge() => characterFaceIcon.Texture.GetWidth() > MaxWidth;
	}
}
