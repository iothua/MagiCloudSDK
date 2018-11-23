using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.UIFrame.Data
{
    /// <summary>
    /// 加载数据
    /// </summary>
    [CreateAssetMenu(menuName = "MagiCloud/UIFrame/LoadData(Loaa数据配置)")]
    public class LoadData : ScriptableObject
    {
        public List<LoadSpriteData> SpriteDatas;
        public List<LoadObjectData> ObjectDatas;

        public int loadCount = 2;
        public float loadTime = 2.0f;
    }

    [Serializable]
    public class LoadSpriteData
    {
        public string key;
        public Sprite Value;
    }

    [Serializable]
    public class LoadObjectData
    {
        public string key;
        public GameObject Value;
    }
}
