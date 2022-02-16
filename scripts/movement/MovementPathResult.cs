using Godot;

namespace FaffLatest.scripts.movement{
    public class GetMovementPathResult{
        public readonly Vector3[] Path;
        public readonly bool CanFindPath;
        public readonly bool NotEnoughMovementDistanceToFullyReach;

        public GetMovementPathResult()
        {
        }

        public GetMovementPathResult(Vector3[] path, bool canFindPath, bool notEnoughMovementDistanceToFullyReach)
        {
            Path = path;
            CanFindPath = canFindPath;
            NotEnoughMovementDistanceToFullyReach = notEnoughMovementDistanceToFullyReach;
        }

         public GetMovementPathResult(Vector3[] path, bool notEnoughMovementDistanceToFullyReach)
        {
            Path = path;
            CanFindPath = true;
            NotEnoughMovementDistanceToFullyReach = notEnoughMovementDistanceToFullyReach;
        }

        
         public GetMovementPathResult(bool isSuccess)
        {
            Path = null;
            CanFindPath = isSuccess;
            NotEnoughMovementDistanceToFullyReach = false;
        }
    }
}