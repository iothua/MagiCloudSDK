using System.Collections;
using Chemistry.Liquid;
using UnityEngine;


namespace Chemistry.Equipments.Actions
{
    /// <summary>
    /// 仪器液面变化（用于容器内液体的增减）
    /// 开发者：阮榆皓
    /// </summary>
    public class EA_EquipmentLiquidChange : MonoBehaviour
    {
        //改变液体量协程
        private Coroutine changeCoroutine;

        //下降动画及其曲线
        [SerializeField, Header("量筒液面下降曲线")]
        private AnimationCurve animationCurve;
        private float animationCurveTime;       //使用AnimationCurve的时间累积

        public LiquidSystem LiquidEffect;
        //容器容积
        private float ContainerVolume;

        //初始化
        public void OnInit(LiquidSystem liquidEffect, float containerVolume)
        {
            LiquidEffect = liquidEffect;
            ContainerVolume = containerVolume;
        }

        /// <summary>
        /// 液体量变化
        /// </summary>
        /// <param name="startV">起始变化量</param>
        /// <param name="changeVolume">变化量(正为增，负为减)</param>
        /// <param name="time">变化时间（为0时突变）</param>
        /// <returns>变化后液体量</returns>
        public float ChangeLiquid(float startV,float changeVolume, float time = 0.5f)
        {
            Debug.Log(this.name + "液面变化量为" + changeVolume);
            if (time < 0.0f)
            {
                Debug.LogError("时间不能为负");
                return startV;
            }

            //若改变量趋近于0，则不变--防止误差
            if (Mathf.Abs(changeVolume)<0.001f)
            {
                return startV;
            }

            float endV = startV + changeVolume;

            

            //防止溢出
            //startV = DrugSystemIns.AllDrugs[DrugName].Volume;
            //endV = startV + changeVolume;

            if (endV > ContainerVolume)
            {
                Debug.Log("液体溢出" + endV + "容器已满");
                endV = ContainerVolume;
            }

            //耗完液体
            if (endV < 0.0f)
            {
                Debug.Log("液体已耗完" + endV);
                endV = 0.0f;
            }


            Debug.Log("开始值" + startV);
            Debug.Log("结束值" + endV);


            if (time == 0.0f)
            {
                LiquidEffect.SetValue(endV, ContainerVolume);
                return endV;
            }

            //是否正在变化--防止冲突
            if (changeCoroutine != null)
            {
                Debug.Log("当前最终值" + startV);
                startV = endV;

                Debug.Log("重设最终值" + endV);

                Keyframe tmpKey = animationCurve[0];

                StopCoroutine(changeCoroutine);

                changeCoroutine = StartCoroutine(OnLiquidChange(tmpKey.value, endV, time));

                return startV;
            }


            changeCoroutine = StartCoroutine(OnLiquidChange(startV, endV, time));

            return endV;
        }



        /// <summary>
        /// 液面改变协程
        /// </summary>
        /// <param name="_startV">开始体积</param>
        /// <param name="_endV">结束体积</param>
        /// <param name="time">变化时间</param>
        /// <returns></returns>
        IEnumerator OnLiquidChange(float _startV, float _endV, float time)
        {
            //Debug.Log(this.name);

            //突变
            if (time <= 0.0f)
            {
                Debug.Log("time <=0.0f");
            }

            //初始键值
            animationCurve.MoveKey(0, new Keyframe(0.0f, _startV));

            //最终键值
            animationCurve.MoveKey(1, new Keyframe(time + 0.1f, _endV));


            


            while (true)
            {
                animationCurveTime += Time.deltaTime;

                if (animationCurveTime <= time)
                {
                    LiquidEffect.SetValue(animationCurve.Evaluate(animationCurveTime), ContainerVolume);
                }
                else
                {
                    animationCurveTime = 0.0f;

                    if (changeCoroutine != null)
                    {
                        StopCoroutine(changeCoroutine);
                        changeCoroutine = null;
                    }

                }

                yield return null;
            }
        }

        private void OnDestroy()
        {
            if (changeCoroutine != null)
            {
                StopCoroutine(changeCoroutine);
                changeCoroutine = null;
            }
        }

    }
}