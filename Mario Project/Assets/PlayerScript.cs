using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public  CircleCollider2D GroundCheckCollider;
    
    public float jumpSpeed;
    public float speed;
    public bool isOnGround = false;
    public float moveSpeed = 5; // Movement speed
    private Vector2 moveDirection; // Direction to move in
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        //gameObject.name = "MarioPlayer";

    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        //movement
        //jump
        //moveJump();
        //right
        //moveRight();
        //left
        //moveLeft();
        //

        //
    }
    //Movement
    //jump
    private void moveJump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) == true) && (isOnGround == true))
        {
            myRigidBody.linearVelocity = Vector2.up * jumpSpeed;
        }
    }
    //moveRight
    private void moveRight() {
        if (Input.GetKeyDown(KeyCode.D) == true)
        {
            myRigidBody.linearVelocity = Vector2.right * speed;
        }
    }
    //moveLeft
    private void moveLeft()
    {
        if (Input.GetKeyDown(KeyCode.A) == true)
        {
            myRigidBody.linearVelocity = Vector2.left * speed;
        }

    }

    

    //
    private void OnCollisionStay2D(Collision2D Collider2d)
    {
        if (Collider2d.collider == GroundCheckCollider)
        {
            isOnGround = true;
        }

    }
    //
    private void OnCollisionExit2D(Collision2D Collider2d)
    {
        if (Collider2d.collider == GroundCheckCollider)
        {
            isOnGround = false;
        }
    }

    //
    void HandleInput()
    {
        // Reset moveDirection to zero every frame
        moveDirection = Vector2.zero;

        // Check for horizontal and vertical input (e.g., arrow keys, WASD)
        if ((Input.GetKey(KeyCode.Space))&&(isOnGround)) // Move up
        {
            myRigidBody.linearVelocity = Vector2.up * jumpSpeed;
        }
        if (Input.GetKey(KeyCode.S)) // Move down
        {
            moveDirection.y = -1;
        }
        if (Input.GetKey(KeyCode.A)) // Move left
        {
            moveDirection.x = -1;
        }
        if (Input.GetKey(KeyCode.D)) // Move right
        {
            moveDirection.x = 1;
        }

        // Move the player in the desired direction
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}

