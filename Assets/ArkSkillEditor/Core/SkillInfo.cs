using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace ArkSkill.Core
{
    [CreateAssetMenu(fileName = "New SkillInfo", menuName = "SkillInfo")]
    public class SkillInfo : ScriptableObject
    {
        [Header("Skill Info")]
        public int m_skillID;
        [SerializeField] bool m_loop=false;
        public bool Loop { get => m_loop; }
        [SerializeField] float m_endTime;
        public float EndTime { get => m_endTime; }
        [HideInInspector] public List<BDele> startDeles = new List<BDele>();//��ʼ����ί��
        /* [HideInInspector]*/ public List<TimePoint> timePoints = new List<TimePoint>();
        [HideInInspector] public List<BDele> stopDeles = new List<BDele>();//{ get; set; }//�жϲ���ί��
        [HideInInspector] public List<BDele> exitDeles = new List<BDele>();//��������ί��
        [HideInInspector] public List<BSkill> bSkills = new List<BSkill>();//Skill Behavior�б�
        public SkillInfo() { skillInfos.Add(this); /*Debug.Log("������SkillInfo"); */}
        /// <summary>
        /// ��������ʵ��
        /// </summary>
        /// <returns>����ʵ��</returns>
        public SkillInstance CreatEntity()
        {
            SkillInstance skill = new SkillInstance();
            skill.Bind(this);
            return skill;
        }
        private void OnEnable()
        {
            //if (skillInfos == null) skillInfos = new List<SkillInfo>(FindObjectsOfType<SkillInfo>());
        }
        internal void DrawGizmos(Vector3 position, float direct)
        {
            foreach (var dele in startDeles) dele?.OnDraw(position, direct);
            foreach (var dele in stopDeles) dele?.OnDraw(position, direct);
            foreach (var dele in exitDeles) dele?.OnDraw(position, direct);
            foreach (var timePoint in timePoints) timePoint.OnDraw(position, direct);
            foreach (var skillBehavior in bSkills) skillBehavior?.OnDraw(position, direct);
        }
        [ContextMenu("ShowChildrenAsset")]
        void ShowChildrenAsset()
        {
            var path = AssetDatabase.GetAssetPath(this);
            foreach (var item in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                item.hideFlags = HideFlags.None;
            }
            AssetDatabase.ImportAsset(path);
        }
        [ContextMenu("HideChildrenAsset")]
        void HideChildrenAsset()
        {
            var path = AssetDatabase.GetAssetPath(this);
            foreach (var item in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                item.hideFlags = HideFlags.HideInHierarchy;
            }
            AssetDatabase.ImportAsset(path);
        }

        //static
        public static List<SkillInfo> skillInfos = new List<SkillInfo>(); // FindObjectsOfType<SkillInfo>();
        /// <summary>
        /// ͨ������ID��ȡ�����߼�ʵ��
        /// </summary>
        /// <param name="id">����ID</param>
        /// <returns>�����߼�ʵ��</returns>
        public static SkillInfo GetSkillInfoBySkillId(int id)
        {
            foreach (var elem in skillInfos)
            {
                if (elem.m_skillID == id) return elem;
            }
            return null;
        }
    }
    [Serializable]
    public struct TimePoint:IInfo<IDeleInstance>//,IDeleInstance
    {
        [Range(0,1)]public float effectTime;
        public BDele bDele;//δ����ת��Ϊ����ί����
        public IDeleInstance GetInstance() { return new TimePointInstance(this); }
        public void OnDraw(Vector3 position, float direct)
        {
            bDele?.OnDraw(position, direct);
        }
    }
    public class TimePointInstance : IDeleInstance
    {
        private TimePoint m_Info;
        private IDeleInstance deleInstance;
        public TimePointInstance(TimePoint timePoint){m_Info=timePoint; deleInstance = timePoint.bDele.GetInstance();}
        public IEnumerator CreatInvokeIEnumerator(SkillManager skillManager, float endTime, SkillInfo skillInfo)
        {
            yield return new WaitForSecondsRealtime(m_Info.effectTime*endTime);
            deleInstance.Invoke(skillManager, skillInfo);
        }
        public void Invoke(SkillManager skillManager, SkillInfo skillInfo)
        {
            deleInstance.Invoke(skillManager, skillInfo);
        }
        public void OnStart(SkillManager skillManager, SkillInfo skillInfo)
        {
            deleInstance.OnStart(skillManager, skillInfo);
        }
    }
    /// <summary>
    /// Extions(���������չ����)
    /// </summary>
    public static class ArkSkillExtion
    {
        public static List<IDeleInstance> GetInstance<T>(this List<T> deleInfos)where T:IInfo<IDeleInstance>
        {
            List<IDeleInstance> deleInstances = new List<IDeleInstance>();
            foreach(var deleInfo in deleInfos)
            {
                deleInstances.Add(deleInfo.GetInstance());
            }
            return deleInstances;
        }
        public static List<ISkillInstance> GetInstances<X>(this List<X> deleInfos) where X : IInfo<ISkillInstance>
        {
            List<ISkillInstance> deleInstances = new List<ISkillInstance>();
            foreach (var deleInfo in deleInfos)
            {
                deleInstances.Add(deleInfo.GetInstance());
            }
            return deleInstances;
        }
    }
}