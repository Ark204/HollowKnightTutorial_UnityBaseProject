using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill.Core;

public class PlayParticleDele : BDele
{
    [SerializeField] Vector3 offset;
    [SerializeField] float scale = 1;
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        var player = skillManager.GetComponent<PlayerCtrl>();
        player.PlayAttackParticle(offset,scale);
    }
}
