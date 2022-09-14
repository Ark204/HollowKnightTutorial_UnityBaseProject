using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill.Core;

public class XuLiZhangDele : RangeAttackDele
{
    public override IDeleInstance GetInstance()
    {
        return new XuLiZhangInstance(this);
    }
}
public class XuLiZhangInstance : RangeAttackInstance
{
    private float startTime;
    public XuLiZhangInstance(RangeAttackDele rangeDele):base(rangeDele) { }

    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        float xuliTime = Time.realtimeSinceStartup - startTime;
        m_deleInfo.damage = xuliTime;
        Debug.Log(m_deleInfo.damage);
        base.Invoke(skillManager, skillInfo);
    }

    public override void OnStart(SkillManager skillManager, SkillInfo skillInfo)
    {
        startTime = Time.realtimeSinceStartup;
    }
}
