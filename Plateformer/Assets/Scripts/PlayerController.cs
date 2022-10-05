using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float horizontalGroundSpeed;
    [SerializeField] private float horizontalAirSpeed;
    [SerializeField] private float verticalImpulse;
    [SerializeField] private float verticalMaxSpeed;
    [SerializeField] private float gravityValue;
    [SerializeField, Range(0, 1)] private float wallFriction;
    [SerializeField] private float fallGravityFactor;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField, Range(0, 1)] private float longJumpThreshold;
    [SerializeField] private int jumpNumber = 2;
    [SerializeField] private float platformClipSpeed;
    [SerializeField, Range(0,30)] private float inertia;
    [SerializeField, Range(0,20)] private float dashBrake;
    [SerializeField] private float dashValue;
    [SerializeField] private float dashTimer;
    [SerializeField, Range(0,1)] private float bouncyPlatformBounciness;
    [SerializeField] private float jumpTimeTolerance;
    //[SerializeField] private float 
    [SerializeField] private float wallGrabDuration;


    
    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private bool isCollidingWallLeft;
    private bool isCollidingWallRight;
    private Vector2 speed;
    private float currentMaxXSpeed;
    private int jumpsLeft;
    private bool mayJumpMidAir;
    private string platformTag;
    private float dashStartTime;
    private float wallGrabStopStartTime;
    private bool isGrabingWall;
    private Collider2D groundCollider2D;
    private bool isBouncing;
    private float jumpBufferTime;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        jumpsLeft = jumpNumber;
        dashStartTime = 0f;
        isGrabingWall = false;
        //Time.timeScale = .1f;
        
    }

    // Update is called once per frame
    void Update()
    {
        var curTime = Time.time;
        Vector2 groundedBoxCheck = (Vector2)transform.position + new Vector2(0, -.17f);
        Vector3 boxScale = new Vector3(transform.localScale.x * 0.9f, transform.localScale.y * .69f, transform.localScale.z);
        groundCollider2D = Physics2D.OverlapBox(groundedBoxCheck, boxScale, 0, groundLayer);
        isGrounded = groundCollider2D != null;
        platformTag = isGrounded ? groundCollider2D.tag : "";
        Vector2 wallCollisionsBoxCheck = (Vector2)transform.position;
        isCollidingWallLeft = Physics2D.OverlapBox(wallCollisionsBoxCheck + new Vector2(-.26f, 0f), new Vector3(transform.localScale.x * .5f, transform.localScale.y - .1f, transform.localScale.z), 0, wallLayer);
        isCollidingWallRight = Physics2D.OverlapBox(wallCollisionsBoxCheck + new Vector2(.26f, 0f), new Vector3(transform.localScale.x * .5f, transform.localScale.y - .1f, transform.localScale.z), 0, wallLayer);

        currentMaxXSpeed = isGrounded ? horizontalGroundSpeed : horizontalAirSpeed;
        if (Input.GetAxis("Jump") != 0 && jumpsLeft == 0) jumpBufferTime = jumpTimeTolerance;
        jumpBufferTime -= Time.deltaTime;
        if (isGrounded) jumpsLeft = jumpNumber;
        else if (!isGrounded && jumpsLeft == jumpNumber) jumpsLeft--;
        

        if (isCollidingWallLeft && isCollidingWallRight && isGrounded && speed.y <= platformClipSpeed && platformTag.Equals("SoftPlatform"))
        {
            speed.y = platformClipSpeed;
        }
        else if (isCollidingWallRight && isCollidingWallLeft)
        {
            //speed.x = 0;
        }
        else if (isCollidingWallRight)
        {
            speed.x = Input.GetAxis("Horizontal") > 0 ? 0 : (Input.GetAxis("Horizontal") * currentMaxXSpeed * Vector3.right).x;
            if (!isGrabingWall && Input.GetAxis("Horizontal") > 0)
            {
                isGrabingWall = true;
                speed.y = 0;
                jumpsLeft = 1;
            }
            else if(Input.GetAxis("Horizontal") == 0)
            {
                if(isGrabingWall)
                    wallGrabStopStartTime = Time.time;
                isGrabingWall = false;
            }
        }
        else if (isCollidingWallLeft)
        {
            speed.x = Input.GetAxis("Horizontal") < 0 ? 0 : (Input.GetAxis("Horizontal") * currentMaxXSpeed * Vector3.right).x;
            if (!isGrabingWall && Input.GetAxis("Horizontal") < 0)
            {
                isGrabingWall = true;
                speed.y = 0;
                jumpsLeft = 1;
            }
            else if (Input.GetAxis("Horizontal") == 0)
            {
                if (isGrabingWall)
                    wallGrabStopStartTime = Time.time;
                isGrabingWall = false;
            }
        } 
        else 
        {
            if (Mathf.Abs(speed.x) >= currentMaxXSpeed) speed.x *= Mathf.Max((1 - dashBrake * Time.deltaTime), 0);
            
            if ((Input.GetAxisRaw("Horizontal") == 0f) && isGrounded) speed.x *= Mathf.Max((1 - inertia*Time.deltaTime),0);
            else speed.x = Mathf.Abs(speed.x) > currentMaxXSpeed ? speed.x : (Input.GetAxis("Horizontal") * currentMaxXSpeed * Vector3.right).x;

            if (isGrabingWall)
                wallGrabStopStartTime = Time.time;
            isGrabingWall = false;
        }

        if ((isGrounded && speed.y <= 0 && !platformTag.Equals("BouncyPlatform")) || (platformTag.Equals("BouncyPlatform") && Mathf.Abs(speed.y) < .5f))
        {
            speed.y = 0;
            isBouncing = false;

        }
        if (isGrounded && platformTag.Equals("BouncyPlatform") && speed.y < -.5f)
        {
            isBouncing = true;
            speed.y = Mathf.Min(speed.y * bouncyPlatformBounciness * -1f / Mathf.Sqrt(fallGravityFactor), verticalMaxSpeed / Mathf.Sqrt(fallGravityFactor));
        }
        if ((Input.GetAxisRaw("Jump") > 0 || (jumpBufferTime > 0f && isGrounded)) && mayJumpMidAir && jumpsLeft > 0)
        {
            jumpBufferTime = 0f;
            mayJumpMidAir = false;
            jumpsLeft--;
            if (isGrabingWall || curTime < wallGrabStopStartTime + wallGrabDuration)
            {
                WallJump();
            }
            else
            {
                Jump();
            }
        }
        if (Input.GetAxisRaw("Jump") == 0 && !isBouncing)
        {
            mayJumpMidAir = true;
            if (speed.y >= verticalImpulse * longJumpThreshold) speed.y *= (longJumpThreshold);
        }
        if (Input.GetAxisRaw("Dash") != 0 && (curTime > dashStartTime + dashTimer))
        {
            Dash();
        }
        transform.position += (Vector3)speed * Time.deltaTime;
        ApplyPhysics();

    }

    private void OnDrawGizmos()
    {
        // Gizmos.DrawCube((Vector2)transform.position + new Vector2(0, -.17f), new Vector3(transform.localScale.x, transform.localScale.y * .69f, transform.localScale.z));
        //Gizmos.DrawCube((Vector2)transform.position + new Vector2(-.26f, 0f), new Vector3(transform.localScale.x * .5f, transform.localScale.y, transform.localScale.z));
        //Gizmos.DrawCube((Vector2)transform.position + new Vector2(.26f, 0f), new Vector3(transform.localScale.x * .5f, transform.localScale.y, transform.localScale.z));
    }

    private void Jump()
    {
        speed.y = verticalImpulse;
    }

    private void ApplyPhysics()
    {
        float gravityUsed = isGrabingWall ? gravityValue * wallFriction : gravityValue;
        if (speed.y > 0) speed.y -= gravityValue * Time.deltaTime;
        else if (speed.y > -verticalMaxSpeed) speed.y -= gravityUsed * fallGravityFactor * Time.deltaTime;
        else speed.y = -verticalMaxSpeed;
        
    }

    private void Dash()
    {
        dashStartTime = Time.time;
        float movDirX = speed.x == 0 ? 0 : speed.x / Mathf.Abs(speed.x);
        speed.x += dashValue * movDirX * currentMaxXSpeed;
    }

    private void WallJump()
    {
        speed.y = verticalImpulse;
        dashStartTime = Time.time;
        float movDirX = Input.GetAxisRaw("Horizontal");
        if (isCollidingWallRight)
            movDirX = -1;
        if(isCollidingWallLeft)
            movDirX = 1;
        speed.x += dashValue * movDirX * currentMaxXSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ColliderDistance2D distance = boxCollider.Distance(collision);
        if (distance.normal.x != 0)
        {
            transform.position += new Vector3(distance.normal.x * distance.distance, 0, 0);
        }
        if (distance.normal.y != 0)
        {
            transform.position += new Vector3(0, distance.normal.y * distance.distance, 0);
        }
        if (collision.tag.Equals("HardPlatform")) speed.y = 0;
        //if (collision.tag.Equals("BouncyPlatform")) speed.y = verticalImpulse  * -1f;
    }


}
