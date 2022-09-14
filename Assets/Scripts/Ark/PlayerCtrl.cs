using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Core.Character;
using Core;
using ArkSkill.Core;
using Core.UI;
using Baracuda.Monitoring;

public delegate BeAttackedable[] GetTargets(Transform transform);

public class PlayerCtrl :MonitoredBehaviour/*MonoBehaviour*/
{
    PlayerController moveCtrl;
    SkillManager skillManager;
    Animator animator;
    Rigidbody2D rigidbody2D;
    [Header("Attack Particle")]
    public ParticleSystem particle;

    [SerializeField] PlayerData playerData;

    string PlayerHpUI(int currentHp)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("Hp: ");
        sb.Append(currentHp.ToString("00"));
        sb.Append('/');
        sb.Append(MaxHp.ToString("00"));
        sb.Append(' ');

        var color =new Color(.25f, .25f, .3f);
        sb.Append('▐', currentHp);
        sb.Append("<color=#");
        sb.Append(ColorUtility.ToHtmlStringRGB(color));
        sb.Append('>');
        sb.Append('▐', MaxHp - currentHp);
        sb.Append("</color>");

        return sb.ToString();
    }

    [MonitorProperty]
    [MFontSize(16)]
    [MGroupElement(false)]
    [MPosition(UIPosition.UpperLeft)]
    [MFontName("JetBrainsMono-Regular")]
    [MTextColor(ColorPreset.Red)]
    [MValueProcessor(nameof(PlayerHpUI))]
    public int Hp
    {
        get => playerData.HP;
        set {
            if (value < 0) playerData.HP = 0;
            else if (value > MaxHp) playerData.HP = MaxHp;
            else playerData.HP = value;
        }
    }
    [Monitor]public int Energe
    {
        get => playerData.Energe;
        set {
            if (value < 0) playerData.Energe = 0;
            else if (value > MaxEnerge) playerData.Energe = MaxEnerge;
            else playerData.Energe = value;
        }
    }
    public int MaxHp { get => playerData.maxHP; private set => playerData.maxHP = value; }
    public int MaxEnerge { get => playerData.MaxEnerge; private set => playerData.MaxEnerge = value; }
    public float AttackPower { get => playerData.attackPower; private set => playerData.attackPower = value; }
    public float pushPower = 1f;
    /// <summary>
    /// 设置是否禁用移动模块
    /// </summary>
    public bool EnableMoveCtrl { set { moveCtrl.Controllable = value; } }
    public bool EnableJump { set { moveCtrl.Jumpable = value; } }
    protected override void Awake()
    {
        base.Awake();
        skillManager = GetComponent<SkillManager>();
        moveCtrl = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        //Camera
        cam = Camera.main;
        camController = cam.GetComponent<CameraController>();
    }
    private void Update()
    {
        if (!CanBeHit) return;//受伤无敌状态 直接返回
        if (skillManager.currSkill == null)
        {
            if (Input.GetButton("Attack")) skillManager.UseSkill(0);
            if (Input.GetButtonDown("Dash")) Dash();
            if (Input.GetButtonDown("Replay")) RePlay();
            if (Input.GetButtonDown("Storage")) skillManager.UseSkill(3);
            if (Input.GetButtonDown("Cure") && Energe > 0 && moveCtrl.isOnGround) skillManager.UseSkill(4);
            if (Input.GetButtonDown("Shield") && Energe > 2 && moveCtrl.isOnGround) { skillManager.UseSkill(5); }
        }
        if (Input.GetButtonUp("Storage")) skillManager.StopSkill(3);
        if (Input.GetButtonUp("Cure") || Energe <= 0) skillManager.StopSkill(4);
        if (Input.GetButtonUp("Shield")) skillManager.StopSkill(5);
        if (Input.GetKeyDown(KeyCode.B)) rigidbody2D.AddForce(new Vector2(2700, 7700));
    }

    #region DestoryChild
    public event Action<UnityEngine.Object, float> onChildDestory;
    public void DestoryChild(UnityEngine.Object obj, float durTime)
    {
        onChildDestory.Invoke(obj, durTime);
    }
    #endregion

    #region PlayAttackParticle
    public event Action<Vector3, float> onPlarticleEnd;
    public void PlayAttackParticle(Vector3 offset, float partScale)
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
        StartCoroutine(TQueueExtion.DelayFunc(onPlarticleEnd, offset, partScale, effect.main.startLifetime.constantMax));//设置结束点
    }
    #endregion

    #region Attack
    public event Action<Return<float>, GetTargets> onAttack;
    /// <summary>
    /// 攻击函数
    /// </summary>
    /// <param name="caculateDamage">计算伤害委托</param>
    /// <param name="getTargets">获取目标委托</param>
    public void Attack(Return<float> caculateDamage, GetTargets getTargets)
    {
        float damage = caculateDamage == null ? AttackPower : caculateDamage();//计算伤害
        foreach (var target in getTargets?.Invoke(transform))
        {
            target?.OnAttackHit(transform.position, new Vector2(moveCtrl.facingDirection, 0) * pushPower, damage);
            if(target.gameObject.layer==10) Energe++;//每命中一个敌人增加一点能量
        }
        onAttack?.Invoke(caculateDamage, getTargets);
    }
    #endregion

    #region Dash
    private const float dashCd = 0.4f;//cd
    private float lastUse_dash = -dashCd;//上次使用的时间
    //按下Dash后调用此函数
    void Dash()
    {
        //TODO: 处理cd时间与同一时间内其他技能使用冲突
        if (Time.realtimeSinceStartup - lastUse_dash > dashCd)//若现在时间-上次使用时间>cd时间
        {
            skillManager.UseSkill(1);//使用技能
            lastUse_dash = Time.realtimeSinceStartup;//刷新上次使用时间
        }
    }
    #endregion

    #region RePlay
    private const float rePlayCd = 3f;//cd
    private float lastUse_RePlay = -rePlayCd;//上次使用的时间
    //按下Dash后调用此函数
    void RePlay()
    {
        if (Time.realtimeSinceStartup - lastUse_RePlay > rePlayCd)//若现在时间-上次使用时间>cd时间
        {
            skillManager.UseSkill(2);//使用技能
            lastUse_RePlay = Time.fixedTime;//刷新上次使用时间
        }
    }
    #endregion

    #region Hurt and Die also Camera
    // Hit Protection(受击无敌)
    public GameObject hurtFlashObject;
    [Monitor]
    private bool isDying;

    [SerializeField] float hitProtectionDuration = 1.0f;
    [SerializeField] SharedCounter<bool> canBeHit = new SharedCounter<bool>(true, () => { return false; }, (ref bool bert) => { bert = true; });
    [Monitor] public bool CanBeHit {get{ return canBeHit.GetRef(); }
        set 
        {
            if (!value) canBeHit.Increase();
            else canBeHit.Decrease();
        }
    }

    public bool Invincible { get; set; }

    public event Action OnDeath;
    // Camera
    private CameraController camController;
    private Camera cam;
    /// <summary>
    /// 外部调用使主体受伤函数
    /// </summary>
    /// <param name="damage">伤害值</param>
    /// <param name="recoilForce">击退力</param>
    /// <param name="vignetteIntensity">不知道</param>
    /// <param name="killRecoil">不知道</param>
    public void Hurt(int damage, Vector2 recoilForce, float vignetteIntensity = 0.7f, bool killRecoil = true)
    {
        if (!CanBeHit || isDying) return; //处于无敌状态||正在死亡=>返回、

        Hp -= damage;//扣血

        float shakeIntensity = 0.5f;
        camController.ShakeCamera(shakeIntensity);//相机抖动

        if (Hp <= 0)//血量低于等于0
        {
            if (!Invincible)//非无敌
            {
                KillPlayer(killRecoil);//抹杀主角
                animator.SetTrigger(CharacterAnimations.Die);//播放死亡动画
            }
        }

        CanBeHit = false;//进入无敌状态
        StartCoroutine(TQueueExtion.DelayFunc(()=>{CanBeHit = true; }, hitProtectionDuration)) ;//持续时间后移除
        EnableMoveCtrl = false;//禁用移动模块
        StartCoroutine(TQueueExtion.DelayFunc(()=> { EnableMoveCtrl = true; }, hitProtectionDuration/2));//持续时间后移除

        Instantiate(hurtFlashObject, transform.position, Quaternion.identity);//受伤特效
        GuiManager.Instance.FadeHurtVignette(vignetteIntensity);//不知道
        GameManager.Instance.FreezeTime(0.02f);//冻结时间

        
        skillManager.Interrupt();//中断技能
        
        rigidbody2D.velocity = recoilForce;//击退
        
        // Recoil
       // DoRecoil(recoilForce);
    }

    /// <summary>
    /// 使主体击退
    /// </summary>
    /// <param name="recoilForce">作用力</param>
    /// <param name="resetVelocity">是否重置速度</param>
    public void DoRecoil(Vector2 recoilForce, bool resetVelocity = false)
    {
        if (resetVelocity)
            rigidbody2D.velocity = Vector3.zero;

        rigidbody2D.AddForce(recoilForce);
    }
    /// <summary>
    /// 使角色死亡
    /// </summary>
    /// <param name="recoil"></param>
    public void KillPlayer(bool recoil)
    {
        if (isDying)
            return;

        // Fade screen and respawn player at last save position
        // Restore HP and other stuff
        Invincible = true;

        if (recoil)
        {
            DoRecoil(new Vector2(0, 700.0f));
        }

        //SoundManager.Instance.PlaySound(deathSound, 1, audioSource);//死亡音效
        isDying = true;//正在死亡
        animator.SetBool("IsDying", true);//播放死亡动画

        OnDeath?.Invoke();//死亡委托
    }
    #endregion
}
