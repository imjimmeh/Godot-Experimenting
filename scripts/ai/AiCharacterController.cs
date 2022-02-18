using System;
using System.Collections.Generic;
using FaffLatest.scripts.attacks;
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
                AttackTarget();
            }
            else if (TurnFinished)
            {
                EndTurn();
            }
        }

        private bool TurnFinished => AiCharacterFinishedMovement && !parent.Stats.EquippedWeapon.CanAttack;

        private bool AiCharacterFinishedMovement => parent.Body.MovementStats.HasMovedThisTurn && pathMover.Path == null;

        private bool ShouldGetMovementDestination => !HaveMovementTarget && !parent.Body.MovementStats.HasMovedThisTurn;

        private bool ShouldGetMovementPath =>
            HaveMovementTarget && !parent.Body.MovementStats.HasMovedThisTurn && pathMover.Path == null;

        private bool IsAttackPhase => AiCharacterFinishedMovement && parent.Stats.EquippedWeapon.CanAttack;

        public void SetOurTurn()
        {
            isActiveCharacter = true;
        }

        private async void AttackTarget()
        {
            GetAttackTargetAndPosition();

            if (!HaveAttackTarget)
                EndTurn();

            var successfulAttack = await AttackHelpers.TryAttack(parent, currentAttackTarget);

            if(!successfulAttack)
                EndTurn();
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
            var distance = parent.Body.GlobalTransform.origin.DistanceToIgnoringHeight(targetPosition);

            if (Mathf.IsZeroApprox(distance))
            {
                parent.Body.MovementStats.EndMovementTurn();
                return;
            }

            var foundEmptyPosition = AStarNavigator.Instance.TryGetNearestEmptyPointToLocation(
                target: targetPosition,
                origin: parent.Body.GlobalTransform.origin,
                out Vector3 foundPoint);

            if (!foundEmptyPosition)
            {
                parent.Body.MovementStats.EndMovementTurn();
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
            start: parent.Body.GlobalTransform.origin,
            end: currentMovementDestination.Value,
            character: parent);

            if (result == null || !result.CanFindPath)
            {
                parent.Body.MovementStats.EndMovementTurn();
                return;
            }

            pathMover.MoveWithPath(result.Path);
            parent.IsActive = true;
        }

        private void EndTurn()
        {
            parent.Stats.EquippedWeapon.EndTurn();
            parent.Body.MovementStats.EndMovementTurn();
            isActiveCharacter = false;
            parent.IsActive = false;

            currentAttackTarget = null;
            currentMovementDestination = null;

            EmitSignal(nameof(_AiCharacter_TurnFinished), parent);
        }

    }
}