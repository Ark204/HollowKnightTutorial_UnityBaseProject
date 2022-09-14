using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArkSkill.SkillEditor
{
    using Core;
    [CustomPropertyDrawer(typeof(BDele))]
    public class BDeleDrawer : PropertyDrawer
    {
        private static readonly float singleLineHeight = EditorGUIUtility.singleLineHeight;
        private static readonly float baseHeight = 2.5f * singleLineHeight;
        float childHeight = 0f;
        public bool showing;//= false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                position.height = singleLineHeight;
                var rect = new Rect(position);

                //Button
                if (EditorGUI.DropdownButton(rect, new GUIContent("Set Dele"), FocusType.Passive))
                {
                    string path = AssetDatabase.GetAssetPath(property.serializedObject.targetObject);//获取目标对象路径
                    var menu = new GenericMenu();
                    foreach (var monoScript in SkillInfoEditor.DeriveClassHandler<BDele>.DeriveScripts)
                    {
                        menu.AddItem(new GUIContent(monoScript.name), false, () =>
                        {
                            var deleType = monoScript.GetClass();
                            BDele dele = ScriptableObject.CreateInstance(deleType) as BDele;
                            if (dele == null) Debug.Log(string.Format("技能实例创建失败，技能类型为{1}", deleType));
                        //dele.hideFlags = HideFlags.HideInHierarchy;//在渲染面板中隐藏
                        AssetDatabase.AddObjectToAsset(dele, path);//在路径下添加Asset
                        if (property.objectReferenceValue != null)//已经有BDele
                        {
                                Object.DestroyImmediate(property.objectReferenceValue, true);//删除原来的BDele
                        }
                            property.objectReferenceValue = dele;//将引用替换为新的引用
                        property.serializedObject.ApplyModifiedProperties();//更新（内存中）
                        AssetDatabase.ImportAsset(path);//更新路径
                    });
                    }
                    menu.AddItem(new GUIContent("Remove Dele"), false, () =>
                    {
                        BDele dele = property.objectReferenceValue as BDele;
                        dele.Remove();
                        property.objectReferenceValue = null;
                        property.serializedObject.ApplyModifiedProperties();//更新（内存中）
                });
                    menu.DropDown(rect);
                }
                //DrawBDele
                rect.y += singleLineHeight + 2;//DrawBDele
                if (property.objectReferenceValue == null)
                {
                    EditorGUI.LabelField(rect, "null Dele");
                    return;
                }
                SerializedObject bSkillSO = new SerializedObject(property.objectReferenceValue);//生成对应Object的SerializedObject
                var enumerator = bSkillSO.GetIterator();//获取迭代器
                showing = EditorGUI.Foldout(rect, showing, property.objectReferenceValue.GetType().Name);
                if (showing)
                {
                    EditorGUI.indentLevel++;
                    enumerator.NextVisible(true);//第一个属性: script
                    childHeight = 0f;
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    while (enumerator.NextVisible(false))
                    {
                        EditorGUI.PropertyField(rect, enumerator, true);
                        childHeight += EditorGUI.GetPropertyHeight(enumerator, true);
                        rect.y += singleLineHeight + 2;
                    }
                    EditorGUI.indentLevel--;
                    bSkillSO.ApplyModifiedProperties();
                }
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = baseHeight;
            if (showing) height += childHeight;
            return height;
        }
    }
}
