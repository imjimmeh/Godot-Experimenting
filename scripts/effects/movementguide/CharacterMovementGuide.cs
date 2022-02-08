using System;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.effects.movementguide
{
    public class CharacterMovementGuide : Spatial
    {
        private bool isVisible = false;
        private AStarNavigator astar;

        private CharacterMovementGuideCell[] existingMovementGuide;

        private Character parent;
        private KinematicBody body;

        private Basis initialRotation;

        [Export]
        public Material MeshMaterial;

        public override void _Ready()
        {
            base._Ready();

            astar = GetNode<AStarNavigator>("/root/Root/Systems/AStarNavigator");
            parent = GetNode<Character>("../../");
            GetBody();

            ConnectSignals();
            Hide();
        }

        private void GetBody()
        {
            body = parent.CharacterKinematicBody as KinematicBody;
        }

        private void ConnectSignals()
        {
            var gsm = GetNode("/root/Root/Systems/GameStateManager");
            gsm.Connect(SignalNames.Characters.SELECTED, this, SignalNames.Characters.SELECTED_METHOD);
            gsm.Connect(SignalNames.Characters.SELECTION_CLEARED, this, SignalNames.Characters.SELECTION_CLEARED_METHOD);

            parent.Connect("_Character_Ready", this, "_On_Character_Ready");
        }

        private void _On_Character_Ready(Node character)
        {
            CreateMeshes();
        }

        private void _On_Character_SelectionCleared()
        {
            Hide();
            isVisible = false;
        }

        private void _On_Character_Selected(Node character)
        {
            if(character != parent)
            {
                _On_Character_SelectionCleared();                   
                return;
            }

            if (body == null)
                GetBody();

            if (body == null)
                throw new Exception("Player body is null?");

            RotationDegrees = body.RotationDegrees * -1;
            for (var x = 0; x < existingMovementGuide.Length; x++)
            {
                existingMovementGuide[x].CalculateVisiblity(body.Transform.origin);
            }

            Show();
        }

        public void CreateMeshes()
        {
            var halfMovementDistance = parent.Stats.MovementDistance / 2;

            var pos = Vector3.Zero;

            (var x, var z) = (pos.x, pos.z);

            (var maxX, var maxZ) = GetMaxPossibleValues(parent, x, z);

            (var minX, var minZ) = GetMinimumPossibleValues(parent, x, z);

            int meshCount = 0;
            int totalPossibleMeshCount = (int)((maxX - minX) * (maxZ - minZ));

            existingMovementGuide = new CharacterMovementGuideCell[totalPossibleMeshCount];
            for (var a = minX; a <= maxX; a += astar.GridSize)
            {
                for (var b = minZ; b <= maxZ; b += astar.GridSize)
                {
                    (var success, var count) = ProcessCoordinates(pos, meshCount, a, b);

                    meshCount = count;
                }
            }

            Array.Resize(ref existingMovementGuide, meshCount);
        }

        private (bool valid, int meshCount) ProcessCoordinates(Vector3 pos, int meshCount, float a, float b)
        {
            var currentVector = new Vector3(a, pos.y, b);

            var distanceToCharacter = (pos - currentVector).Abs();

            var distanceCalc = distanceToCharacter.x + distanceToCharacter.z;
            var tooFar = distanceCalc > parent.Stats.MovementDistance;

            if (tooFar)
            {
                return (false, meshCount);
            }

            var meshInstance = CreateMesh(currentVector);
            AddChild(meshInstance);
            existingMovementGuide[meshCount] = meshInstance;

            meshCount++;

            return (true, meshCount);
        }

        private CharacterMovementGuideCell CreateMesh(Vector3 currentVector)
        {
            var meshInstance = new CharacterMovementGuideCell();
            meshInstance.Transform = new Transform(new Quat(0, 100,0,0), currentVector);
            meshInstance.Mesh = new PlaneMesh
            {
                Size = new Vector2(1, 1)
            };

            meshInstance.AStar = astar;
            var shader = MeshMaterial as ShaderMaterial;
            shader.SetShaderParam("Colour", new Vector3(1.0f, 0.0f, 0.0f));
            shader.SetShaderParam("Alpha", 0.8f);

            meshInstance.SetSurfaceMaterial(0, shader);
            return meshInstance;
        }


        private static (float x, float z) GetMinimumPossibleValues(Character character, float x, float z)
        {
            (var minX, var minZ) = (x - character.Stats.MovementDistance, z - character.Stats.MovementDistance);
            return (minX, minZ);
        }

        private static (float x, float z) GetMaxPossibleValues(Character character, float x, float z)
        {
            (var maxX, var maxZ) = (x + character.Stats.MovementDistance, z + character.Stats.MovementDistance);
            return (maxX, maxZ);
        }

        private static float Clamp(float current, float max) => current > max ? max : current;
        private static float Min(float current, float min) => current < min ? min : current;

    }
}
