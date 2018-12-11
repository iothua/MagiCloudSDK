using System;

namespace Chemistry.Chemicals
{
    /// <summary>
    /// 一次反应 产物信息
    /// </summary>
    [Serializable]
    public class ProductDrugInfo
    {
        /// <summary>
        /// 药品
        /// </summary>
        public Drug drugInfo;

        /// <summary>
        /// 每帧变化量
        /// </summary>
        public float deltaProduct;

        /// <summary>
        /// 本次反应总变化量
        /// </summary>
        public float sumProduct;

        /// <summary>
        /// 消耗速度
        /// </summary>
        public float speed;

        public ProductDrugInfo(float speed, Drug drug)
        {
            drugInfo = drug;
            this.speed = speed;
        }

        public override string ToString()
        {
            return "名字：" + drugInfo.Name + "--每帧产物：" + deltaProduct + "--总产物：" + sumProduct + "--速度" + speed;
        }

    }

}