using UnityEngine;
using System;
using Chemistry.Data;
using Chemistry.Liquid;
using System.Collections.Generic;
using System.Linq;

namespace Chemistry.Chemicals
{
    /*
    疑问：
        1、现在容器中存在一种混合物，如何取出？如何计算。
        2、现在烧杯中有一种溶质，往烧杯倒水，溶质溶解，这时候进行取液操作，如何进行。
        3、现在只是一种纯净物，进行取液操作，如何进行计算。
        4、现在烧杯中，存在两种液体溶液，这时候又该如何计算。
        5、现在对药品进行反应，药品之间的反应如何计算。
        6、往药品系统中，注入混合物，如何计算。
    
    解决：
        1、两个集合，一个是混合物集合，另一个是纯净物集合。
        2、当添加混合物的时候，从混合物集合进行计算，当添加纯净物的时候，从纯净物中计算。
        3、当计算整个容器的体积时，两个集合进行计算。
        4、当进行取药时，根据需要取的药的名称，从药品系统中寻找。
        5、当进行反应时或者消耗时，只需要对集合中的数据进行处理即可。
        6、并且在进行密度排序的时候，也可以进行计算排序。
        7、当进行溶质计算的时候，也只需要针对这两个集合进行算法出来就好。

    倒水：
        1、DrugSystem 当前量、总量
        2、添加药（当前倒多少）、移除药
        3、混合、反应、密度分层
        4、烧杯1——烧杯2
        5、只需要获取到烧杯1 Drug object 还有它的值 最外层的溶液的数据。GetDrug()->传到另一个烧杯2就好。
    */

    /// <summary>
    /// 药品种类
    /// </summary>
    public enum DrugStyle
    {
        无,
        纯净物,
        混合物
    }

    public class DrugSystem
    {
        private float _volume;

        //纯净物药品集合
        private Dictionary<string, Drug> _dicAllDrugs = new Dictionary<string, Drug>();
        //混合物药品集合
        private Dictionary<string, DrugMixture> _dicAllDrugMixtures = new Dictionary<string, DrugMixture>();

        private Dictionary<string, DrugData> _dicAllDrugDatas = new Dictionary<string, DrugData>();

        #region 属性


        /// <summary>
        /// 容量（所在容器的容量）
        /// </summary>
        public float Volume {
            get {
                return _volume;
            }
        }

        /// <summary>
        /// 纯净物集合
        /// </summary>
        private Dictionary<string, Drug> AllDrugs {
            get { return _dicAllDrugs ?? (_dicAllDrugs = new Dictionary<string, Drug>()); }
        }

        /// <summary>
        /// 混合物集合
        /// </summary>
        private Dictionary<string, DrugMixture> AllDrugMixtures {
            get {
                return _dicAllDrugMixtures ?? (_dicAllDrugMixtures = new Dictionary<string, DrugMixture>());
            }
        }

        /// <summary>
        /// 药品系统集合
        /// </summary>
        public Dictionary<string, DrugData> AllDrugDatas {
            get {

                if (_dicAllDrugs == null)
                {
                    _dicAllDrugs = new Dictionary<string, Drug>();
                }

                _dicAllDrugDatas.Clear(); //清理元素，保证最新的

                foreach (var item in AllDrugs)
                {
                    _dicAllDrugDatas.Add(item.Key, new DrugData(item.Value));
                }

                foreach (var item in AllDrugMixtures)
                {
                    _dicAllDrugDatas.Add(item.Key, new DrugData(item.Value));
                }

                return _dicAllDrugDatas;
            }
        }

        /// <summary>
        /// 当前总药品体积(不包含气体)
        /// </summary>
        public float CurSumVolume
        {
            get
            {

                float sumVolume = 0;

                //遍历纯净物
                foreach (var item in AllDrugs)
                {
                    Drug drug = item.Value;

                    if (drug.DrugType == EDrugType.Empty || drug.DrugType == EDrugType.Gas) continue;

                    sumVolume += item.Value.Volume;
                }

                //遍历混合物
                foreach (var item in AllDrugMixtures)
                {
                    sumVolume += item.Value.Volume;
                }

                return sumVolume;
            }
        }

        /// <summary>
        /// 获取到当前空气体积
        /// </summary>
        public float CurAirVolume
        {
            get
            {
                float airVolume = 0;

                if (_volume == 0) return airVolume = 0;

                airVolume = _volume - CurSumVolume;

                return airVolume;
            }
        }

        /// <summary>
        /// 获取体积百分比
        /// </summary>
        public float Percent
        {
            get
            {
                if (_volume == 0) return 0;

                return Mathf.Clamp(CurSumVolume / _volume,0,1.0f);
            }
        }

        /// <summary>
        /// 获取药品系统的第一个元素
        /// </summary>
        public string FirstName {
            get {
                if (AllDrugDatas.Count == 0) return string.Empty;
                return AllDrugDatas.First().Key;
            }
        }

        /// <summary>
        /// 获取到药品系统的最后一个元素
        /// </summary>
        public string LastName {
            get {
                if (AllDrugDatas.Count == 0) return string.Empty;

                return AllDrugDatas.Last().Key;
            }
        }

        /// <summary>
        /// 获取纯净物中的溶剂(Liquid)
        /// 用与溶质的溶解（当溶解时，会生成新的混合物）
        /// </summary>
        public float DrugSolventMass
        {
            get
            {
                float vol = 0;
                foreach (var item in AllDrugs.Values)
                {
                    if (item.DrugType == EDrugType.Liquid)
                        vol += item.Mass;
                }

                return vol;
            }
        }

        #endregion

        public DrugSystem()
        {
            _volume = -1;
        }

        public DrugSystem(float volume)
        {
            _volume = volume;
        }

        #region 公有方法

        /// <summary>
        /// 查找药品名称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="drugObject"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public bool FindDrugForName(string name,out DrugData drugObject)
        {
            drugObject = GetDrug(name);

            if (drugObject.DrugObject == null) return false;
            else return true;
        }

        /// <summary>
        /// 是否存在指定药
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsHaveDrugForName(string name)
        {
            if (AllDrugs.ContainsKey(name))
            {
                return true;
            }

            if (AllDrugMixtures.ContainsKey(name))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取药品
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DrugData GetDrug(string name)
        {
            if (AllDrugs.ContainsKey(name))
            {
                Drug drug;
                AllDrugs.TryGetValue(name,out drug);

                return new DrugData(drug);
            }

            if (AllDrugMixtures.ContainsKey(name))
            {
                DrugMixture drugMixture;
                AllDrugMixtures.TryGetValue(name,out drugMixture);

                return new DrugData(drugMixture);
            }

            return new DrugData();
        }

        /// <summary>
        /// 取药
        /// </summary>
        /// <returns></returns>
        public DrugData OnTakeDrug(EDrugType drugType)
        {
            switch (drugType)
            {
                case EDrugType.Liquid:

                    //如果是液体，取相应的药
                    if (AllDrugMixtures.Count > 0)
                    {
                        var drug = AllDrugMixtures.First().Value;
                        
                        return new DrugData(drug);
                    }
                    else
                    {
                        return TakeDrug(drugType);
                    }

                case EDrugType.Solid:
                case EDrugType.Solid_Powder:

                    return TakeDrug(drugType);

                case EDrugType.Gas:

                    return TakeDrug(drugType);
                default:
                    return new DrugData();
            }
        }

        
        /// <summary>
        /// 添加药品
        /// </summary>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        public void AddDrug(string name,float volume,EMeasureUnit unit = EMeasureUnit.ml)
        {

            DrugData drugData = DrugDatabase.AddDrug(name, volume, unit);

            switch (drugData.drugStyle)
            {
                case DrugStyle.纯净物:
                    AddDrugPure((Drug)drugData.DrugObject);

                    break;
                case DrugStyle.混合物:
                    AddDrugMixture((DrugMixture)drugData.DrugObject);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 添加药品
        /// </summary>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        public void AddDrug(string name,float volume,out DrugData drug)
        {
            drug = DrugDatabase.AddDrug(name, volume);

            switch (drug.drugStyle)
            {
                case DrugStyle.纯净物:
                    AddDrugPure((Drug)drug.DrugObject);

                    break;
                case DrugStyle.混合物:
                    AddDrugMixture((DrugMixture)drug.DrugObject);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 减少（消耗）药品
        /// </summary>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        /// <param name="isClear"></param>
        public void ReduceDrug(string name,float volume,bool isClear = true)
        {
            DrugData drugObject;

            if (FindDrugForName(name,out drugObject))
            {

                if (drugObject.drugStyle == DrugStyle.纯净物)
                {


                    Drug drug = AllDrugs[drugObject.DrugName];
                    drug.ReduceDrug(volume);

                    if (drug.Volume <= 0 && isClear)
                    {
                        drug.SetDrugVolume(0);

                        AllDrugs.Remove(name);
                    }

                }

                if (drugObject.drugStyle == DrugStyle.混合物)
                {
                    DrugMixture drugMixture = AllDrugMixtures[drugObject.DrugName];
                    drugMixture.ReduceDrugMixture(volume);

                    if (drugMixture.Volume <= 0 && isClear)
                    {
                        AllDrugMixtures.Remove(name);
                    }
                }
            }
        }

        /// <summary>
        /// 消耗药品
        /// </summary>
        /// <returns>The drug.</returns>
        /// <param name="volume">消耗量.</param>
        /// <param name="isClear">移除 <c>true</c> 不移除.</param>
        public string ReduceDrug(float volume,bool isClear)
        {
            string drugName = FirstName;
            ReduceDrug(drugName, volume);
            return drugName;
        }

        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <returns></returns>
        public IWaterColor GetColor()
        {
            IWaterColor wc = new LiquidColorNode();
            if (IsHaveDrugForName("硫酸铜溶液"))
            {
                return new LiquidColorBlue();
            }
            if (IsHaveDrugForName("菜油"))
            {
                return new LiquidColorYellow_Oil();
            }

            if (IsHaveDrugForName("紫色石磊"))
                return new LiquidColorPurple();

            if (IsHaveDrugForName("碘酒"))
                return new LiquidColorRed();

            if (IsHaveDrugForName("泥土"))
                return new LiquidColorYellow_Soil();


            return wc;
        }

        /// <summary>
        /// 根据名称，获取到溶液对象信息
        /// </summary>
        /// <param name="drugName"></param>
        /// <returns></returns>
        public static IWaterColor GetColor(string drugName)
        {
            switch (drugName)
            {
                case "硫酸铜溶液":
                    return new LiquidColorBlue();
                case "菜油":
                    return new LiquidColorYellow_Oil();
                case "紫色石磊":
                    return new LiquidColorPurple();
                case "碘酒":
                    return new LiquidColorRed();
                case "泥土":
                    return new LiquidColorYellow_Soil();
                default:
                    return new LiquidColorNode();
            }
        }

        /// <summary>
        /// 移除药品
        /// </summary>
        /// <param name="name"></param>
        public void RemoveDrug(string name)
        {
            DrugData drugData;

            if (FindDrugForName(name,out drugData))
            {
                if (drugData.drugStyle == DrugStyle.纯净物)
                {
                    AllDrugs.Remove(name);
                    return;
                }

                if (drugData.drugStyle == DrugStyle.混合物)
                {
                    AllDrugMixtures.Remove(name);
                    return;
                }
            }
        }

        /// <summary>
        /// 清理数据为零的数据
        /// </summary>
        private void ClearDicDrugDate()
        {
            _dicAllDrugs = (Dictionary<string,Drug>)AllDrugs.Where(obj => obj.Value.Volume > 0);

            _dicAllDrugMixtures = (Dictionary<string,DrugMixture>)AllDrugMixtures.Where(obj => obj.Value.Volume > 0);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 添加纯净物
        /// </summary>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        private Drug AddDrugPure(string name, float volume, EMeasureUnit unit = EMeasureUnit.ml)
        {
            Drug drug = (Drug)GetDrug(name).DrugObject;
            if (drug != null)
            {
                // TODO...暂时只适用于单个的液体倒水
                if (drug.DrugType == EDrugType.Liquid)
                {
                    if (drug.Volume + volume > Volume)
                        volume = Volume - drug.Volume;
                }
                drug.AddDrug(volume);
            }
            else
            {
                drug = new Drug(name, volume, unit);
                AllDrugs.Add(name, drug);
            }
            return drug;
        }

        /// <summary>
        /// 添加纯净物
        /// </summary>
        /// <param name="drug"></param>
        /// <returns></returns>
        private Drug AddDrugPure(Drug drug)
        {
            Drug d = (Drug)GetDrug(drug.Name).DrugObject;

            if (drug == null) return d;

            if (d != null)
            {
                d.AddDrug(drug.Volume);
            }
            else
            {
                AllDrugs.Add(drug.Name, drug);
            }
            return d;
        }

        /// <summary>
        /// 添加混合物
        /// </summary>
        /// <param name="name"></param>
        /// <param name="vol"></param>
        private void AddDrugMixture(string name, float vol)
        {
            DrugMixture mixture = new DrugMixture(name, vol);

            //如果不存在的话，就添加。
            //当获取混合物的时候，根据这个混合物的比例。
            //去获取到最新的药品量，所以混合物之间就不需要额外添加了。
            AddDrugMixture(mixture);

        }

        private void AddDrugMixture(DrugMixture drug)
        {
            var drugMixture = (DrugMixture)GetDrug(drug.Name).DrugObject;

            if (drugMixture == null)
            {
                AllDrugMixtures.Add(drug.Name, drug);
            }
            else
            {
                drugMixture.AddDrugMixture(drug);
            }
        }

        DrugData TakeDrug( EDrugType drugType)
        {
            var drugs = GetDrug(drugType);

            if (drugs.Count == 0)
            {
                return new DrugData();
            }
            else {
                return new DrugData(drugs[0]);
            }

        }

        ///// <summary>
        ///// 获取纯净物
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //private Drug GetDrug(string name)
        //{
        //    if (AllDrugs.ContainsKey(name))
        //    {
        //        Drug drug;
        //        AllDrugs.TryGetValue(name, out drug);

        //        return drug;
        //    }

        //    return null;
        //}

        private List<Drug> GetDrug(EDrugType type)
        {
            return AllDrugs.Values.Where(obj => obj.DrugType.Equals(type)).ToList();
        }

        /// <summary>
        /// 获取混合物
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private DrugMixture GetDrugMixture(string name)
        {
            if (AllDrugMixtures.ContainsKey(name))
            {
                DrugMixture drugMixture;
                AllDrugMixtures.TryGetValue(name, out drugMixture);

                return drugMixture;
            }

            return null;
        }


        #endregion
    }
}
