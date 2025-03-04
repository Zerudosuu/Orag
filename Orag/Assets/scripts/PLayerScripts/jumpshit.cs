using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class jumpshit : MonoBehaviour
{

    [Header("PLAYER REFERENCES")]
    [Tooltip("Mga kaipuhan iset sa player")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform Slime_Position;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;



    [Header("CUSTOMIZABLE PLAYER STATS")]
    [Range(0.1f, 20)]
    public float speed = 3f; //values may differ based from u bch
    [Range(0.1f, 20)]
    public float  jumpingPower = 9f;


    [Header("PLAYER ADDTIONAL MOVESET")]
    [SerializeField] private bool allow_DoubleJump;
    [SerializeField] private bool allow_Dash;


    [Header("BETTER JUMP")]
    [SerializeField] private float coyoteTime = 0.15f; // 
    private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime = 0.3f; //
    private float jumpBufferCounter;

    private bool stopcounter; //pang stop counter sa buffer time
    private bool isJumping;

    [SerializeField] private float jumpcd = 0.3f;
    private bool isGrounded;

    private bool jump2; // for tracking the 2nd jump 



    [Header("DASHING")]
    public float dashCooldown = 3;
    public float dashLength = 0.5f;
    public float dashSpeed = 10;

    private float dashCounter;
    private float dashCoolCounter;
    private float DashDir = 1;



    [Header("CUSTOM GRAVITY")]
    [Range(0.1f,6)]
    [SerializeField] private float fallMuliplier = 3f;
    [Range(0.1f, 6)]
    [SerializeField] private float lowJumpMultiplier = 2f;
    [Range(-1, -12)]
    [SerializeField] private float maxFallSpeed;
    private float currentFallMuliplier;


    [Header("OTHERS")]
    //PLAYER MOVEMENT
    private bool moveleft;
    private bool moveright;
    private float X_Input;
    float HorizontalMove;

    //PLAYER INTERACTION
    private bool freezePlayer; // might change theapproach
    private float freezeDuration;

    // PARA SA FLIP
    bool facingRight = true;

    public enum PlayerState { Idle, Walking ,Rising ,Falling, Dashing, Death, Hurt};
    public PlayerState playerState;

    void Start()
    {
        //dashCooldown = 5;
        animator = GetComponent<Animator>();
        currentFallMuliplier = fallMuliplier;
    }

    private void Update()
    {     
        Flip(); // flip sprite
        IsGrounded(); // detect ground logic
        Dashh(); // dash
        KeyboardControls(); // kb controls, might change after new input system implemented

    }

    void Flip()
    {
        if (X_Input < 0 && facingRight)//if you press the left arrow and your facing right then you will face left;
        {
            facingRight = !facingRight;//if facing right is true it will be false and if it is false it wili be true
            Slime_Position.transform.Rotate(0f, 180f, 0f);
        }
        else if (X_Input > 0 && !facingRight)
        {
            facingRight = !facingRight;
            Slime_Position.transform.Rotate(0f, 180f, 0f);

        }

    }



    private void FixedUpdate()
    {
        //  Debug.Log(rb.velocity,);
      
        if (!freezePlayer)
        {
            MovePlayer();        

            if (dashCounter > 0 && allow_Dash) //means dashing
            {
                //rb.linearVelocity = new Vector2(DashDir * speed, rb.linearVelocity.y); // mahihiro ang y
                rb.linearVelocity = new Vector2(DashDir * speed, 0); // paa pa gnagdash dae mahihiro ang velocity y 
                currentFallMuliplier = 0f;
            }
            else
            {

                rb.linearVelocity = new Vector2(X_Input * speed, rb.linearVelocity.y); //for moving physics
                currentFallMuliplier = fallMuliplier;
            }


        }
        else
        {
            //  
            if (isGrounded)
                rb.linearVelocity = new Vector2(0, 0);
            else
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
          
        }

        JumpRising_FallingGravity();
        CayoteTime_JumpBuffer();
        AnimationHandler();


    }


    // player interaction
    public void FreezePlayer(bool freeze, float freezeTime)
    {

        // Debug.Log("freezee");
        freezePlayer = freeze;
        freezeDuration = freezeTime;
        StartCoroutine(DontMove());


    }
    IEnumerator DontMove()
    {

        yield return new WaitForSeconds(freezeDuration);
        freezePlayer = false;
    }



    void AnimationHandler()
    {
        //0.idle 1.walk 2.rising 3.falling 4.dash

        // Dash
        if (dashCounter > 0) { HorizontalMove = 4; playerState = PlayerState.Dashing; }
        // Idle
        else if (rb.linearVelocity.x == 0 && isGrounded) { HorizontalMove = 0; playerState = PlayerState.Idle;   } 
        // Walking
        else if ((rb.linearVelocity.x < -0.1f || rb.linearVelocity.x > 0.1f) && isGrounded) { HorizontalMove = 1; playerState = PlayerState.Walking;  } // < -2.1 ta yan ang at least na bilis kang pag bagsak mo bago iplay si falling animation
        // Rising
        else if (rb.linearVelocity.y > 0.1 && !isGrounded) { HorizontalMove = 2; playerState = PlayerState.Rising;  }
        // Falling
        else if (rb.linearVelocity.y < 0 && !isGrounded) { HorizontalMove = 3; playerState = PlayerState.Falling;  }
      

        animator.SetFloat("Move", HorizontalMove);


    }

    void JumpRising_FallingGravity()
    {

        if (rb.linearVelocity.y < 0.1f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (currentFallMuliplier - 1) * Time.deltaTime;
            //  Debug.Log("falling");

            // Clamp the Y velocity to the maximum fall speed
            if (rb.linearVelocity.y < maxFallSpeed)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxFallSpeed);
            }
        }
        else if (rb.linearVelocity.y > 0)
        {

            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            //  Debug.Log("rising");
        }

    }



    void CayoteTime_JumpBuffer()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }


        if (jumpBufferCounter == 0)
        {

        }
        else if (stopcounter)
        {
            jumpBufferCounter -= Time.deltaTime;

        }


        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
        {
            if(allow_DoubleJump)
            jump2 = true;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);

            jumpBufferCounter = 0f;

            StartCoroutine(JumpCooldown());
        }


    }


    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(jumpcd);
        isJumping = false;
    }


    private void IsGrounded()
    {

        //  Debug.Log("mayo sa daga");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.3f, groundLayer);
        if (colliders.Length > 0)
        {
            isGrounded = true;
            // Debug.Log("nasa daga");

            if (allow_DoubleJump)
                jump2 = false;

        }
        else
        {
            isGrounded = false;
            //  Debug.Log("mayo daga");
        }
       
    }



    void KeyboardControls()
    {
        // A - move left
        if (Input.GetKey(KeyCode.A))
            moveleft = true;
        else
            moveleft = false;

        // B - move right
        if (Input.GetKey(KeyCode.D))
            moveright = true;
        else
            moveright = false;

        // space - jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumppressed();
        }

        // shift - dash
        if (Input.GetKeyDown(KeyCode.LeftShift) & allow_Dash)
        {
            if (dashCoolCounter <= 0)
            {
                speed = dashSpeed;
                dashCounter = dashLength;
            }

        }



    }


    public void jumppressed()
    {
        //  Debug.Log("jumppressesss"); // just set it on a button ui

        jumpBufferCounter = jumpBufferTime;
        stopcounter = true;


        if (jump2 == true &&  allow_DoubleJump)
        {

            jumpBufferCounter = jumpBufferTime;
            stopcounter = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);

            jump2 = false;

            Debug.Log("jumpparessesss");
        }




        if (rb.linearVelocity.y > 0f)
        {
            //  rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;

        }


    }

    void Dashh()
    {
        if (!allow_Dash) return;

        if (dashCounter > 0)
        {

            dashCounter -= Time.deltaTime;

            if (dashCounter <= 0)
            {

                speed = 3;
                dashCoolCounter = dashCooldown;

            }

        }

        if (dashCoolCounter > 0)
        {
            dashCoolCounter -= Time.deltaTime;
         //   DashButton.image.color = Color.gray; // on cd
        }
        else
        {
          //  DashButton.image.color = Color.white; // available
        }
    }


    private void MovePlayer()
    {
        //if i press the left button
        if (moveleft)
        {
            DashDir = -1;
            X_Input = -1;
        }
        //if i press the right button
        else if (moveright)
        {
            DashDir = 1;
            X_Input = 1;
        }

        //if i am not pressing any button
        else
            X_Input = 0;
    }

 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(groundCheck.transform.position, 0.3f);
       // Gizmos.DrawWireSphere(groundCheck.position + new Vector3(0, -1, 0), 0.15f); //para sa bounce fall sna
    }


}
