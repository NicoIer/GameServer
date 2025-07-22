using Sirenix.OdinInspector;
using UnityEngine;

namespace Soccer
{
    public class MixamoBonesModify : MonoBehaviour
    {
        public string originName = "mixamorig6";
        public string targetName = "mixamorig";

        [Button]
        private void ModifyBones()
        {
            // 所有子物体中含有 originName 的物体 替换 originName 为 targetName
            Modify(transform);
        }

        private void Modify(Transform transform)
        {
            // 所有子物体中含有 originName 的物体 替换 originName 为 targetName
            foreach (Transform child in transform)
            {
                if (child.name.Contains(originName))
                {
                    string newName = child.name.Replace(originName, targetName);
                    child.name = newName;
                    Debug.Log($"Modified {child.name} to {newName}");
                }

                if (!child.name.Contains(targetName))
                {
                    //加前缀
                    child.name = targetName + ":" + child.name;
                }

                Modify(child);
            }
        }
    }
}