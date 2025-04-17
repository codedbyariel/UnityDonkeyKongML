using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 2f;
    private float jumpingPower = 4f;
    private bool isFacingRight = true;
    private int tempScore;
    //player
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform playerPOS;
    //ground
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    //barrel
    [SerializeField] private Transform barrelCheck;
    [SerializeField] private LayerMask barrelLayer;
    //hammer
    [SerializeField] private Transform HammerHitBarrel;
    [SerializeField] private bool isHammerHitBarrel;
    //
    [SerializeField] private LayerMask HammerLayer;
    [SerializeField] private float HammerTimer;
    public float targetTime;
    //score
    [SerializeField] private int Score;
    //text score
    [SerializeField] private UnityEngine.UI.Text ScoreText;
    void Start()
    {
        Score = 0;
        tempScore = 0;
        targetTime = HammerTimer;
    }

    void Update()
    {
        //barrel timer
        if (isHammerHitBarrel)
        {
            targetTime -= Time.deltaTime;
        }
        //Player Movement
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
        //
        ItsHammerTime();
        //
        Flip();
        //Score Barrel
        if ((HasJumpedOverBarrel())&&(!IsGrounded()))
        {
            tempScore++;
        }
        if ((tempScore != 0) && (IsGrounded()))
        {
            Score++;
            tempScore = 0;
        }
        //Debug.Log("Score:"+Score);
        scoreChange(Score);
        HammerHitBarrelFunc();
        stopHammerTime();
        End_Game();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }
    // is player on ground
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    //is player is over a barrel
    private bool HasJumpedOverBarrel()
    {
        return Physics2D.OverlapCircle(barrelCheck.position, 0.2f, barrelLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal > 0f || !isFacingRight && horizontal < 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    //Hammer
    private void HammerHitBarrelFunc()
    {
        if((Physics2D.OverlapCircle(HammerHitBarrel.position, 0.2f, barrelLayer))&&(isHammerHitBarrel))
        {
            Debug.Log(Physics2D.OverlapCircle(HammerHitBarrel.position, 0.2f, barrelLayer).gameObject.transform);
            Destroy(Physics2D.OverlapCircle(HammerHitBarrel.position, 0.2f, barrelLayer).gameObject);
            Score = Score + 5;
        }
    }
    //
    private void ItsHammerTime()
    {
        if (Physics2D.OverlapCircle(rb.position, 0.2f, HammerLayer))
        {
            Debug.Log("ItsHammerTime");
            isHammerHitBarrel = true;
            Destroy(Physics2D.OverlapCircle(rb.position, 0.2f, HammerLayer).gameObject);
        }
    }
    //
    private void stopHammerTime()
    {
        if (targetTime <= 0.0f)
        {
            isHammerHitBarrel = false;
        }
    }
    //
    private void scoreChange(int score)
    {
        int tmp = score * 100;
        ScoreText.text= "Score:"+tmp.ToString();

    }

    private void End_Game()
    {
        if((Physics2D.OverlapCircle(playerPOS.position, 0.2f, barrelLayer))){
            EditorApplication.isPlaying = false;
        }
    }
}
