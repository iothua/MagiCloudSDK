using Chemistry.Equipments;
using System;
using UnityEngine;

namespace Chemistry.Effects
{
    /// <summary>
    /// 胶头滴管特效
    /// </summary>
    public class Effect_Dropper : EffectBase
    {
        [Header("水滴特效")]
        public GameObject dropObj;
        [Header("水面扩散特效")]
        public GameObject poppleObj;
        public Animator animator { get; private set; }

        public override void OnInitialize()
        {

        }

        public override void OnInitialize_Editor()
        {
            base.OnInitialize_Editor();

        }

        public void SetPoppleEffect(float height)
        {

        }

        /// <summary>
        /// 显示涟漪特效
        /// </summary>
        public void ShowPoppleEffect(I_ET_D_Drip drip, int number)
        {
            poppleObj.SetActive(true);
            poppleObj.transform.localPosition = new Vector3(0, drip.LocalPositionYForEffect, 0);    //控制涟漪特效位置在接触面高度
            Invoke("HidePoppleEffect", number * 0.5f);          //number滴后关闭特效
            Invoke("HideDripEffect", number * 0.5f);
        }

        /// <summary>
        /// 关闭涟漪特特效
        /// </summary>
        public void HidePoppleEffect()
        {
            poppleObj.SetActive(false);
        }

        /// <summary>
        /// 设置水滴特效
        /// </summary>
        public void SetDripEffect()
        { }

        /// <summary>
        /// 显示水滴特效
        /// </summary>
        public void ShowDripEffect(I_ET_D_Drip drip, int number)
        {
            dropObj.SetActive(true);
            ParticleSystem particleSystem = dropObj.GetComponent<ParticleSystem>();
            particleSystem.Pause();
            UnityEngine.ParticleSystem.VelocityOverLifetimeModule module = particleSystem.velocityOverLifetime;
            module.y = drip.LowestY;        //控制水滴下落的距离
            particleSystem.Play();
        }

        /// <summary>
        /// 隐藏水滴特效
        /// </summary>
        public void HideDripEffect()
        {
            dropObj.SetActive(false);

        }
    }
}
