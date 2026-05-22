using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public float viewDistance = 5f;
    public float viewAngle = 90f;
    public Color patrolColor  = new Color(1f, 0f, 0f, 0.2f);
    public Color chaseColor   = new Color(1f, 0f, 0f, 0.8f);
    public Color searchColor  = new Color(1f, 1f, 0f, 0.5f);

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material coneMaterial;
    private NPCPatrol npcPatrol;

    void Start()
    {
        npcPatrol = GetComponent<NPCPatrol>();

        GameObject coneObject = new GameObject("VisionCone");
        coneObject.transform.SetParent(transform);
        coneObject.transform.localPosition = Vector3.zero;

        meshFilter = coneObject.AddComponent<MeshFilter>();
        meshRenderer = coneObject.AddComponent<MeshRenderer>();

        mesh = new Mesh();
        meshFilter.mesh = mesh;

        coneMaterial = new Material(Shader.Find("Sprites/Default"));
        coneMaterial.color = patrolColor;
        meshRenderer.material = coneMaterial;
        meshRenderer.sortingOrder = -1;
    }

    void LateUpdate()
    {
        DrawVisionCone();
        UpdateConeColor();
    }

    void UpdateConeColor()
    {
        if (npcPatrol == null) return;

        coneMaterial.color = npcPatrol.CurrentState switch
        {
            NPCPatrol.State.Chase  => chaseColor,
            NPCPatrol.State.Search => searchColor,
            _                      => patrolColor,
        };
    }

    void DrawVisionCone()
    {
        int segments = 20;
        float angleStep = viewAngle / segments;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        float currentAngle = -viewAngle / 2;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float direction = sr.flipX ? -1f : 1f;

        for (int i = 0; i <= segments; i++)
        {
            float rad = currentAngle * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(
                Mathf.Sin(rad) * viewDistance * direction,
                Mathf.Cos(rad) * viewDistance,
                0
            );

            if (i < segments)
            {
                triangles[i * 3]     = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            currentAngle += angleStep;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
