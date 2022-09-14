using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierAcceptor : MonoBehaviour
{
    public List<BModifier> modifiers=new List<BModifier>();
    [SerializeField] int count = 0;
    public PlayerCtrl playerCtrl;
    private void Start()
    {
        playerCtrl = GetComponent<PlayerCtrl>();
       // EventCenter.GetInstance().AddListener<BBehaviorCtrl>(EventType.PlayerGetHurt, OnGetHurt);
        //EventCenter.GetInstance().AddListener<BBehaviorCtrl>(EventType.PlayerTouchGround, OnTouchGround);
    }
    private void OnDestroy()
    {
       // EventCenter.GetInstance().RemoveListener<BBehaviorCtrl>(EventType.PlayerGetHurt, OnGetHurt);
        //EventCenter.GetInstance().RemoveListener<BBehaviorCtrl>(EventType.PlayerTouchGround, OnTouchGround);
    }
    private void FixedUpdate()
    {
        count = modifiers.Count;
        foreach(var modifier in modifiers)
        {
            modifier.OnFixedUpdate(playerCtrl);
        }
    }
    public void AddModifier(BModifier modifier)
    {
        modifiers.Add(modifier);
        modifier.OnAdd(playerCtrl);
        if (modifier.duration != -1) StartCoroutine(RemoveModifier(modifier.duration, modifier));
    }
    public void RemoveModifier(BModifier modifier)
    {
        if (modifiers.Contains(modifier))
        {
            modifier.OnRemove(playerCtrl);
            modifiers.Remove(modifier);
        }
    }
    private IEnumerator RemoveModifier(float time, BModifier modifier)
    {
        yield return new WaitForSecondsRealtime(time);
        RemoveModifier(modifier);
    }
    void OnGetHurt(PlayerCtrl playerCtrl)
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            modifiers[i].OnGetHurt(playerCtrl);
        }
    }
    void OnTouchGround(PlayerCtrl playerCtrl)
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            modifiers[i].OnTouchGround(playerCtrl);
        }
    }
}
