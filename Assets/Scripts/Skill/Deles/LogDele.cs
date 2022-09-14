using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill.Core;

public class LogDele :BDele
{
    [SerializeField] string text;  
    public override void Invoke(SkillManager skillManager, SkillInfo skillInfo)
    {
        Debug.Log(text+Time.realtimeSinceStartup);
    }
}
