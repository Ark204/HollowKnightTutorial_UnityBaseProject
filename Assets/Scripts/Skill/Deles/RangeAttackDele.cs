using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ArkSkill.Core;

[Serializable]
public class RangeAttackDele:BDele
{
    public bool lockDirect;//�Ƿ���ǰҡ��������
    public float damage;//�����˺�
    public int maxTarget;//���Ŀ��
    [SerializeField]public CircleRange attackRange;//��Χ
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
    private int direct;//����
    public RangeAttackInstance(RangeAttackDele deleInfo) { m_deleInfo = deleInfo; }
    public IEnumerator CreatInvokeIEnumerator(SkillManager skillManager, float effectTime, SkillInfo skillInfo)
    {
        yield return new WaitForSecondsRealtime(effectTime);
        Invoke(skillManager, skillInfo);
    }
    public virtual void OnStart(SkillManager skillManager, SkillInfo skillInfo)
    {
        direct = (int)skillManager.transform.localScale.x;//��ȡ���ܸտ�ʼʱ�ĳ���
    }
    public virtual void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        //��������Ĺ�������������1Ϊ�˺�ֵ���㷽��������2Ŀ���ȡ������
        skillManager.GetComponent<PlayerCtrl>().Attack(
            () => { return m_deleInfo.damage; }, 
            (transform) =>
        {
            Vector3 position = transform.position;
            if (!m_deleInfo.lockDirect) direct = (int)transform.localScale.x;//��û��ǰҡ�������������ȡ���ڳ���
            Collider2D[] collider2Ds = m_deleInfo.attackRange.GetAttackableTarget(position, direct);//collider�Ļ�ȡʹ�õ�������transform.position
            List<BeAttackedable> beAttackedables = new List<BeAttackedable>();
            BeAttackedable attackedable;
            foreach (var elem in collider2Ds)
            {
                if (elem.TryGetComponent(out attackedable)&&attackedable.ContainAttackType(BeAttackedable.AttackType.Physics)) beAttackedables.Add(attackedable);
                if (beAttackedables.Count >= m_deleInfo.maxTarget) break;//maxTarget(����)
            }
            return beAttackedables.ToArray();
        });
    }
    //void Attack1RePlay(PlayerShadow shadow)
    //{
    //    Transform transform = shadow.transform;
    //    Collider2D[] collider2Ds = m_deleInfo.attackRange.GetAttackableTarget(transform.position, transform.position.x);
    //    for (int i = 0; i < Mathf.Min(m_deleInfo.maxTarget, collider2Ds.Length); i++)//��ȡ���Ŀ�����뷶Χ����ײ��������Сֵ
    //    {
    //        EnemyCtrl attackedable;
    //        if (collider2Ds[i].TryGetComponent(out attackedable))//����Χ����ײ�����ܻ����
    //        {
    //            attackedable.OnGetHurt((int)m_deleInfo.damage);//���ø��ܻ�������ܻ�����
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
    public Collider2D[] GetAttackableTarget(Vector3 position, float direct)//��ȡ��Χ����ײ��
    {
        direct /= Mathf.Abs(direct);
        return Physics2D.OverlapCircleAll(position + new Vector3(center.x * direct, center.y), radius);
    }
}