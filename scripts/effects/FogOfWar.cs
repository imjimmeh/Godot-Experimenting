using FaffLatest.scripts.state;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FogOfWar : MeshInstance
{
    [Export]
    public ShaderMaterial FogOfWarMaterial { get; private set; }

    [Export]
    public float Y = 0.1f;

    public void CreateFogOfWar()
    {
        var characterPositions = CharacterManager.Instance.PlayerCharacters.Select(character =>
        {
            var position = character.Body.Transform.origin;

            var vision = character.Stats.LineOfSight;

            character.Body.PathMover.Connect("_Character_ReachedPathPart", this, "_On_Character_ReachedPathPart");
            return (position, vision);
        }).ToList();


        var surfaceTool = new SurfaceTool();
        surfaceTool.SetMaterial(FogOfWarMaterial);

        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        surfaceTool.AddColor(Colors.Red);
        surfaceTool.AddUv(new Vector2(0, 0));

        var emptyPositions = new List<Vector3>();

        for (var x = 0; x < 50; x++)
        {
            for (var z = 0; z < 50; z++)
            {
                if (characterPositions.Any((v) =>
                {
                    var distance = v.position.DistanceTo(new Vector3(x, v.position.y, z));

                    return distance <= v.vision;
                }))
                {
                    emptyPositions.Add(new Vector3(x, Y, z));
                    continue;
                }

                surfaceTool.CreateQuad(new Vector3(x, Y, z), 1f, 1f, 1f, FaffLatest.scripts.shared.MeshFacingDirection.Up);
            }
        }

        foreach(var emptyPos in emptyPositions)
        {
            bool createUpWall = !emptyPositions.Any(empty => (empty.x == emptyPos.x) && emptyPos.z > empty.z);
            bool createRightWall = !emptyPositions.Any(empty => (empty.z == emptyPos.z) && emptyPos.x > empty.x);

            if (createUpWall)
            {
                GD.Print($"Could not find any positiosn further north for {emptyPos}");
                surfaceTool.CreateQuad(new Vector3(emptyPos.x, 0, emptyPos.z), 1f, Y, 1f, FaffLatest.scripts.shared.MeshFacingDirection.Front);
            }
            
            if(createRightWall)
            {
                GD.Print($"Could not find any positiosn further right for {emptyPos}");
                surfaceTool.CreateQuad(new Vector3(emptyPos.x, 0, emptyPos.z), 1f, Y, 1f, FaffLatest.scripts.shared.MeshFacingDirection.Left);
            }
        }

        this.Mesh = surfaceTool.Commit();
        this.Mesh.SurfaceSetMaterial(0, FogOfWarMaterial);
    }


    private void _On_Character_ReachedPathPart(Node character, Vector3 part)
    {
        CreateFogOfWar();
        //var test = character.GetParent<Spatial>().GlobalTransform.origin;
        //Vector3 position = new Vector3(part.x, y, part.z);
        //foreach(var vertice in vertices)
        //{
        //    var matches = vertice == position;
        //    var matchestwo = vertice.Equals(position);
        //    var testthree = vertice.IsEqualApprox(position);
        //    var testfour = vertice.x == position.x && vertice.z == position.z;
        //    var xMatches = vertice.x == position.x;
        //    var zMatches = vertice.z == position.z;

        //    if(matches && matchestwo && testthree){
        //        var asdasd = "";
        //    }
        //    else if(matchestwo)
        //    {
        //        var asdasd = "";
        //    }
        //    else if(testthree)
        //    {
        //        var gsdfsafsd = "";
        //    }
        //    else if(matches)
        //    {
        //        var dfdsasafs = "";
        //    }
        //    else if(testfour)
        //    {
        //        var asdasdsa = "";
        //    }
        //    else if(xMatches || zMatches)
        //    {
        //        if(xMatches && zMatches)
        //        {
        //            var btrbtrb = "";
        //        }
        //        var sdasdasdasd = "";
        //    }           
        //}
        //var removed = vertices.Remove(position);
        //BuildMesh();
    }
}
