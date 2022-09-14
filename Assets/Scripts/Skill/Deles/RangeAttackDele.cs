using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ArkSkill.Core;

[Serializable]
public class RangeAttackDele:BDele
{
    public bool lockDirect;//是否开启前摇方向锁定
    public float damage;//攻击伤害
    public int maxTarget;//最大目标
    [SerializeField]public CircleRange attackRange;//范围
    public override void OnDraw(Vector3 position, float direct)
    {
        attackRange.OnDraw(position, direct);
    }
    public override IDeleInstance GetInstance()
    {
        return new RangeAttackInstance(this);
    }
}
public class RangeAttackInstance : IDeleInstance
{
    protected RangeAttackDele m_deleInfo;
    private int direct;//朝向
    public RangeAttackInstance(RangeAttackDele deleInfo) { m_deleInfo = deleInfo; }
    public IEnumerator CreatInvokeIEnumerator(SkillManager skillManager, float effectTime, SkillInfo skillInfo)
    {
        yield return new WaitForSecondsRealtime(effectTime);
        Invoke(skillManager, skillInfo);
    }
    public virtual void OnStart(SkillManager skillManager, SkillInfo skillInfo)
    {
        direct = (int)skillManager.transform.localScale.x;//获取技能刚开始时的朝向
    }
    public virtual void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        //调用主体的攻击函数（参数1为伤害值计算方法、参数2目标获取方法）
        skillManager.GetComponent<PlayerCtrl>().Attack(
            () => { return m_deleInfo.damage; }, 
            (transform) =>
        {
            Vector3 position = transform.position;
            if (!m_deleInfo.lockDirect) direct = (int)transform.localScale.x;//若没有前摇朝向锁定，则获取现在朝向
            Collider2D[] collider2Ds = m_deleInfo.attackRange.GetAttackableTarget(position, direct);//collider的获取使用的是主体transform.position
            List<BeAttackedable> beAttackedables = new List<BeAttackedable>();
            BeAttackedable attackedable;
            foreach (var elem in collider2Ds)
            {
                if (elem.TryGetComponent(out attackedable)&&attackedable.ContainAttackType(BeAttackedable.AttackType.Physics)) beAttackedables.Add(attackedable);
                if (beAttackedables.Count >= m_deleInfo.maxTarget) break;//maxTarget(过滤)
            }
            return beAttackedables.ToArray();
        });
    }
    //void Attack1RePlay(PlayerShadow shadow)
    //{
    //    Transform transform = shadow.transform;
    //    Collider2D[] collider2Ds = m_deleInfo.attackRange.GetAttackableTarget(transform.position, transform.position.x);
    //    for (int i = 0; i < Mathf.Min(m_deleInfo.maxTarget, collider2Ds.Length); i++)//获取最大目标数与范围内碰撞体数的最小值
    //    {
    //        EnemyCtrl attackedable;
    //        if (collider2Ds[i].TryGetComponent(out attackedable))//若范围内碰撞体有受击组件
    //        {
    //            attackedable.OnGetHurt((int)m_deleInfo.damage);//调用该受击组件的受击函数
    //        }
    //    }
    //}
}
//public abstract class BRange
//{
//    public abstract void ShowRange();
//}
//public class BoxRange : BRange
//{
//    public override void ShowRange() { }
//    public Bounds bounds;
//}
[Serializable]
public class CircleRange //: BRange
{
    public Vector3 center;
    public float radius;
    public void OnDraw(Vector3 position, float direct)
    {
        Gizmos.color = Color.yellow;
        direct /= Mathf.Abs(direct);
        Gizmos.DrawWireSphere(position + new Vector3(center.x * direct, center.y), radius);
    }
    public Collider2D[] GetAttackableTarget(Vector3 position, float direct)//获取范围内碰撞体
    {
        direct /= Mathf.Abs(direct);
        return Physics2D.OverlapCircleAll(position + new Vector3(center.x * direct, center.y), radius);
    }
}