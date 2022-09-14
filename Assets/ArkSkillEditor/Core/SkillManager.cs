using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace ArkSkill.Core
{
    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private List<SkillInstance> skills = new List<SkillInstance>();
        public new Transform transform
        {
            get
            {
                if (m_transform == null) m_transform = base.transform;
                return m_transform;
            }
            private set { m_transform = value; }
        }
        private Transform m_transform;
        public SkillInstance currSkill { get; private set; }

        
        public void UseSkill(int id)
        {
            if (currSkill != null) return;//若现在有技能正在使用，则返回
            //尝试获取技能
            SkillInstance skill = FindSkillInstanceByID(id);
            if (skill == null) return;
            skill.Start();//调用Skill的Start()
            //保存当前使用技能信息
            currSkill = skill;
            //后摇点设置： 若非循环技能 则抵达后摇点时调用退出，若为循环技能 则不设置退出
            if (!skill.Loop) StartCoroutine(DelayFunc(ExitSkill, skill.endTime));//非循环：抵达后摇点时退出
        }
        [ContextMenu("Interrupt")]
        public void Interrupt()
        {
            if (currSkill == null) return;//若当前没有技能正在使用，则直接返回
            ExitSkill();
            Debug.Log("Interrupt");
        }
        public void StopSkill(int id)
        {
            if (currSkill == null) return;//若当前没有技能正在使用，则直接返回
                                          //尝试获取技能
            SkillInstance skill = FindSkillInstanceByID(id);
            if (skill != currSkill || !skill) return;//若当前正使用的技能不是用户想要终止的技能或用户想要终止的技能不存在，则直接返回
            ExitSkill();
            skill.Stop();//调用技能的主动退出函数
        }

        public void AddSkill(int id)
        {
            SkillInstance skill = SkillInfo.GetSkillInfoBySkillId(id).CreatEntity();
            skill.Bind(this);
            if (!skill) Debug.Log("技能绑定出错");
            else AddSkill(skill);//添加新技能
        }

        public void RemoveSkill(int id)
        {
            SkillInstance targetSkill = FindSkillInstanceByID(id);
            if (targetSkill) RemoveSkill(targetSkill);
        }
        /// <summary>
        /// 检查SkillManager中是否含有该ID的技能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(int id)
        {
            return skills.Contains(FindSkillInstanceByID(id));
        }
        [ContextMenu("Stop Curr Skill")]
        void StopCurr()
        {
            if (currSkill == null) return;
            int id = currSkill.infoId;
            StopSkill(id);
        }
        //退出当前技能
        void ExitSkill()
        {
            StopAllCoroutines();//停止所有协程
            currSkill.Exit();//调用当前技能的退出函数
            currSkill = null;//清空当前技能信息
        }
        //只向外界暴露技能ID
        void AddSkill(SkillInstance skillInstance)
        {
            if (!skills.Contains(skillInstance))//如果不存在skillInstance
            {
                skills.Add(skillInstance);
                skillInstance.Add();
            }
        }
        void RemoveSkill(SkillInstance skillInstance) { skills.Remove(skillInstance); skillInstance.Remove(); }
        //根据技能ID查找SkillManager中的技能
        SkillInstance FindSkillInstanceByID(int id)
        {
            foreach (var skill in skills)
            {
                if (skill.infoId == id) return skill;
            }
            return null;
        }
        private void OnDrawGizmos() { foreach (var skill in skills) skill.DrawRange(); }
        #region 协程辅助函数
        /// <summary>
        /// 协程函数，用于延迟调用0参数函数
        /// </summary>
        /// <param name="callBack">将延迟调用的0参数函数</param>
        /// <param name="time">延迟时间,默认为0</param>
        /// <returns></returns>
        private IEnumerator DelayFunc(Action callBack, float time = 0)
        {
            yield return new WaitForSecondsRealtime(time);
            callBack?.Invoke();
        }
        /// <summary>
        /// 协程函数，用于延迟调用1参数函数
        /// </summary>
        /// <typeparam name="T">调用函数的参数类型</typeparam>
        /// <param name="callBack">将延迟调用的1参数函数</param>
        /// <param name="arg1">调用函数的具体参数1</param>
        /// <param name="time">延迟时间,默认为0</param>
        /// <returns></returns>
        private IEnumerator DelayFunc<T>(Action<T> callBack, T arg1, float time = 0)
        {
            yield return new WaitForSecondsRealtime(time);
            callBack?.Invoke(arg1);
        }
        private IEnumerator DelayFunc<T, X>(Action<T, X> callBack, T arg1, X arg2, float time = 0)
        {
            yield return new WaitForSecondsRealtime(time);
            callBack?.Invoke(arg1, arg2);
        }
        #endregion
    }

}