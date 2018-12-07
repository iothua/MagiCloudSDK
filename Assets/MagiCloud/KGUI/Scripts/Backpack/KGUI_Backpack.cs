using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.U2D;
using MagiCloud.Core.Events;

namespace MagiCloud.KGUI
{
    
    /// <summary>
    /// KGUI背包
    /// </summary>
    public class KGUI_Backpack : MonoBehaviour
    {
        public Transform content;                           //父对象
        public GameObject backpackItem;                     //物体子项预制体

        [Header("背包区域")]
        public KGUI_Panel areaPanel;                          //背包界面
        //子项对象
        private Dictionary<string, KGUI_BackpackItem> _dicBackpackItems = new Dictionary<string, KGUI_BackpackItem>();

        private bool _isHandOnBag;                          //标记手是否在背包上
        private int _handIndex;                             //标记哪只手在背包

        /// <summary>
        /// 背包图集
        /// </summary>
        public SpriteAtlas backpackIcons;

        /// <summary>
        /// 通用路径
        /// </summary>
        public string universalPath;

        /// <summary>
        /// 背包Json文件名称
        /// </summary>
        public string backpackJsonFileNmae;

        //public TriggerType triggerType; //触发方式

        public KGUI_ButtonCustom backpackTrigger;//背包触发

        //打开坐标、关闭坐标
        public Vector2 openPosition, closePosition;

        /// <summary>
        /// 背包数据配置
        /// </summary>
        [HideInInspector]
        public KGUI_ItemDataConfig dataConfig;

        public bool IsOpen = true; //背包打开状态

        private GameObject currentObject;

        public bool AutoInitialize = true;

        private void Start()
        {
            if (AutoInitialize)
                OnInitialize();
        }

        /// <summary>
        /// 初始化背包
        /// </summary>
        public void OnInitialize()
        {
            DeleteBagItem();

            if (backpackItem == null)
                backpackItem = Resources.Load<GameObject>("Prefabs\\backpackItem");

            string jsonData = Json.JsonHelper.ReadJsonString(Application.streamingAssetsPath + "/Backpack/JsonData/" + backpackJsonFileNmae + ".json");

            if (string.IsNullOrEmpty(jsonData)) return;

            dataConfig = Json.JsonHelper.JsonToObject<KGUI_ItemDataConfig>(jsonData);

            if (dataConfig == null) return;

            areaPanel.onEnter.AddListener(OnEnter);
            areaPanel.onExit.AddListener(OnExit);

            backpackTrigger.onEnter.AddListener(OnEnter);

            CloseBag();

            CreateItems();

            //根据数量设置高度
            RectTransform rectTransform = content.GetComponent<RectTransform>();
            int count = dataConfig.ItemDatas.Count / 2 + dataConfig.ItemDatas.Count % 2;
            GridLayoutGroup gridLayout = rectTransform.GetComponent<GridLayoutGroup>();

            //计算高度
            float height = gridLayout.cellSize.y * count + gridLayout.padding.top + gridLayout.spacing.y * (count - 1);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
            float positionY = -(rectTransform.sizeDelta.y - rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y) / 2;

            //赋予初始位置
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, positionY, rectTransform.localPosition.z);

            

            //kinect事件注册
            EventHandReleaseObject.AddListener( HandIdle, Core.ExecutionPriority.High);
        }

        void DeleteBagItem()
        {
            foreach (var item in _dicBackpackItems)
            {
                //删除各个子项
                item.Value.OnDestroyItems();

                Destroy(item.Value.gameObject);
            }

            _dicBackpackItems.Clear();
        }

        /// <summary>
        /// 背包重置
        /// </summary>
        public void OnReset()
        {
            DeleteBagItem();

            CloseBag();

            CreateItems();
        }

        private void CreateItems()
        {

            foreach (var itemData in dataConfig.ItemDatas)
            {
                var go = Instantiate(backpackItem, content);
                go.SetActive(true);

                var item = go.GetComponent<KGUI_BackpackItem>();

                item.OnInitialized(this, itemData);

                if (!_dicBackpackItems.ContainsKey(itemData.Name))
                    _dicBackpackItems.Add(itemData.Name, item);
            }
        }

        private void OnDestroy()
        {
            EventHandReleaseObject.RemoveListener(HandIdle);
        }

        private void HandIdle(GameObject arg1, int arg2)
        {
            if (_handIndex != arg2) return;

            KGUI_BackPackMark equ = arg1.GetComponent<KGUI_BackPackMark>();

            if (equ == null) return;

            if (!_isHandOnBag)
            {
                //SetObjectFrontUI(true);
                equ.ObjectFrontUI.OnReset();
                //释放时，离开背包的
                _handIndex = -1;

                return;
            }

            _handIndex = -1;

            //不支持组合删除
            if (!equ.IsFeaturesObjectEquals())
            {
                equ.ObjectFrontUI.OnReset();
                return;
            }

            if (!equ.IsCanPutInBag())
            {
                equ.ObjectFrontUI.OnReset();
                return;
            }

            DestroyEquipment(equ);
        }


        /// <summary>
        /// 抓取背包创建
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public GameObject CreatEquipment(KGUI_BackpackItem item, int handIndex)
        {
            KGUI_Backpack_ItemData itemData = item.dataConfig;

            var go = GenerateEquipment(item, itemData.ItemPath);

            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -16.3f);
            KGUI_BackPackMark mark = go.GetComponent<KGUI_BackPackMark>() ?? go.AddComponent<KGUI_BackPackMark>();
            if (mark != null)
            {
                //仪器背包创建调用
                mark.OnCreateForBag(item, itemData.Name, handIndex);
            }

            //KinectTransfer.SetObjectGrab(go, handIndex);
            MOperateManager.SetObjectGrab(go,  handIndex, itemData.zValue);
            this._handIndex = handIndex;

            SetObjectFrontUI(false);

            return go;
        }

        /// <summary>
        /// 从资源中生成仪器
        /// </summary>
        /// <returns>The equipment.</returns>
        /// <param name="item">Item.</param>
        /// <param name="ItemPath">Item path.</param>
        public GameObject GenerateEquipment(KGUI_BackpackItem item,string ItemPath)
        {

            var itemObject = Resources.Load<GameObject>(universalPath + ItemPath);

            var go = Instantiate(itemObject) as GameObject;

            return go;
        }

        /// <summary>
        /// 删除仪器
        /// </summary>
        /// <param name="name"></param>
        public void DestroyEquipment(KGUI_BackPackMark mark)
        {
            mark.OnDestroyForBag();

            KGUI_BackpackItem backpackItem;

            if (_dicBackpackItems.TryGetValue(mark.TagName, out backpackItem))
            {
                //删除物体
                Destroy(mark.Target);
            }
        }

        public void CloseBag()
        {
            if (!IsOpen) return;

            backpackTrigger.OnCustomExit(_handIndex);

            _isHandOnBag = false;

            //_handIndex = -1;

            IsOpen = false;

            //areaPanel.enabled = false;
            areaPanel.transform.DOLocalMove(closePosition, 1.0f);

        }

        public void OpenBag(int handIndex)
        {

            if (IsOpen) return;

            IsOpen = true;

            _isHandOnBag = true;
            _handIndex = handIndex;
            
            var tween = areaPanel.transform.DOLocalMove(openPosition, 1.0f);

        }

        private void OnExit(int arg0)
        {
            CloseBag();

            ////获取到正在抓取的物体
            //SetObjectFrontUI(true);
        }

        private void OnEnter(int arg0)
        {
            //获取到如果抓取的物体不能放回背包，则不做任何处理

            var grabObject = MOperateManager.GetObjectGrab(arg0);

            if (grabObject != null)
            {
                KGUI_BackPackMark equ = grabObject.GetComponent<KGUI_BackPackMark>();
                if (equ == null) return;
                if (!equ.IsFeaturesObjectEquals()) return;
            }

            OpenBag(arg0);
            SetObjectFrontUI(false);
        }

        void SetObjectFrontUI(bool IsReset)
        {
            //获取到正在抓取的物体
            var grabObject = MOperateManager.GetObjectGrab(_handIndex);

            if (grabObject == null) return;

            var frontUI = grabObject.GetComponent<KGUI_ObjectFrontUI>();

            if (frontUI != null)
            {
                if (IsReset)
                    frontUI.OnReset();
                else
                    frontUI.OnSet();
            }
        }


        /*
         1、问题1，打开背包
         2、问题2，抓取物体打开背包
         3、背景触发有问题。
         
         */
    }
}

