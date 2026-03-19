using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    public float fov = 90f;
    public float viewDistance = 10f;
    private float startingAngle;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate()
    {
        int rayCount = 50;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;

        Vector2 worldOrigin = transform.position;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            //Vector2 dir = GetVectorFromAngle(angle);
            //RaycastHit2D hit = Physics2D.Raycast(worldOrigin, dir, viewDistance, layerMask);

            //if (hit.collider == null)
            //    vertices[vertexIndex] = (Vector3)(dir * viewDistance);
            //else
            //    vertices[vertexIndex] = hit.point - worldOrigin;

            //if (i > 0)
            Vector2 dir = GetVectorFromAngle(angle);
            RaycastHit2D hit = Physics2D.Raycast(worldOrigin, dir, viewDistance, layerMask);

            Vector3 worldEndPoint;
            if (hit.collider == null)
            {
                // Nếu không trúng gì, điểm cuối nằm ở khoảng cách tối đa
                worldEndPoint = (Vector3)worldOrigin + (Vector3)dir * viewDistance;
            }
            else
            {
                // Nếu trúng, điểm cuối chính là điểm va chạm
                worldEndPoint = hit.point;
            }

            // Chuyển đổi điểm từ World Space sang Local Space của FOV object
            vertices[vertexIndex] = transform.InverseTransformPoint(worldEndPoint);

            if (i > 0)
            {
                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = vertexIndex - 1;
                triangles[triangleIndex++] = vertexIndex;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }

    public static Vector2 GetVectorFromAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }
}