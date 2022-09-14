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
        playerCtrl.EnableMoveCtrl = false;//�����ƶ�ģ��
        Rigidbody2D rigidbody2D = skillManager.GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = new Vector2(skillManager.transform.localScale.x*speed, 0);//�����ٶ�
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;//��ֹY���ƶ�
        //behaviorCtrl.modifierAcceptor.AddModifier(m_modifier2);//����޵�״̬
    }
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        PlayerCtrl playerCtrl = skillManager.GetComponent<PlayerCtrl>();
        Rigidbody2D rigidbody2D = skillManager.GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;//��ԭY���ƶ�
        //behaviorCtrl.modifierAcceptor.RemoveModifier(m_modifier2);//�Ƴ��޵�״̬
        playerCtrl.EnableMoveCtrl = true;//���ƶ����ƽ����ƶ�ģ��
    }
}
