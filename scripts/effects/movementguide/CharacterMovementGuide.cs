using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.shared;
using Godot;
using Godot.Collections;

namespace FaffLatest.scripts.effects.movementguide
{
    public class CharacterMovementGuide : Spatial
    {
        private const string CALCULATE_VISIBLITY = "CalculateVisiblity";
        private HashSet<CharacterMovementGuideCell> currentPath;

        [Signal]
        public delegate void _Character_MoveOrder(Vector3 position);

        [Signal]
        public delegate void _Character_MoveGuide_CalculateCellVisiblity(int amountLeftToMoveThisTurn);

        private Godot.Collections.Dictionary<Vector2, CharacterMovementGuideCell> existingMovementGuide;

        private Character parent;
        private KinematicBody body;

        [Export]
        public PackedScene MovementGuideCellScene { get; private set; }
        public AStarNavigator AStar { get; private set; }

        public override void _Ready()
        {
            base._Ready();
        }

        public void Initialise()
        {
            AStar = GetNode<AStarNavigator>(AStarNavigator.GLOBAL_SCENE_PATH);

            GetCharacterNode();
            GetBody();

            ConnectSignals();

            Show();
            CallDeferred("hide");
        }

        private void GetCharacterNode()
        {
            parent = GetNode<Character>("../../");
        }

        private void GetBody()
        {
            body = parent.ProperBody;
        }

        private void ConnectSignals()
        {
            var gsm = GetNode(NodeReferences.Systems.GAMESTATE_MANAGER);
            gsm.Connect(SignalNames.Characters.SELECTED, this, SignalNames.Characters.SELECTED_METHOD);
            gsm.Connect(SignalNames.Characters.SELECTION_CLEARED, this, SignalNames.Characters.SELECTION_CLEARED_METHOD);

            Connect(SignalNames.Characters.MOVE_ORDER, GetNode(NodeReferences.Systems.INPUT_MANAGER), SignalNames.Characters.MOVE_ORDER_METHOD);

        }

        private void _On_Character_Ready(Node character)
        {
            Initialise();
            CallDeferred("CreateMeshes");
            currentPath = new HashSet<CharacterMovementGuideCell>();
        }

        private void _On_Character_Selected(Character character)
        {
            if (character != parent)
            {
                CallDeferred("hide");
                return;
            }

            if (body == null)
                GetBody();

            if (body == null)
                throw new Exception("Player body is null?");

            RotationDegrees = body.RotationDegrees * -1;
            CallDeferred("ShowCells");
        }

        private void ShowCells()
        {
            EmitSignal("_Character_MoveGuide_CalculateCellVisiblity", parent.ProperBody.MovementStats.AmountLeftToMoveThisTurn);
            CallDeferred("show");
        }

        public async void CreateMeshes()
        {
            var max = parent.ProperBody.MovementStats.MaxMovementDistancePerTurn;
            var min = max * -1;

            existingMovementGuide = new Godot.Collections.Dictionary<Vector2, CharacterMovementGuideCell>();

            for (var x = min; x <= max; x += AStar.GridSize)
            {
                for (var y = min; y <= max; y += AStar.GridSize)
                {
                    ProcessCoordinates(x, y);
                }
            }
        }

        private void ProcessCoordinates(float x, float y)
        {
            var currentVector = new Vector3(x: x, y: 0, z: y);

            bool withinMovementDistance = parent.ProperBody.MovementStats.IsCellWithinMovementDistance(Vector3.Zero, currentVector);

            if (!withinMovementDistance)
                return;

            BuildMesh(x, y, currentVector);
        }

        private void BuildMesh(float a, float b, Vector3 currentVector)
        {
            var mesh = this.CreateMeshInstanceForPosition(currentVector);

            this.ConnectCellSignals(mesh)
                .AddCellToArray(mesh, a, b);

            mesh.SetParentCharacterTransform(parent.ProperBody);
            mesh.CallDeferred("show");
            Connect(SignalNames.MovementGuide.CELL_CALCULATE_VISIBLITY, mesh, CALCULATE_VISIBLITY);
            
            CallDeferred("add_child", mesh);
        }

        private async void _On_Cell_Mouse_Entered(CharacterMovementGuideCell node)
        {
            var result = await AStar.TryGetMovementPathAsync(body.Transform.origin, node.GlobalTransform.origin, parent.ProperBody.MovementStats.MaxMovementDistancePerTurn);
            
            if (!result.IsSuccess)
            {
                ClearExistingPath();
                return;
            }

            var newPath = new HashSet<CharacterMovementGuideCell>(result.Path.Length);


            for (var x = 0; x < result.Path.Length; x++)
            {
                var matchingCell = GetCellFromLocalTransform(result.Path[x] - body.GlobalTransform.origin);
                newPath.Add(matchingCell);
                matchingCell.SetPartOfPath(true);

                if(currentPath.Contains(matchingCell))
                    currentPath.Remove(matchingCell);
            }

            ClearExistingPath();

            currentPath = newPath;
        }

        private bool IsValidPath(GetMovementPathResult result)
        {
            return result == null || !result.IsSuccess || result.Path == null || result.Path.Length > parent.ProperBody.MovementStats.AmountLeftToMoveThisTurn
            || result.Path.Length == 0;
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
                foreach(var path in currentPath)
                {
                    path.SetPartOfPath(false);
                }
            }

            currentPath.Clear();
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
                CallDeferred("hide");
            }
        }

        public void AddCellToArray(CharacterMovementGuideCell cell, float a, float b)
        {
            existingMovementGuide[new Vector2(a, b)] = cell;
        }

        private void _On_Character_SelectionCleared()
        {
            CallDeferred("hide");
        }
    }
}
