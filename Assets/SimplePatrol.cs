using UnityEngine;

public class SimplePatrol : MonoBehaviour
{
    public float speed = 2f;
    public float leftBound = -6f;
    public float rightBound = 6f;
    
    private bool movingRight = true;
    
    void Update()
    {
        // Move
        if (movingRight)
            transform.position += Vector3.right * speed * Time.deltaTime;
        else
            transform.position += Vector3.left * speed * Time.deltaTime;
        
        // Check bounds and turn around
        if (transform.position.x >= rightBound)
            movingRight = false;
        else if (transform.position.x <= leftBound)
            movingRight = true;
    }
}

