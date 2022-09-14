using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArkSkill;

namespace ArkSkill.Core
{
    /// <summary>
    /// ����ʵ������SkillManager���ͳһ����
    /// </summary>
    [System.Serializable]
    public class SkillInstance
    {
        [SerializeField] bool show;//�Ƿ���ʾ���ܷ�Χ
        [SerializeField] SkillInfo skillInfo;//�����߼�������Ϣ
        [HideInInspector] public SkillManager skillManager;//���ص�SkillManager
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
        List<ISkillInstance> m_bSkills;//�Զ��弼�ܽű��������ػ���
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
        public void Start() //����ʹ��
        {
            foreach (var bSkill in Skills) bSkill?.OnStart(skillManager);
            foreach (var startDele in StartDeles) startDele.Invoke(skillManager,skillInfo);//�������ÿ�ʼ����ί��
            if(skillInfo.Loop) foreach (var stopDele in StopDeles) stopDele.OnStart(skillManager,skillInfo);
            foreach (var exitDele in ExitDeles) exitDele.OnStart(skillManager,skillInfo);//������������ί�е�OnStart()
            foreach (var timePoint in TimePoints) timePoint.OnStart(skillManager,skillInfo);

            InvokeTimePoints();
        }
        #region InvokeTimePoints
        void InvokeTimePoints()
        {
            foreach (var timePoint in TimePoints)
            {
                StartCoroutine(timePoint.CreatInvokeIEnumerator(skillManager, skillInfo.EndTime,skillInfo));//�ɹ��ص�SkillManagerЭ�̵���ʱ���
            }
            if (Loop)//���ѭ��
                StartCoroutine(LoopSkill());
        }
        IEnumerator LoopSkill()
        {
            yield return new WaitForSecondsRealtime(endTime);
            InvokeTimePoints();
        }
        #endregion
        public void Stop()//�����˳����ܽӿ�
        {
            if (!skillInfo.Loop) return;//����ѭ������ ��ֱ���˳�
            foreach (var bSkill in Skills) bSkill?.OnStop(skillManager);
            foreach (var stopDele in StopDeles)//�ȵ��õ�ǰ���ܵ������˳�����ί��
            {
                stopDele.Invoke(skillManager,skillInfo);
            }
        }
        public void Exit()//�����˳�ʱ�ؽ�����(SkillManager����)
        {
            //ֹͣ�������ϵ�Э�̲�ֱ�ӵ���
            foreach (var bSkill in Skills) bSkill?.OnExit(skillManager);
            foreach (var exitDele in ExitDeles)
            {
                exitDele.Invoke(skillManager,skillInfo);
            }
        }

        //�ݲ�ʵ�� public void Update() { }

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