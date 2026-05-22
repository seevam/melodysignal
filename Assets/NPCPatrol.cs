using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif


public class NPCPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    
    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public float detectionAngle = 90f;
    public LayerMask playerLayer;
    
    private int currentPointIndex = 0;
    private bool movingForward = true;
    private float waitCounter = 0f;
    private bool isWaiting = false;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    
    // NEW: Public property for VisionCone to check
    public bool IsPlayerDetected { get; private set; }
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        if (patrolPoints.Length < 2)
        {
            Debug.LogError("Need at least 2 patrol points!");
            enabled = false;
        }
    }
    
    void FixedUpdate()
    {
        Patrol();
        CheckForPlayer();
    }
    
    void Patrol()
    {
        if (isWaiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0)
            {
                isWaiting = false;
                
                // Switch direction
                if (currentPointIndex == 0)
                {
                    movingForward = true;
                    currentPointIndex = 1;
                }
                else if (currentPointIndex == patrolPoints.Length - 1)
                {
                    movingForward = false;
                    currentPointIndex = patrolPoints.Length - 2;
                }
                else
                {
                    currentPointIndex += movingForward ? 1 : -1;
                }
                
                Debug.Log("Moving to patrol point " + currentPointIndex);
            }
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPoint.position);
        
        if (distance < 0.3f)
        {
            // Reached patrol point
            isWaiting = true;
            waitCounter = waitTime;
            rb.linearVelocity = Vector2.zero;
            Debug.Log("Reached patrol point " + currentPointIndex);
        }
        else
        {
            // Move toward point
            rb.linearVelocity = direction * moveSpeed;
            
            // Flip sprite
            if (direction.x > 0)
                spriteRenderer.flipX = false;
            else if (direction.x < 0)
                spriteRenderer.flipX = true;
        }
    }
    
    void CheckForPlayer()
    {
        if (CanSeePlayer())
        {
            spriteRenderer.color = Color.red;
            IsPlayerDetected = true; // NEW LINE
        }
        else
        {
            spriteRenderer.color = new Color(0.97f, 0.47f, 0.56f, 1f);
            IsPlayerDetected = false; // NEW LINE
        }
    }
    
    bool CanSeePlayer()
    {
        if (player == null) return false;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange) return false;
        
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector2 facing = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        float angleToPlayer = Vector2.Angle(facing, directionToPlayer);
        
        if (angleToPlayer > detectionAngle / 2) return false;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, playerLayer);
        return hit.collider != null && hit.collider.CompareTag("Player");
    }
    
    void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        
        Gizmos.color = Color.yellow;
        foreach (Transform point in patrolPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position, 0.5f);
            }
        }
        
        Gizmos.color = Color.red;
        for (int i = 0; i < patrolPoints.Length - 1; i++)
        {
            if (patrolPoints[i] != null && patrolPoints[i + 1] != null)
            {
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (spriteRenderer == null) return;
        
        Vector2 facing = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        
        float halfAngle = detectionAngle / 2f;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -halfAngle) * facing * detectionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, halfAngle) * facing * detectionRange;
        
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
    }
}



