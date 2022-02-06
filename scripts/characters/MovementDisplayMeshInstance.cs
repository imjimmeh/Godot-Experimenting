using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using Godot;
using System;

public class MovementDisplayMeshInstance : MeshInstance
{
	private Character parent;
	private KinematicBody body;

	private PlaneMesh mesh;

	public override void _Ready()
	{
		base._Ready();

		GetNodes();
		ConnectSignals();

		Visible = false;
	}

	private void GetNodes()
	{
		parent = GetNode<Character>("../../");
		body = parent.GetNode<KinematicBody>("KinematicBody");
		mesh = Mesh as PlaneMesh;
	}

	private void ConnectSignals()
	{
		var gsm = GetNode("/root/Root/Systems/GameStateManager");
		gsm.Connect(SignalNames.Characters.SELECTED, this, SignalNames.Characters.SELECTED_METHOD);
		gsm.Connect(SignalNames.Characters.SELECTION_CLEARED, this, SignalNames.Characters.SELECTION_CLEARED_METHOD);
	}

	public void ChangeSize()
	{
		GD.Print($"{parent.Stats.MovementDistance} vs {parent.Stats.Get("MovementDistance")}");
		mesh.Size = new Vector2(parent.Stats.MovementDistance * 2, parent.Stats.MovementDistance * 2);
		GD.Print($"Our new size is {mesh.Size}");
	}

	private void _On_Character_Selected(Character character)
	{
		GD.Print("Movement mesh received signal");
		if(character != parent)
		{
			GD.Print("Character is not same");

			return;
		}

		ChangeSize();
		UpdateShaderParams();

		Visible = true;
		GD.Print("Should see me now?");
	}

	private void _On_Character_SelectionCleared()
	{
		Visible = false;
		GD.Print("Cya");
	}

	public override void _Process(float delta)
	{
		base._Process(delta);
	}

	private void UpdateShaderParams()
	{
		var currentMaterial = GetActiveMaterial(0) as ShaderMaterial;

		var playerPosition = body.Transform.origin;

		currentMaterial.SetShaderParam("playerPosition", playerPosition);
		currentMaterial.SetShaderParam("playerMovementDistance", parent.Stats.MovementDistance);

		GD.Print(currentMaterial.GetShaderParam("playerPosition"));
		GD.Print(currentMaterial.GetShaderParam("playerMovementDistance"));
	}
}
