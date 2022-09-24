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


    private Rigidbody2D rigidBody;
    private bool isJumping;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        isJumping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)
        {
            transform.position += Input.GetAxis("Horizontal") * horizontalGroundSpeed * Vector3.right * Time.deltaTime;  
        }
        if (Input.GetAxis("Jump") > 0 && !isJumping)
        {
            isJumping=true;
            rigidBody.AddForce(new Vector2(0, verticalImpulse), ForceMode2D.Impulse);
        }
        
        
    }
}
