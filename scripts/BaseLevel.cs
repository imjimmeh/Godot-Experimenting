using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.map;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.state;
using FaffLatest.scripts.ui;
using FaffLatest.scripts.world;
using Godot;
using System;

public class BaseLevel : Spatial
{ 
	[Export]
	public Texture FaceSprites { get; set; }

	[Signal]
	public delegate void _Level_Loaded();

	public override void _Ready()
	{
		base._Ready();
	}

	public async void LoadLevel(MapInfo map)
    {
        var worldManager = GetNode(NodeReferences.BaseLevel.WORLD_MANAGER);
        worldManager.CallDeferred("InitialiseMap", map);
        await ToSignal(worldManager, nameof(WorldManager._World_Loaded));
        _On_Level_Loaded();
    }

    private void _On_Level_Loaded()
    {
        EmitSignal(nameof(_Level_Loaded));
        CharacterManager.Instance.Connect(nameof(CharacterManager._Faction_Killed), GameStateManager.Instance, $"_On{nameof(CharacterManager._Faction_Killed)}");
        var ui = GetNode(NodeReferences.Systems.UI_MANAGER);

        ui.CallDeferred("ShowUi");
        
        UiLabelFactory.GenerateUiLabel("Testing", new FontValues(Colors.White, 12, Colors.Black, 3), new Vector3(5, 1, 5), null, 10);
    }
}
