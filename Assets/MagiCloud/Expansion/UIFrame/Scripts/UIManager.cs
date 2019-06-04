using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using MagiCloud.Core.UI;

namespace MagiCloud.UIFrame
{
    /*
    思路
        1、UILoading ： 只能看不能操作
        2、一级目录 ：单手操作
        3、二级目录 ：单手操作/双手操作
        4、三级目录 ：双手操作，自动生成控件
        5、新手指引 ：提示加可操作手势
        6、设置界面 ：支持鼠标点击，禁止手势操作
        7、提示界面 ：支持鼠标点击，手势操作
        8、注册界面 ：注册鼠标操作
         
        1）有一个标记脚本，用于标志位于第几阶
        2）判断UI属于Sprite/Canvas。然后会生成在不同的区域内
        3）每一阶都会有不同的操作
        4）获取整体UI布局，以及每一层的级别
        5）同阶中深度排序
        6）不同阶中也有默认深度
        7）摄像机管理
    */


    /// <summary>
    /// UI管理
    /// </summary>
    public class UIManager :MonoBehaviour
    {

        public Stack<UI_Base> mainInterfaces = new Stack<UI_Base>();

        public List<UI_Base> UIs;

        private LinkedList<UI_Base> canvasUIs = new LinkedList<UI_Base>();
        private LinkedList<UI_Base> spriteUIs = new LinkedList<UI_Base>();

        //设置默认值
        public string defaultStr;//默认值

        private static UIManager instance;

        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<UIManager>();

                return instance;
            }
        }



        /// <summary>
        /// UI动作管理端
        /// </summary>
        public UIActionManager actionManager = new UIActionManager();

        void Awake()
        {
            if (actionManager == null)
                actionManager = new UIActionManager();

            //初始化目前所有的UI界面
            UI_Base[] Bases = gameObject.GetComponentsInChildren<UI_Base>();

            //添加到UI中
            foreach (var item in Bases)
            {
                AddUI(item);

                if (item.TagID.Equals(defaultStr))
                {
                    item.OnOpen();

                    CurrentUI = item;
                }
                else
                {
                    item.OnClose();
                }
            }
        }
        private void Update()
        {
            if (actionManager != null)
                actionManager.OnExcute();
        }

        private void OnDestroy()
        {
            UIs.Clear();
            mainInterfaces.Clear();
        }

        /// <summary>
        /// 当前UI
        /// </summary>
        public UI_Base CurrentUI
        {
            get; set;
        }

        private ICanvas canvas;
        private ISpriteRenderer spriteRenderer;

        public ICanvas Canvas
        {
            get
            {
                if (canvas == null)
                    canvas = gameObject.GetComponentInChildren<ICanvas>();

                return canvas;
            }
        }

        public ISpriteRenderer SpriteRenderer
        {
            get
            {
                if (spriteRenderer == null)
                    spriteRenderer = gameObject.GetComponentInChildren<ISpriteRenderer>();

                return spriteRenderer;
            }
        }


        /// <summary>
        /// 添加UI
        /// </summary>
        /// <param name="ui"></param>
        public void AddUI(UI_Base ui)
        {

            if (UIs == null)
                UIs = new List<UI_Base>();

            if (!IsContains(ui))
                UIs.Add(ui);
            //ui.OnInitialize();//初始化
            switch (ui.type)
            {
                case UIType.SpriteRender:
                    AddSpriteUI(ui);
                    break;
                case UIType.Canvas:
                    AddCanvasUI(ui);
                    break;
                default:
                    break;
            }
            if (ui.IsShow)
            {
                ui.OnOpen();
            }
            else
            {
                ui.OnClose();//默认关闭处理
            }
        }
        /// <summary>
        /// 添加到Canvas下
        /// </summary>
        /// <param name="ui"></param>
        private void AddCanvasUI(UI_Base ui)
        {
            if (canvasUIs.Contains(ui)) return;
            ui.transform.SetParent(Canvas.transform);
            LinkedListNode<UI_Base> cur = canvasUIs.First;
            while (cur!=null)
            {
                if (ui.Priority>cur.Value.Priority)
                    break;
                cur=cur.Next;
            }
            if (cur!=null)
            {
                canvasUIs.AddBefore(cur,ui);
                ui.transform.SetSiblingIndex(cur.Value.transform.GetSiblingIndex()+1);
            }
            else
            {
                canvasUIs.AddLast(ui);
                ui.transform.SetAsFirstSibling();
            }

        }


        /// <summary>
        /// 添加到Sprite下
        /// </summary>
        /// <param name="ui"></param>
        private void AddSpriteUI(UI_Base ui)
        {
            if (!spriteUIs.Contains(ui))
            {
                LinkedListNode<UI_Base> cur = spriteUIs.First;
                while (cur!=null)
                {
                    if (ui.Priority>cur.Value.Priority)
                        break;
                    cur=cur.Next;
                }
                if (cur!=null)
                    spriteUIs.AddBefore(cur,ui);
                else
                    spriteUIs.AddLast(ui);
            }
        }

        /// <summary>
        /// 添加UI
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="position"></param>
        public void AddUI(UI_Base ui,Vector3 position)
        {
            if (ui.type == UIType.Canvas)
            {
                ui.transform.SetParent(Canvas.transform);
            }
            else
            {
                ui.transform.SetParent(SpriteRenderer.transform);
            }

            ui.transform.localPosition = position;
            ui.transform.localScale = Vector3.one;

            AddUI(ui);
        }

        public bool IsContains(string TagID)
        {
            if (UIs == null)
            {
                Debug.LogError("UIMnagaer UIs字典为Null，执行RemoveUI失败");
                return false;
            }

            var ui = UIs.Find(obj => obj.TagID.Equals(TagID));

            return ui == null ? false : true;
        }

        public bool IsContains(UI_Base Base)
        {
            return UIs.Contains(Base);
        }

        /// <summary>
        /// 移除UI对象
        /// </summary>
        /// <param name="TagID"></param>
        public UI_Base RemoveUI(string TagID)
        {
            if (UIs == null)
            {
                //Debug.LogError("UIMnagaer UIs字典为Null，执行RemoveUI失败");
                return null;
            }

            UIs = UIs.FindAll(obj => obj != null);
            var ui = UIs.Find(obj => obj.TagID.Equals(TagID));

            if (ui == null)
            {
                //Debug.LogError("UIs字典中不存在此UI，移除失败");
                return null;
            }

            UIs.Remove(ui);

            return ui;
        }

        /// <summary>
        /// 移除UI对象
        /// </summary>
        /// <param name="ui"></param>
        public void RemoveUI(UI_Base ui)
        {
            if (ui == null)
                return;

            RemoveUI(ui.TagID);
        }

        /// <summary>
        /// 获取到指定名称的UI对象
        /// </summary>
        /// <param name="TagID"></param>
        public UI_Base GetUI(string TagID)
        {
            var ui = UIs.Find(obj => obj.TagID.Equals(TagID));

            return ui;
        }

        public UI_Base GetUI(UI_Base @base)
        {
            var ui = UIs.Find(obj => obj.Equals(@base));

            return ui;
        }

        /// <summary>
        /// 对UI进行排序
        /// </summary>
        public void SortUI()
        {

        }

        /// <summary>
        /// 设置当前UI
        /// </summary>
        /// <param name="TagID"></param>
        public void SetUI(string TagID)
        {
            UI_Base ui = GetUI(TagID);

            SetUI(ui);
        }

        /// <summary>
        /// 设置UI
        /// </summary>
        /// <param name="ui"></param>
        public void SetUI(UI_Base ui)
        {
            if (CurrentUI == ui) return;

            if (CurrentUI != null)
                CurrentUI.OnClose();

            CurrentUI = ui;
        }

        /// <summary>
        /// 添加UI界面到栈中
        /// </summary>
        /// <param name="mainInterface"></param>
        public static void AddUIToStack(UI_Base mainInterface)
        {
            Instance.mainInterfaces.Push(mainInterface);
        }

        /// <summary>
        /// 将当前UI添加到栈顶中
        /// </summary>
        public static void AddCurrentUIToStack()
        {
            Instance.mainInterfaces.Push(Instance.CurrentUI);
        }

        /// <summary>
        /// 获取栈顶中的值
        /// </summary>
        /// <returns></returns>
        public static UI_Base GetUIStackPop()
        {
            if (Instance.mainInterfaces.Count == 0) return null;

            return Instance.mainInterfaces.Pop();
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        /// <param name="TagID"></param>
        public void HideUI(string TagID)
        {
            UI_Base ui = GetUI(TagID);
            ui.OnClose();
        }

        /// <summary>
        /// 隐藏当前UI
        /// </summary>
        public void HideUI()
        {
            if (CurrentUI == null) return;
            CurrentUI.OnClose();
        }

        /// <summary>
        /// 获取UI组件集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetUIComponents<T>()
            where T : UI_Base
        {
            List<UI_Base> t = UIs.FindAll(obj => obj is T);

            List<T> ts = new List<T>();

            foreach (var item in t)
            {
                ts.Add((T)item);
            }

            return ts;
        }

        /// <summary>
        /// 根据TagID,获取UI集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public List<T> GetUIComponents<T>(string tagID)
            where T : UI_Base
        {
            List<T> ts = GetUIComponents<T>();

            return ts.FindAll(obj => obj.TagID.Equals(tagID));
        }

        /// <summary>
        /// 根据TagID获取到UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public T GetUIComponent<T>(string tagID)
            where T : UI_Base
        {
            var ts = GetUIComponents<T>(tagID);

            if (ts.Count == 0)
                return default(T);
            else
            {
                return ts[0];
            }
        }

        /// <summary>
        /// 根据类型获取UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUIComponent<T>()
            where T : UI_Base
        {
            var ts = GetUIComponents<T>();

            if (ts.Count == 0)
                return default(T);
            else
            {
                return ts[0];
            }
        }

        public static void SetUIShow(string TagID)
        {
            UI_Base ui = Instance.GetUIComponent<UI_Base>(TagID);
            if (ui == null) return;

            ui.OnOpen();
        }

        public static void SetUIHide(string TagID)
        {
            UI_Base ui = Instance.GetUIComponent<UI_Base>(TagID);
            if (ui == null) return;
            ui.OnClose();
        }

        public static void AddAction(UI_Action ui,Action action,Action start = default(Action),Action end = default(Action))
        {
            Instance.actionManager.AddAction(ui,action,start,end);
        }
    }
}

