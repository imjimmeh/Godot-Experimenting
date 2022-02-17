using System;
using System.Collections.Generic;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.shared;
using FaffLatest.scripts.weapons;
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


        private Character currentAttackTarget;
        private Vector3? currentMovementDestination;
        private bool haveMovementPath => pathMover.Path != null;
        public bool HaveAttackTarget => currentAttackTarget != null;
        public bool HaveMovementTarget => currentMovementDestination != null;

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

            if (!isActiveCharacter)
                return;

            if (ShouldGetMovementDestination)
            {
                GetMovementTarget();
                return;
            }

            if (ShouldGetMovementPath)
            {
                GetPathToMovementDestination();
                return;
            }
            else if (IsAttackPhase)
            {
                GD.Print($"Zombie trying to attack target");
                AttackTarget();
            }
            else if (TurnFinished)
            {
                GD.Print($"Ending turn");
                EndTurn();
            }
        }

        private bool TurnFinished => AiCharacterFinishedMovement && !parent.Stats.EquippedWeapon.CanAttack;

        private bool AiCharacterFinishedMovement => parent.ProperBody.MovementStats.HasMovedThisTurn && pathMover.Path == null;

        private bool ShouldGetMovementDestination => !HaveMovementTarget && !parent.ProperBody.MovementStats.HasMovedThisTurn;

        private bool ShouldGetMovementPath =>
            HaveMovementTarget && !parent.ProperBody.MovementStats.HasMovedThisTurn && pathMover.Path == null;

        private bool IsAttackPhase => AiCharacterFinishedMovement && parent.Stats.EquippedWeapon.CanAttack;

        public void SetOurTurn()
        {
            isActiveCharacter = true;
        }

        private void AttackTarget()
        {
            GetAttackTargetAndPosition();

            if (!HaveAttackTarget)
                EndTurn();

            var attackResult = parent.TryAttackTarget(currentAttackTarget);
            var shouldEndTurn = ShouldEndTurnAfterAttack(attackResult);

            if(shouldEndTurn)
                EndTurn();
        }

        private bool ShouldEndTurnAfterAttack(AttackResult result)
        {
            switch(result)
            {
                case weapons.AttackResult.OutOfRange:
                {
                    parent.Stats.EquippedWeapon.EndTurn();
                    return true;
                }
                case AttackResult.OutOfAttacksForTurn:
                {
                    return true;
                }
            }

            return false;        
        }

        private Vector3 GetAttackTargetAndPosition()
        {
            var (character, targetPosition) = parent.GetNearestOpponentCharacter();
            currentAttackTarget = character;
            return targetPosition;
        }

        private void GetMovementTarget()
        {
            var targetPosition = GetAttackTargetAndPosition();
            var vector = (parent.ProperBody.Transform.origin - targetPosition);
            var distance = Mathf.Abs(vector.x + vector.z);

            if (distance < 1.00001f)
            {
                parent.ProperBody.MovementStats.SetCantMoveAnymoreThisTurn();
                return;
            }

            var foundEmptyPosition = AStarNavigator.Instance.TryGetNearestEmptyPointToLocation(
                target: targetPosition,
                origin: parent.ProperBody.Transform.origin,
                out Vector3 foundPoint);

            if (!foundEmptyPosition)
            {
                parent.ProperBody.MovementStats.SetCantMoveAnymoreThisTurn();
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
                parent.ProperBody.MovementStats.SetCantMoveAnymoreThisTurn();
                return;
            }

            pathMover.MoveWithPath(result.Path);
            parent.IsActive = true;
        }

        private void EndTurn()
        {
            parent.Stats.EquippedWeapon.EndTurn();
            parent.ProperBody.MovementStats.SetCantMoveAnymoreThisTurn();
            isActiveCharacter = false;
            parent.IsActive = false;

            currentAttackTarget = null;
            currentMovementDestination = null;

            EmitSignal(nameof(_AiCharacter_TurnFinished), parent);
        }

    }
}