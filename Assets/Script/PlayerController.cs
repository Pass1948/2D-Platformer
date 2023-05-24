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

    [SerializeField] LayerMask groundLayer;
 
    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpPower;
    private bool isGround;

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

    private void FixedUpdate()
    {
        GroundCheck();
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

        if (moveVec.x < 0 && rd.velocity.x < maxSpeed * (-1))
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
        if (isGround)
        {
            rd.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    void OnJump()
    {
        Jump();
    }

    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 1.5f, groundLayer);
        
        if (hit.collider != null)
        {
            //Debug.Log(hit.collider.gameObject.name);
            isGround = true; 
            anima.SetBool("IsGround", true);
            Debug.DrawRay(transform.position, new Vector3(hit.point.x, hit.point.y, 0) - transform.position, Color.red);
        }
        else
        {
            isGround = false;
            anima.SetBool("IsGround", false);
            Debug.DrawRay(transform.position, Vector3.down * 1.5f, Color.green);
        }
    }


    // 몬스터랑 충돌할 경우 1
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            Debug.Log("쿵했음");
            float h = Input.GetAxisRaw("Horizontal");
            rd.AddForce(Vector2.right * h * 5f, ForceMode2D.Impulse);
            rd.AddForce(Vector2.up * h * 5f, ForceMode2D.Impulse);

        }

    }


    // 점프 무한 방지법 1
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isGround = true;                        // 땅에 있을경우 bool형 체크
        anima.SetBool("IsGround", true);        // 애니메이션

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isGround = false;
        anima.SetBool("IsGround", false); 
    }
    */
}


