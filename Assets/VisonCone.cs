using UnityEngine;


public class VisionCone : MonoBehaviour
{
    public float viewDistance = 5f;
    public float viewAngle = 90f;
    public Color coneColor = new Color(1f, 0f, 0f, 0.2f); // Normal color
    public Color detectedColor = new Color(1f, 0f, 0f, 0.8f); // Bright red when detected
    
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material coneMaterial; // NEW
    private NPCPatrol npcPatrol; // NEW
    
    void Start()
    {
        // Get reference to patrol script
        npcPatrol = GetComponent<NPCPatrol>(); // NEW
        
        GameObject coneObject = new GameObject("VisionCone");
        coneObject.transform.SetParent(transform);
        coneObject.transform.localPosition = Vector3.zero;
        
        meshFilter = coneObject.AddComponent<MeshFilter>();
        meshRenderer = coneObject.AddComponent<MeshRenderer>();
        
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        
        coneMaterial = new Material(Shader.Find("Sprites/Default"));
        coneMaterial.color = coneColor;
        meshRenderer.material = coneMaterial;
        meshRenderer.sortingOrder = -1;
    }
    
    void LateUpdate()
    {
        DrawVisionCone();
        UpdateConeColor(); // NEW
    }
    
    void UpdateConeColor() // NEW METHOD
    {
        // Change color based on detection
        if (npcPatrol != null)
        {
            SpriteRenderer guardSprite = GetComponent<SpriteRenderer>();
            if (guardSprite != null && guardSprite.color == Color.red)
            {
                // Player detected - bright red cone
                coneMaterial.color = detectedColor;
            }
            else
            {
                // Normal patrol - dim red cone
                coneMaterial.color = coneColor;
            }
        }
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



