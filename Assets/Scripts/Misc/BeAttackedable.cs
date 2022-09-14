using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BeAttackedable : MonoBehaviour
{
    [Flags]
    public enum AttackType//攻击类型
    {
        Physics=1,
        Magic=2<<1
    }

    [SerializeField] AttackType m_acceptType=AttackType.Physics;//受击器将接受的攻击类型(可多选)
    public void AddAttackType(AttackType type) { m_acceptType |= type; }
    public void RemoveAttackType(AttackType type) { m_acceptType &= (~type); }
    public bool ContainAttackType(AttackType type) { return (m_acceptType & type) != 0; }
    public event Action<Vector2, Vector2,float> OnHit;
    public virtual void OnAttackHit(Vector2 position, Vector2 force, float damage)
    {
        OnHit?.Invoke(position,force,damage);
    }
}

