using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill.Core;

public class SetEnerge : BDele
{
    [SerializeField] int data;
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        skillManager.GetComponent<PlayerCtrl>().Energe += data;
    }
}
