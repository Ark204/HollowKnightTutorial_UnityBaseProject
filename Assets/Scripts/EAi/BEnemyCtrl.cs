using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Baracuda.Monitoring;
using System;

public abstract class BEnemyCtrl :MonitoredBehaviour//MonoBehaviour
{
    [SerializeField] RangeTarget getPlayer;
    [SerializeField] BeAttackedable beAttackedable;
    [SerializeField] int m_maxHp = 100;
    public int MaxHp { get => m_maxHp; }
    [SerializeField] int m_Hp;
    [Monitor][MPosition(UIPosition.UpperRight)]public int Hp
    {
        get => m_Hp;
        set
        {
            if (value > m_maxHp) m_Hp = m_maxHp;
            else if (value < 0) m_Hp = 0;
            else m_Hp = value;
        }
    }
    [Monitor] [MPosition(UIPosition.UpperRight)] [SerializeField] protected Transform target;
    protected override void Awake()
    {
        base.Awake();
        getPlayer.onTargetEnter += OnTargetEnter;
        getPlayer.onTargetExit += OnTargetExit;
        beAttackedable.OnHit += EnemyCtrl_OnHit;
    }

    protected virtual void EnemyCtrl_OnHit(Vector2 arg1, Vector2 arg2, float arg3)
    {
        Hp -= (int)arg3;//ËðÑª
        if (Hp <= 0) Destroy(gameObject);//ËÀÍö
        // GetComponent<Rigidbody2D>().velocity = arg2;//»÷ÍË
        //m_transform.Translate(arg2);²»±»»÷ÍË
    }

    protected virtual void OnTargetExit(Collider2D obj)
    {
        if (target == obj.transform) target = null;
    }

    protected virtual void OnTargetEnter(Collider2D obj)
    {
        target = obj.transform;
    }
}
