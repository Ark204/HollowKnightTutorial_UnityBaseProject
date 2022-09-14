using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill;

namespace ArkSkill.Core
{
    /// <summary>
    /// 技能实例，由SkillManager组件统一管理
    /// </summary>
    [System.Serializable]
    public class SkillInstance
    {
        [SerializeField] bool show;//是否显示技能范围
        [SerializeField] SkillInfo skillInfo;//技能逻辑数据信息
        [HideInInspector] public SkillManager skillManager;//挂载的SkillManager
        public int infoId { get => skillInfo.m_skillID; }
        public float endTime { get => skillInfo.EndTime; }
        public bool Loop { get => skillInfo.Loop; }
        
        List<IDeleInstance> m_timePoints;
        private List<IDeleInstance> TimePoints
        {
            get { if (m_timePoints == null) m_timePoints = skillInfo.timePoints.GetInstance(); return m_timePoints; }
        }
        List<IDeleInstance> m_startDeles;
        private List<IDeleInstance> StartDeles 
        {
            get { if(m_startDeles==null) m_startDeles=skillInfo.startDeles.GetInstance();return m_startDeles; }
        }
        List<IDeleInstance> m_stopDeles;
        private List<IDeleInstance> StopDeles
        {
            get { if (m_stopDeles == null) m_stopDeles = skillInfo.stopDeles.GetInstance(); return m_stopDeles; }
        }
        List<IDeleInstance> m_exitDeles;
        private List<IDeleInstance> ExitDeles
        {
            get { if (m_exitDeles == null) m_exitDeles = skillInfo.exitDeles.GetInstance(); return m_exitDeles; }
        }
        List<ISkillInstance> m_bSkills;//自定义技能脚本（技能特化）
        List<ISkillInstance> Skills
        {
            get { if (m_bSkills == null) m_bSkills = skillInfo.bSkills.GetInstances();return m_bSkills; }
        }


        public SkillInstance(SkillInfo skillInfo)
        {
            this.skillInfo = skillInfo;
        }
        public SkillInstance() { }
        internal void Bind(SkillInfo info) => skillInfo = info;
        internal void Bind(SkillManager skillManager) => this.skillManager = skillManager;
        public static implicit operator bool(SkillInstance skillInstance) => skillInstance != null && skillInstance.skillInfo && skillInstance.skillManager;

        //interfaces
        //use by AttackCtrl
        public void Start() //技能使用
        {
            foreach (var bSkill in Skills) bSkill?.OnStart(skillManager);
            foreach (var startDele in StartDeles) startDele.Invoke(skillManager,skillInfo);//遍历调用开始操作委托
            if(skillInfo.Loop) foreach (var stopDele in StopDeles) stopDele.OnStart(skillManager,skillInfo);
            foreach (var exitDele in ExitDeles) exitDele.OnStart(skillManager,skillInfo);//遍历调用其他委托的OnStart()
            foreach (var timePoint in TimePoints) timePoint.OnStart(skillManager,skillInfo);

            InvokeTimePoints();
        }
        #region InvokeTimePoints
        void InvokeTimePoints()
        {
            foreach (var timePoint in TimePoints)
            {
                StartCoroutine(timePoint.CreatInvokeIEnumerator(skillManager, skillInfo.EndTime,skillInfo));//由挂载的SkillManager协程调用时间点
            }
            if (Loop)//如果循环
                StartCoroutine(LoopSkill());
        }
        IEnumerator LoopSkill()
        {
            yield return new WaitForSecondsRealtime(endTime);
            InvokeTimePoints();
        }
        #endregion
        public void Stop()//主动退出技能接口
        {
            if (!skillInfo.Loop) return;//若非循环技能 则直接退出
            foreach (var bSkill in Skills) bSkill?.OnStop(skillManager);
            foreach (var stopDele in StopDeles)//先调用当前技能的主动退出操作委托
            {
                stopDele.Invoke(skillManager,skillInfo);
            }
        }
        public void Exit()//技能退出时必将调用(SkillManager调用)
        {
            //停止结束点上的协程并直接调用
            foreach (var bSkill in Skills) bSkill?.OnExit(skillManager);
            foreach (var exitDele in ExitDeles)
            {
                exitDele.Invoke(skillManager,skillInfo);
            }
        }

        //暂不实现 public void Update() { }

        //use by SkillManager
        public void Add() { /*foreach (var bSkill in Skills) bSkill?.OnStart(skillManager); */}//SkillManager
        public void Remove() { /*foreach (var bSkill in Skills) bSkill?(skillManager);*/ }//SkillManager

        //use by SkillManager
        internal void DrawRange()
        {
            if (show) skillInfo?.DrawGizmos(skillManager.transform.position, skillManager.transform.localScale.x);
        }

        Coroutine StartCoroutine(IEnumerator routine) { return skillManager.StartCoroutine(routine); }
        void StopAllCoroutines() => skillManager.StopAllCoroutines();
        void StopCoroutine(IEnumerator routine) => skillManager.StopCoroutine(routine);
        void StopCoroutine(Coroutine routine) => skillManager.StopCoroutine(routine);
    }
}