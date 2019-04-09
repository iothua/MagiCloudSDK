using MagiCloud.KGUI;
using MagiCloud.NetWorks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.UISystem
{
    /// <summary>
    /// UI系统，所有的UI操作入口
    /// </summary>
    public class UISystem :MonoBehaviourSingleton<UISystem>
    {
        public const int DEPTHFACTOR = 10000;
        private Dictionary<string,UIGroup> uiGroups = new Dictionary<string,UIGroup>();
        private int depth = 0;
        private int idIndex = 0;              //分配ID计数
        private Canvas canvas;
        public Transform spriteNode;

        public Transform CanvasNode => UICanvas.transform;      //放置UI的节点
        public Transform SpriteNode => spriteNode;
        public Canvas UICanvas
        {
            get
            {
                if (canvas == null)
                    canvas = gameObject.GetComponentInChildren<Canvas>();

                return canvas;
            }
        }

        #region Unity生命周期
        private void Start()
        {
            UICanvas.overrideSorting=true;
            UICanvas.sortingOrder=DEPTHFACTOR*depth;
        }

        private void Update()
        {
            foreach (var item in uiGroups)
            {
                item.Value.OnUpdate();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveAllUIs();
            uiGroups.Clear();
        }

        private void RemoveAllUIs()
        {
            IUIBase[] uis = GetAllUIs();
            foreach (var item in uis)
            {
                if (!HasUI(item.Name)) continue;
                RemoveUI(item.Name);
            }
        }
        #endregion
        public void SetDepth(int depth)
        {
            this.depth=depth;
            UICanvas.overrideSorting=true;
            UICanvas.sortingOrder=DEPTHFACTOR*depth;
        }

        #region UIGroup
        /// <summary>
        /// 是否存在UI组
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasUIGroup(string name)
        {
            return uiGroups.ContainsKey(name);
        }
        /// <summary>
        /// 获取UI组
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IUIGroup GetGroup(string name)
        {
            UIGroup group = null;
            if (uiGroups.TryGetValue(name,out group))
            {
                return group;
            }
            return null;
        }
        /// <summary>
        /// 获取所有UI组
        /// </summary>
        /// <returns></returns>
        public IUIGroup[] GetAllUIGroups()
        {
            int i = 0;
            IUIGroup[] groups = new IUIGroup[uiGroups.Count];
            foreach (var item in uiGroups)
            {
                groups[i++]=item.Value;
            }
            return groups;
        }
        /// <summary>
        /// 获取所有UI组
        /// </summary>
        /// <param name="groups"></param>
        public void GetAllUIGroups(List<IUIGroup> groups)
        {
            if (groups==null)
            {
                throw new ArgumentNullException(nameof(groups));
            }
            groups.Clear();
            foreach (var item in uiGroups)
            {
                groups.Add(item.Value);
            }
        }
        /// <summary>
        /// 添加UI组
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool AddGroup(IUIGroup group)
        {
            return AddGroup(group,0);
        }
        /// <summary>
        /// 添加UI组
        /// </summary>
        /// <param name="group"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public bool AddGroup(IUIGroup group,int depth)
        {
            if (HasUIGroup(group.Name)) return false;
            UIGroup temp = (UIGroup)group;
            Transform node = temp.transform;
            switch (group.UIType)
            {
                case EUIType.Canvas:
                    node.SetParent(CanvasNode);
                    break;
                case EUIType.Sprite:
                    node.SetParent(SpriteNode);
                    break;
                default:
                    break;
            }
            //UI组节点重置
            node.localPosition=Vector3.zero;
            node.localScale=Vector3.one;
            temp.Depth=depth;
            uiGroups.Add(group.Name,temp);
            return true;
        }



        #endregion


        #region UIBase
        /// <summary>
        /// 添加UI
        /// </summary>
        /// <param name="uiBase"></param>
        /// <param name="groupName"></param>
        /// <param name="pauseCoveredUI"></param>
        public void AddUI(IUIBase uiBase,string groupName,bool pauseCoveredUI = false)
        {
            UIGroup group = GetGroup(groupName) as UIGroup;
            if (group==null)
            {
                throw new ArgumentNullException(nameof(group));
            }
            int id = idIndex++;
            uiBase.OnInit(id,group,pauseCoveredUI);
            group.AddUI(uiBase);
        }
        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IUIBase GetUI(string name)
        {
            foreach (var item in uiGroups)
            {
                IUIBase ui = item.Value.GetUI(name);
                if (ui!=null) return ui;
            }
            return null;
        }
        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IUIBase GetUI(int id)
        {
            foreach (var item in uiGroups)
            {
                IUIBase ui = item.Value.GetUI(id);
                if (ui!=null) return ui;
            }
            return null;
        }
        /// <summary>
        /// 获取所有同名UI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IUIBase[] GetUIs(string name)
        {
            List<IUIBase> uis = new List<IUIBase>();
            foreach (var item in uiGroups)
            {
                uis.AddRange(item.Value.GetUIs(name));
            }
            return uis.ToArray();
        }

        public IUIBase[] GetAllUIs()
        {
            List<IUIBase> uis = new List<IUIBase>();
            foreach (var item in uiGroups)
            {
                uis.AddRange(item.Value.GetAllUIs());
            }
            return uis.ToArray();
        }
        /// <summary>
        /// 是否存在UI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasUI(string name)
        {
            foreach (var item in uiGroups)
            {
                if (item.Value.HasUI(name))
                    return true;
            }
            return false;
        }

        public bool HasUI(int id)
        {
            foreach (var item in uiGroups)
            {
                if (item.Value.HasUI(id))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 移除UI
        /// </summary>
        /// <param name="name"></param>
        public void RemoveUI(string name)
        {
            foreach (var item in uiGroups)
            {
                IUIBase ui = item.Value.GetUI(name);
                if (ui!=null)
                    item.Value.RemoveUI(ui);
            }
        }
        public void RemoveUI(int id)
        {
            foreach (var item in uiGroups)
            {
                IUIBase ui = item.Value.GetUI(id);
                if (ui!=null)
                    item.Value.RemoveUI(ui);
            }
        }
        #endregion

        /// <summary>
        /// 显示UI
        /// </summary>
        public void OnOpen(string name)
        {
            foreach (var item in uiGroups)
            {
                IUIBase ui = item.Value.GetUI(name);
                if (ui!=null)
                    item.Value.OnOpen(ui);
            }
        }
        /// <summary>
        /// 显示UI
        /// </summary>
        public void OnOpen(int id)
        {
            foreach (var item in uiGroups)
            {
                IUIBase ui = item.Value.GetUI(id);
                if (ui!=null)
                    item.Value.OnOpen(ui);
            }
        }

        /// <summary>
        ///隐藏UI
        /// </summary>
        public void OnClose(string name)
        {
            foreach (var item in uiGroups)
            {
                IUIBase ui = item.Value.GetUI(name);
                if (ui!=null)
                    item.Value.OnClose(ui);
            }
        }
        public void OnClose(int id)
        {
            foreach (var item in uiGroups)
            {
                IUIBase ui = item.Value.GetUI(id);
                if (ui!=null)
                    item.Value.OnClose(ui);
            }
        }


        /// <summary>
        /// 启用UI
        /// </summary>
        public void EnableUI(string name)
        {
            EnableUI(GetUI(name));
        }
        public void EnableUI(int id)
        {
            EnableUI(GetUI(id));
        }

        public void EnableUI(IUIBase uiBase)
        {
            if (uiBase==null)
            {
                throw new System.ArgumentNullException(nameof(uiBase));
            }
            uiBase.OnEnableUI();
        }
        /// <summary>
        /// 禁用UI
        /// </summary>
        public void DisableUI(string name)
        {
            DisableUI(GetUI(name));
        }
        public void DisableUI(int id)
        {
            DisableUI(GetUI(id));
        }
        public void DisableUI(IUIBase uiBase)
        {
            if (uiBase==null)
            {
                throw new System.ArgumentNullException(nameof(uiBase));
            }

            uiBase.OnDisableUI();
        }

    }
}
