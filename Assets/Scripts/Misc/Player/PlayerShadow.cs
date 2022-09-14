using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerShadow : MonoBehaviour
{
    public delegate void AttackAction(PlayerShadow shadow);
    public Stack<TimeInfo2> timeStack2 { get; private set; }
    public float timer { get;private set; }
    private Animator animator;
    public void Init(Stack<TimeInfo2> timePoints)
    {
        animator = GetComponentInChildren<Animator>();
        timeStack2 = timePoints;
        timer = 0f;
        StartCoroutine(TQueueExtion.DelayFunc(Destroy,gameObject,3f));
    }
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if(timeStack2!=null && timeStack2.Count>0&& Time.fixedTime - timeStack2.Peek().triggerTime <= 2 * timer)
        {
            foreach(var item in timeStack2.Pop().infoList)
            {
                ReadInfo(item);
            }
        }
    }

    #region PlayParticle
    [SerializeField] ParticleSystem particle;//特效预制体
    void PlayAttackParticle(Vector3 offset,float partScale)
    {
        if (particle == null) return;
        //TODO:特效单例
        var effect = Instantiate(particle, transform);//将特效设置为子物体
        Transform effectTransform = effect.transform;
        effectTransform.localPosition = offset;//偏移
        effectTransform.localScale *= partScale;//特效尺寸倍数
        effect?.Play();

        //持续时间结束后销毁
        var duration = effect.main.duration + effect.main.startLifetime.constantMax;
        effect.gameObject.AddComponent<Core.Util.Disposable>().lifetime = duration;
    }
    #endregion

    #region Attack
    void Attack(Return<float> caculateDamage, GetTargets getTargets)
    {
        float damage = caculateDamage == null ? 0 : caculateDamage();//计算伤害
        foreach (var target in getTargets?.Invoke(transform))
        {
            target?.OnAttackHit(transform.position, new Vector2(transform.localScale.x, 0) * 3.5f/*pushPower*/, damage);
        }
    }
    #endregion

    void ReadInfo(KeyValuePair<TimeInfo2.InfoType,object> timeInfo)
    {
        switch (timeInfo.Key)
        {
            case TimeInfo2.InfoType.Position:
                {
                    Vector3 vector = (Vector3)timeInfo.Value;
                    transform.position = vector; break;
                }
            case TimeInfo2.InfoType.Localscale:
                {
                    transform.localScale = (Vector3)timeInfo.Value; break;
                }
            case TimeInfo2.InfoType.AnimatorState:
                {
                    animator.Play((int)(timeInfo.Value)); break;
                }
            case TimeInfo2.InfoType.AttackPoint:
                {
                    object[] vs = timeInfo.Value as object[];
                    Attack(vs[0] as Return<float>,vs[1] as GetTargets);
                    break;
                }
            case TimeInfo2.InfoType.Particle:
                {
                    object[] vs = timeInfo.Value as object[];
                    PlayAttackParticle((Vector3)vs[1],(float) vs[0]);
                    break;
                }
            case TimeInfo2.InfoType.Prefab:
                {
                    object[] obj = timeInfo.Value as object[];
                   var  prefab = (GameObject)Instantiate(obj[0] as GameObject, transform);//加载预制体//设为子物体
                    StartCoroutine(TQueueExtion.DelayFunc(Destroy, prefab, (float)obj[1]));
                    break;
                }
            default: break;
        }
    }
}
