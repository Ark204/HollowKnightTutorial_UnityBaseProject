using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ArkSkill.Core
{
    [System.Serializable]
    public abstract class BDele : ScriptableObject,IDeleInstance,IInfo<IDeleInstance>//:Object
    {
        public IEnumerator CreatInvokeIEnumerator(SkillManager skillManager, float effectTime, SkillInfo skillInfo)
        {
            yield return new WaitForSecondsRealtime(effectTime);
            Invoke(skillManager,skillInfo);
        }
        public virtual void OnStart(SkillManager skillManager,SkillInfo skillInfo) { }
        public virtual void Invoke(SkillManager skillManager,SkillInfo skillInfo) { }
        public virtual IDeleInstance GetInstance() { return this; }//默认未单列模式
        public virtual void OnDraw(Vector3 position, float direct) { }
        [ContextMenu("Remove")]
        public void Remove()
        {
            string path = AssetDatabase.GetAssetPath(this);
            DestroyImmediate(this, true);
            AssetDatabase.ImportAsset(path);
        }
    }
}
