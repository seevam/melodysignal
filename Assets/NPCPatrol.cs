using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NPCPatrol : MonoBehaviour
{
    public enum State { Patrol, Chase, Search }

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public float detectionAngle = 90f;
    public LayerMask playerLayer;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    public float searchDuration = 3f;

    public State CurrentState { get; private set; } = State.Patrol;
    public bool IsPlayerDetected => CurrentState == State.Chase;

    private int currentPointIndex = 0;
    private bool movingForward = true;
    private float waitCounter = 0f;
    private bool isWaiting = false;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private Vector2 lastKnownPosition;
    private float searchCounter = 0f;

    private static readonly Color colorPatrol = new Color(0.97f, 0.47f, 0.56f, 1f);
    private static readonly Color colorChase  = Color.red;
    private static readonly Color colorSearch = Color.yellow;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (patrolPoints.Length < 2)
        {
            Debug.LogError("Need at least 2 patrol points!");
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        switch (CurrentState)
        {
            case State.Patrol:
                Patrol();
                if (CanSeePlayer()) TransitionTo(State.Chase);
                break;

            case State.Chase:
                ChasePlayer();
                if (!CanSeePlayer()) TransitionTo(State.Search);
                break;

            case State.Search:
                Search();
                if (CanSeePlayer()) TransitionTo(State.Chase);
                break;
        }

        UpdateVisuals();
    }

    void TransitionTo(State next)
    {
        if (next == State.Search)
        {
            lastKnownPosition = player.position;
            searchCounter = searchDuration;
        }

        CurrentState = next;
    }

    void Patrol()
    {
        if (isWaiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0)
            {
                isWaiting = false;

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
            }
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        Transform target = patrolPoints[currentPointIndex];
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance < 0.3f)
        {
            isWaiting = true;
            waitCounter = waitTime;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            FaceDirection(direction.x);
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);
        FaceDirection(direction.x);
    }

    void Search()
    {
        float distToLastKnown = Vector2.Distance(transform.position, lastKnownPosition);

        if (distToLastKnown > 0.3f)
        {
            Vector2 direction = (lastKnownPosition - (Vector2)transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            FaceDirection(direction.x);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        searchCounter -= Time.deltaTime;
        if (searchCounter <= 0)
            TransitionTo(State.Patrol);
    }

    void UpdateVisuals()
    {
        spriteRenderer.color = CurrentState switch
        {
            State.Chase  => colorChase,
            State.Search => colorSearch,
            _            => colorPatrol,
        };
    }

    void FaceDirection(float x)
    {
        if (x > 0) spriteRenderer.flipX = false;
        else if (x < 0) spriteRenderer.flipX = true;
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > detectionRange) return false;

        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        Vector2 facing = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        float angle = Vector2.Angle(facing, dirToPlayer);
        if (angle > detectionAngle / 2f) return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, detectionRange, playerLayer);
        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        foreach (Transform point in patrolPoints)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, 0.5f);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < patrolPoints.Length - 1; i++)
        {
            if (patrolPoints[i] != null && patrolPoints[i + 1] != null)
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (spriteRenderer == null) return;

        Vector2 facing = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);

        float halfAngle = detectionAngle / 2f;
        Vector3 right = Quaternion.Euler(0, 0, -halfAngle) * facing * detectionRange;
        Vector3 left  = Quaternion.Euler(0, 0,  halfAngle) * facing * detectionRange;

        Gizmos.DrawLine(transform.position, transform.position + right);
        Gizmos.DrawLine(transform.position, transform.position + left);

        // Draw last known position when searching
        if (CurrentState == State.Search)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastKnownPosition, 0.4f);
        }
    }
}
