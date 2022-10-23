using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    #region Inspector Parameters

    #region Jump

    [Header("Jump")]
    [Space(4)]
    [SerializeField] private float verticalImpulse;
    public float VerticalImpulse { get { return this.verticalImpulse; } set { this.verticalImpulse = value; } }
    [SerializeField, Range(0, 1)] private float longJumpThreshold;
    public float LongJumpThreshold { get { return this.longJumpThreshold; } set { this.longJumpThreshold = value; } }
    [SerializeField] private int jumpNumber = 2;
    public int JumpNumber { get { return this.jumpNumber; } set { this.jumpNumber = value; } }
    [SerializeField] private float jumpTimeTolerance;
    public float JumpTimeTolerance { get { return this.jumpTimeTolerance; } set { this.jumpTimeTolerance = value; } }

    [SerializeField] private float toleranceJumpDuration;
    public float ToleranceJumpDuration { get { return this.toleranceJumpDuration; } set { this.toleranceJumpDuration = value; } }
    [Space(10)]

    #endregion

    #region Dash

    [Header("Dash")]
    [Space(4)]
    [SerializeField, Range(0, 20)] private float dashBrake;
    public float DashBrake { get { return this.dashBrake; } set { this.dashBrake = value; } }
    [SerializeField] private float dashForce;
    public float DashForce { get { return this.dashForce; } set { this.dashForce = value; } }
    [SerializeField] private float dashDelay;
    public float DashDelay { get { return this.dashDelay; } set { this.dashDelay = value; } }
    [Space(10)]

    #endregion

    #region Ground Movement

    [Header("Ground Movement")]
    [Space(4)]
    [SerializeField] private float horizontalGroundSpeed;
    public float HorizontalGroundSpeed { get { return this.horizontalGroundSpeed; }  set { this.horizontalGroundSpeed = value; } }
    [SerializeField, Range(0,30)] private float brakeForce;
    public float BrakeForce { get { return this.brakeForce; }  set { this.brakeForce = value; } }
    [SerializeField, Range(1,3)] private float sprintSpeedFactor;
    public float SprintSpeedFactor { get { return this.sprintSpeedFactor; } set { this.sprintSpeedFactor = value; } }
    [Space(10)]

    #endregion

    #region Air Movement

    [Header("Air Movement")]
    [Space(4)]
    [SerializeField] private float horizontalAirSpeed;
    public float HorizontalAirSpeed { get { return this.horizontalAirSpeed; } set { this.horizontalAirSpeed = value; } }
    [SerializeField, Range(0, 2)] private float fallGravityFactor;
    public float FallGravityFactor { get { return this.fallGravityFactor; } set { this.fallGravityFactor = value; } }
    [SerializeField] private float gravityValue;
    public float GravityValue { get { return this.gravityValue; } set { this.gravityValue = value; } }
    [SerializeField] private float verticalMaxSpeed;
    public float VerticalMaxSpeed { get { return this.verticalMaxSpeed; } set { this.verticalMaxSpeed = value; } }
    [Space(10)]

    #endregion

    #region Wall Jump

    [Header("Wall Jump")]
    [Space(4)]
    [SerializeField, Range(0, 1)] private float wallFriction;
    public float WallFriction { get { return this.wallFriction; } set { this.wallFriction = value; } }
    [SerializeField] private float wallGrabDuration;
    public float WallGrabDuration { get { return this.wallGrabDuration; } set { this.wallGrabDuration = value; } }
    [Space(10)]

    #endregion

    #region Special Platforms

    [Header("Special Platforms")]
    [Space(4)]
    [SerializeField] private float platformClipSpeed;
    public float PlatformClipSpeed { get { return this.platformClipSpeed; } set { this.platformClipSpeed = value; } }

    [SerializeField] private float descendingPlatformSpeed;
    public float DescendingPlatformSpeed { get { return this.descendingPlatformSpeed; } set { this.descendingPlatformSpeed = value; } }

    [SerializeField, Range(0, 1)] private float bouncyPlatformBounciness;
    public float BouncyPlatformBounciness { get { return this.bouncyPlatformBounciness; } set { this.bouncyPlatformBounciness = value; } }
    [Space(10)]

    #endregion

    #region Collisions

    [Header("Collisions")]
    [Space(4)]
    [SerializeField] private LayerMask groundLayer;
    public LayerMask GroundLayer { get { return this.groundLayer; } set { this.groundLayer = value; } }
    [SerializeField] private LayerMask wallLayer;
    public LayerMask WallLayer { get { return this.wallLayer; } set { this.wallLayer = value; } }
    [Space(10)]

    #endregion

    #endregion

    #region Private field

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
    private float isGroundedStopStartTime;
    private bool isGrabingWall;
    private Collider2D groundCollider2D;
    private bool isBouncing;
    private float jumpBufferTime;
    private Vector3 startPosition;
    private Feedbacks feedbacks;
    private bool prevGroundedStatus;
    private Vector2 horizontalMovDirection;
    private float currentHorizontalSpeed;
    public float CurrentHorizontalSpeed { get { return currentHorizontalSpeed; } }
    private bool isDescending;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        jumpsLeft = JumpNumber;
        dashStartTime = 0f;
        isGrabingWall = false;
        startPosition = transform.position;
        feedbacks = gameObject.GetComponent<Feedbacks>();
        isDescending = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D ground = Physics2D.CircleCast(transform.position, transform.localScale.x * 0.45f, Vector2.down, boxCollider.bounds.extents.y + .5f, groundLayer);
        horizontalMovDirection = Quaternion.Euler(0, 0, -90) * ground.normal;

        prevGroundedStatus = isGrounded;
        var curTime = Time.time;
        Vector2 groundedBoxCheck = (Vector2)transform.position + (new Vector2(this.transform.up.x, this.transform.up.y) * -0.17f);
        Vector3 boxScale = new Vector3(transform.localScale.x * 0.9f, transform.localScale.y * .69f, transform.localScale.z);
        groundCollider2D = Physics2D.OverlapBox(groundedBoxCheck, boxScale, -transform.rotation.eulerAngles.z, GroundLayer);
        isGrounded = groundCollider2D != null;
        platformTag = isGrounded ? groundCollider2D.tag : "";
        Vector2 wallCollisionsBoxCheck = (Vector2)transform.position;

        isCollidingWallLeft = Physics2D.OverlapBox(wallCollisionsBoxCheck + (new Vector2(this.transform.right.x, this.transform.right.y) * -0.32f), new Vector3(transform.localScale.x * .5f, transform.localScale.y - .2f, transform.localScale.z), -transform.rotation.eulerAngles.z, WallLayer);
        isCollidingWallRight = Physics2D.OverlapBox(wallCollisionsBoxCheck + (new Vector2(this.transform.right.x, this.transform.right.y) * 0.32f), new Vector3(transform.localScale.x * .5f, transform.localScale.y - .2f, transform.localScale.z), -transform.rotation.eulerAngles.z, WallLayer);

        currentMaxXSpeed = isGrounded ? HorizontalGroundSpeed : HorizontalAirSpeed;

        if (isGrounded && Input.GetAxisRaw("Sprint") > 0) currentMaxXSpeed = HorizontalGroundSpeed * SprintSpeedFactor;

        if (Input.GetAxis("Jump") != 0 && jumpsLeft == 0)
            jumpBufferTime = JumpTimeTolerance;
        jumpBufferTime -= Time.deltaTime;

        if(!isGrounded && isGrounded != prevGroundedStatus)
            isGroundedStopStartTime = Time.time;

        if (isGrounded)
        {
            if (isGrounded != prevGroundedStatus)
            {
                feedbacks.JumpParticles();
            }
            if (platformTag.Equals("Spike")) feedbacks.OnDamageTaken();
            jumpsLeft = JumpNumber;
        }
        else if (jumpsLeft >= JumpNumber && curTime > isGroundedStopStartTime + ToleranceJumpDuration)
        {
            jumpsLeft = JumpNumber - 1;
        }

        if(Input.GetAxisRaw("Vertical") < 0 && platformTag.Equals("SoftPlatform"))
            isDescending = true;

        if (isDescending)
        {
            speed.y = -DescendingPlatformSpeed;
            Debug.Log("Vertical" + speed.y);
            if (!platformTag.Equals("SoftPlatform"))
                isDescending = false;
        }
        else if (isCollidingWallLeft && isCollidingWallRight && isGrounded && speed.y <= PlatformClipSpeed && platformTag.Equals("SoftPlatform"))
        {
            isBouncing = false;
            speed.y = PlatformClipSpeed;
            feedbacks.PlaySound(Feedbacks.sounds.succion);
        }
        else if (isCollidingWallRight && isCollidingWallLeft)
        {
            isBouncing = false;
            if (platformTag.Equals("SoftPlatform")) feedbacks.PlaySound(Feedbacks.sounds.succion);
            //transform.position = startPosition;
        }
        else if (isCollidingWallRight)
        {
            isBouncing = false;
            speed.x = Input.GetAxis("Horizontal") > 0 ? 0 : Input.GetAxis("Horizontal") * currentMaxXSpeed;
            if (!isGrabingWall && Input.GetAxis("Horizontal") > 0)
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
            currentHorizontalSpeed = speed.x;
        }
        else if (isCollidingWallLeft)
        {
            isBouncing = false;
            speed.x = Input.GetAxis("Horizontal") < 0 ? 0 : Input.GetAxis("Horizontal") * currentMaxXSpeed;
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
            currentHorizontalSpeed = speed.x;

        }
        else
        {

            if (Mathf.Abs(speed.x) >= currentMaxXSpeed) speed.x *= Mathf.Max((1 - DashBrake * Time.deltaTime), 0);
            
            if ((Input.GetAxisRaw("Horizontal") == 0f) && isGrounded) speed.x *= Mathf.Max((1 - BrakeForce * Time.deltaTime),0);
            else speed.x = Mathf.Abs(speed.x) > currentMaxXSpeed ? speed.x : Input.GetAxis("Horizontal") * currentMaxXSpeed;

            if (platformTag.Equals("Slope"))
            {
                var tmp = speed.x;
                speed.x = tmp * horizontalMovDirection.normalized.x;
                speed.y = tmp * horizontalMovDirection.normalized.y;
                currentHorizontalSpeed = Mathf.Abs(tmp);
            }
            else
            {
                currentHorizontalSpeed = speed.x;
            }
            

            if (isGrabingWall)
                wallGrabStopStartTime = Time.time;
            isGrabingWall = false;
        }
        if (isGrounded && platformTag.Equals("BouncyPlatform") && speed.y < -5f)
        {
            isBouncing = true;
            Bounce();
        }
        else if ((isGrounded && speed.y <= 0 && !platformTag.Equals("BouncyPlatform") && !isDescending) || (platformTag.Equals("BouncyPlatform") && speed.y > -5f))
        {
            isBouncing = false;
            if (!platformTag.Equals("Slope")) speed.y = 0;
            if (prevGroundedStatus != isGrounded) transform.position += new Vector3(0, -.02f, 0);
        }
        //Debug.Log("grabbing wall ? " + isGrabingWall);
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
        if (Input.GetAxisRaw("Dash") != 0 && (curTime > dashStartTime + dashDelay))
        {
            Dash();
        }

        if (ground.collider != null)
        {
            float angle = Vector2.SignedAngle(Vector2.right, horizontalMovDirection);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }

        float movDirX = speed.x == 0 ? 0 : speed.x / Mathf.Abs(speed.x);
        RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position, (Vector2)transform.localScale, Vector2.SignedAngle(Vector2.right, horizontalMovDirection), speed, speed.magnitude * Time.deltaTime, wallLayer);
        if (hit.collider == null || hit.collider.tag.Equals("SoftPlatform"))
        {
            transform.position += (Vector3)speed * Time.deltaTime;
        }
        else
        {
            ColliderDistance2D distance = boxCollider.Distance(hit.collider);
            if (distance.normal.x != 0)
            {
                transform.position += new Vector3(distance.normal.x * distance.distance, 0, 0);
            }
            if (distance.normal.y != 0)
            {
                transform.position += new Vector3(0, distance.normal.y * distance.distance, 0);
            }
        }

        if (!platformTag.Equals("Slope"))
        {
            ApplyPhysics();
        }
        feedbacks.Stretch(currentMaxXSpeed + DashForce, verticalMaxSpeed);

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
        speed.x += DashForce * movDirX * currentMaxXSpeed;
    }

    private void WallJump()
    {
        speed.y = verticalImpulse;
        dashStartTime = Time.time;
        float movDirX = Input.GetAxisRaw("Horizontal");
        if (isCollidingWallRight)
            movDirX = -1;
        if (isCollidingWallLeft)
            movDirX = 1;
        speed.x += DashForce * movDirX * currentMaxXSpeed;
    }

    private void Bounce()
    {
        speed.y = Mathf.Min(speed.y * BouncyPlatformBounciness * -1f / Mathf.Sqrt(FallGravityFactor), VerticalMaxSpeed / Mathf.Sqrt(FallGravityFactor));
        feedbacks.OnBounce();
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
        if (collision.tag.Equals("HardPlatform") || collision.tag.Equals("Slope")) speed.y = 0;

        if (collision.tag.Equals("Finish")) feedbacks.OnVictory();
    }




}
