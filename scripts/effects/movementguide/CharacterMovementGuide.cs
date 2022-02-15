using System;
using System.Collections.Generic;
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
        private CharacterMovementGuideCell[] currentPath;

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

            Hide();
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
            CreateMeshes();
        }

        private void _On_Character_Selected(Character character)
        {
            if (character != parent)
            {
                Hide();
                return;
            }
            GD.Print("MoveGuide");

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
            Show();
        }

        private void _On_Character_SelectonCleared()
        {
            Hide();
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

        private Task<CharacterMovementGuideCell> ProcessCoordinates(float x, float y)
        {
            var currentVector = new Vector3(x: x, y: 0, z: y);

            bool withinMovementDistance = parent.ProperBody.MovementStats.IsCellWithinMovementDistance(Vector3.Zero, currentVector);

            if (!withinMovementDistance)
                return null;

            return BuildMesh(x, y, currentVector);
        }

        private async Task<CharacterMovementGuideCell> BuildMesh(float a, float b, Vector3 currentVector)
        {
            var mesh = this.CreateMeshInstanceForPosition(currentVector);

            this.ConnectCellSignals(mesh)
                .AddCellToArray(mesh, a, b);

            mesh.SetParentCharacterTransform(parent.ProperBody);

            Connect(SignalNames.MovementGuide.CELL_CALCULATE_VISIBLITY, mesh, CALCULATE_VISIBLITY);
            
            this.CallDeferred("AddMeshChild", mesh);

            return mesh;
        }

        private void AddMeshChild(Node mesh) => AddChild(mesh);


        private async void _On_Cell_Mouse_Entered(CharacterMovementGuideCell node)
        {
            var clearPathTask = ClearExistingPath();

            var result = await AStar.TryGetMovementPathAsync(body.Transform.origin, node.GlobalTransform.origin, parent.ProperBody.MovementStats.MaxMovementDistancePerTurn);

            if (result == null || !result.IsSuccess || result.Path.Length > parent.ProperBody.MovementStats.AmountLeftToMoveThisTurn)
                return;

            currentPath = new CharacterMovementGuideCell[result.Path.Length];

            int pathCount = 0;

            await clearPathTask;
            
            for (var x = 0; x < result.Path.Length; x++)
            {
                var nextCell = GetCellAndSetAsPathPart(result.Path[x] - body.GlobalTransform.origin);
                currentPath[x] = nextCell;
                pathCount++;
            }

            System.Array.Resize(ref currentPath, pathCount);
        }

        private CharacterMovementGuideCell GetCellAndSetAsPathPart(Vector3 targetVector)
        {
            var newPathCell = GetCellFromLocalTransform(targetVector);

            if (newPathCell == null)
            {
                return null;
            }

            newPathCell.CallDeferred("SetPartOfPath",true);
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

        private async Task ClearExistingPath()
        {
            if (currentPath != null)
            {
                //GD.Print($"Clearing path");
                for (var x = 0; x < currentPath.Length; x++)
                {
                    if (currentPath[x] == null)
                        continue;

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

        public void AddCellToArray(CharacterMovementGuideCell cell, float a, float b)
        {
            existingMovementGuide[new Vector2(a, b)] = cell;
        }

        private void _On_Character_SelectionCleared()
        {
            Hide();
        }
    }
}
