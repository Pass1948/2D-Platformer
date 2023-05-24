using BeeState;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class Bee : MonoBehaviour
{
    
    public float detectRange;
    public float attackRange;
    public float speed;


    public  Transform[] patrolPoints;
    private StateBase[] states;
    private State curState;

    // 밑 세개는 나중에 모델클래스에 따로 빼는게 좋음
    public Transform player;
    public Vector3 returnPosition;
    public int patrolIndex = 0;

    //================================================================================

    private void Awake()
    {
        states = new StateBase[(int)State.Size];
        states[(int)State.Idle] = new IdelState(this);
        states[(int)State.Trace] = new TraceState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Return] = new ReturnState(this);
        states[(int)State.Patrol] = new PatrolState(this);
    }

    private void Start()
    {
        curState = State.Idle;
        states[(int)curState].Enter();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        returnPosition = transform.position;
    }

    private void Update()
    {
        states[(int)curState].Update();                 // 현재상태 업데이트
    }

    public void ChangeState(State state)
    {
        states[(int)curState].Exit();
        curState = state;
        states[(int)curState].Enter();
    }
}


// =================================상태클래스들=================================
namespace BeeState
{
    public enum State
    {
        Idle,
        Trace,
        Attack,
        Return,
        Patrol,
        Size,
    }                   // Size는 현재 배열의 크기를 시각적으로 나타내주기위한 요소임

    public class IdelState : StateBase
    {
        private Bee bee;
        public IdelState (Bee bee)
        {
            this.bee = bee;
        }

        float idelTime = 0;
        public override void Update()
        {
            // 아무것도 안하기
            // 만약 플레이어에 근접하면 추적상태로 변경
            if (idelTime > 2)
            {
                idelTime = 0;
                bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length;
                bee.ChangeState(State.Patrol);
            }
            idelTime += Time.deltaTime;
            if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                bee.ChangeState(State.Trace);
            }
        }

        public override void Enter()
        {
            Debug.Log("IdelEnter");
        }

        public override void Exit()
        {
            Debug.Log("IdelExit");
        }
    }

    public class TraceState : StateBase
    {
        private Bee bee;
        public TraceState(Bee bee)
        {
            this.bee = bee;
        }

        public override void Enter()
        {
            Debug.Log("TraceEnter");
        }

        public override void Exit()
        {
            Debug.Log("TraceExit");
        }

        public override void Update()
        {
            // 플레이어 쫒아가기
            Vector2 dir = (bee.player.position - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.speed * Time.deltaTime);

            // 플레이어가 범위 밖일경우
            if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.detectRange)
            {
                bee.ChangeState(State.Return);
            }
            //공격범위 안에 있을때
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.attackRange)
            {
                bee.ChangeState(State.Attack);
            }
        }
    }
    public class ReturnState : StateBase
    {
        private Bee bee;
        public ReturnState(Bee bee)
        {
            this.bee = bee;
        }

        public override void Enter()
        {
            Debug.Log("ReturnEnter");
        }

        public override void Exit()
        {
            Debug.Log("ReturnExit");
        }

        public override void Update()
        {
            // 원래자리로 돌아가기
            Vector2 dir = (bee.returnPosition - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.speed * Time.deltaTime);

            // 원래자리로 도착했을때
            if (Vector2.Distance(bee.transform.position, bee.returnPosition) < 0.2f)
            {
                bee.ChangeState(State.Idle);
            }
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                bee.ChangeState(State.Trace);
            }
        }
    }
    public class AttackState : StateBase
    {
        private Bee bee;
        public AttackState(Bee bee)
        {
            this.bee = bee;
        }
        float lastAttackTime = 0;
        public override void Update()
        {
            if (lastAttackTime > 1.5f)
            {
                Debug.Log("쏘기!");
                lastAttackTime = 0;
            }
            lastAttackTime += Time.deltaTime;       // 단위시간만큼 누적

            if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.attackRange)
            {
                bee.ChangeState(State.Trace);
            }
        }

        public override void Enter()
        {
            Debug.Log("AttackEnter");
        }

        public override void Exit()
        {
            Debug.Log("AttackExit");
        }
    }
    public class PatrolState : StateBase
    {
        private Bee bee;
        public PatrolState(Bee bee)
        {
            this.bee = bee;
        }

        public override void Enter()
        {
            Debug.Log("PatrolEnter");
        }

        public override void Exit()
        {
            Debug.Log("PatrolExit");
        }

        public override void Update()
        {
            // 순찰 진행
            Vector2 dir = (bee.patrolPoints[bee.patrolIndex].position - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.speed * Time.deltaTime);

            if (Vector2.Distance(bee.transform.position, bee.patrolPoints[bee.patrolIndex].position) < 0.2f)
            {
                bee.ChangeState(State.Idle);
            }
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                bee.ChangeState(State.Trace);
            }
        }
    }
}
    //================================캡슐화전의 코드=====================
    //===========
    /*
    private void Update()
    {
        switch (curState)
        {
            case State.Idle:
                IdelUpdate();
                break;
            case State.Trace:
                TraceUpdate();
                break;
            case State.Return:
                ReturnUpdate();
                break;
            case State.Attack:
                AttackUpdate();
                break;
            case State.Patrol:
                PatrolUpdate();
                break;
        }
    }

    
    private void IdelUpdate()
    {
    float idelTime = 0;
        // 아무것도 안하기
        // 만약 플레이어에 근접하면 추적상태로 변경
        if (idelTime > 2)
        {
            idelTime = 0;
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            curState = State.Patrol;
        }
        idelTime += Time.deltaTime;
        if (Vector2.Distance(player.position, transform.position)<detectRange)
        {
            curState=State.Trace;
        }

    }
    private void TraceUpdate()
    {
        // 플레이어 쫒아가기
        Vector2 dir = (player.position - transform.position).normalized;
        transform.Translate(dir * speed *Time.deltaTime);

        // 플레이어가 범위 밖일경우
        if (Vector2.Distance(player.position, transform.position) > detectRange)
        {
            curState = State.Return;
        }
        //공격범위 안에 있을때
        else if (Vector2.Distance(player.position, transform.position) < attackRange)
        {
            curState = State.Attack;
        }
    }
    private void ReturnUpdate()
    {
        // 원래자리로 돌아가기
        Vector2 dir = (returnPosition - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime);

        // 원래자리로 도착했을때
        if (Vector2.Distance( transform.position, returnPosition) < 0.2f)
        {
            curState = State.Idle;
        }
        else if (Vector2.Distance(player.position, transform.position) < detectRange)
        {
            curState = State.Trace;
        }
    }
    private void AttackUpdate()
    {
        // 공격하기(3초마다)
        float lastAttackTime = 0;
        if(lastAttackTime > 1.5f)
        {
            Debug.Log("쏘기!");
            lastAttackTime = 0;
        }
        lastAttackTime += Time.deltaTime;       // 단위시간만큼 누적

        if (Vector2.Distance(player.position, transform.position) > attackRange)
        {
            curState = State.Trace;
        }

    }

    private void PatrolUpdate()
    {
        // 순찰 진행
        Vector2 dir = (patrolPoints[patrolIndex].position - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, patrolPoints[patrolIndex].position) < 0.2f)
        {
            curState = State.Idle;
        }
        else if (Vector2.Distance(player.position, transform.position) < detectRange)
        {
            curState = State.Trace;
        }

    }
    */

