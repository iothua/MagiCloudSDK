
using UnityEngine;

namespace Chemistry.Help
{
    public static class EquipmentUtility
    {
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static Transform CreateNode(Transform node,string nodeName)
        {
            Transform nodeItem = GetNode(node,nodeName);

            if (nodeItem == null)
            {
                nodeItem = new GameObject(nodeName).transform;
                nodeItem.SetParent(node);
                nodeItem.localPosition = Vector3.zero;
                nodeItem.localRotation = Quaternion.identity;
                nodeItem.localScale = Vector3.one;
            }

            return nodeItem;
        }

        /// <summary>
        /// 查找节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static Transform GetNode(Transform node,string nodeName)
        {
            return node.Find(nodeName);
        }

        /// <summary>
        /// 获取到选中的Trnsform
        /// </summary>
        /// <returns></returns>
        public static Transform GetSeleteTransform()
        {
            Transform[] selectedObject = null;
#if UNITY_EDITOR
            selectedObject=UnityEditor.Selection.GetTransforms(UnityEditor.SelectionMode.TopLevel | UnityEditor.SelectionMode.ExcludePrefab);
#endif
            return selectedObject.Length == 0 ? null : selectedObject[0];
        }

        /// <summary>
        /// 创建控件
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="target"></param>
        public static GameObject CreateObject(Transform parent,GameObject target)
        {
            if (target == null) return null;
            GameObject newObject = parent == null ? GameObject.Instantiate(target) : GameObject.Instantiate(target,parent);

            newObject.name = target.name;
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = target;
#endif
            return newObject;
        }
    }
}
