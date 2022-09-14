using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArkSkill.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">是该信息类的一个实例的类型</typeparam>
    public interface IInfo<T>where T:IInstance { public T GetInstance(); }
    public interface IInstance { }
    public interface IDeleInstance:IInstance
    {
        public IEnumerator CreatInvokeIEnumerator(SkillManager skillManager, float effectTime, SkillInfo skillInfo);
        public void OnStart(SkillManager skillManager, SkillInfo skillInfo);
        public void Invoke(SkillManager skillManager, SkillInfo skillInfo);
    }
    public interface ISkillInstance:IInstance
    {
        public void OnStart(SkillManager skillManager);
        public void OnStop(SkillManager skillManager);
        public void OnExit(SkillManager skillManager);
    }
}
