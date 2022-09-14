using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill.Core;

//召唤物委托
public class AddModifierDele : BDele
{
    [SerializeField] int modifierId;
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        //skillManager.GetComponent<PlayerCtrl>().AddModifier(modifierId);
        GameObject prefab = (GameObject)Instantiate(Resources.Load("Prefabs/Player (2)"));//加载影子
        Transform transform = skillManager.transform;
        prefab.transform.position = transform.position;
        prefab.transform.localScale = transform.localScale;
        prefab.GetComponent<PlayerShadow>().Init(skillManager.GetComponent<TimeRecordModifier>().timeQueue.ToStack());
    }
}
