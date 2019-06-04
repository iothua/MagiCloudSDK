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

        public bool isGenerate = false;
        public int generateCount; //生成数量
        //坐标
        public float xPosition, yPosition, zPosition;

        [JsonIgnore]
        public Vector3 Position{
            get{
                return new Vector3(xPosition, yPosition, zPosition);
            }
            set{
                xPosition = value.x;
                yPosition = value.y;
                zPosition = value.z;
            }
        }
    }

    /// <summary>
    /// 子项数据配置
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class KGUI_ItemDataConfig
    {

        public List<KGUI_Backpack_ItemData> ItemDatas;

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
