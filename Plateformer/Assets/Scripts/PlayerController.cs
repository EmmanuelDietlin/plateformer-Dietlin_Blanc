using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float horizontalGroundSpeed;
    [SerializeField] private float horizontalAirSpeed;
    [SerializeField] private float verticalImpulse;
    [SerializeField] private float DashValue;
    [SerializeField] private float gravityValue;
    [SerializeField] private LayerMask groundLayer;


    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private bool isGrounded;
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
        Debug.Log(isGrounded);
        if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)
        {
            transform.position += Input.GetAxis("Horizontal") * horizontalGroundSpeed * Vector3.right * Time.deltaTime;  
        }
        if (Input.GetAxis("Jump") > 0 && isGrounded)
        {
            rigidBody.AddForce(new Vector2(0, verticalImpulse), ForceMode2D.Impulse);
        }
        
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube((Vector2)transform.position + new Vector2(0, -.01f), transform.localScale);
    }
}
