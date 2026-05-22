using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public float viewDistance = 5f;
    public float viewAngle = 90f;
    public Color normalColor = new Color(1f, 0f, 0f, 0.2f); // Dim red when patrolling
    public Color detectedColor = new Color(1f, 0f, 0f, 0.8f); // Bright red when player detected
    
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material coneMaterial;
    private NPCPatrol npcPatrol;
    
    void Start()
    {
        // Get reference to NPCPatrol script
        npcPatrol = GetComponent<NPCPatrol>();
        
        if (npcPatrol == null)
        {
            Debug.LogError("VisionCone requires NPCPatrol script on same GameObject!");
        }
        
        // Create vision cone object
        GameObject coneObject = new GameObject("VisionCone");
        coneObject.transform.SetParent(transform);
        coneObject.transform.localPosition = Vector3.zero;
        
        meshFilter = coneObject.AddComponent<MeshFilter>();
        meshRenderer = coneObject.AddComponent<MeshRenderer>();
        
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        
        // Create material
        coneMaterial = new Material(Shader.Find("Sprites/Default"));
        coneMaterial.color = normalColor;
        meshRenderer.material = coneMaterial;
        meshRenderer.sortingOrder = -1; // Behind other sprites
    }
    
    void LateUpdate()
    {
        DrawVisionCone();
        UpdateConeColor();
    }
    
    void UpdateConeColor()
    {
        if (npcPatrol == null) return;
        
        // Change cone color based on detection state
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
        
        vertices[0] = Vector3.zero; // Center point
        
        // Get sprite renderer to check facing direction
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;
        
        // Determine base direction based on sprite flip
        // When flipX = false, guard faces RIGHT (0 degrees)
        // When flipX = true, guard faces LEFT (180 degrees)
        float baseAngle = sr.flipX ? 180f : 0f;
        
        float startAngle = baseAngle - (viewAngle / 2f);
        float currentAngle = startAngle;
        
        for (int i = 0; i <= segments; i++)
        {
            float rad = currentAngle * Mathf.Deg2Rad;
            
            // Generate cone pointing RIGHT (0°) or LEFT (180°)
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
        mesh.vertices = vert
