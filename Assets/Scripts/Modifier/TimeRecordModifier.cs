using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRecordModifier : MonoBehaviour
{
    public CircleQueue<TimeInfo2> timeQueue { get; private set; }
    PlayerCtrl playerCtrl;
    private void Awake()
    {
        timeQueue = TQueueExtion.CircleQueue(3f);
        playerCtrl=GetComponent<PlayerCtrl>();
        playerCtrl.onPlarticleEnd += OnPlayerParticleEnd;
        playerCtrl.onAttack += OnPlayerAttack;
        playerCtrl.onChildDestory += OnPlayerDestoryChild;
    }
    private void FixedUpdate()
    {
        Animator animator = GetComponentInChildren<Animator>();
        timeQueue.Push(TimeInfo2.InfoType.Position, transform.position);
        timeQueue.Push(TimeInfo2.InfoType.Localscale, transform.localScale);
        timeQueue.Push(TimeInfo2.InfoType.AnimatorState, animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        if (!timeQueue.IsEmpty() && Time.fixedTime - timeQueue.FrontItem().triggerTime > 3)//队列非空且队头时间距当前时间间隔大于3s
        {
            timeQueue.DeQueue();
        }
    }
    void OnPlayerParticleEnd(Vector3 offset,float partScale)
    {
        object[] vs = new object[2];
        vs[0] = partScale;
        vs[1] = offset;
        timeQueue.Push(TimeInfo2.InfoType.Particle, vs);
    }
    void OnPlayerAttack(Return<float> caculateDamage, GetTargets getTargets)
    {
        object[] vs = new object[2];
        vs[0] = caculateDamage;
        vs[1] = getTargets;
        timeQueue.Push(TimeInfo2.InfoType.AttackPoint, vs);
    }
    void OnPlayerDestoryChild(Object obj,float durTime)
    {
        object[] vs = new object[2];
        vs[0] = obj;
        vs[1] = durTime;
        timeQueue.Push(TimeInfo2.InfoType.Prefab, vs);
    }
}
