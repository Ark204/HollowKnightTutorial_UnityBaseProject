using ArkSkill.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZhaoHuan :BDele
{
    [SerializeField] Object obj;
    private GameObject prefab;
    private float startTime;//TODO:从Info中移到Instance中
    public override void OnStart(SkillManager skillManager, SkillInfo skillInfo)
    {
        prefab = (GameObject)Instantiate(obj, skillManager.transform);//加载预制体//设为子物体
        startTime = Time.realtimeSinceStartup;
    }
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        Destroy(prefab);
        skillManager.GetComponent<PlayerCtrl>().DestoryChild(obj,Time.realtimeSinceStartup-startTime);
    }
}
