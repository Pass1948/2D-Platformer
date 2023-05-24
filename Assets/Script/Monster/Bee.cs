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

    // �� ������ ���߿� ��Ŭ������ ���� ���°� ����
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
        states[(int)curState].Update();                 // ������� ������Ʈ
    }

    public void ChangeState(State state)
    {
        states[(int)curState].Exit();
        curState = state;
        states[(int)curState].Enter();
    }
}


// =================================����Ŭ������=================================
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
    }                   // Size�� ���� �迭�� ũ�⸦ �ð������� ��Ÿ���ֱ����� �����

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
            // �ƹ��͵� ���ϱ�
            // ���� �÷��̾ �����ϸ� �������·� ����
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
            // �÷��̾� �i�ư���
            Vector2 dir = (bee.player.position - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.speed * Time.deltaTime);

            // �÷��̾ ���� ���ϰ��
            if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.detectRange)
            {
                bee.ChangeState(State.Return);
            }
            //���ݹ��� �ȿ� ������
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
            // �����ڸ��� ���ư���
            Vector2 dir = (bee.returnPosition - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.speed * Time.deltaTime);

            // �����ڸ��� ����������
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
                Debug.Log("���!");
                lastAttackTime = 0;
            }
            lastAttackTime += Time.deltaTime;       // �����ð���ŭ ����

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
            // ���� ����
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
    //================================ĸ��ȭ���� �ڵ�=====================
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
        // �ƹ��͵� ���ϱ�
        // ���� �÷��̾ �����ϸ� �������·� ����
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
        // �÷��̾� �i�ư���
        Vector2 dir = (player.position - transform.position).normalized;
        transform.Translate(dir * speed *Time.deltaTime);

        // �÷��̾ ���� ���ϰ��
        if (Vector2.Distance(player.position, transform.position) > detectRange)
        {
            curState = State.Return;
        }
        //���ݹ��� �ȿ� ������
        else if (Vector2.Distance(player.position, transform.position) < attackRange)
        {
            curState = State.Attack;
        }
    }
    private void ReturnUpdate()
    {
        // �����ڸ��� ���ư���
        Vector2 dir = (returnPosition - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime);

        // �����ڸ��� ����������
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
        // �����ϱ�(3�ʸ���)
        float lastAttackTime = 0;
        if(lastAttackTime > 1.5f)
        {
            Debug.Log("���!");
            lastAttackTime = 0;
        }
        lastAttackTime += Time.deltaTime;       // �����ð���ŭ ����

        if (Vector2.Distance(player.position, transform.position) > attackRange)
        {
            curState = State.Trace;
        }

    }

    private void PatrolUpdate()
    {
        // ���� ����
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

