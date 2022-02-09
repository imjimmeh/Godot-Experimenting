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
		[Signal]
		public delegate void _FaceIcon_Clicked(Node character);

		[Export]
		public DynamicFont FontToUse { get; set; }

		[Export]
		public int FontSize { get; set; } = 32;

		[Export]
		public float MaxWidthOfIcon { get; set; } = 32;


		private TextureRect characterFaceIcon;
		private Label characterName;
		private Character character;


		private bool mouseIsOver = false;

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
			GetChildren();

			InitialiseFont();

			characterName.AddFontOverride("font", FontToUse);

			Connect("_FaceIcon_Clicked", GetNode("/root/Root/Cameras/MainCamera"), "_On_Character_FaceIconClicked");
		}

		private void InitialiseFont()
		{
			FontToUse.Size = FontSize;
			FontToUse.OutlineSize = 1;
			FontToUse.OutlineColor = new Color(0, 0, 0, 1);
			FontToUse.UseFilter = true;
		}

		private void GetChildren()
		{
			characterFaceIcon = GetNode<TextureRect>("Icon");
			characterName = GetNode<Label>("CharacterName");
		}

		public void SetCharacter(Character character)
		{
			this.character = character;
		}

		public override void _Process(float delta)
		{
			base._Process(delta);
			if (TextureNotInitialisedButCharacterIs)
			{
				SetValues();
			}

			if(mouseIsOver)
			{
				var mousePos = GetGlobalMousePosition();


				characterName.RectGlobalPosition = new Vector2(mousePos.x + (characterName.Text.Length), mousePos.y + 10);
				characterName.Show();
			}
			else
			{
				characterName.Hide();
			}
		}

		private void SetValues()
		{
			characterFaceIcon.Texture = character.Stats.FaceIcon;

			if (TextureIsTooLarge())
			{
				ResizeTexture();
			}

			characterName.Text = character.Stats.CharacterName;
		}

		private bool TextureNotInitialisedButCharacterIs => characterFaceIcon.Texture == null && character != null && character?.Stats != null && character.Stats.FaceIcon != null;

		private void ResizeTexture()
		{
			var div = MaxWidthOfIcon / characterFaceIcon.Texture.GetWidth();
			RectScale = new Vector2(div, div);
		}

		private bool TextureIsTooLarge() => characterFaceIcon.Texture.GetWidth() > MaxWidthOfIcon;

		public override void _Notification(int what)
		{
			base._Notification(what);
			CheckMouseEnterOrExit(what);
		}

		private void CheckMouseEnterOrExit(int what)
		{
			switch (what)
			{
				case NotificationMouseEnter:
					{
						mouseIsOver = true;
						break;
					}
				case NotificationMouseExit:
					{
						mouseIsOver = false;
						break;
					}
			}
		}

		public override void _Input(InputEvent inputEvent)
		{
			base._Input(inputEvent);
			if(mouseIsOver && inputEvent is InputEventMouseButton mouseButton)
			{
				if(mouseButton.ButtonIndex == 1)
				{
					EmitSignal("_FaceIcon_Clicked", character);
				}
			}
		}
	}
}
