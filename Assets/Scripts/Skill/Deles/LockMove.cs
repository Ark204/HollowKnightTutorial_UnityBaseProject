using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill.Core;

public class LockMove :BDele
{
    [SerializeField] bool lockMove;
    [SerializeField] bool lockJump;
    public override void OnStart(SkillManager skillManager, SkillInfo skillInfo)
    {
        var player = skillManager.GetComponent<PlayerCtrl>();
        if (lockMove) player.EnableMoveCtrl = false;
        else if (lockJump) player.EnableJump = false;
    }
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        var player = skillManager.GetComponent<PlayerCtrl>();
        if (lockJump) player.EnableJump = true;
        else if (lockMove) player.EnableMoveCtrl = true;
    }
}
