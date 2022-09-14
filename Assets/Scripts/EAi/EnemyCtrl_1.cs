using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl_1 : MonoBehaviour
{
    [SerializeField] int m_maxHp = 100;
    public int MaxHp { get => m_maxHp; }
    [SerializeField] int m_Hp;
    public int attackPower = 1;
    public float pushPower = 4;
    public int Hp
    {
        get => m_Hp;
        set
        {
            if (value > m_maxHp) m_Hp = m_maxHp;
            else if (value < 0) m_Hp = 0;
            else m_Hp = value;
        }
    }
    [HideInInspector] public Transform m_transform;
    [HideInInspector] public Rigidbody2D m_rigidbody2D;
    Animator m_animator;


    Vector3 baseCenter;//初始巡逻中心点=怪物初始位置
    public float defenseRange = 6;
    [SerializeField] float high;
    [SerializeField] Vector3 centerOffset;
    public float attackRange=2;

    private void Awake()
    {
        GetComponent<BeAttackedable>().OnHit += EnemyCtrl_OnHit;
        Hp = MaxHp;
        m_transform = transform;
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) Test();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            float distance=Vector2.Distance(collision.transform.position, m_transform.position);
            if (distance > defenseRange) m_animator.SetBool("defense", false);//TODO:在防守范围外 进入攻击模式(idle)
            else//在防守范围内 进入防守模式
            {
                m_animator.SetBool("defense", true);
                if (distance < attackRange)//小于本体碰撞范围
                {//进行碰撞攻击
                    Vector3 force = player.transform.position - transform.position;
                    force = force.normalized * pushPower;
                    collision.GetComponent<PlayerCtrl>().Hurt(attackPower, force);
                }
            }
           
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_animator.SetTrigger("attack");
            m_animator.SetBool("defense", false);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(baseCenter + centerOffset, new Vector3(defenseRange, high, 0));
        Gizmos.DrawWireCube(baseCenter + centerOffset, new Vector3(attackRange, high, 0));
    }

    private void EnemyCtrl_OnHit(Vector2 arg1, Vector2 arg2, float arg3)
    {
        if (m_animator.GetBool("defense")) return;//如果处于防御状态，则返回
        Hp -= (int)arg3;//损血
        if (Hp <= 0) Destroy(this.gameObject);//死亡
       // GetComponent<Rigidbody2D>().velocity = arg2;//击退
        //m_transform.Translate(arg2);不被击退
    }
    //test use
    public PlayerCtrl player;
    public void Test()
    {
        Vector3 force = player.transform.position - transform.position;
        force = force.normalized * pushPower;
        Debug.Log("force:  " + force);
        player.Hurt(1, force);
    }
}
