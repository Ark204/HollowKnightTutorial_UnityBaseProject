using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using ArkSkill.Core;

namespace ArkSkill.SkillEditor
{
    [CustomEditor(typeof(SkillManager), true)]
    public class SkillManagerEditor : Editor
    {
        ReorderableList reorderable;
        SkillManager skillManager;//ֱ��ʹ��Ŀ������ñ༭skillScript(δ�����ܻ��޸�)
        private void OnEnable()
        {
            skillManager = target as SkillManager;
            if (skillScripts == null) skillScripts = GetSkillScripts();

            var prop = serializedObject.FindProperty("skills");
            if (prop == null) { Debug.Log("null prop"); return; }
            reorderable = new ReorderableList(serializedObject, prop);
            reorderable.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, prop.displayName); };
            reorderable.elementHeightCallback = (index) =>
            {
                var elem = prop.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(elem, true);
            };
            reorderable.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = prop.GetArrayElementAtIndex(index);
                rect.height -= 4;
                rect.y += 2;
                EditorGUI.PropertyField(rect, element, true);
            };
            //reorderable.onMouseUpCallback = (list) =>
            //{
            //    var menu = new GenericMenu();
            //    foreach (var monoScript in skillScripts)
            //    {
            //        menu.AddItem(new GUIContent(monoScript.name), false, () =>
            //        {
            //            //var element = prop.GetArrayElementAtIndex(list.index);//skillInstanceProp
            //            var skillType = monoScript.GetClass();
            //            BSkill skill = Activator.CreateInstance(skillType) as BSkill;
            //            if (skill == null)
            //            {
            //                throw new Exception(string.Format("����ʵ������ʧ�ܣ���������Ϊ{1}", skillType));
            //            }
            //            ////element.ref = skill;
            //            skillmanager.skills[list.index].bSkill = skill;
            //        });
            //    }
            //    menu.AddItem(new GUIContent("Remove Script"), false, () =>
            //    {
            //        //list[list.index] skillInfo.skillScript = null;
            //        skillmanager.skills[list.index].bSkill = null;
            //    });
            //    Rect rect = new Rect(Event.current.mousePosition, new Vector2(30, 30));
            //    menu.DropDown(rect);
            //};
            reorderable.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
            {
                var menu = new GenericMenu();
                foreach (var info in SkillInfo.skillInfos)
                {
                    menu.AddItem(new GUIContent(info.m_skillID.ToString()), false, () =>
                    {
                    //prop.arraySize++;
                    //list.index = prop.arraySize - 1;
                    //var element = prop.GetArrayElementAtIndex(list.index);
                    //element.objectReferenceValue = SkillInfo.GetSkillInfoBySkillId(info.m_skillID).CreatEntity();
                    ////???????????????????
                    //serializedObject.ApplyModifiedProperties();
                    //------------------------���䷨��Ӽ���------------------------------
                    //var method = target.GetType().GetMethod("AddSkillInstanceByID");
                    //if (method == null) { Debug.Log("�Ҳ���ָ������"); return; }
                    //object[] para = new object[1];
                    //para[0] = info.m_skillID;
                    //method.Invoke(target, para);
                    //-------------------------ֱ�ӷ���Ӽ���------------------------------
                    skillManager.AddSkill(info.m_skillID);
                    });
                }
                menu.DropDown(buttonRect);
            };
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //base.OnInspectorGUI();
            reorderable.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// ���ؼ̳���BSkill����Ľű�
        /// </summary>
        /// <returns></returns>
        List<MonoScript> GetSkillScripts()
        {
            //Debug.Log("call"); //���Ըú����ĵ���
            var monoScripts = new List<MonoScript>();
            foreach (var script in MonoImporter.GetAllRuntimeMonoScripts())
            {
                var type = script.GetClass();
                if (type == null) continue;
                var baseType = type.BaseType;  //��ȡ����
                while (baseType != null)  //��ȡ���л���
                {
                    //Debug.Log(baseType.Name);//TestUse:Get each name of type
                    if (baseType.Name == typeof(BSkill).Name)
                    {
                        monoScripts.Add(script);
                        break;
                    }
                    else baseType = baseType.BaseType;
                }
            }
            return monoScripts;
        }
        /// <summary>
        /// �̳���BSkill����Ľű�����(����)
        /// </summary>
        public static List<MonoScript> skillScripts;
    }
}
