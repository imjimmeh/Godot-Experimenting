using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.effects.movementguide
{
    public class CharacterMovementGuideCell : MeshInstance
    {
        public AStarNavigator AStar;

        public void SetVisiblity(bool visible)
        {
            if (!visible)
                Hide();
            else
                Show();
        }

        public void CalculateVisiblity(Vector3 parentOrigin)
        {
            var worldPosition = Transform.origin + parentOrigin;
            var shouldHide = worldPosition.x < 1 || worldPosition.x >= 50 || worldPosition.z < 1 || worldPosition.z >= 50;

            if (!shouldHide)
            {
                shouldHide = PositionIsOccupied(worldPosition);

                if(shouldHide)
                {
                    GD.Print($"And we are at {Transform.origin}");
                }
            }

            SetVisiblity(!shouldHide);
        }

        private bool PositionIsOccupied(Vector3 worldPosition)
        {
            (int intx, int inty) = ((int)worldPosition.x, (int)worldPosition.z);

            var astarPoint = AStar.GetPointInfoForVector3(worldPosition);
            var occupied = astarPoint.OccupyingNode != null;

            if (occupied)
            {
                GD.Print($"{intx}, {inty} has {astarPoint.OccupyingNode?.Name} in it");
            }

            return occupied;
        }
    }
}
