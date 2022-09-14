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
    //������������ʵ���Ǵ���
    public virtual void OnAdd(PlayerCtrl playerCtrl) { }
    public virtual void OnRemove(PlayerCtrl playerCtrl) { }
    public virtual void OnGetHurt(PlayerCtrl playerCtrl) { }
    public virtual void OnTouchGround(PlayerCtrl playerCtrl) { }
    public virtual void OnSkillExit(PlayerCtrl playerCtrl) { }
    public virtual void OnFixedUpdate(PlayerCtrl playerCtrl) { }
    public virtual void OnPlayerAttack(PlayerCtrl playerCtrl) { }
}