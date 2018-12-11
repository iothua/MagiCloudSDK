

using Chemistry.Data;

namespace Chemistry.Chemicals
{
    /// <summary>
    /// 混合物
    /// 溶液 = 物体 + 水
    /// </summary>
    public class DrugMixture
    {
        //溶剂 + 溶质
        private string _name;
        //已知体积，含量
        private float _volume;                                  //体积
        private float _percent;                                 //当前百分比
        private float _solubility;                              //溶解度

        /// <summary>
        /// 溶质
        /// </summary>
        private Drug _solute;

        /// <summary>
        /// 溶剂
        /// </summary>
        private Drug _solvent;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name {
            get { return _name; }
        }

        /// <summary>
        /// 当前百分比
        /// </summary>
        public float Percent {
            get {
                return _percent;
            }
        }

        public float Solubility {
            get {
                return _solute.CurrentSolubility;
            }
        }

        /// <summary>
        /// 体积（ml）
        /// </summary>
        public float Volume
        {
            get { return _volume; }
        }

        public float Mass {
            get {
                return Solute.Mass + Solvent.Mass;
            }
        }

        /// <summary>
        /// 溶质
        /// </summary>
        public Drug Solute
        {
            get { return _solute; }

            set { _solute = value; }
        }
        /// <summary>
        /// 溶剂
        /// </summary>
        public Drug Solvent
        {
            get { return _solvent; }

            set { _solvent = value; }
        }

        /// <summary>
        /// 实例化混合溶液
        /// 从数据里边获取混合溶液
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="volume">量</param>
        public DrugMixture(string name, float volume)
        {
            _volume = volume;
            _name = name;

            DI_DrugMixtureInfo di = GetDrugMixtureInfo(name);
            if (di == null) return;

            var soluteVolume = Volume * di.percent;

            //溶质赋值
            _solute = new Drug(di.soluteName, soluteVolume);
            //溶剂赋值
            _solvent = new Drug(di.solventName, Volume - soluteVolume);

            _solute.CurrentSolubility = _solvent.Mass;

        }

        /*溶剂 溶质 的加减*/
        /// <summary>
        /// 添加溶质
        /// </summary>
        /// <param name="val"></param>
        public void AddSolute(float val)
        {
            Solute.AddDrug(val);
            //计算体积
            ComputeVolume();
        }
        /// <summary>
        /// 减少溶质
        /// </summary>
        /// <param name="val"></param>
        public void ReduceSolute(float val)
        {
            Solute.ReduceDrug(val);
            ComputeVolume();
        }
        /// <summary>
        /// 添加溶剂
        /// </summary>
        /// <param name="val"></param>
        public void AddSolvent(float val)
        {
            Solvent.AddDrug(val);
            ComputeVolume();
        }
        /// <summary>
        /// 减少溶剂
        /// </summary>
        /// <param name="val"></param>
        public void ReduceSolvent(float val)
        {
            Solvent.ReduceDrug(val);
            ComputeVolume();
        }

        /// <summary>
        /// 添加混合物
        /// </summary>
        /// <param name="drugMixture"></param>
        public void AddDrugMixture(DrugMixture drugMixture)
        {
            AddSolute(drugMixture.Solute.Volume);
            AddSolvent(drugMixture.Solvent.Volume);
        }

        /// <summary>
        /// 减少混合物
        /// </summary>
        /// <param name="volume"></param>
        public void ReduceDrugMixture(float volume)
        {
            //减少
            //每减少一g溶剂，该减少多少溶质
        }

        /// <summary>
        /// 减少混合物（根据质量）
        /// </summary>
        /// <param name="mass"></param>
        public void ReduceDrugMixtureMass(float mass)
        {

        }

        /// <summary>
        /// 计算体积
        /// </summary>
        public void ComputeVolume()
        {
            //TODO…计算体积

            //根据溶质和溶剂的体积，计算出此时的体积。要加一个药品系统，如果量大的话，则需要生成新的药品对象
        }

        /// <summary>
        /// 根据名称，获取混合物的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DI_DrugMixtureInfo GetDrugMixtureInfo(string name)
        {
            DI_DrugMixtureInfo di;
            DataLoading.DicDrugMixtureLoadingInfo.TryGetValue(name, out di);

            return di;
        }
    }
}


