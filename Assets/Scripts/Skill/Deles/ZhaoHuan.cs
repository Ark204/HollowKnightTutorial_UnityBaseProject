using ArkSkill.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZhaoHuan :BDele
{
    [SerializeField] Object obj;
    private GameObject prefab;
    private float startTime;//TODO:��Info���Ƶ�Instance��
    public override void OnStart(SkillManager skillManager, SkillInfo skillInfo)
    {
        prefab = (GameObject)Instantiate(obj, skillManager.transform);//����Ԥ����//��Ϊ������
        startTime = Time.realtimeSinceStartup;
    }
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        Destroy(prefab);
        skillManager.GetComponent<PlayerCtrl>().DestoryChild(obj,Time.realtimeSinceStartup-startTime);
    }
}
