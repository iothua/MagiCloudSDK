using MagiCloud.Core.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MagiCloud.KGUI
{

    /*
        1、单击下拉框，弹出下拉选项 
        2、自动排版选项
        3、当点击下拉选项时，自动赋值
        4、当增加到一定值时，自动添加滚动条

        5、自动生成选项
        6、根据选项自动进行滚动条的匹配
        7、按下某个选项回馈到下拉框中


        存在两个问题：
        1、根据数量，自动生成子项。并且计算出容器的大小，以及滚动条的大小。
        2、自动填充滚动条
    */

    /// <summary>
    /// KGUI下拉框
    /// </summary>
    [ExecuteInEditMode]
    public class KGUI_Dropdown :KGUI_ButtonBase
    {
        [Header("是否触碰展开，false为点击展开")]
        public bool isTouchExpand = false;
        //子项名称
        public List<string> Names;

        ////滚动条
        //public KGUI_Slider Scrollbar;

        public KGUI_ScrollView scrollView; //滚动视图

        //模板对象
        public GameObject Template;

        [Serializable]
        public class DropdownEvent :UnityEvent<int>
        {

        }
        public DropdownEvent onChangeValue;
        //显示的内容
        public KGUI_Text textName;

        private KGUI_DropdownItem currentItem;//当前选项

        [HideInInspector]
        public List<KGUI_DropdownItem> Items;//选项集合

        /// <summary>
        /// 组对象
        /// </summary>
        public KGUI_ButtonGroup buttonGroup { get; private set; } //Button组

        //排版对象
        public GridLayoutGroup gridLayout; //
        public GameObject dropdownItem; //子项预知物体

        private int handIndex;

        protected override void Awake()
        {
            base.Awake();
            if (onChangeValue==null) onChangeValue=new DropdownEvent();
            if (dropdownItem == null)
                dropdownItem = Resources.Load<GameObject>("Prefabs\\DropdownItem");
        }

        protected override void Start()
        {
            base.Start();

            if (buttonGroup == null)
                buttonGroup = GetComponent<KGUI_ButtonGroup>() ?? gameObject.AddComponent<KGUI_ButtonGroup>();

            if (gridLayout == null)
                gridLayout = gameObject.GetComponentInChildren<GridLayoutGroup>();

            Template.SetActive(false);

            EventHandIdle.AddListener(OnButtonRelease,Core.ExecutionPriority.High);
            //CreatePanel();
            if (isTouchExpand)
                onEnter.AddListener(OnExpand);
            else
                onClick.AddListener(OnExpand);

            buttonGroup.SetButton(Items[0]);
        }

        /// <summary>
        /// 创建一个容器，用于接收容器事件，从而来处理下拉框的关闭
        /// </summary>
        private void CreatePanel()
        {
            GameObject panelObject = new GameObject("panel");
            RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
            rectTransform.SetParent(transform);
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;

            var parentRect = GetComponent<RectTransform>();

            var templateRect = Template.GetComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(parentRect.sizeDelta.x,parentRect.sizeDelta.y + templateRect.sizeDelta.y);

            rectTransform.localPosition = new Vector3(0,-(rectTransform.sizeDelta.y / 2 - parentRect.sizeDelta.y / 2),0);

        }

        void OnButtonRelease(int handIndex)
        {
            if (!enabled) return;

            //如果当前手的值不等于-1，并且释放时，跟当前手不符，则直接跳过
            if (this.handIndex != -1 && this.handIndex != handIndex) return;

            handIndex = -1;

            GameObject panelObject = new GameObject("panel");
            RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
            rectTransform.SetParent(transform);
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;

            var parentRect = GetComponent<RectTransform>();

            var templateRect = Template.GetComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(parentRect.sizeDelta.x,parentRect.sizeDelta.y + templateRect.sizeDelta.y);

            rectTransform.localPosition = new Vector3(0,-(rectTransform.sizeDelta.y / 2 - parentRect.sizeDelta.y / 2),0);

            if (!MUtility.IsAreaContains(rectTransform,handIndex))
            {
                Template.SetActive(false);
            }

            DestroyImmediate(panelObject);
        }

        public void OnCreateItem(List<string> names)
        {
            Names = names;
            OnCreateItem();
        }

        /// <summary>
        /// 创建子项
        /// </summary>
        public void OnCreateItem()
        {

            //如果值没有发生改变，则直接跳过

            //删除子项
            OnDeleteItem();

            //生成子项
            foreach (var name in Names)
            {
                var go = Instantiate(dropdownItem,gridLayout.transform);
                go.SetActive(true);

                var item = go.GetComponent<KGUI_DropdownItem>();

                item.OnInitialized(this,name);

                Items.Add(item);
            }

            RectTransform content = gridLayout.GetComponent<RectTransform>();

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

            //设置子项的高度
            Vector2 delta = content.sizeDelta;

            int count = Names.Count;

            if (gridLayout.constraintCount>1)
            {
                count = Names.Count / gridLayout.constraintCount + (Names.Count % gridLayout.constraintCount == 0 ? 0 : 1);
            }

            //设置背景框的高度
            content.sizeDelta = new Vector2(delta.x,gridLayout.cellSize.y * count + (count - 1) * gridLayout.spacing.y);

            //因为设置了瞄点，所以需要根据父对象的高度，重新进行Y轴坐标的计算。

            float y = content.parent.GetComponent<RectTransform>().sizeDelta.y / 2 - content.sizeDelta.y / 2;

            content.localPosition = new Vector3(content.localPosition.x,y,content.localPosition.z);

            //设置滚动数值，自动填充
            scrollView.SetRectData();

            if (gridLayout.constraintCount == 1)
                //没有激活的话，则显示全部
                gridLayout.cellSize = new Vector2(gridLayout.GetComponent<RectTransform>().sizeDelta.x,gridLayout.cellSize.y);

            //设置所有子项的大小

            //设置默认参数
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].SetItemData(gridLayout.cellSize);
                //设置默认值
                if (i == 0)
                {
                    Items[i].OnClick(0); //设置被选中状态
                }
            }
        }

        private void Update()
        {

            if (Names.Count == Items.Count)
            {
                bool result = false;

                for (int i = 0; i < Names.Count; i++)
                {
                    if (Names[i] != Items[i].Name)
                    {
                        //如果有一项不相等，则重新生成;
                        result = true;
                        continue;
                    }
                }

                if (!result)
                    return;
            }

            //根据生成的物体，填充子项
            OnCreateItem();
        }

        /// <summary>
        /// 删除子项
        /// </summary>
        private void OnDeleteItem()
        {
            foreach (var item in Items)
            {
                if (item == null) continue;
                buttonGroup.RemoveButton(item);


                DestroyImmediate(item.gameObject);
            }


            Items.Clear();
        }

        /// <summary>
        /// 展开
        /// </summary>
        /// <param name="handIndex"></param>
        public void OnExpand(int handIndex)
        {
            if (!enabled) return;
            //    base.OnClick(handIndex);

            if (this.handIndex != -1 && this.handIndex != handIndex) return;

            this.handIndex = handIndex;

            Template.SetActive(!Template.activeSelf);
        }

        /// <summary>
        /// 设置当前显示值
        /// </summary>
        /// <param name="item"></param>
        public void OnSetDropdownText(KGUI_DropdownItem item)
        {
            currentItem = item;
            textName.Text = item.Name;
            if (onChangeValue!=null)
                onChangeValue.Invoke(Items.IndexOf(item));
            Template.SetActive(false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (isTouchExpand)
                onEnter.RemoveListener(OnExpand);
            else
                onClick.RemoveListener(OnExpand);
        }
    }
}

