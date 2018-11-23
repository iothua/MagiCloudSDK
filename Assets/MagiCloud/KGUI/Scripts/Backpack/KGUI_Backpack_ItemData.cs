using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// 背包子项数据
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class KGUI_Backpack_ItemData
    {
        public int ID;

        //默认纹理、禁用纹理
        public string normalSpritePath, disableSpritePath;

        //数量
        public int number;

        public string ItemPath;

        public string Name; //子项名称

        [Header("物体相对摄像机的位置")]
        public float zValue;//z轴值

        //private GameObject _itemObject;
        //private Sprite _normalSprite;
        //private Sprite _disableSprite;

        //[JsonIgnore]
        //public GameObject ItemObject {
        //    get {
        //        if (_itemObject == null)
        //        {
        //            _itemObject = Resources.Load<GameObject>(ItemPath);
        //        }

        //        return _itemObject;
        //    }
        //}

        //[JsonIgnore]
        //public Sprite NormalSprite {
        //    get {
        //        if (_normalSprite == null)
        //        {
        //            _normalSprite = Resources.Load<Sprite>(normalSpritePath);
        //        }

        //        return _normalSprite;
        //    }
        //}

        //[JsonIgnore]
        //public Sprite DisableSprite {
        //    get {
        //        if (_disableSprite == null)
        //        {
        //            _disableSprite = Resources.Load<Sprite>(disableSpritePath);
        //        }
        //        return _disableSprite;
        //    }
        //}
    }

    /// <summary>
    /// 子项数据配置
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class KGUI_ItemDataConfig
    {
        #region 注释

        //private Sprite _normalIcon;
        //private Sprite _enterIcon;
        //private Sprite _disableIcon;

        //public string normalIconPath;
        //public string enterIconPath;
        //public string disableIconPath;

        #endregion

        public List<KGUI_Backpack_ItemData> ItemDatas;

        //[JsonIgnore]
        //public Sprite normalIcon {
        //    get {

        //        if (_normalIcon == null)
        //        {
        //            _normalIcon = Resources.Load<Sprite>(normalIconPath);
        //        }
        //        return _normalIcon;
        //    }
        //}

        //[JsonIgnore]
        //public Sprite enterIcon {
        //    get {
        //        if (_enterIcon == null)
        //        {
        //            _enterIcon = Resources.Load<Sprite>(enterIconPath);
        //        }
        //        return _enterIcon;
        //    }
        //}

        //[JsonIgnore]
        //public Sprite disableIcon {
        //    get {
        //        if (_disableIcon == null)
        //        {
        //            _disableIcon = Resources.Load<Sprite>(disableIconPath);
        //        }
        //        return _disableIcon;
        //    }
        //}

        public KGUI_ItemDataConfig()
        {
            ItemDatas = new List<KGUI_Backpack_ItemData>();
        }

        /// <summary>
        /// 添加子项
        /// </summary>
        /// <param name="itemData"></param>
        public void AddItem(KGUI_Backpack_ItemData itemData)
        {
            var item = ItemDatas.Find(obj => obj.Name.Equals(itemData.Name));
            if (item != null)
            {
                Debug.LogError("添加的仪器信息，名称与已经添加到集合中的仪器冲突，请重新更改合适的名称");
                return;
            }

            ItemDatas.Add(itemData);
        }

        /// <summary>
        /// 移除子项
        /// </summary>
        /// <param name="itemData"></param>
        public void RemoveItem(KGUI_Backpack_ItemData itemData)
        {
            if (!ItemDatas.Contains(itemData)) return;

            ItemDatas.Remove(itemData);
        }

        public void RemoveItem(string name)
        {
            var item = ItemDatas.Find(obj => obj.Name.Equals(name));

            RemoveItem(item);
        }

    }

}
