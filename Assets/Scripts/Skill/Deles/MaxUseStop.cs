using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill.Core;

public class MaxUseStop : BDele
{
    [SerializeField] int skillId;
    [SerializeField] float stopTime;
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        skillManager.StartCoroutine(TQueueExtion.DelayFunc(skillManager.StopSkill, skillId, stopTime));
    }
}
