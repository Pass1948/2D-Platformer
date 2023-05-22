using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rd;
    private Vector2 moveVec;
    private Animator anima;
    private SpriteRenderer render;

    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpPower;
    private bool isJump;

    private void Awake()
    {
        rd = GetComponent<Rigidbody2D>();
        anima = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // rd.AddForce(moveVec.normalized * speed, ForceMode2D.Force);
        // rd.AddForce(Vector2.right * moveVec.x * speed, ForceMode2D.Force);
        /*
        if (moveVec.x < 0 && rd.velocity.x > -maxSpeed)
            rd.AddForce(moveVec.normalized * speed, ForceMode2D.Force );
        else if (moveVec.x > 0 && rd.velocity.x < maxSpeed)
            rd.AddForce(moveVec.normalized * speed, ForceMode2D.Force);
        */

        float h = Input.GetAxisRaw("Horizontal");
        rd.AddForce(Vector2.right * h * speed, ForceMode2D.Force);

        if (moveVec.x < 0 && rd.velocity.x < maxSpeed*(-1))
            rd.velocity = new Vector2(maxSpeed * (-1), rd.velocity.y);
        else if (moveVec.x > 0 && rd.velocity.x > maxSpeed)
            rd.velocity = new Vector2(maxSpeed, rd.velocity.y);


    }

    private void OnMove(InputValue value)
    {
        moveVec = value.Get<Vector2>();
        anima.SetFloat("MoveSpeed", Mathf.Abs(moveVec.x));
        if (moveVec.x < 0)
        {
            render.flipX = true;
        }
        else
        {
            render.flipX = false;
        }
    }


    void Jump()
    {
        if (!isJump)
        {
            rd.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    void OnJump()
    {
        Jump();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        anima.SetBool("IsGround", true);
        isJump = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        anima.SetBool("IsGround", false);
        isJump = true;
    }
    
}
    

