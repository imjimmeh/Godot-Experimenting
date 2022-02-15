using Godot;

namespace FaffLatest.scripts.movement{
    public class GetMovementPathResult{
        public readonly Vector3[] Path;
        public bool IsSuccess;

        public GetMovementPathResult()
        {
        }

        public GetMovementPathResult(Vector3[] path, bool isSuccess)
        {
            Path = path;
            IsSuccess = isSuccess;
        }

         public GetMovementPathResult(Vector3[] path)
        {
            Path = path;
            IsSuccess = true;
        }

        
         public GetMovementPathResult(bool isSuccess)
        {
            Path = null;
            IsSuccess = isSuccess;
        }
    }
}