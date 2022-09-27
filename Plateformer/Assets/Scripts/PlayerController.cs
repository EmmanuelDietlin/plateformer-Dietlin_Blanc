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
    [SerializeField] private LayerMask groundLayer;


    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private Vector2 speed;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 groundedBoxCheck = (Vector2)transform.position + new Vector2(0, -.01f);
        isGrounded = Physics2D.OverlapBox(groundedBoxCheck, transform.localScale, 0, groundLayer);
        if (Input.GetAxis("Horizontal") != 0)
        {
            speed.x = (Input.GetAxis("Horizontal") * horizontalGroundSpeed * Vector3.right).x;  

        } else 
        {
            speed.x = (Input.GetAxis("Horizontal") * horizontalGroundSpeed * Vector3.right).x;
        }
        if (isGrounded) speed.y = 0;
        if (Input.GetAxis("Jump") > 0 && isGrounded)
        {
            Jump();
        }
        transform.position += (Vector3)speed * Time.deltaTime;
        ApplyPhysics();

        
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube((Vector2)transform.position + new Vector2(0, -.01f), transform.localScale);
    }

    private void Jump()
    {
        speed.y = verticalImpulse;
    }

    private void ApplyPhysics()
    {
        speed.y = speed.y < -verticalMaxSpeed ? -verticalMaxSpeed : speed.y - gravityValue * Time.deltaTime;
        
    }


}
