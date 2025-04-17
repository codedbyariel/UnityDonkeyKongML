using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FireEnemyScript : MonoBehaviour
{
    private float horizontal, vertical;
    private float moveX = 0;
    private float moveY = 0;
    private bool isFacingRight = true;
    private bool isLadder;
    private bool isClimbing;
    [SerializeField] private Rigidbody2D rb;
    private float speed = 1.2f;
    [SerializeField] private float waitTimer;
    [SerializeField] private float timerStart;
    private float timer;
    [SerializeField] private GameObject RestarterObject;
    [SerializeField] private Vector3 StartPOS;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = waitTimer;
        StartPOS= transform.position;
    }
    private void Restarter()
    {
        timer = waitTimer;
        transform.position= StartPOS;
        horizontal = 0;
        vertical = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if ((RestarterObject.GetComponent<SpriteRenderer>().material.color == Color.green))
        {
            Restarter();
        }
        HandleInput();
        Flip();
        HandleLadder();
        timer -= Time.deltaTime;
    }
    //
    private void FixedUpdate()
    {
        HandleMovement();
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * speed);
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }
    //
    private void HandleMovement()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * speed);
        }
        else
        {
            rb.gravityScale = 1f;
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
    }
    private void HandleInput()
    {
        if (timerEnded())
        {
            horizontal =  Random.value < 0.5f ? -1 : 1;//moveX; //Input.GetAxisRaw("Horizontal");
            vertical =  Random.value < 0.5f ? -1 : 1;//moveY;//Input.GetAxisRaw("Vertical");
        }
        moveX = horizontal;
        moveY = vertical;
    }
    //
    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    //
    private void HandleLadder()
    {
        vertical = moveY;//Input.GetAxisRaw("Vertical");

        if (isLadder && Mathf.Abs(moveY) > 0f)
        {
            isClimbing = true;
        }
    }
    //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
    }
    //
    private bool timerEnded()
    {
        if (timer <= 0)
        {
            timer = timerStart;
            return true;
        }
        return false;
    }
}
