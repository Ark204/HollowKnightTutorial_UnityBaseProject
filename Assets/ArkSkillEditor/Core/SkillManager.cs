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
            if (currSkill != null) return;//�������м�������ʹ�ã��򷵻�
            //���Ի�ȡ����
            SkillInstance skill = FindSkillInstanceByID(id);
            if (skill == null) return;
            skill.Start();//����Skill��Start()
            //���浱ǰʹ�ü�����Ϣ
            currSkill = skill;
            //��ҡ�����ã� ����ѭ������ ��ִ��ҡ��ʱ�����˳�����Ϊѭ������ �������˳�
            if (!skill.Loop) StartCoroutine(DelayFunc(ExitSkill, skill.endTime));//��ѭ�����ִ��ҡ��ʱ�˳�
        }
        [ContextMenu("Interrupt")]
        public void Interrupt()
        {
            if (currSkill == null) return;//����ǰû�м�������ʹ�ã���ֱ�ӷ���
            ExitSkill();
            Debug.Log("Interrupt");
        }
        public void StopSkill(int id)
        {
            if (currSkill == null) return;//����ǰû�м�������ʹ�ã���ֱ�ӷ���
                                          //���Ի�ȡ����
            SkillInstance skill = FindSkillInstanceByID(id);
            if (skill != currSkill || !skill) return;//����ǰ��ʹ�õļ��ܲ����û���Ҫ��ֹ�ļ��ܻ��û���Ҫ��ֹ�ļ��ܲ����ڣ���ֱ�ӷ���
            ExitSkill();
            skill.Stop();//���ü��ܵ������˳�����
        }

        public void AddSkill(int id)
        {
            SkillInstance skill = SkillInfo.GetSkillInfoBySkillId(id).CreatEntity();
            skill.Bind(this);
            if (!skill) Debug.Log("���ܰ󶨳���");
            else AddSkill(skill);//����¼���
        }

        public void RemoveSkill(int id)
        {
            SkillInstance targetSkill = FindSkillInstanceByID(id);
            if (targetSkill) RemoveSkill(targetSkill);
        }
        /// <summary>
        /// ���SkillManager���Ƿ��и�ID�ļ���
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
        //�˳���ǰ����
        void ExitSkill()
        {
            StopAllCoroutines();//ֹͣ����Э��
            currSkill.Exit();//���õ�ǰ���ܵ��˳�����
            currSkill = null;//��յ�ǰ������Ϣ
        }
        //ֻ����籩¶����ID
        void AddSkill(SkillInstance skillInstance)
        {
            if (!skills.Contains(skillInstance))//���������skillInstance
            {
                skills.Add(skillInstance);
                skillInstance.Add();
            }
        }
        void RemoveSkill(SkillInstance skillInstance) { skills.Remove(skillInstance); skillInstance.Remove(); }
        //���ݼ���ID����SkillManager�еļ���
        SkillInstance FindSkillInstanceByID(int id)
        {
            foreach (var skill in skills)
            {
                if (skill.infoId == id) return skill;
            }
            return null;
        }
        private void OnDrawGizmos() { foreach (var skill in skills) skill.DrawRange(); }
        #region Э�̸�������
        /// <summary>
        /// Э�̺����������ӳٵ���0��������
        /// </summary>
        /// <param name="callBack">���ӳٵ��õ�0��������</param>
        /// <param name="time">�ӳ�ʱ��,Ĭ��Ϊ0</param>
        /// <returns></returns>
        private IEnumerator DelayFunc(Action callBack, float time = 0)
        {
            yield return new WaitForSecondsRealtime(time);
            callBack?.Invoke();
        }
        /// <summary>
        /// Э�̺����������ӳٵ���1��������
        /// </summary>
        /// <typeparam name="T">���ú����Ĳ�������</typeparam>
        /// <param name="callBack">���ӳٵ��õ�1��������</param>
        /// <param name="arg1">���ú����ľ������1</param>
        /// <param name="time">�ӳ�ʱ��,Ĭ��Ϊ0</param>
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