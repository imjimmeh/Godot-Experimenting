using FaffLatest.scripts.movement;
using FaffLatest.scripts.shared;
using FaffLatest.scripts.world;
using Godot;
using System;
using System.Threading.Tasks;

public class WorldFloorMesh : MeshInstance
{
	[Export]
	public PackedScene DirtPatch { get;  private set; }

	[Export]
	public PackedScene[] Trees { get; private set; }
	public override void _Ready()
	{
		base._Ready();
		GenerateWorldObjects();
	}

	private async void GenerateWorldObjects()
	{
		var mesh = this.Mesh as PlaneMesh;

		var topLeft = (new Vector2(GlobalTransform.origin.x, GlobalTransform.origin.z) - mesh.Size).Ceil();

		for (float x = 0; x < mesh.Size.x; x += 2f)
		{
			for (float y = 0; y < mesh.Size.y; y += 2f)
            {
				Spatial spatial = null;

				var random = RandomHelper.RNG.RandiRange(0, 100);

				bool worldBlocker = false;
				if(random == 0)
				{
					spatial = DirtPatch.Instance() as Spatial;
				}
				else if(random < 10)
				{
					spatial = RandomHelper.GetRandomFromArray(Trees).Instance<Spatial>();
					worldBlocker = true;
				}

				if(spatial != null)
				{
					await CreateObject(spatial, topLeft, x, y, worldBlocker);
				}
            }
        }
	}

    private async Task CreateObject(Spatial spatial, Vector2 topLeft, float x, float y, bool isWorldBlocker)
    {
        var position = new Vector3(
            x: GetRandomisedPosition(topLeft.x, x),
            y: 0,
            z: GetRandomisedPosition(topLeft.y, y));

        var basis = new Basis(new Vector3(0, 1, 0), GetRandomisedPosition(0, 15));
        spatial.Transform = new Transform(basis, position);
        CallDeferred("add_child", spatial);

		if(!isWorldBlocker)
			return;

		await ToSignal(spatial, "ready");
		var wo = spatial.GetNode("StaticBody") as WorldObject;
		wo.RegisterWithAStar(AStarNavigator.Instance);
    }

    private static float GetRandomisedPosition(float value, float index)
		=> value + index + RandomHelper.RNG.RandfRange(0.0f, 0.01f);
}
