using FaffLatest.scripts.shared;
using Godot;

public static class SurfaceToolExtensions
{
    public static void CreateQuad(this SurfaceTool st, Vector3 centre, float width, float height, float depth, MeshFacingDirection direction)
    {
        Vector3[] corners = null;

        switch (direction)
        {
            case MeshFacingDirection.Up:
                {
                    corners = centre.CreateUpFacingCorners(width, depth);
                    break;
                }
                case MeshFacingDirection.Left:
                {
                    corners = centre.CreateLeftFacingCorners(width, height);
                    break;
                }
                case MeshFacingDirection.Front:
                {
                    corners = centre.CreateFrontFacingCorners(width, height);
                    break;
                }
        }

        st.CreateTri(new [] { corners[0], corners[2], corners[1] });
        st.CreateTri(new [] { corners[1], corners[2], corners[3] });
    }
    
    private static void CreateTri(this SurfaceTool st, Vector3[] vertices)
    {
        st.AddVertex(vertices[0]);
        st.AddVertex(vertices[1]);
        st.AddVertex(vertices[2]);
    }

    private static Vector3[] CreateUpFacingCorners(this Vector3 centre, float width, float depth)
        => new[] { new Vector3(0, 0, 0) + centre,
            new Vector3(0, 0, depth) + centre,
            new Vector3(width, 0, 0) + centre,
            new Vector3(width, 0, depth) + centre };

    private static Vector3[] CreateLeftFacingCorners(this Vector3 centre, float width, float height)
        => new[] { new Vector3(0, 0, 0) + centre,
            new Vector3(0, 0, width) + centre,
            new Vector3(0, height, 0) + centre,
            new Vector3(0, height, width) + centre };

    private static Vector3[] CreateFrontFacingCorners(this Vector3 centre, float width, float height)
    => new[] { new Vector3(0, 0, 0) + centre,
            new Vector3(width, 0, 0) + centre,
            new Vector3(0, height, 0) + centre,
            new Vector3(width, height, 0) + centre };

}