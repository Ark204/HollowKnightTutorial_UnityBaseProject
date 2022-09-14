using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill.Core;

public class Cure : BDele
{
    [SerializeField] int data=1;
    //在TimePoint中调用
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        var player = skillManager.GetComponent<PlayerCtrl>();
        player.Hp += data;
    }
}

