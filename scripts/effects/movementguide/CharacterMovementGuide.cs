using System;
using System.Collections.Generic;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.ui;
using Godot;

namespace FaffLatest.scripts.effects.movementguide
{
    public class CharacterMovementGuide : Spatial
    {        
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

        private Thread thread = new Thread();

        private Control pathLengthDisplay;

        public override void _Ready()
        {
            base._Ready();
        }

        public void Initialise()
        {
            AStar = AStarNavigator.Instance;

            GetCharacterNode();
            GetBody();

            ConnectSignals();
            Show();
            CallDeferred("hide");
        }

        private void GetCharacterNode()
        {
            parent = GetParent().GetParent<Character>();
        }

        private void GetBody()
        {
            body = parent.Body;
        }

        private void ConnectSignals()
        {
            var gsm = GetNode(NodeReferences.Systems.GAMESTATE_MANAGER);
            gsm.Connect(SignalNames.Characters.SELECTED, this, SignalNames.Characters.SELECTED_METHOD);
            gsm.Connect(SignalNames.Characters.SELECTION_CLEARED, this, SignalNames.Characters.SELECTION_CLEARED_METHOD);

            Connect(SignalNames.Characters.MOVE_ORDER, GetNode(NodeReferences.Systems.INPUT_MANAGER), SignalNames.Characters.MOVE_ORDER_METHOD);

            parent.Connect("_Character_Attacking", this, "_On_Character_Attacking");
        }

        private void _On_Character_Ready(Node character)
        {
            Initialise();
            currentPath = new HashSet<CharacterMovementGuideCell>();
            this.CreateMeshes(ref existingMovementGuide, parent);
        }

        private void _On_Character_Selected(Character character)
        {
            if (character != parent)
            {
                _On_Character_SelectionCleared();
                return;
            }
            
            RotationDegrees = body.RotationDegrees * -1;

            if(thread.IsActive())
                thread.WaitToFinish();

            thread.Start(this, "ShowCells");
        }

        private void ShowCells()
        {
            var amountLeftToMoveThisTurn = parent.Body.MovementStats.AmountLeftToMoveThisTurn;
            
            if(amountLeftToMoveThisTurn == 0 && parent.Stats.EquippedWeapon.AttacksLeftThisTurn == 0)
                return;
                
            foreach(var pathPart in existingMovementGuide)
            {
                pathPart.Value.CalculateVisiblity(amountLeftToMoveThisTurn, parent.Stats.EquippedWeapon.Range);
            }

            Show();
        }

        private void _On_Cell_Mouse_Entered(CharacterMovementGuideCell node)
        {
            ClearLabel();

            var result = AStar.TryGetMovementPath(
                start: body.GlobalTransform.origin,
                end: node.GlobalTransform.origin,
                character: parent);

            if (!result.CanFindPath)
            {
                ClearExistingPath();
                return;
            }

            var newPath = new HashSet<CharacterMovementGuideCell>(result.Path.Length);

            
            for (var x = 0; x < result.Path.Length; x++)
            {
                ProcessNewPathPart(result, newPath, x);
            }

            if(result.Path.Length > 0)
            {
                CreateLengthLabel(result);
            }
            
            ClearExistingPath();
            currentPath = newPath;
        }

        private void CreateLengthLabel(GetMovementPathResult result)
        {
            Vector3 position = result.Path[result.Path.Length - 1];

            pathLengthDisplay = UIManager.Instance.CreateLabelAtWorldPosition(result.Path.Length.ToString(),
            new FontValues(Colors.White, 16, Colors.Black, 1), position);
        }

        private void ClearLabel()
        {
            if (pathLengthDisplay != null)
            {
                pathLengthDisplay.QueueFree();
                pathLengthDisplay = null;
            }
        }

        private void ProcessNewPathPart(GetMovementPathResult result, HashSet<CharacterMovementGuideCell> newPath, int x)
        {
            var matchingCell = GetCellFromLocalTransform(result.Path[x] - body.GlobalTransform.origin);
            newPath.Add(matchingCell);
            matchingCell.SetPartOfPath(true);

            if (currentPath.Contains(matchingCell))
                currentPath.Remove(matchingCell);
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
        }

        private void _On_Cell_Clicked(Node node, InputEventMouseButton mouseInput)
        {
            if (mouseInput.ButtonIndex == 2)
            {
                ClearLabel();
                var cell = node as CharacterMovementGuideCell;
                var worldPos = cell.GlobalTransform.origin;
                EmitSignal(SignalNames.Characters.MOVE_ORDER, worldPos);
                Hide();
            }
        }

        public void AddCellToArray(CharacterMovementGuideCell cell, float a, float b) =>existingMovementGuide[new Vector2(a, b)] = cell;

        private void _On_Character_SelectionCleared()
        {
            if(pathLengthDisplay != null)
            {
                pathLengthDisplay.QueueFree();
                pathLengthDisplay = null;
            }

            Hide();
        }

        private void _On_Character_Attacking(Node attacker, Node target)
        {
            _On_Character_SelectionCleared();
        }
    }
}
