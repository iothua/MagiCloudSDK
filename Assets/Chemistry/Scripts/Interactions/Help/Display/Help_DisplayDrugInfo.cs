using System.Collections.Generic;
using UnityEngine;
using System;
using Chemistry.Chemicals;

namespace Chemistry.Help
{
    /// <summary>
    /// 帮助查看数据-药品系统
    /// </summary>
    public class Help_DisplayDrugInfo : MonoBehaviour
    {
        public float curSumVolume;
        private IDrugSystem ids;
        public List<DisplayDrug> LstDisplayDrugs = new List<DisplayDrug>();

        void Start()
        {
            ids = GetComponent<IDrugSystem>();
        }

        void Update()
        {
            if (ids.DrugSystemIns == null) return;

            curSumVolume = ids.DrugSystemIns.CurSumVolume;
            LstDisplayDrugs.Clear();
            foreach (KeyValuePair<string, DrugData> item in ids.DrugSystemIns.AllDrugDatas)
            {
                LstDisplayDrugs.Add(new DisplayDrug(item.Value.DrugName, item.Value.Volume, item.Value.Mass));
            }
        }
    }

    [Serializable]
    public class DisplayDrug
    {
        public string name;
        public float volume;
        public float mass;

        public DisplayDrug(string na, float vo, float ma)
        {
            name = na;
            volume = vo;
            mass = ma;
        }
    }

}