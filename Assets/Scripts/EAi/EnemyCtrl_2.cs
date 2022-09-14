using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl_2 : BEnemyCtrl//MonoBehaviour
{
    [SerializeField] RangeTarget selfTrigger;
    [SerializeField] int attackPower;
    [SerializeField] float pushPower;
    Transform m_transform;
    Animator m_animator;

    //defense
    [SerializeField] float defenseRange=6f;//�����뾶
    protected override void Awake()
    {
        base.Awake();
        //Components
        m_transform = transform;
        m_animator = GetComponentInChildren<Animator>();
        // events
        selfTrigger.onTargetEnter += HurtTarget;
    }

    protected override void EnemyCtrl_OnHit(Vector2 arg1, Vector2 arg2, float arg3)
    {
        if (m_animator.GetBool("defense")) return;//������ڷ���״̬���򷵻�
        base.EnemyCtrl_OnHit(arg1, arg2, arg3);
    }

    private void Update()
    {
        if (target && TargetDistance() <= defenseRange) m_animator.SetBool("defense", true);//��Ŀ���ڷ����뾶�ڣ���������״̬
        else m_animator.SetBool("defense", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, defenseRange);//���Ʒ�����Χ
    }

    private void HurtTarget(Collider2D obj)
    {
        var player = obj.GetComponent<PlayerCtrl>();
        Vector3 force = player.transform.position - transform.position;
        force = force.normalized * pushPower;
        Debug.Log("force:  " + force);
        player.Hurt(attackPower, force);
    }

    protected override void OnTargetExit(Collider2D obj)
    {
        base.OnTargetExit(obj);
        m_animator.GetBehaviour<EIdle>().nextState = null;//���ٹ���
    }

    protected override void OnTargetEnter(Collider2D obj)
    {
        base.OnTargetEnter(obj);
        m_animator.GetBehaviour<EIdle>().nextState = "attack";
    }

    float TargetDistance() { return Vector2.Distance(target.transform.position, m_transform.position); }
}
