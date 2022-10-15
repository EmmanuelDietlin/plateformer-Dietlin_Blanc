using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Rendering;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [Space(4)]
    [SerializeField] private float verticalImpulse;
    public float VerticalImpulse { get { return this.verticalImpulse; }  set { this.verticalImpulse = value; } }
    [SerializeField, Range(0, 1)] private float longJumpThreshold;
    public float LongJumpThreshold { get { return this.longJumpThreshold; }  set { this.longJumpThreshold = value; } }
    [SerializeField] private int jumpNumber = 2;
    public int JumpNumber { get { return this.jumpNumber; }  set { this.jumpNumber = value; } }
    [SerializeField] private float jumpTimeTolerance;
    public float JumpTimeTolerance { get { return this.jumpTimeTolerance; }  set { this.jumpTimeTolerance = value; } }
    [Space(10)]

    [Header("Dash")]
    [Space(4)]
    [SerializeField, Range(0,20)] private float dashBrake;
    public float DashBrake { get { return this.dashBrake; }  set { this.dashBrake = value; } }
    [SerializeField] private float dashValue;
    public float DashValue { get { return this.dashValue; }  set { this.dashValue = value; } }
    [SerializeField] private float dashTimer;
    public float DashTimer { get { return this.dashTimer; }  set { this.dashTimer = value; } }
    [Space(10)]

    [Header("Ground Movement")]
    [Space(4)]
    [SerializeField] private float horizontalGroundSpeed;
    public float HorizontalGroundSpeed { get { return this.horizontalGroundSpeed; }  set { this.horizontalGroundSpeed = value; } }
    [SerializeField, Range(0,30)] private float inertia;
    public float Inertia { get { return this.inertia; }  set { this.inertia = value; } }
    [SerializeField, Range(1,3)] private float sprintSpeedFactor;
    public float SprintSpeedFactor { get { return this.sprintSpeedFactor; } set { this.sprintSpeedFactor = value; } }
    [Space(10)]

    [Header("Air Movement")]
    [Space(4)]
    [SerializeField] private float horizontalAirSpeed;
    public float HorizontalAirSpeed { get { return this.horizontalAirSpeed; }  set { this.horizontalAirSpeed = value; } }
    [SerializeField, Range(0, 2)] private float fallGravityFactor;
    public float FallGravityFactor { get { return this.fallGravityFactor; }  set { this.fallGravityFactor = value; } }
    [SerializeField] private float gravityValue;
    public float GravityValue { get { return this.gravityValue; }  set { this.gravityValue = value; } } 
    [SerializeField] private float verticalMaxSpeed;
    public float VerticalMaxSpeed { get { return this.verticalMaxSpeed; }  set { this.verticalMaxSpeed = value; } }
    [Space(10)]

    [Header("Wall Jump")]
    [Space(4)]
    [SerializeField, Range(0, 1)] private float wallFriction;
    public float WallFriction { get { return this.wallFriction; }  set { this.wallFriction = value; } } 
    [SerializeField] private float wallGrabDuration;
    public float WallGrabDuration { get { return this.wallGrabDuration; }  set { this.wallGrabDuration = value; } }
    [Space(10)]

    [Header("Special Platforms")]
    [Space(4)]
    [SerializeField] private float platformClipSpeed;
    public float PlatformClipSpeed { get { return this.platformClipSpeed; }  set { this.platformClipSpeed = value; } }
    [SerializeField, Range(0,1)] private float bouncyPlatformBounciness;
    public float BouncyPlatformBounciness { get { return this.bouncyPlatformBounciness; }  set { this.bouncyPlatformBounciness = value; } }
    [Space(10)]

    [Header("Collisions")]
    [Space(4)]
    [SerializeField] private LayerMask groundLayer;
    public LayerMask GroundLayer { get { return this.groundLayer; }  set { this.groundLayer = value; } }
    [SerializeField] private LayerMask wallLayer;
    public LayerMask WallLayer { get { return this.wallLayer; }  set { this.wallLayer = value; } }
    [Space(10)]





    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private bool isCollidingWallLeft;
    private bool isCollidingWallRight;
    private Vector2 speed;
    public Vector2 Speed { get { return speed; } }
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
    private Vector3 startPosition;
    private Feedbacks feedbacks;
    private bool prevGroundedStatus;
    private Vector2 horizontalMovDirection;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        jumpsLeft = JumpNumber;
        dashStartTime = 0f;
        isGrabingWall = false;
        startPosition = transform.position;
        feedbacks = gameObject.GetComponent<Feedbacks>();
    }

    /*
     * Pour impl�menter les pentes :
     * - appliquer un raycast vertical dans la direction de d�placement
     * - r�cup�rer la normale de la surface
     * - Vecteur de direction de d�placement = rotation de 90� de la normale (sens horaire ou antihoraire ?)
     * - se d�placer selon cette direction 
     * Remarque : n'appliquer cela que pour les mouvements horizontaux
     * Remarque bis : rendre la pente max modifiable dans l'inspecteur ainsi que l'�ventuelle 
     * facteur d'adh�sion � la pente (i.e. le personnage glisse en arri�re)
     */


    // Update is called once per frame
    void Update()
    {
        RaycastHit2D ground = Physics2D.Raycast(transform.position, Vector2.down, boxCollider.bounds.extents.y + .1f, groundLayer);
        Debug.Log(ground.normal);
        horizontalMovDirection = Quaternion.Euler(0, 0, 90) * ground.normal;
        //Debug.Log(horizontalMovDirection);
        prevGroundedStatus = isGrounded;
        var curTime = Time.time;
        Vector2 groundedBoxCheck = (Vector2)transform.position + new Vector2(0, -.17f);
        Vector3 boxScale = new Vector3(transform.localScale.x * 0.9f, transform.localScale.y * .69f, transform.localScale.z);
        groundCollider2D = Physics2D.OverlapBox(groundedBoxCheck, boxScale, transform.rotation.z, GroundLayer);
        isGrounded = groundCollider2D != null;
        platformTag = isGrounded ? groundCollider2D.tag : "";
        Vector2 wallCollisionsBoxCheck = (Vector2)transform.position;
        isCollidingWallLeft = Physics2D.OverlapBox(wallCollisionsBoxCheck + new Vector2(-.26f, 0f), new Vector3(transform.localScale.x * .5f, transform.localScale.y - .1f, transform.localScale.z), transform.rotation.z, WallLayer);
        isCollidingWallRight = Physics2D.OverlapBox(wallCollisionsBoxCheck + new Vector2(.26f, 0f), new Vector3(transform.localScale.x * .5f, transform.localScale.y - .1f, transform.localScale.z), transform.rotation.z, WallLayer);

        currentMaxXSpeed = isGrounded ? HorizontalGroundSpeed : HorizontalAirSpeed;
        if (isGrounded && Input.GetAxisRaw("Sprint") > 0) currentMaxXSpeed = HorizontalGroundSpeed * SprintSpeedFactor;

        if (Input.GetAxis("Jump") != 0 && jumpsLeft == 0) 
            jumpBufferTime = JumpTimeTolerance;
        jumpBufferTime -= Time.deltaTime;
        if (isGrounded)
        {
            if (isGrounded != prevGroundedStatus)
            {
                feedbacks.JumpParticles();
            }
            jumpsLeft = JumpNumber;
        }
        else if (!isGrounded && jumpsLeft == JumpNumber && groundCollider2D != null && groundCollider2D.transform.position.y > transform.position.y)
        {
            jumpsLeft--;
        }
        if (isCollidingWallLeft && isCollidingWallRight && isGrounded && speed.y <= PlatformClipSpeed && platformTag.Equals("SoftPlatform"))
        {
            isBouncing = false;
            speed.y = PlatformClipSpeed;
        }
        else if (isCollidingWallRight && isCollidingWallLeft)
        {
            isBouncing = false;
            //transform.position = startPosition;
        }
        else if (isCollidingWallRight)
        {
            isBouncing = false;
            speed.x = Input.GetAxis("Horizontal") > 0 ? 0 : (Input.GetAxis("Horizontal") * currentMaxXSpeed * Vector3.right).x;
            if (!isGrabingWall && Input.GetAxis("Horizontal") > 0)
            {
                isGrabingWall = true;
                speed.y = 0;
                jumpsLeft++;
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
            isBouncing = false;
            speed.x = Input.GetAxis("Horizontal") < 0 ? 0 : (Input.GetAxis("Horizontal") * currentMaxXSpeed * Vector3.right).x;
            if (!isGrabingWall && Input.GetAxis("Horizontal") < 0)
            {
                isGrabingWall = true;
                speed.y = 0;
                jumpsLeft++;
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
            if (Mathf.Abs(speed.x) >= currentMaxXSpeed) speed.x *= Mathf.Max((1 - DashBrake * Time.deltaTime), 0);
            
            if ((Input.GetAxisRaw("Horizontal") == 0f) && isGrounded) speed.x *= Mathf.Max((1 - Inertia*Time.deltaTime),0);
            else speed.x = Mathf.Abs(speed.x) > currentMaxXSpeed ? speed.x : (Input.GetAxis("Horizontal") * currentMaxXSpeed * Vector3.right).x;

            if (isGrabingWall)
                wallGrabStopStartTime = Time.time;
            isGrabingWall = false;
        }
      
        if (isGrounded && platformTag.Equals("BouncyPlatform") && speed.y < -.5f)
        {
            isBouncing = true;
            Bounce();
        } 
        else if ((isGrounded && speed.y <= 0 && !platformTag.Equals("BouncyPlatform")) || (platformTag.Equals("BouncyPlatform") && Mathf.Abs(speed.y) < .5f))
        {
            isBouncing = false;
            speed.y = 0;
            if (prevGroundedStatus != isGrounded) transform.position += new Vector3(0, -.02f, 0);
        }
        if ((Input.GetAxisRaw("Jump") > 0 || (jumpBufferTime > 0f && isGrounded)) && mayJumpMidAir && jumpsLeft > 0)
        {
            jumpBufferTime = 0f;
            mayJumpMidAir = false;
            jumpsLeft--;
            if (isGrabingWall || curTime < wallGrabStopStartTime + WallGrabDuration)
            {
                WallJump();
            }
            else
            {
                Jump();
            }
        }
        if (Input.GetAxisRaw("Jump") == 0)
        {
            mayJumpMidAir = true;
            if (speed.y >= VerticalImpulse * LongJumpThreshold && !isBouncing)
            {
                speed.y *= (LongJumpThreshold);
            }
        }
        if (Input.GetAxisRaw("Dash") != 0 && (curTime > dashStartTime + DashTimer))
        {
            Dash();
        }

        if (!isGrounded)
        {
            Vector2 rightEdge = (Vector2)transform.position + new Vector2(boxCollider.bounds.extents.x, -boxCollider.bounds.extents.y);
            Vector2 leftEdge = (Vector2)transform.position + new Vector2(-boxCollider.bounds.extents.x, -boxCollider.bounds.extents.y);
            Vector2 bottomCenter = (Vector2)transform.position + new Vector2(0, -boxCollider.bounds.extents.y);
            RaycastHit2D rightHit = Physics2D.Raycast(rightEdge, Vector2.down, Mathf.Abs(speed.y) * Time.deltaTime, groundLayer);
            RaycastHit2D leftHit = Physics2D.Raycast(leftEdge, Vector2.down, Mathf.Abs(speed.y) * Time.deltaTime, groundLayer);
            Debug.DrawRay(leftEdge, Vector2.down * (Mathf.Abs(speed.y) * Time.deltaTime), Color.white, 1);
            Debug.DrawRay(rightEdge, Vector2.down * (Mathf.Abs(speed.y) * Time.deltaTime), Color.white, 1);
            if (rightHit.collider != null && rightHit.collider.tag.Equals("Slope"))
            {
                float angle = Vector2.Angle(Vector2.down, -rightHit.normal);
                Debug.Log(angle);
                transform.RotateAround(leftEdge, -Vector3.back, angle);
                bottomCenter = Quaternion.Euler(0, 0, angle) * bottomCenter;
                RaycastHit2D slope = Physics2D.Raycast(bottomCenter, -transform.up, 1f, groundLayer);
                transform.Translate(-slope.normal * slope.distance, Space.Self);
            }
            else if (leftHit.collider != null && leftHit.collider.tag.Equals("Slope"))
            {
                float angle = Vector2.Angle(Vector2.down, -leftHit.normal);
                Debug.Log(angle);
                transform.RotateAround(rightEdge, -Vector3.back, angle);
                bottomCenter = Quaternion.Euler(0, 0, angle) * bottomCenter;
                RaycastHit2D slope = Physics2D.Raycast(bottomCenter, -transform.up, 1f, groundLayer);
                transform.Translate(-slope.normal * slope.distance, Space.Self);
            } else
            {
                transform.rotation = Quaternion.identity;
            }
            
        }

        transform.position += (Vector3)speed * Time.deltaTime;
        ApplyPhysics();
        feedbacks.Stretch(currentMaxXSpeed + dashValue, verticalMaxSpeed);

    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube((Vector2)transform.position + new Vector2(0, -.17f), new Vector3(transform.localScale.x, transform.localScale.y * .69f, transform.localScale.z));
        //Gizmos.DrawCube((Vector2)transform.position + new Vector2(-.26f, 0f), new Vector3(transform.localScale.x * .5f, transform.localScale.y, transform.localScale.z));
        //Gizmos.DrawCube((Vector2)transform.position + new Vector2(.26f, 0f), new Vector3(transform.localScale.x * .5f, transform.localScale.y, transform.localScale.z));
    }

    private void Jump()
    {
        speed.y = verticalImpulse;
    }

    private void ApplyPhysics()
    {
        float gravityUsed = isGrabingWall ? GravityValue * WallFriction : GravityValue;
        if (speed.y > 0) speed.y -= GravityValue * Time.deltaTime;
        else if (speed.y > -VerticalMaxSpeed) speed.y -= gravityUsed * FallGravityFactor * Time.deltaTime;
        else speed.y = -VerticalMaxSpeed;
        
    }

    private void Dash()
    {
        dashStartTime = Time.time;
        float movDirX = speed.x == 0 ? 0 : speed.x / Mathf.Abs(speed.x);
        speed.x += DashValue * movDirX * currentMaxXSpeed;
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
        speed.x += DashValue * movDirX * currentMaxXSpeed;
    }

    private void Bounce()
    {
        speed.y = Mathf.Min(speed.y * BouncyPlatformBounciness * -1f / Mathf.Sqrt(FallGravityFactor), VerticalMaxSpeed / Mathf.Sqrt(FallGravityFactor));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ColliderDistance2D distance = boxCollider.Distance(collision);
        if (!collision.tag.Equals("Slope"))
        {
            if (distance.normal.x != 0)
            {
                transform.position += new Vector3(distance.normal.x * distance.distance, 0, 0);
            }
            if (distance.normal.y != 0)
            {
                transform.position += new Vector3(0, distance.normal.y * distance.distance, 0);
            }
            if (collision.tag.Equals("HardPlatform")) speed.y = 0;
            if (collision.tag.Equals("Spike")) feedbacks.OnDamageTaken();
            if (collision.tag.Equals("Finish")) feedbacks.OnVictory();
        } else
        {
            transform.position += (Vector3)(distance.normal * distance.distance);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Slope"))
        {
            transform.rotation = Quaternion.identity;
        }
    }


}
