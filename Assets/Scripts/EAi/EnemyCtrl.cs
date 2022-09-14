using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    [SerializeField] int m_maxHp=100;
    public int MaxHp { get => m_maxHp; }
    [SerializeField] int m_Hp;
    public int attackPower = 1;
    public float pushPower=4;
    public int Hp { get => m_Hp;
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

    [Header("Patrol")]
    Vector3 baseCenter;//初始巡逻中心点=怪物初始位置
     [SerializeField] float lenth;
    [SerializeField] float high;
    [SerializeField] Vector3 centerOffset;
    [SerializeField] float minDistance;//下一个随机巡逻点的最小距离
    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
        GetComponent<BeAttackedable>().OnHit += EnemyCtrl_OnHit;
        Hp = MaxHp;
        m_transform = transform;
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        baseCenter = m_transform.position;
    }

    private void OnWaitEnd()
    {
        m_animator.SetBool("isMoving", true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) Test();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Vector3 force = player.transform.position - transform.position;
            force = force.normalized * pushPower;
            collision.GetComponent<PlayerCtrl>().Hurt(attackPower, force);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(baseCenter + centerOffset,new Vector3(lenth,high,0));
    }
    public Vector3 GetRandomTarget()
    {
        float x=Random.Range((baseCenter + centerOffset).x - lenth / 2, (baseCenter + centerOffset).x + lenth / 2 - 2 * minDistance);
        if (x > m_transform.position.x - minDistance) x += 2 * minDistance;
        return new Vector3(x, m_transform.position.y, m_transform.position.z);
    }
    
    private void EnemyCtrl_OnHit(Vector2 arg1, Vector2 arg2, float arg3)
    {
        Hp -= (int)arg3;//损血
        if (Hp <= 0) Destroy(this.gameObject);//死亡
                                   // GetComponent<Rigidbody2D>().velocity = arg2;//击退
        m_transform.Translate(arg2);
    }
    //test use
    public PlayerCtrl player;
    public void Test() 
    {
         Vector3 force=player.transform.position - transform.position;
        force=force.normalized*pushPower;
        Debug.Log("force:  "+force);
        player.Hurt(1, force);
    }
}
