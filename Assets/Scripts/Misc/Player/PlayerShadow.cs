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
    [SerializeField] ParticleSystem particle;//��ЧԤ����
    void PlayAttackParticle(Vector3 offset,float partScale)
    {
        if (particle == null) return;
        //TODO:��Ч����
        var effect = Instantiate(particle, transform);//����Ч����Ϊ������
        Transform effectTransform = effect.transform;
        effectTransform.localPosition = offset;//ƫ��
        effectTransform.localScale *= partScale;//��Ч�ߴ籶��
        effect?.Play();

        //����ʱ�����������
        var duration = effect.main.duration + effect.main.startLifetime.constantMax;
        effect.gameObject.AddComponent<Core.Util.Disposable>().lifetime = duration;
    }
    #endregion

    #region Attack
    void Attack(Return<float> caculateDamage, GetTargets getTargets)
    {
        float damage = caculateDamage == null ? 0 : caculateDamage();//�����˺�
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
                   var  prefab = (GameObject)Instantiate(obj[0] as GameObject, transform);//����Ԥ����//��Ϊ������
                    StartCoroutine(TQueueExtion.DelayFunc(Destroy, prefab, (float)obj[1]));
                    break;
                }
            default: break;
        }
    }
}
