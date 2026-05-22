using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 12f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log("Player ready to move!");
    }
    
    void Update()
    {
        // Get horizontal input
        float moveInput = Input.GetAxisRaw("Horizontal");
        
        // Check if sprinting
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        
        // Calculate speed
        float speed = isSprinting ? runSpeed : walkSpeed;
        
        // Move player
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
        
        // Flip sprite based on direction
        if (moveInput > 0)
            spriteRenderer.flipX = true;
        else if (moveInput < 0)
            spriteRenderer.flipX = false;
        
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            Debug.Log("Jump!");
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Landed on ground");
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
