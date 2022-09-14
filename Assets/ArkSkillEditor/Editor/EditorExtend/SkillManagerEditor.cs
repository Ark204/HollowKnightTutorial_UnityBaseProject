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
        SkillManager skillManager;//直接使用目标的引用编辑skillScript(未来可能会修改)
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
            //                throw new Exception(string.Format("技能实例创建失败，技能类型为{1}", skillType));
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
                    //------------------------反射法添加技能------------------------------
                    //var method = target.GetType().GetMethod("AddSkillInstanceByID");
                    //if (method == null) { Debug.Log("找不到指定函数"); return; }
                    //object[] para = new object[1];
                    //para[0] = info.m_skillID;
                    //method.Invoke(target, para);
                    //-------------------------直接法添加技能------------------------------
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
        /// 返回继承自BSkill的类的脚本
        /// </summary>
        /// <returns></returns>
        List<MonoScript> GetSkillScripts()
        {
            //Debug.Log("call"); //测试该函数的调用
            var monoScripts = new List<MonoScript>();
            foreach (var script in MonoImporter.GetAllRuntimeMonoScripts())
            {
                var type = script.GetClass();
                if (type == null) continue;
                var baseType = type.BaseType;  //获取基类
                while (baseType != null)  //获取所有基类
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
        /// 继承自BSkill的类的脚本数组(单例)
        /// </summary>
        public static List<MonoScript> skillScripts;
    }
}
