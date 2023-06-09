using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    enum State
    {
        Idle,
        Patrol,
        Jump,
        Hit,
        Attack,
    }
    private Rigidbody2D rd;
    private Animator anim;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] LayerMask groundMask;
    private bool isJuming;
    private bool isHited;
    [SerializeField] private float moveSpeed;

    private void Awake()
    {
        rd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
            Move();
            if (!IsGroundExist())
            {
                Turn();
            }
    }

    private void FixedUpdate()
    {
       
    }

    private void Move()
    {
        rd.velocity = new Vector2(transform.right.x * -1 * moveSpeed, rd.velocity.y);
    }

    public void Turn() 
    { 
        transform.Rotate(Vector3.up, 180); 
    }

    private bool IsGroundExist()
    {
        Debug.DrawRay(groundCheckPoint.position, Vector2.down, Color.red);
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, 1f, groundMask);
    }
}
