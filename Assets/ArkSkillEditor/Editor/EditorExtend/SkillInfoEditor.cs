using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEditorInternal;
using ArkSkill.Core;

namespace ArkSkill.SkillEditor
{
    [CustomEditor(typeof(SkillInfo), true)]
    public class SkillInfoEditor : Editor
    {
        SkillInfo skillInfo;//直接使用目标的引用编辑skillScript(未来可能会修改)

        //---------------startDeles---------------
        AbstractListView<BDele> startDeleListView;//用于缓存startDeles列表
        int index1 = 0;
        //----------stopDeles-----------
        AbstractListView<BDele> stopDeleListView;//用于缓存stopDeles列表
        int index = 0;
        //--------------exitDeles------------------
        AbstractListView<BDele> exitDeleListView;//用于缓存exitDeles列表
        int index2 = 0;
        //---------------timePoints------------------------
        bool showTimePoints = false;
        ReorderableList reorderable;
        //---------------timePoints------------------------
        AbstractListView<BSkill> bSkillListView;//用于缓存bSkills列表
        int index3 = 0;
        private void OnEnable()
        {
            skillInfo = target as SkillInfo;
            string path = AssetDatabase.GetAssetPath(skillInfo);//获取SkillInfo的路径
            //startDeles
            startDeleListView = new AbstractListView<BDele>(skillInfo.startDeles, "Start Deles");
            startDeleListView.addEvent += () =>//元素添加事件
            {
                BDele newDele = null;
                if (index1 <DeriveClassHandler<BDele>.DeriveScripts.Count) newDele = CreateInstance(DeriveClassHandler<BDele>.DeriveScripts[index1].GetClass()) as BDele;
                //newDele.hideFlags = HideFlags.HideInHierarchy;//在渲染面板中隐藏
                AssetDatabase.AddObjectToAsset(newDele, path);
                AssetDatabase.ImportAsset(path);
                return newDele;
            };
            startDeleListView.onDrawAddButtton += () =>
            {
                EditorGUILayout.LabelField("选择您要添加的Dele类型");
                index1 = EditorGUILayout.Popup(index1, DeriveClassHandler<BDele>.DeriveScriptsOptions);
            };//元素添加按钮绘制
            //stopDeles
                stopDeleListView = new AbstractListView<BDele>(skillInfo.stopDeles, "Stop Deles");
                stopDeleListView.addEvent += () =>
                {
                    BDele newDele = null;
                    if (index < DeriveClassHandler<BDele>.DeriveScripts.Count) newDele = CreateInstance(DeriveClassHandler<BDele>.DeriveScripts[index].GetClass()) as BDele;
                //newDele.hideFlags = HideFlags.HideInHierarchy;//在渲染面板中隐藏
                AssetDatabase.AddObjectToAsset(newDele, path);
                    AssetDatabase.ImportAsset(path);
                    return newDele;
                };
                stopDeleListView.onDrawAddButtton += () => { EditorGUILayout.LabelField("选择您要添加的Dele类型"); index = EditorGUILayout.Popup(index, DeriveClassHandler<BDele>.DeriveScriptsOptions); };//元素添加按钮绘制
            
                                                                                                                                            
            //exitDeles
            exitDeleListView = new AbstractListView<BDele>(skillInfo.exitDeles, "Exit Deles");
            exitDeleListView.addEvent += () =>
            {
                BDele newDele = null;
                if (index2 < DeriveClassHandler<BDele>.DeriveScripts.Count) newDele = CreateInstance(DeriveClassHandler<BDele>.DeriveScripts[index2].GetClass()) as BDele;
                //newDele.hideFlags = HideFlags.HideInHierarchy;//在渲染面板中隐藏
                AssetDatabase.AddObjectToAsset(newDele, path);
                AssetDatabase.ImportAsset(path);
                return newDele;
            };
            exitDeleListView.onDrawAddButtton += () => { EditorGUILayout.LabelField("选择您要添加的Dele类型"); index2 = EditorGUILayout.Popup(index2, DeriveClassHandler<BDele>.DeriveScriptsOptions); };//元素添加按钮绘制
            //bSkills
            bSkillListView = new AbstractListView<BSkill>(skillInfo.bSkills, "Skill Behaviors");
            bSkillListView.addEvent += () =>
              {
                  BSkill newSkill = null;
                  if (index3 < DeriveClassHandler<BSkill>.DeriveScripts.Count) newSkill = CreateInstance(DeriveClassHandler<BSkill>.DeriveScripts[index3].GetClass()) as BSkill;
                  //newDele.hideFlags = HideFlags.HideInHierarchy;//在渲染面板中隐藏
                  AssetDatabase.AddObjectToAsset(newSkill, path);
                  AssetDatabase.ImportAsset(path);
                  return newSkill;
              };
            bSkillListView.onDrawAddButtton += () =>
            {
                EditorGUILayout.LabelField("选择您要添加的Skill Behavior类型");
                index3 = EditorGUILayout.Popup(index3, DeriveClassHandler<BSkill>.DeriveScriptsOptions);
            };//元素添加按钮绘制

            //timePoints
            var prop = serializedObject.FindProperty("timePoints");
            if (prop == null) { Debug.Log("null prop"); return; }
            reorderable = new ReorderableList(serializedObject, prop);
            reorderable.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, prop.displayName); };
            reorderable.elementHeightCallback = (index) =>
            {
                var elem = prop.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(elem, true);
            };
            reorderable.drawElementCallback = (rect, index, isActive, isFoused) =>
              {
                  var elem = prop.GetArrayElementAtIndex(index);
                  rect.height -= 4;
                  rect.y += 2;
                  EditorGUI.PropertyField(rect, elem, true);
              };
            reorderable.onAddCallback = (list) =>
            {
                prop.arraySize++;
                list.index = prop.arraySize - 1;
                var elem = prop.GetArrayElementAtIndex(list.index);
                elem.FindPropertyRelative("bDele").objectReferenceValue = null;
            };
            reorderable.onRemoveCallback = (list) =>
            {
                var elem = prop.GetArrayElementAtIndex(list.index);
                UnityEngine.Object obj = elem.FindPropertyRelative("bDele").objectReferenceValue;
                string path = AssetDatabase.GetAssetPath(obj);
                DestroyImmediate(obj, true);
                AssetDatabase.ImportAsset(path);
                prop.arraySize--;
                list.index = prop.arraySize - 1;
            };
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //base.OnInspectorGUI();
            var property = serializedObject.FindProperty("m_skillID");
            EditorGUILayout.PropertyField(property);
            property = serializedObject.FindProperty("m_loop");
            EditorGUILayout.PropertyField(property);
            property = serializedObject.FindProperty("m_endTime");
            EditorGUILayout.PropertyField(property);
            //show startDeles
            startDeleListView._DoLayout();
            //show stopDeles
            if(skillInfo.Loop) stopDeleListView._DoLayout();
            //show exitDeles
            exitDeleListView._DoLayout();
            //show timePoints
            string space = new string(' ', 65 - 11);
            showTimePoints = EditorGUILayout.Foldout(showTimePoints, "Time Points" + space + "Size:   " + reorderable.count, showTimePoints);
            if (showTimePoints) reorderable.DoLayoutList();
            //show bSkills
            bSkillListView._DoLayout();
            serializedObject.ApplyModifiedProperties();
        }
        //------------static------------------
        #region 派生自BDele的所有MonoScript的获取
        
        public static class DeriveClassHandler<T>
        {
            public static List<MonoScript> DeriveScripts
            {
                get { if (deriveScripts == null) deriveScripts = GetDeriveScripts(); return deriveScripts; }
                private set { deriveScripts = value; }
            }
            public static string[] DeriveScriptsOptions
            {
                get
                {
                    if (deriveScriptsOptions == null)
                    {
                        deriveScriptsOptions = new string[DeriveScripts.Count];
                        for (int i = 0; i < DeriveScripts.Count; i++)
                        {
                            deriveScriptsOptions[i] = DeriveScripts[i].name;
                        }
                    }
                    return deriveScriptsOptions;
                }
                private set { deriveScriptsOptions = value; }
            }
            static string[] deriveScriptsOptions;
            /// <summary>
            /// 继承自T的类的脚本数组(单例、延迟初始化)
            /// </summary>
            static List<MonoScript> deriveScripts;
            /// <summary>
            /// 返回继承自T的类的脚本
            /// </summary>
            /// <returns></returns>
            static List<MonoScript> GetDeriveScripts()
            {
                var monoScripts = new List<MonoScript>();
                foreach (var script in MonoImporter.GetAllRuntimeMonoScripts())
                {
                    var type = script.GetClass();
                    if (type == null) continue;//跳过对自身的检索
                    var baseType = type.BaseType;  //获取基类
                    while (baseType != null)  //获取所有基类
                    {
                        //Debug.Log(baseType.Name);//TestUse:Get each name of type
                        if (baseType.Name == typeof(T).Name)
                        {
                            monoScripts.Add(script);
                            break;
                        }
                        else baseType = baseType.BaseType;
                    }
                }
                return monoScripts;
            }
        }
        #endregion
    }
    public class AbstractListView<T> where T : UnityEngine.ScriptableObject
    {
        //类内内部定义
        public delegate T AddDele();
        public delegate void DrawAddButton();
        public class ItemCache<X> where X : ScriptableObject
        {
            Editor currItemEditor;
            X item;
            bool showing;
            public static implicit operator X(ItemCache<X> itemCache)
            {
                return itemCache.item;
            }
            public ItemCache(X newItem)
            {
                item = newItem;
                currItemEditor = Editor.CreateEditor(newItem);//为元素创造Editor
                showing = true;
            }
            public bool ShowItem()
            {
                if (item != null && currItemEditor != null)
                {
                    if (showing = EditorGUILayout.InspectorTitlebar(showing, item)) currItemEditor.OnInspectorGUI();
                    return true;
                }
                else if (currItemEditor != null) ScriptableObject.DestroyImmediate(currItemEditor);//如果item为空，则销毁Editor
                return false;
            }
        }
        const int MaxWidth = 65;
        const int NumList = 2;//双缓冲
        public event AddDele addEvent;
        public event DrawAddButton onDrawAddButtton;//绘制添加按钮事件
        AbstractListView() { }//私有化无参数构造函数

        string name;
        bool showing = false;
        List<T> trueListRef;//真正的列表引用
        List<ItemCache<T>>[] itemCacheLists = new List<ItemCache<T>>[NumList];//缓存列表数组
        int activeList;//正活跃的缓冲列表在数组中的索引
        public AbstractListView(List<T> source, string listName)
        {
            name = listName;
            trueListRef = source;//初始化真正列表引用
            for (int i = 0; i < NumList; i++)//遍历缓存列表数组
            {
                itemCacheLists[i] = new List<ItemCache<T>>(source.Count); //初始化每一个元素缓存列表的长度=真实列表的长度
                foreach (var elem in source)//遍历真实列表进行浅复制
                {
                    itemCacheLists[i].Add(new ItemCache<T>(elem));//根据每个元素的引用构造缓存元素
                }
            }
        }
        public void _DoLayout()//二代layout 运用缓存数组
        {
            string space = new string(' ', MaxWidth - name.Length);
            showing = EditorGUILayout.Foldout(showing, name + space + "Size:    " + trueListRef.Count);
            if (showing) Show();//展示
        }
        void Show()
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < itemCacheLists[activeList].Count; i++)//遍历展示活跃的列表
            {
                if (!itemCacheLists[activeList][i].ShowItem())
                {
                    trueListRef.RemoveAt(i); itemCacheLists[(activeList + 1) % NumList].RemoveAt(i);
                }//如果元素展示失败，移除相应位置上的元素
            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            //添加元素按钮绘制
            onDrawAddButtton?.Invoke();
            DefaultDrawAddButton();//绘制默认按钮
                                   //刷新缓冲区
            activeList = (activeList + 1) % NumList;//active指向修改后的最新的列表
                                                    //更新最后修改的列表
            itemCacheLists[(activeList + 1) % NumList].Clear();
            foreach (var elem in itemCacheLists[activeList])//遍历修改后最新列表
            {
                itemCacheLists[(activeList + 1) % NumList].Add(elem);
            }
        }
        void DefaultDrawAddButton()
        {
            //默认构造按钮控制元素的添加
            if (EditorGUILayout.DropdownButton(new GUIContent("Add Dele"), FocusType.Passive))
            {
                T newElem = addEvent.Invoke();
                if (newElem != null)
                {
                    trueListRef.Add(newElem);
                    itemCacheLists[(activeList + 1) % NumList].Add(new ItemCache<T>(newElem));
                }
            }
        }
    }
}