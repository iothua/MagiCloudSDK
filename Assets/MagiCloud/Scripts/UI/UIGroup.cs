using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.UISystem
{
    /// <summary>
    /// UI组
    /// </summary>
    public class UIGroup :MonoBehaviour, IUIGroup
    {
        private LinkedList<UIBase> uis = new LinkedList<UIBase>();        //所有UI缓存
        private LinkedList<IUIBase> openUIs = new LinkedList<IUIBase>();
        private int depth;
        public EUIType uIType = EUIType.Canvas;
        /// <summary>
        /// 获取或设置可用属性
        /// </summary>
        private bool pause;

        public string Name => name;
        public int Depth
        {
            get { return depth; }
            set
            {
                if (depth==value) return;
                depth=value;
                UISystem.Instance.SetDepth(depth);
                Refresh();
            }
        }

        public bool Pause
        {
            get { return pause; }
            set
            {
                if (pause==value) return;
                pause=value;
                Refresh();
            }
        }
        public int UICount => uis.Count;
        public IUIBase CurrentUI { get { return uis.First!=null ? uis.First.Value : null; } }
        public RectTransform GetRectTransform => GetComponent<RectTransform>();
        public EUIType UIType => uIType;

        private void Start()
        {

            GetRectTransform.anchorMin=Vector2.zero;
            GetRectTransform.anchorMax=Vector2.one;
            GetRectTransform.anchoredPosition=Vector2.zero;
            GetRectTransform.sizeDelta=Vector2.zero;
        }

        public void Refresh()
        {
            LinkedListNode<IUIBase> cur = openUIs.First;
            bool isPause = pause;
            bool cover = false;
            int i = UICount;
            while (cur!=null)
            {
                LinkedListNode<IUIBase> next = cur.Next;
                cur.Value.OnDepthChanged(Depth,i--);
                if (isPause)
                {
                    //不可用时，设置UI遮挡，并禁用UI
                    OnCovered(cur.Value);
                    OnPause(cur.Value);
                }
                else
                {
                    //可用时,取消暂停
                    OnResume(cur.Value);
                    //暂停被覆盖的UI
                    if (cur.Value.PauseCoveredUI)
                    {
                        isPause=true; ;
                    }
                    if (cover)
                    {
                        //覆盖
                        OnCovered(cur.Value);
                    }
                    else
                    {
                        //取消覆盖
                        OnReveal(cur.Value);
                        cover=true;
                    }
                }
                cur=next;
            }
        }


        /// <summary>
        /// 添加ui
        /// </summary>
        /// <param name="ui"></param>
        public void AddUI(IUIBase ui)
        {
            UIBase temp = ui as UIBase;
            Transform node = temp.transform;
            node.SetParent(transform);
            node.localScale=Vector3.one;
            uis.AddFirst(temp);
            Refresh();
        }

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IUIBase GetUI(string name)
        {
            foreach (var item in uis)
            {
                if (item.Name==name)
                    return item;
            }
            return null;
        }
        public IUIBase GetUI(int id)
        {
            foreach (var item in uis)
            {
                if (item.ID==id)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// 获取所有UI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IUIBase[] GetAllUIs()
        {
            List<IUIBase> temp = new List<IUIBase>();
            foreach (var item in uis)
            {
                temp.Add(item);
            }
            return temp.ToArray();
        }

        /// <summary>
        /// 获取所有同名UI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IUIBase[] GetUIs(string name)
        {
            List<IUIBase> temp = new List<IUIBase>();
            foreach (var item in uis)
            {
                if (item.Name==name)
                    temp.Add(item);
            }
            return temp.ToArray();
        }

        /// <summary>
        /// 是否存在该UI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasUI(string name)
        {
            foreach (var item in uis)
            {
                if (item.Name==name) return true;
            }
            return false;
        }
        public bool HasUI(int id)
        {
            foreach (var item in uis)
            {
                if (item.ID==id) return true;
            }
            return false;
        }
        public void OnUpdate()
        {
            LinkedListNode<UIBase> cur = uis.First;
            while (cur!=null)
            {
                if (cur.Value.Pause) break;
                LinkedListNode<UIBase> next = cur.Next;
                cur.Value.OnUpdate();
                cur=next;
            }
        }
        /// <summary>
        /// 移除UI
        /// </summary>
        /// <param name="name"></param>
        public void RemoveUI(string name)
        {
            RemoveUI(GetUI(name));
        }

        public void RemoveUI(IUIBase ui)
        {
            if (ui==null)
            {
                throw new ArgumentNullException(nameof(ui));
            }
            UIBase temp = ui as UIBase;
            OnCovered(temp);
            OnPause(temp);
            uis.Remove(temp);
        }



        private void OnCovered(IUIBase ui)
        {
            if (!ui.IsCovered)
            {
                ui.IsCovered=true;
                ui.OnCovered();
            }
        }
        private void OnReveal(IUIBase ui)
        {
            if (ui.IsCovered)
            {
                ui.IsCovered=false;
                ui.OnReveal();
            }
        }
        private void OnPause(IUIBase ui)
        {
            if (!ui.Pause)
            {
                ui.Pause=true;
                ui.OnDisableUI();
            }
        }
        private void OnResume(IUIBase ui)
        {
            if (ui.Pause)
            {
                ui.Pause=false;
                ui.OnEnableUI();
            }
        }
        public void OnOpen(IUIBase ui)
        {
            if (openUIs.Contains(ui))
                openUIs.Remove(ui);
            openUIs.AddFirst(ui);
            ui.OnOpen();
            Refresh();

        }
        public void OnClose(IUIBase ui)
        {

            if (openUIs.Contains(ui))
            {
                OnCovered(ui);
                OnPause(ui);
                openUIs.Remove(ui);
                ui.OnClose();
                Refresh();
            }
        }
    }
}
