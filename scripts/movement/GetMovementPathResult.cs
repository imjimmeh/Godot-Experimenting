using Godot;

namespace FaffLatest.scripts.movement
{
    public class GetMovementPathResult
    {
        public readonly bool IsSuccess;

        public readonly Vector3[] Path;

        public readonly Vector3 StartOrigin;

        public readonly Vector3 EndOrigin;

        public readonly PointInfo StartPoint;

        public readonly PointInfo EndPoint;

        public readonly Vector3 FoundStartPoint;

        public readonly Vector3 FoundEndPoint;

        public GetMovementPathResult()
        {
        }

        public GetMovementPathResult(bool isSuccess, Vector3[] path, Vector3 startOrigin, Vector3 endOrigin, PointInfo startPoint, PointInfo endPoint, Vector3 foundStartPoint, Vector3 foundEndPoint)
        {
            IsSuccess = isSuccess;
            Path = path;
            StartOrigin = startOrigin;
            EndOrigin = endOrigin;
            StartPoint = startPoint;
            EndPoint = endPoint;
            FoundStartPoint = foundStartPoint;
            FoundEndPoint = foundEndPoint;
        }
    }
}