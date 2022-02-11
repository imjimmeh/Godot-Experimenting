using System;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;
using Godot.Collections;

namespace FaffLatest.scripts.effects.movementguide
{
    public class CharacterMovementGuide : Spatial, ICharacterMovementGuide
    {
        private CharacterMovementGuideCell[] currentPath;

        [Signal]
        public delegate void _Character_MoveOrder(Vector3 position);


        private Dictionary<Vector2, CharacterMovementGuideCell> existingMovementGuide;
        private int movementGuideCount = 0;

        private Character parent;
        private KinematicBody body;

        [Export]
        public PackedScene MovementGuideCellScene { get; private set; }
        public AStarNavigator AStar { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            AStar = GetNode<AStarNavigator>(AStarNavigator.GLOBAL_SCENE_PATH);

            GetCharacterNode();
            GetBody();

            ConnectSignals();

            Hide();
        }

        private void GetCharacterNode()
        {
            parent = GetNode<Character>("../../");
        }

        private void GetBody()
        {
            GD.Print($"Getting body");
            body = parent.Body as KinematicBody;
        }

        private void ConnectSignals()
        {
            var gsm = GetNode(NodeReferences.Systems.GAMESTATE_MANAGER);
            gsm.Connect(SignalNames.Characters.SELECTED, this, SignalNames.Characters.SELECTED_METHOD);
            //gsm.Connect(SignalNames.Characters.SELECTION_CLEARED, this, SignalNames.Characters.SELECTION_CLEARED_METHOD);

            parent.Connect(SignalNames.Characters.READY, this, SignalNames.Characters.READY_METHOD);

            Connect(SignalNames.Characters.MOVE_ORDER, GetNode(NodeReferences.Systems.INPUT_MANAGER), SignalNames.Characters.MOVE_ORDER_METHOD);
        }

        private void _On_Character_Ready(Node character)
        {
            CreateMeshes();
        }

        private void _On_Character_Selected(Node character)
        {
            if (character != parent)
            {
                Hide();
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

            for (var a = minX; a <= maxX; a += AStar.GridSize)
            {
                for (var b = minZ; b <= maxZ; b += AStar.GridSize)
                {
                    ProcessCoordinates(pos, a, b);
                }
            }
        }

        private void ProcessCoordinates(Vector3 pos, float a, float b)
        {
            var currentVector = new Vector3(a, pos.y, b);

            bool withinMovementDistance = parent.Stats.IsCellWithinMovementDistance(pos, currentVector);

            if (!withinMovementDistance)
            {
                return;
            }

            BuildMesh(a, b, currentVector);

            return;
        }

        private void BuildMesh(float a, float b, Vector3 currentVector)
        {
            var mesh = this.CreateMeshInstanceForPosition(currentVector);

            this.ConnectCellSignals(mesh)
                .AddCellToArray(mesh, a, b);

            AddChild(mesh);
        }


        private void _On_Cell_Mouse_Entered(Node node)
        {
            //GD.Print($"Entered {node.Name}");
            if (node is CharacterMovementGuideCell cell)
            {
                //GD.Print($"cell");
                var path = AStar.GetMovementPathNew(body.Transform.origin, cell.GlobalTransform.origin, parent.Stats.MaxMovementDistancePerTurn);

                ClearExistingPath();

                if (path == null || path.Length == 0)
                    return;

                currentPath = new CharacterMovementGuideCell[path.Length];
                int pathCount = 0;
                for (var x = 0; x < path.Length; x++)
                {
                    if (path[x] == null)
                    {
                        break;
                    }

                    currentPath[pathCount] = GetCellAndSetAsPathPart(path[x] - body.Transform.origin);
                    pathCount++;
                }

                System.Array.Resize(ref currentPath, pathCount);

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
                //GD.Print($"Clearing path");
                for (var x = 0; x < currentPath.Length; x++)
                {
                    currentPath[x].SetPartOfPath(false);
                }
            }
        }

        private void _On_Cell_Mouse_Exited(Node node)
        {
            //GD.Print($"Exited {node.Name}");
        }

        private void _On_Cell_Clicked(Node node, InputEventMouseButton mouseInput)
        {
            if (mouseInput.ButtonIndex == 2)
            {
                var cell = node as CharacterMovementGuideCell;
                var worldPos = cell.GlobalTransform.origin;
                EmitSignal(SignalNames.Characters.MOVE_ORDER, worldPos);
                Hide();
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

        public void AddCellToArray(CharacterMovementGuideCell cell, float a, float b)
        {
            existingMovementGuide[new Vector2(a, b)] = cell;
            movementGuideCount++;
        }
    }
}
