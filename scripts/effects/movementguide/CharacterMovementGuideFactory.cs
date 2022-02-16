using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;
using Godot.Collections;

namespace FaffLatest.scripts.effects.movementguide
{
    public static class CharacterMovementGuideFactory
    {
        public static void CreateMeshes(this CharacterMovementGuide cmg, ref Dictionary<Vector2, CharacterMovementGuideCell> movementGuide, Character character)
        {
            var max = character.ProperBody.MovementStats.MaxMovementDistancePerTurn;
            var min = max * -1;

            movementGuide = new Dictionary<Vector2, CharacterMovementGuideCell>();

            for (var x = min; x <= max; x += cmg.AStar.GridSize)
            {
                for (var y = min; y <= max; y += cmg.AStar.GridSize)
                {
                    cmg.ProcessCoordinates(character, x, y);
                }
            }
        }

        private static void ProcessCoordinates(this CharacterMovementGuide cmg, Character character, float x, float y)
        {
            var currentVector = new Vector3(x: x, y: 0, z: y);

            bool withinMovementDistance = character.ProperBody.MovementStats.IsCellWithinMovementDistance(Vector3.Zero, currentVector);

            if (!withinMovementDistance)
                return;

            cmg.BuildMesh(character, x, y, currentVector);
        }

        public static void BuildMesh(this CharacterMovementGuide cmg, Character character, float a, float b, Vector3 currentVector)
        {
            var mesh = cmg.CreateMeshInstanceForPosition(currentVector);

            cmg.ConnectCellSignals(mesh)
                .AddCellToArray(mesh, a, b);

            mesh.SetParentCharacterTransform(character.ProperBody);
            mesh.CallDeferred("show");
            
            cmg.Connect(SignalNames.MovementGuide.CELL_CALCULATE_VISIBLITY, mesh, nameof(CharacterMovementGuideCell.CalculateVisiblity));       
            cmg.CallDeferred("add_child", mesh);
        }
    }
}