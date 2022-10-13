using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private float moveTime;
    [SerializeField] private float idleTime;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private LayerMask platformLayer;

    private Vector2 colliderBoxScale;

    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        colliderBoxScale = transform.localScale + new Vector3(.2f, .2f, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube((Vector2)transform.position, colliderBoxScale);
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position, colliderBoxScale, 0, ~platformLayer);
        string colliderTag = collider != null ? collider.tag : "";
        if (timer < moveTime)
        {
            transform.position += (Vector3)(speed * moveDirection * Time.deltaTime);
            if (colliderTag.Equals("Player")) collider.transform.position += (Vector3)(speed * moveDirection * Time.deltaTime);
        }
        else if (timer > moveTime + idleTime && timer < 2 * moveTime + idleTime)
        {
            transform.position -= (Vector3)(speed * moveDirection * Time.deltaTime);
            if (colliderTag.Equals("Player")) collider.transform.position -= (Vector3)(speed * moveDirection * Time.deltaTime);
        }
        else if (timer > 2 * moveTime + 2 * idleTime)
            timer = 0f;
        timer += Time.deltaTime;
    }


}
