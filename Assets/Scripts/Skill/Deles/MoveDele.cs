using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill.Core;

public class MoveDele : BDele
{
    [SerializeField] float speed = 60;
    //[SerializeField] float cdTime = 0.5f;
    //private UnCtrlableModifier m_modifier1 = new UnCtrlableModifier();
    //private InvincibleModifier m_modifier2 = new InvincibleModifier();
    public override void OnStart(SkillManager skillManager, SkillInfo skillInfo)
    {
        PlayerCtrl playerCtrl = skillManager.GetComponent<PlayerCtrl>();
        playerCtrl.EnableMoveCtrl = false;//禁用移动模块
        Rigidbody2D rigidbody2D = skillManager.GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = new Vector2(skillManager.transform.localScale.x*speed, 0);//设置速度
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;//禁止Y轴移动
        //behaviorCtrl.modifierAcceptor.AddModifier(m_modifier2);//添加无敌状态
    }
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        PlayerCtrl playerCtrl = skillManager.GetComponent<PlayerCtrl>();
        Rigidbody2D rigidbody2D = skillManager.GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;//还原Y轴移动
        //behaviorCtrl.modifierAcceptor.RemoveModifier(m_modifier2);//移除无敌状态
        playerCtrl.EnableMoveCtrl = true;//将移动控制交还移动模块
    }
}
