using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifier
{
    public float duration { get; }
}
[System.Serializable]
public abstract class BModifier : IModifier
{
    public virtual float duration { get; }
    //名字是条件，实现是处理
    public virtual void OnAdd(PlayerCtrl playerCtrl) { }
    public virtual void OnRemove(PlayerCtrl playerCtrl) { }
    public virtual void OnGetHurt(PlayerCtrl playerCtrl) { }
    public virtual void OnTouchGround(PlayerCtrl playerCtrl) { }
    public virtual void OnSkillExit(PlayerCtrl playerCtrl) { }
    public virtual void OnFixedUpdate(PlayerCtrl playerCtrl) { }
    public virtual void OnPlayerAttack(PlayerCtrl playerCtrl) { }
}