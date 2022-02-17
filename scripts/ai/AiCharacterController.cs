using System;
using System.Collections.Generic;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.ai
{
    public class AiCharacterController : Node
    {
        [Signal]
        public delegate void _AiCharacter_TurnFinished(Character character);

        private Character parent;
        private PathMover pathMover;
        private bool isActiveCharacter;


        private Node currentAttackTarget;
        private Vector3? currentMovementDestination;
        private bool haveMovementPath => pathMover.Path != null;
        public bool HaveAttackTarget => currentAttackTarget != null;
        public bool HaveMovementTarget => currentMovementDestination != null;

        public bool CanMoveStill => !parent.ProperBody.MovementStats.HasMovedThisTurn;

        public override void _Ready()
        {
            base._Ready();

            GetNodes();
        }

        private void GetNodes()
        {
            var body = GetParent();
            parent = body.GetParent<Character>();
            pathMover = body.GetNode<PathMover>("PathMover");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if(!isActiveCharacter)
                return;

            if(!HaveMovementTarget && CanMoveStill)
            {
                GetMovementTarget();
                return;
            }

            if(HaveMovementTarget && CanMoveStill && !haveMovementPath)
            {
                GetPathToMovementDestination();
                return;
            }
            else if(!CanMoveStill && !haveMovementPath)
            {
                EndTurn();
            }
        }

        public void SetOurTurn()
        {
            isActiveCharacter = true;
        }

        private void GetMovementTarget()
        {
            var (_, targetPosition) = parent.GetNearestOpponentCharacter();

            var vector = (parent.ProperBody.Transform.origin - targetPosition);
            var distance = Mathf.Abs(vector.x + vector.z);

            if (distance < 1.00001f)
            {
                EndTurn();
                return;
            }
            
            var foundEmptyPosition = AStarNavigator.Instance.TryGetNearestEmptyPointToLocation(
                target: targetPosition,
                origin: parent.ProperBody.Transform.origin,
                out Vector3 foundPoint);

            if (!foundEmptyPosition)
            {
                EndTurn();
                return;
            }
            else
            {
                currentMovementDestination = foundPoint;
            }
        }

        private void GetPathToMovementDestination()
        {
            var result = AStarNavigator.Instance.TryGetMovementPath(
            start: parent.ProperBody.Transform.origin,
            end: currentMovementDestination.Value,
            character: parent);

            if (result == null || !result.CanFindPath)
            {
                EndTurn();
                return;
            }

            pathMover.MoveWithPath(result.Path);
            parent.IsActive = true;
        }

        private void EndTurn()
        {
            parent.ProperBody.MovementStats.SetCantMoveAnymoreThisTurn();
            isActiveCharacter = false;
            parent.IsActive = false;
            EmitSignal(nameof(_AiCharacter_TurnFinished), parent);
        }
        
    }
}