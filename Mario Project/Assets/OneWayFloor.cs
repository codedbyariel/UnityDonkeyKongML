using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{/*
    private Collider2D platformCollider;
    private bool hasPassed=false;

    private void Start()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        // Get the Rigidbody2D of the colliding object
        Rigidbody2D rb = collision.attachedRigidbody;
        Debug.Log("linearVelocity.y: " + rb.linearVelocity.y);

        if (rb != null)
        {
            Vector2 linearVelocity = rb.linearVelocity;

            // Allow the player to pass through if moving upwards
            if ((linearVelocity.y > 0)&&(hasPassed==false))
            {
                Physics2D.IgnoreCollision(collision, platformCollider, true);
                Debug.Log("Collision ignored: " + (linearVelocity.y));
            }
            if((linearVelocity.y == 0)||hasPassed)
            {
                // Re-enable collision if moving downwards or stationary
                Physics2D.IgnoreCollision(collision, platformCollider, false);
                Debug.Log("Collision ignored: " + (linearVelocity.y ));
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        hasPassed=true;
    }*/
}



