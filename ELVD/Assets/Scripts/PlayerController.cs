using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 3;

    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 15;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashTimer = 1f;
    private bool isDashing;
    private bool canDash;

    bool attack = false;
    float timeBetweenAttack, timeSinceAttack;

    PlayerStateList pState;
    public static PlayerController Instance;
    private Rigidbody2D rb;
    private float xAxis;
    Animator anim;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        pState = GetComponent<PlayerStateList>();

        rb = GetComponent<Rigidbody2D>();
        canDash = true;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }
        GetInputs();
        UpdateJumpVariables();
        Flip();
        Move();
        Jump();
        Attack();
        if (Input.GetKeyDown(KeyCode.Z) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void GetInputs()
    {
        xAxis = Input.GetAxis("Horizontal");
        attack = Input.GetMouseButton(0);
    }

    void Flip()
    {
        if (xAxis < 0 && transform.localScale.x > 0) // Check if not already facing left
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
            // Adjust the position when facing left
            transform.position = new Vector2(transform.position.x - 0.338f, transform.position.y);
        }
        else if (xAxis > 0 && transform.localScale.x < 0) // Check if not already facing right
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            // Reset the position when facing right
            transform.position = new Vector2(transform.position.x + 0.338f, transform.position.y);
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2 (walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("isWalking", rb.velocity.x != 0 && Grounded());
        
    }

    public bool Grounded()
    {
        if(Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            pState.jumping = false;
        }

        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
                pState.jumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;

                airJumpCounter++;

                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
        }

        anim.SetBool("isJumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded()) {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        } else {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferFrames;
        } else {
            jumpBufferCounter--;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rb.velocity = new Vector2(xAxis * dashSpeed, rb.velocity.y);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        yield return new WaitForSeconds(dashTimer);
        canDash = true;
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if(attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
        }
        anim.SetTrigger("Attacking");
    }
}
