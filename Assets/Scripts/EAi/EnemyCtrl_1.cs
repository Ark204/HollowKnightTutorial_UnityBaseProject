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


    Vector3 baseCenter;//��ʼѲ�����ĵ�=�����ʼλ��
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
            if (distance > defenseRange) m_animator.SetBool("defense", false);//TODO:�ڷ��ط�Χ�� ���빥��ģʽ(idle)
            else//�ڷ��ط�Χ�� �������ģʽ
            {
                m_animator.SetBool("defense", true);
                if (distance < attackRange)//С�ڱ�����ײ��Χ
                {//������ײ����
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
        if (m_animator.GetBool("defense")) return;//������ڷ���״̬���򷵻�
        Hp -= (int)arg3;//��Ѫ
        if (Hp <= 0) Destroy(this.gameObject);//����
       // GetComponent<Rigidbody2D>().velocity = arg2;//����
        //m_transform.Translate(arg2);��������
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
