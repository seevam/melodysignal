using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public float viewDistance = 5f;
    public float viewAngle = 90f;
    public Color normalColor = new Color(1f, 0f, 0f, 0.2f);
    public Color detectedColor = new Color(1f, 0f, 0f, 0.8f);
    
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material coneMaterial;
    private NPCPatrol npcPatrol;
    
    void Start()
    {
        npcPatrol = GetComponent<NPCPatrol>();
        
        if (npcPatrol == null)
        {
            Debug.LogError("VisionCone requires NPCPatrol script on same GameObject!");
        }
        
        GameObject coneObject = new GameObject("VisionCone");
        coneObject.transform.SetParent(transform);
        coneObject.transform.localPosition = Vector3.zero;
        
        meshFilter = coneObject.AddComponent<MeshFilter>();
        meshRenderer = coneObject.AddComponent<MeshRenderer>();
        
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        
        coneMaterial = new Material(Shader.Find("Sprites/Default"));
        coneMaterial.color = normalColor;
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
        
        if (npcPatrol.IsPlayerDetected)
        {
            coneMaterial.color = detectedColor;
        }
        else
        {
            coneMaterial.color = normalColor;
        }
    }
    
    void DrawVisionCone()
    {
        int segments = 20;
        float angleStep = viewAngle / segments;
        
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];
        
        vertices[0] = Vector3.zero;
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;
        
        float baseAngle = sr.flipX ? 180f : 0f;
        float startAngle = baseAngle - (viewAngle / 2f);
        float currentAngle = startAngle;
        
        for (int i = 0; i <= segments; i++)
        {
            float rad = currentAngle * Mathf.Deg2Rad;
            
            vertices[i + 1] = new Vector3(
                Mathf.Cos(rad) * viewDistance,
                Mathf.Sin(rad) * viewDistance,
                0
            );
            
            if (i < segments)
            {
                triangles[i * 3] = 0;
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
