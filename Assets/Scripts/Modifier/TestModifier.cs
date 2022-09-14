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
//public class InvincibleModifier : BModifier//无敌状态
//{
//    public override float duration => m_duration;
//    private const float m_duration = 5f;
//    public override void OnAdd(BBehaviorCtrl bBehaviorCtrl)
//    {
//        //取消碰撞
//        Physics2D.IgnoreLayerCollision(10, 13);
//        Debug.Log("Invin Invoke");
//    }
//    public override void OnRemove(BBehaviorCtrl bBehaviorCtrl)
//    {
//        //还原碰撞
//        Physics2D.IgnoreLayerCollision(10, 13, false);
//    }
//}
//public class UnCtrlableModifier : BModifier//无法控制状态
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
//public class FloatModifier : BModifier//漂浮状态
//{
//    public override float duration => m_duration;
//    private const float m_duration = 5f;
//    private CallBack jumpAction;
//    private CallBack runAction;
//    public override void OnAdd(BBehaviorCtrl bBehaviorCtrl)
//    {
//        BehaviorCtrl behaviorCtrl = bBehaviorCtrl as BehaviorCtrl;
//        behaviorCtrl.rb.gravityScale = 0;
//        jumpAction = behaviorCtrl.onJump;//保存跳跃方法
//        behaviorCtrl.onJump = null;//禁用跳跃
//        runAction = behaviorCtrl.onRun;//保存移动方法
//        behaviorCtrl.onRun = BindHelper.Bind(FloatMove, behaviorCtrl);//改变移动方法
//    }
//    public override void OnRemove(BBehaviorCtrl bBehaviorCtrl)
//    {
//        BehaviorCtrl behaviorCtrl = bBehaviorCtrl as BehaviorCtrl;
//        behaviorCtrl.onRun = runAction;//还原移动方法
//        behaviorCtrl.onJump = jumpAction;//还原跳跃方法
//        behaviorCtrl.rb.gravityScale = 6;//还原重力
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
//public class PlayerPassiveModifier : BModifier//主角被动
//{
//    public override float duration => m_duration;
//    private const float m_duration = -1f;//永久被动
//    public override void OnGetHurt(BBehaviorCtrl bBehaviorCtrl) //当主角受伤调用
//    {
//        BehaviorCtrl behaviorCtrl = bBehaviorCtrl as BehaviorCtrl;
//        behaviorCtrl.modifierAcceptor.AddModifier(new InvincibleModifier());//使主角进入5s无敌状态
//        behaviorCtrl.modifierAcceptor.AddModifier(new UnCtrlableModifier());//使主角进入0.5s无法控制状态
//        behaviorCtrl.rb.velocity = new Vector2(behaviorCtrl.transform.localScale.x * (-7), 13);//使主角被击退
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
//    private const float m_duration = -1f;//永久被动
//    private CallBack callBack1;
//    private CallBack callBack2;
//    public override void OnAdd(BBehaviorCtrl bBehaviorCtrl)
//    {
//        BehaviorCtrl playerCtrl = bBehaviorCtrl as BehaviorCtrl;
//        callBack1 = playerCtrl.onRun;//保存移动委托
//        playerCtrl.onRun = null;//禁用移动
//        callBack2 = playerCtrl.onJump;//保存跳跃委托
//        playerCtrl.onJump = null;//禁用跳跃
//        //TODO: 物理上不再受力
//        //playerCtrl.rb.constraints = RigidbodyConstraints2D.FreezeAll;
//        playerCtrl.rb.bodyType = RigidbodyType2D.Static;
//    }
//    public override void OnRemove(BBehaviorCtrl bBehaviorCtrl)
//    {
//        BehaviorCtrl playerCtrl = bBehaviorCtrl as BehaviorCtrl;
//        //TODO: 还原物理受力
//        //playerCtrl.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
//        playerCtrl.rb.bodyType = RigidbodyType2D.Dynamic;
//        playerCtrl.onRun = callBack1;//还原移动
//        playerCtrl.onJump = callBack2;//还原跳跃
//    }
//}
/// <summary>
/// 时间记录Modifier
/// </summary>
public class TimeRecord:BModifier
{
    public override float duration => m_duration;
    private const float m_duration = -1f;//永久被动
    public CircleQueue<TimeInfo2> timeQueue { get;private set; }
    public override void OnFixedUpdate(PlayerCtrl playerCtrl)
    {
        Transform transform = playerCtrl.transform;
        Animator  animator= playerCtrl.GetComponent<Animator>();
        timeQueue.Push(TimeInfo2.InfoType.Position, transform.position);
        timeQueue.Push(TimeInfo2.InfoType.Localscale, transform.localScale);
        timeQueue.Push(TimeInfo2.InfoType.AnimatorState, animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        if (!timeQueue.IsEmpty() && Time.fixedTime - timeQueue.FrontItem().triggerTime > 3)//队列非空且队头时间距当前时间间隔大于3s
        {
            timeQueue.DeQueue();
        }
    }
    public override void OnPlayerAttack(PlayerCtrl playerCtrl)
    {
       // behaviorCtrl.timeQueue.Push(TimeInfo2.InfoType.AttackPoint, new PlayerShadow.AttackAction(Attack1RePlay));
    }
}