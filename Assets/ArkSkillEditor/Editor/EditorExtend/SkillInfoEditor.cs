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
        SkillInfo skillInfo;//ֱ��ʹ��Ŀ������ñ༭skillScript(δ�����ܻ��޸�)

        //---------------startDeles---------------
        AbstractListView<BDele> startDeleListView;//���ڻ���startDeles�б�
        int index1 = 0;
        //----------stopDeles-----------
        AbstractListView<BDele> stopDeleListView;//���ڻ���stopDeles�б�
        int index = 0;
        //--------------exitDeles------------------
        AbstractListView<BDele> exitDeleListView;//���ڻ���exitDeles�б�
        int index2 = 0;
        //---------------timePoints------------------------
        bool showTimePoints = false;
        ReorderableList reorderable;
        //---------------timePoints------------------------
        AbstractListView<BSkill> bSkillListView;//���ڻ���bSkills�б�
        int index3 = 0;
        private void OnEnable()
        {
            skillInfo = target as SkillInfo;
            string path = AssetDatabase.GetAssetPath(skillInfo);//��ȡSkillInfo��·��
            //startDeles
            startDeleListView = new AbstractListView<BDele>(skillInfo.startDeles, "Start Deles");
            startDeleListView.addEvent += () =>//Ԫ������¼�
            {
                BDele newDele = null;
                if (index1 <DeriveClassHandler<BDele>.DeriveScripts.Count) newDele = CreateInstance(DeriveClassHandler<BDele>.DeriveScripts[index1].GetClass()) as BDele;
                //newDele.hideFlags = HideFlags.HideInHierarchy;//����Ⱦ���������
                AssetDatabase.AddObjectToAsset(newDele, path);
                AssetDatabase.ImportAsset(path);
                return newDele;
            };
            startDeleListView.onDrawAddButtton += () =>
            {
                EditorGUILayout.LabelField("ѡ����Ҫ��ӵ�Dele����");
                index1 = EditorGUILayout.Popup(index1, DeriveClassHandler<BDele>.DeriveScriptsOptions);
            };//Ԫ����Ӱ�ť����
            //stopDeles
                stopDeleListView = new AbstractListView<BDele>(skillInfo.stopDeles, "Stop Deles");
                stopDeleListView.addEvent += () =>
                {
                    BDele newDele = null;
                    if (index < DeriveClassHandler<BDele>.DeriveScripts.Count) newDele = CreateInstance(DeriveClassHandler<BDele>.DeriveScripts[index].GetClass()) as BDele;
                //newDele.hideFlags = HideFlags.HideInHierarchy;//����Ⱦ���������
                AssetDatabase.AddObjectToAsset(newDele, path);
                    AssetDatabase.ImportAsset(path);
                    return newDele;
                };
                stopDeleListView.onDrawAddButtton += () => { EditorGUILayout.LabelField("ѡ����Ҫ��ӵ�Dele����"); index = EditorGUILayout.Popup(index, DeriveClassHandler<BDele>.DeriveScriptsOptions); };//Ԫ����Ӱ�ť����
            
                                                                                                                                            
            //exitDeles
            exitDeleListView = new AbstractListView<BDele>(skillInfo.exitDeles, "Exit Deles");
            exitDeleListView.addEvent += () =>
            {
                BDele newDele = null;
                if (index2 < DeriveClassHandler<BDele>.DeriveScripts.Count) newDele = CreateInstance(DeriveClassHandler<BDele>.DeriveScripts[index2].GetClass()) as BDele;
                //newDele.hideFlags = HideFlags.HideInHierarchy;//����Ⱦ���������
                AssetDatabase.AddObjectToAsset(newDele, path);
                AssetDatabase.ImportAsset(path);
                return newDele;
            };
            exitDeleListView.onDrawAddButtton += () => { EditorGUILayout.LabelField("ѡ����Ҫ��ӵ�Dele����"); index2 = EditorGUILayout.Popup(index2, DeriveClassHandler<BDele>.DeriveScriptsOptions); };//Ԫ����Ӱ�ť����
            //bSkills
            bSkillListView = new AbstractListView<BSkill>(skillInfo.bSkills, "Skill Behaviors");
            bSkillListView.addEvent += () =>
              {
                  BSkill newSkill = null;
                  if (index3 < DeriveClassHandler<BSkill>.DeriveScripts.Count) newSkill = CreateInstance(DeriveClassHandler<BSkill>.DeriveScripts[index3].GetClass()) as BSkill;
                  //newDele.hideFlags = HideFlags.HideInHierarchy;//����Ⱦ���������
                  AssetDatabase.AddObjectToAsset(newSkill, path);
                  AssetDatabase.ImportAsset(path);
                  return newSkill;
              };
            bSkillListView.onDrawAddButtton += () =>
            {
                EditorGUILayout.LabelField("ѡ����Ҫ��ӵ�Skill Behavior����");
                index3 = EditorGUILayout.Popup(index3, DeriveClassHandler<BSkill>.DeriveScriptsOptions);
            };//Ԫ����Ӱ�ť����

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
        #region ������BDele������MonoScript�Ļ�ȡ
        
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
            /// �̳���T����Ľű�����(�������ӳٳ�ʼ��)
            /// </summary>
            static List<MonoScript> deriveScripts;
            /// <summary>
            /// ���ؼ̳���T����Ľű�
            /// </summary>
            /// <returns></returns>
            static List<MonoScript> GetDeriveScripts()
            {
                var monoScripts = new List<MonoScript>();
                foreach (var script in MonoImporter.GetAllRuntimeMonoScripts())
                {
                    var type = script.GetClass();
                    if (type == null) continue;//����������ļ���
                    var baseType = type.BaseType;  //��ȡ����
                    while (baseType != null)  //��ȡ���л���
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
        //�����ڲ�����
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
                currItemEditor = Editor.CreateEditor(newItem);//ΪԪ�ش���Editor
                showing = true;
            }
            public bool ShowItem()
            {
                if (item != null && currItemEditor != null)
                {
                    if (showing = EditorGUILayout.InspectorTitlebar(showing, item)) currItemEditor.OnInspectorGUI();
                    return true;
                }
                else if (currItemEditor != null) ScriptableObject.DestroyImmediate(currItemEditor);//���itemΪ�գ�������Editor
                return false;
            }
        }
        const int MaxWidth = 65;
        const int NumList = 2;//˫����
        public event AddDele addEvent;
        public event DrawAddButton onDrawAddButtton;//������Ӱ�ť�¼�
        AbstractListView() { }//˽�л��޲������캯��

        string name;
        bool showing = false;
        List<T> trueListRef;//�������б�����
        List<ItemCache<T>>[] itemCacheLists = new List<ItemCache<T>>[NumList];//�����б�����
        int activeList;//����Ծ�Ļ����б��������е�����
        public AbstractListView(List<T> source, string listName)
        {
            name = listName;
            trueListRef = source;//��ʼ�������б�����
            for (int i = 0; i < NumList; i++)//���������б�����
            {
                itemCacheLists[i] = new List<ItemCache<T>>(source.Count); //��ʼ��ÿһ��Ԫ�ػ����б�ĳ���=��ʵ�б�ĳ���
                foreach (var elem in source)//������ʵ�б����ǳ����
                {
                    itemCacheLists[i].Add(new ItemCache<T>(elem));//����ÿ��Ԫ�ص����ù��컺��Ԫ��
                }
            }
        }
        public void _DoLayout()//����layout ���û�������
        {
            string space = new string(' ', MaxWidth - name.Length);
            showing = EditorGUILayout.Foldout(showing, name + space + "Size:    " + trueListRef.Count);
            if (showing) Show();//չʾ
        }
        void Show()
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < itemCacheLists[activeList].Count; i++)//����չʾ��Ծ���б�
            {
                if (!itemCacheLists[activeList][i].ShowItem())
                {
                    trueListRef.RemoveAt(i); itemCacheLists[(activeList + 1) % NumList].RemoveAt(i);
                }//���Ԫ��չʾʧ�ܣ��Ƴ���Ӧλ���ϵ�Ԫ��
            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            //���Ԫ�ذ�ť����
            onDrawAddButtton?.Invoke();
            DefaultDrawAddButton();//����Ĭ�ϰ�ť
                                   //ˢ�»�����
            activeList = (activeList + 1) % NumList;//activeָ���޸ĺ�����µ��б�
                                                    //��������޸ĵ��б�
            itemCacheLists[(activeList + 1) % NumList].Clear();
            foreach (var elem in itemCacheLists[activeList])//�����޸ĺ������б�
            {
                itemCacheLists[(activeList + 1) % NumList].Add(elem);
            }
        }
        void DefaultDrawAddButton()
        {
            //Ĭ�Ϲ��찴ť����Ԫ�ص����
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