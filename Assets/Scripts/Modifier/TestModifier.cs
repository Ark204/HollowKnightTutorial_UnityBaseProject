using System.Collections;
using System.Collections.Generic;
using UnityEngine;
////public delegate void SkillAction(ISkill skill);
//public class TestModifier :IModifier
//{
//    private const float m_duration = -1f;
//    public float duration { get=>m_duration; }
//    public void Start(BBehaviorCtrl bBehaviorCtrl)
//    {
//        throw new System.NotImplementedException();
//    }
//    public void Update(BBehaviorCtrl bBehaviorCtrl)
//    {
//        throw new System.NotImplementedException();
//    }
//    public void End(BBehaviorCtrl bBehaviorCtrl)
//    {
//        throw new System.NotImplementedException();
//    }
//   // public void HandleEvent(ISkill skill)
//    //{
//        //m_ListFailSkillEvent.Invoke(skill);
//    //}
//}
//public class InvincibleModifier : BModifier//�޵�״̬
//{
//    public override float duration => m_duration;
//    private const float m_duration = 5f;
//    public override void OnAdd(BBehaviorCtrl bBehaviorCtrl)
//    {
//        //ȡ����ײ
//        Physics2D.IgnoreLayerCollision(10, 13);
//        Debug.Log("Invin Invoke");
//    }
//    public override void OnRemove(BBehaviorCtrl bBehaviorCtrl)
//    {
//        //��ԭ��ײ
//        Physics2D.IgnoreLayerCollision(10, 13, false);
//    }
//}
//public class UnCtrlableModifier : BModifier//�޷�����״̬
//{
//    public override float duration => m_duration;
//    private const float m_duration = 1f;
//    public override void OnAdd(BBehaviorCtrl bBehaviorCtrl)
//    {
//        BehaviorCtrl behaviorCtrl = bBehaviorCtrl as BehaviorCtrl;
//        behaviorCtrl.commandShield = true;
//    }
//    public override void OnRemove(BBehaviorCtrl bBehaviorCtrl)
//    {
//        BehaviorCtrl behaviorCtrl = bBehaviorCtrl as BehaviorCtrl;
//        behaviorCtrl.commandShield = false;
//    }
//}
//public class FloatModifier : BModifier//Ư��״̬
//{
//    public override float duration => m_duration;
//    private const float m_duration = 5f;
//    private CallBack jumpAction;
//    private CallBack runAction;
//    public override void OnAdd(BBehaviorCtrl bBehaviorCtrl)
//    {
//        BehaviorCtrl behaviorCtrl = bBehaviorCtrl as BehaviorCtrl;
//        behaviorCtrl.rb.gravityScale = 0;
//        jumpAction = behaviorCtrl.onJump;//������Ծ����
//        behaviorCtrl.onJump = null;//������Ծ
//        runAction = behaviorCtrl.onRun;//�����ƶ�����
//        behaviorCtrl.onRun = BindHelper.Bind(FloatMove, behaviorCtrl);//�ı��ƶ�����
//    }
//    public override void OnRemove(BBehaviorCtrl bBehaviorCtrl)
//    {
//        BehaviorCtrl behaviorCtrl = bBehaviorCtrl as BehaviorCtrl;
//        behaviorCtrl.onRun = runAction;//��ԭ�ƶ�����
//        behaviorCtrl.onJump = jumpAction;//��ԭ��Ծ����
//        behaviorCtrl.rb.gravityScale = 6;//��ԭ����
//    }
//    void FloatMove(BehaviorCtrl behaviorCtrl)
//    {
//        float verticalmove = Input.GetAxisRaw("Vertical");
//        float hormove = Input.GetAxisRaw("Horizontal");
//        behaviorCtrl.rb.velocity = new Vector2(hormove * behaviorCtrl.speed, verticalmove * behaviorCtrl.speed);
//        if (hormove != 0)
//        {
//            behaviorCtrl.transform.localScale = new Vector3(hormove, 1, 1);
//        }
//    }
//}
//public class PlayerPassiveModifier : BModifier//���Ǳ���
//{
//    public override float duration => m_duration;
//    private const float m_duration = -1f;//���ñ���
//    public override void OnGetHurt(BBehaviorCtrl bBehaviorCtrl) //���������˵���
//    {
//        BehaviorCtrl behaviorCtrl = bBehaviorCtrl as BehaviorCtrl;
//        behaviorCtrl.modifierAcceptor.AddModifier(new InvincibleModifier());//ʹ���ǽ���5s�޵�״̬
//        behaviorCtrl.modifierAcceptor.AddModifier(new UnCtrlableModifier());//ʹ���ǽ���0.5s�޷�����״̬
//        behaviorCtrl.rb.velocity = new Vector2(behaviorCtrl.transform.localScale.x * (-7), 13);//ʹ���Ǳ�����
//    }
//    public override void OnTouchGround(BBehaviorCtrl bBehaviorCtrl)
//    {
//        BehaviorCtrl behaviorCtrl = bBehaviorCtrl as BehaviorCtrl;
//        behaviorCtrl.rb.velocity = new Vector2(0, 0);
//        Debug.Log("TouchGround CallBack");
//    }
//}

//public class StabMDF : BModifier
//{
//    public override float duration => m_duration;
//    private const float m_duration = -1f;//���ñ���
//    private CallBack callBack1;
//    private CallBack callBack2;
//    public override void OnAdd(BBehaviorCtrl bBehaviorCtrl)
//    {
//        BehaviorCtrl playerCtrl = bBehaviorCtrl as BehaviorCtrl;
//        callBack1 = playerCtrl.onRun;//�����ƶ�ί��
//        playerCtrl.onRun = null;//�����ƶ�
//        callBack2 = playerCtrl.onJump;//������Ծί��
//        playerCtrl.onJump = null;//������Ծ
//        //TODO: �����ϲ�������
//        //playerCtrl.rb.constraints = RigidbodyConstraints2D.FreezeAll;
//        playerCtrl.rb.bodyType = RigidbodyType2D.Static;
//    }
//    public override void OnRemove(BBehaviorCtrl bBehaviorCtrl)
//    {
//        BehaviorCtrl playerCtrl = bBehaviorCtrl as BehaviorCtrl;
//        //TODO: ��ԭ��������
//        //playerCtrl.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
//        playerCtrl.rb.bodyType = RigidbodyType2D.Dynamic;
//        playerCtrl.onRun = callBack1;//��ԭ�ƶ�
//        playerCtrl.onJump = callBack2;//��ԭ��Ծ
//    }
//}
/// <summary>
/// ʱ���¼Modifier
/// </summary>
public class TimeRecord:BModifier
{
    public override float duration => m_duration;
    private const float m_duration = -1f;//���ñ���
    public CircleQueue<TimeInfo2> timeQueue { get;private set; }
    public override void OnFixedUpdate(PlayerCtrl playerCtrl)
    {
        Transform transform = playerCtrl.transform;
        Animator  animator= playerCtrl.GetComponent<Animator>();
        timeQueue.Push(TimeInfo2.InfoType.Position, transform.position);
        timeQueue.Push(TimeInfo2.InfoType.Localscale, transform.localScale);
        timeQueue.Push(TimeInfo2.InfoType.AnimatorState, animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        if (!timeQueue.IsEmpty() && Time.fixedTime - timeQueue.FrontItem().triggerTime > 3)//���зǿ��Ҷ�ͷʱ��൱ǰʱ��������3s
        {
            timeQueue.DeQueue();
        }
    }
    public override void OnPlayerAttack(PlayerCtrl playerCtrl)
    {
       // behaviorCtrl.timeQueue.Push(TimeInfo2.InfoType.AttackPoint, new PlayerShadow.AttackAction(Attack1RePlay));
    }
}