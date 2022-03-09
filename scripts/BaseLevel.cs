using FaffLatest.scripts.constants;
using FaffLatest.scripts.map;
using FaffLatest.scripts.state;
using FaffLatest.scripts.world;
using Godot;

public class BaseLevel : ViewportContainer
{ 
	[Export]
	public Texture FaceSprites { get; set; }

	[Signal]
	public delegate void _Level_Loaded();

	private Viewport _fogOfWarViewport;
	private ShaderMaterial _mat;

	public override void _Ready()
	{
		base._Ready();
		_fogOfWarViewport = GetNode(NodeReferences.BaseLevel.WORLD_MANAGER + "/FogOfWarViewport") as Viewport;
		_mat = Material as ShaderMaterial;
	}

	public override async void _Process(float delta)
	{
		base._Process(delta);
		(Material as ShaderMaterial).SetShaderParam("FogOfWarTexture", _fogOfWarViewport.GetTexture());
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
	}
}
