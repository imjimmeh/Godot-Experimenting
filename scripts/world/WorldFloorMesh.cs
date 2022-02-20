using FaffLatest.scripts.shared;
using Godot;
using System;

public class WorldFloorMesh : MeshInstance
{
	[Export]
	public PackedScene DirtPatch { get;  private set; }

	public override void _Ready()
	{
		base._Ready();
		GenerateGrass();
	}

	private void GenerateGrass()
	{
		var mesh = this.Mesh as PlaneMesh;

		var topLeft = (new Vector2(GlobalTransform.origin.x, GlobalTransform.origin.z) - mesh.Size).Ceil();

		for (float x = 0; x < mesh.Size.x; x += 2f)
		{
			for (float y = 0; y < mesh.Size.y; y += 2f)
			{

				bool shouldDrawDirtpatch = RandomHelper.RNG.RandiRange(0, 100) < 2;

				if (!shouldDrawDirtpatch)
					continue;

				var position = new Vector3(
					x: GetRandomisedPosition(topLeft.x, x),
					y: 0,
					z: GetRandomisedPosition(topLeft.y, y));

				var basis = new Basis(new Vector3(0, 1, 0), GetRandomisedPosition(0, 15));

				var newGrass = DirtPatch.Instance() as MeshInstance;

				newGrass.Transform = new Transform(basis, position);
				CallDeferred("add_child", newGrass);

			}
		}
	}

	private static float GetRandomisedPosition(float value, float index)
		=> value + index + RandomHelper.RNG.RandfRange(0.0f, 0.01f);
}
