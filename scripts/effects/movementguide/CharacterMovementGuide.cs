using System;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;
using Godot.Collections;

namespace FaffLatest.scripts.effects.movementguide
{
    public class CharacterMovementGuide : Spatial
    {
        private CharacterMovementGuideCell[] currentPath;

        [Signal]
        public delegate void _Character_MoveOrder(Vector3 position);

        private Node currentlyHighlightedNode;

        private AStarNavigator astar;

        private Dictionary<Vector2, CharacterMovementGuideCell> existingMovementGuide;
        private int movementGuideCount = 0;

        private Character parent;
        private KinematicBody body;

        private Basis initialRotation;

        [Export]
        public Material MeshMaterial;

        [Export]
        public PackedScene MovementGuideCellScene;

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
            body = parent.Body as KinematicBody;
        }

        private void ConnectSignals()
        {
            var gsm = GetNode("/root/Root/Systems/GameStateManager");
            gsm.Connect(SignalNames.Characters.SELECTED, this, SignalNames.Characters.SELECTED_METHOD);
            gsm.Connect(SignalNames.Characters.SELECTION_CLEARED, this, SignalNames.Characters.SELECTION_CLEARED_METHOD);

            parent.Connect("_Character_Ready", this, "_On_Character_Ready");

            Connect("_Character_MoveOrder", GetNode("/root/Root/Systems/InputManager"), "_On_Character_MoveOrder");
        }

        private void _On_Character_Ready(Node character)
        {
            CreateMeshes();
        }

        private void _On_Character_SelectionCleared()
        {
            Hide();
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

            foreach (var cell in existingMovementGuide)
                cell.Value.CalculateVisiblity(parent.Stats.AmountLeftToMoveThisTurn);

            Show();
        }

        public void CreateMeshes()
        {
            var halfMovementDistance = parent.Stats.MaxMovementDistancePerTurn / 2;

            var pos = Vector3.Zero;

            (var x, var z) = (pos.x, pos.z);

            (var maxX, var maxZ) = GetMaxPossibleValues(parent, x, z);

            (var minX, var minZ) = GetMinimumPossibleValues(parent, x, z);

            int totalPossibleMeshCount = (int)((maxX - minX) * (maxZ - minZ));

            existingMovementGuide = new Dictionary<Vector2, CharacterMovementGuideCell>();

            for (var a = minX; a <= maxX; a += astar.GridSize)
            {
                for (var b = minZ; b <= maxZ; b += astar.GridSize)
                {
                    ProcessCoordinates(pos, a, b);
                }
            }
        }

        private void ProcessCoordinates(Vector3 pos, float a, float b)
        {
            var currentVector = new Vector3(a, pos.y, b);

            var distanceToCharacter = (pos - currentVector).Abs();

            var distanceCalc = distanceToCharacter.x + distanceToCharacter.z;
            var tooFar = distanceCalc > parent.Stats.MaxMovementDistancePerTurn;

            if (tooFar)
            {
                return;
            }

            BuildMesh(a, b, currentVector);

            return;
        }

        private void BuildMesh(float a, float b, Vector3 currentVector)
        {
            var meshInstance = CreateMeshInstance(currentVector);

            meshInstance.Connect("_Mouse_Entered", this, "_On_Cell_Mouse_Entered");
            meshInstance.Connect("_Mouse_Exited", this, "_On_Cell_Mouse_Exited");

            AddChild(meshInstance);
            existingMovementGuide[new Vector2(a, b)] = meshInstance;
            movementGuideCount++;
        }

        private CharacterMovementGuideCell CreateMeshInstance(Vector3 currentVector)
        {
            var meshInstanceNode = MovementGuideCellScene.Instance();

            var meshInstance = meshInstanceNode as CharacterMovementGuideCell;
            meshInstance.Mesh.ResourceLocalToScene = true;

            meshInstance.Transform = new Transform(new Quat(0, 100,0,0), currentVector);
            meshInstance.AStar = astar;

            meshInstance.Name = currentVector.ToString();
            return meshInstance;
        }

        private void _On_Cell_Mouse_Entered(Node node)
        {
            GD.Print($"Entered {node.Name}");
            if(node is CharacterMovementGuideCell cell)
            {
                GD.Print($"cell");
                var path = astar.GetMovementPath(body.Transform.origin, cell.GlobalTransform.origin, parent.Stats.MaxMovementDistancePerTurn);

                ClearExistingPath();

                if (path == null || path.Length == 0)
                    return;

                currentPath = new CharacterMovementGuideCell[path.Length];

                for(var x = 0; x < path.Length; x++)
                {
                    GD.Print($"Path is : {path[x]?.Destination}");
                    currentPath[x] = GetCellAndSetAsPathPart(path[x].Destination - body.Transform.origin);
                }
            }
        }

        private CharacterMovementGuideCell GetCellAndSetAsPathPart(Vector3 targetVector)
        {
            var newPathCell = GetCellFromLocalTransform(targetVector);

            if (newPathCell == null)
                throw new Exception($"Error - could not find cell for {targetVector}");

            newPathCell.SetPartOfPath(true);

            return newPathCell;
        }

        private CharacterMovementGuideCell GetCellFromLocalTransform(Vector3 targetVector)
        {
            (int intX, int intY) = ((int)targetVector.x, (int)targetVector.z);
            var vector = new Vector2(intX, intY);

            if (existingMovementGuide.TryGetValue(vector, out CharacterMovementGuideCell foundCell))
            {
                return foundCell;
            }

            return null;
        }

        private void ClearExistingPath()
        {
            if (currentPath != null)
            {
                GD.Print($"Clearing path");
                for (var x = 0; x < currentPath.Length; x++)
                {
                    currentPath[x].SetPartOfPath(false);
                }
            }
        }

        private void _On_Cell_Mouse_Exited(Node node)
        {
            GD.Print($"Exited {node.Name}");
        }       

        private void _On_Cell_Clicked(Node node, InputEventMouseButton mouseInput)
        { 
            if(mouseInput.ButtonIndex == 2)
            {
                var cell = node as CharacterMovementGuideCell;
                var worldPos = cell.Transform.origin + body.Transform.origin;
                EmitSignal("_Character_MoveOrder", worldPos);
            }
        }


        private static (float x, float z) GetMinimumPossibleValues(Character character, float x, float z)
        {
            (var minX, var minZ) = (x - character.Stats.MaxMovementDistancePerTurn, z - character.Stats.MaxMovementDistancePerTurn);
            return (minX, minZ);
        }

        private static (float x, float z) GetMaxPossibleValues(Character character, float x, float z)
        {
            (var maxX, var maxZ) = (x + character.Stats.MaxMovementDistancePerTurn, z + character.Stats.MaxMovementDistancePerTurn);
            return (maxX, maxZ);
        }

        private static float Clamp(float current, float max) => current > max ? max : current;
        private static float Min(float current, float min) => current < min ? min : current;

    }
}
