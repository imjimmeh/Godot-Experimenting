using FaffLatest.scripts.characters;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.state;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CSGFogOfWar : Spatial
{
    [Export]
    public ShaderMaterial FogOfWarMaterial { get; private set; }

    [Export]
    public float Y = 0.1f;

    private List<ShapeCharacterPos> _playerVisionShapes;

    private Vector3 _offset;
    public async void CreateFogOfWar()
    {
        _offset = new Vector3(25, 0, 25);
        this.Transform = new Transform(Transform.basis, _offset);

        var characterPositions = CharacterManager.Instance.PlayerCharacters.Select(character =>
        {
            var position = character.Body.Transform.origin;

            var vision = character.Stats.LineOfSight;

            character.Body.PathMover.Connect("_Character_ReachedPathPart", this, "_On_PlayerCharacter_ReachedPathPart");
            return (position, vision, character.Body.PathMover, character.Body);
        }).ToList();


        var fog = new CSGBox();
        fog.Width = 50;
        fog.Height = 2;
        fog.Depth = 50;


        var emptyPositions = new List<Vector3>();

        _playerVisionShapes = new List<ShapeCharacterPos>(characterPositions.Count);

        foreach (var position in characterPositions)
        {
            var playerVisionSphere = new CSGSphere();
            playerVisionSphere.Radius = position.vision;
            playerVisionSphere.Transform = new Transform(Quat.Identity, position.position - _offset);
            _playerVisionShapes.Add(new ShapeCharacterPos(playerVisionSphere, position.Body, position.PathMover, position.vision));

            playerVisionSphere.Operation = CSGShape.OperationEnum.Subtraction;
            playerVisionSphere.Material = FogOfWarMaterial;
            fog.AddChild(playerVisionSphere);
        }

        fog.Material = FogOfWarMaterial;

        CalculateAICharacterVisibilities(true);

        AddChild(fog);
    }

    private void CalculateAICharacterVisibilities(bool addSignals = false)
    {
        foreach (var aiChar in CharacterManager.Instance.AiCharacters)
        {
            CalculateVisibility(aiChar);

            if(addSignals)
                aiChar.Body.PathMover.Connect("_Character_ReachedPathPart", this, "_On_AiCharacter_ReachedPathPart");
        }
    }

    private void CalculateVisibility(Character aiChar)
    {
        if (_playerVisionShapes.Any(CharacterInVisionRange(aiChar)))
        {
            aiChar.Body.Visible = true;
        }
        else
        {
            aiChar.Body.Visible = false;
        }
    }

    private static System.Func<ShapeCharacterPos, bool> CharacterInVisionRange(Character aiChar)
        => sphere => (sphere.CharacterBody.Transform.origin - aiChar.Body.Transform.origin).Length() < sphere.Vision;

    private void _On_AiCharacter_ReachedPathPart(Node character, Vector3 part)
    {
        CalculateVisibility(character.GetParent().GetParent() as Character);
    }

    private void _On_PlayerCharacter_ReachedPathPart(Node character, Vector3 part)
    {
        var matchingShape = _playerVisionShapes.FirstOrDefault(c => c.CharacterPathMover.Equals(character));

        if (matchingShape == null)
            return;

        matchingShape.FogOfWarHider.Transform = new Transform(matchingShape.FogOfWarHider.Transform.basis, matchingShape.CharacterBody.Transform.origin - _offset);
        CalculateAICharacterVisibilities();
    }
}

internal class ShapeCharacterPos
{
    public CSGShape FogOfWarHider;
    public MovingKinematicBody CharacterBody;
    public PathMover CharacterPathMover;
    public float Vision;

    public ShapeCharacterPos(CSGShape fogOfWarHider, MovingKinematicBody body, PathMover mover, float vision)
    {
        FogOfWarHider = fogOfWarHider ?? throw new System.ArgumentNullException(nameof(fogOfWarHider));
        CharacterBody = body ?? throw new System.ArgumentNullException(nameof(body));
        CharacterPathMover = mover ?? throw new System.ArgumentNullException(nameof(mover));
        Vision = vision;
    }
}