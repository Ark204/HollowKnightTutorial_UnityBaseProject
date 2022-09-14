using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill.Core;

//…Ë÷√Œﬁµ–
public class SetInvincible : BDele
{
    public override void OnStart(SkillManager skillManager, SkillInfo skillInfo)
    {
        Physics2D.IgnoreLayerCollision(6, 8);
    }
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        Physics2D.IgnoreLayerCollision(6, 8,false);
    }
}
