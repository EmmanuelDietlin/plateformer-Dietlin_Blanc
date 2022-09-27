using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float horizontalGroundSpeed;
    [SerializeField] private float horizontalAirSpeed;
    [SerializeField] private float verticalImpulse;
    [SerializeField] private float verticalMaxSpeed;
    [SerializeField] private float DashValue;
    [SerializeField] private float gravityValue;
    [SerializeField] private float fallGravityFactor;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField, Range(0, 1)] private float longJumpThreshold;


    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private bool isCollidingWallLeft;
    private bool isCollidingWallRight;
    private Vector2 speed;
    private float currentMaxXSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 groundedBoxCheck = (Vector2)transform.position + new Vector2(0, -.46f);
        Vector3 boxScale = new Vector3(transform.localScale.x, transform.localScale.y / 6, transform.localScale.z);
        isGrounded = Physics2D.OverlapBox(groundedBoxCheck, boxScale, 0, groundLayer);
        Vector2 wallCollisionsBoxCheck = (Vector2)transform.position;
        isCollidingWallLeft = Physics2D.OverlapBox(wallCollisionsBoxCheck + new Vector2(-.01f, 0f), transform.localScale - new Vector3(0,0.05f, 0), 0, wallLayer);
        isCollidingWallRight = Physics2D.OverlapBox(wallCollisionsBoxCheck + new Vector2(.01f, 0f), transform.localScale - new Vector3(0, 0.05f, 0), 0, wallLayer);

        currentMaxXSpeed = isGrounded ? horizontalGroundSpeed : horizontalAirSpeed;
        if (isCollidingWallRight)
        {
            speed.x = Input.GetAxis("Horizontal") > 0 ? 0 : (Input.GetAxis("Horizontal") * currentMaxXSpeed * Vector3.right).x;  

        } else if (isCollidingWallLeft)
        {
            speed.x = Input.GetAxis("Horizontal") < 0 ? 0 : (Input.GetAxis("Horizontal") * currentMaxXSpeed * Vector3.right).x;
        } else
        {

            speed.x = (Input.GetAxis("Horizontal") * currentMaxXSpeed * Vector3.right).x;
        }
        if (isGrounded && speed.y <= 0) speed.y = 0;
        
        if (Input.GetAxis("Jump") > 0 && isGrounded)
        {
            Jump();
        }
        if (Input.GetAxis("Jump") == 0 && speed.y > verticalImpulse * longJumpThreshold) speed.y /= 1.5f;
        transform.position += (Vector3)speed * Time.deltaTime;
        ApplyPhysics();

        
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube((Vector2)transform.position + new Vector2(0, -.01f), transform.localScale);
        Gizmos.DrawCube((Vector2)transform.position, transform.localScale + new Vector3(.02f, 0, 0));
    }

    private void Jump()
    {
        speed.y = verticalImpulse;
    }

    private void ApplyPhysics()
    {
        if (speed.y > 0) speed.y -= gravityValue * Time.deltaTime;
        else if (speed.y > -verticalMaxSpeed) speed.y -= gravityValue * fallGravityFactor * Time.deltaTime;
        else speed.y = -verticalMaxSpeed;
        
    }


}
