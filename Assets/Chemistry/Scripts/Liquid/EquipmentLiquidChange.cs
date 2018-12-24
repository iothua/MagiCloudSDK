using System.Collections;
using Chemistry.Liquid;
using UnityEngine;
using MagiCloud.Core;
using Chemistry.Chemicals;
using System.Collections.Generic;
using System;

namespace Chemistry.Liquid
{
    /// <summary>
    /// 仪器液面变化（用于容器内液体的增减）
    /// 开发者：阮榆皓
    /// </summary>
    public class EquipmentLiquidChange //: MonoBehaviour
    {
        /// <summary>
        ///构造
        /// </summary>
        /// <param name="liquidEffect">液体系统</param>
        /// <param name="containerVolume">容器容积</param>
        /// <param name="spillAction">液体溢出的回调</param>
        /// <param name="overAction">液体用完的回调</param>
        public EquipmentLiquidChange(LiquidSystem liquidEffect, float containerVolume, Action spillAction = null, Action overAction = null)
        {
            LiquidEffect = liquidEffect;
            ContainerVolume = containerVolume;
            liquidSpill = spillAction;
            liquidOver = overAction;
        }

        //改变液体量协程
        private Coroutine changeCoroutine;

        //溢出
        bool hasSpill = false;
        //耗尽
        bool isOver = false;

        Action liquidSpill = null;
        Action liquidOver = null;
        Action<string, float> liquidTrans = null;
        //private MBehaviour mBehaviour;

        ////下降动画及其曲线
        //[SerializeField, Header("量筒液面下降曲线")]
        //private AnimationCurve animationCurve;
        private float animationCurveTime;       //使用AnimationCurve的时间累积

        [HideInInspector]
        public LiquidSystem LiquidEffect;
        //容器容积
        private float ContainerVolume;


        /// <summary>
        /// 液体量变化
        /// </summary>
        /// <param name="startV">起始变化量</param>
        /// <param name="changeVolume">变化量(正为增，负为减)</param>
        /// <param name="time">变化时间（为0时突变）</param>
        /// <returns>变化后液体量</returns>
        public float ChangeLiquid(float startV, float changeVolume, float time = 0.5f)
        {
            //Debug.Log(this.name + "液面变化量为" + changeVolume);
            if (time < 0.0f)
            {
                //Debug.LogError("时间不能为负");
                return startV;
            }

            //若改变量趋近于0，则不变--防止误差
            if (Mathf.Abs(changeVolume) < 0.001f)
            {
                return startV;
            }

            float endV = startV + changeVolume;

            //防止溢出
            //startV = DrugSystemIns.AllDrugs[DrugName].Volume;
            //endV = startV + changeVolume;

            if (endV > ContainerVolume)
            {
                hasSpill = true;
                //Debug.Log("液体溢出" + endV + "容器已满");
                endV = ContainerVolume;
            }

            //耗完液体
            if (endV < 0.0f)
            {
                isOver = true;
                //Debug.Log("液体已耗完" + endV);
                endV = 0.0f;
            }


            //Debug.Log("开始值" + startV);
            //Debug.Log("结束值" + endV);


            if (time == 0.0f)
            {
                LiquidEffect.SetValue(endV, ContainerVolume);
                if (isOver && liquidOver != null)
                {
                    //Debug.Log("液体耗尽的回调被调用");
                    liquidOver.Invoke();
                }
                if (hasSpill && liquidSpill != null)
                {
                    //Debug.Log("液体溢出的回调被调用");
                    liquidSpill.Invoke();
                }
                return endV;
            }

            //是否正在变化--防止冲突
            if (changeCoroutine != null)
            {
                //Debug.Log("当前最终值" + startV);
                startV = endV;

                //Debug.Log("重设最终值" + endV);

                Keyframe tmpKey = LiquidEffect.animationCurve[0];

                LiquidEffect.StopCoroutine(changeCoroutine);

                //StopCoroutine

                changeCoroutine = LiquidEffect.StartCoroutine(OnLiquidChange(tmpKey.value, endV, time));

                return startV;
            }
            

            changeCoroutine = LiquidEffect.StartCoroutine(OnLiquidChange(startV, endV, time));

            return endV;
        }


        /// <summary>
        /// 液体量变化--连接药品系统--实时更新药品量--若对变化过程不严格建议使用上一个
        /// </summary>
        /// <param name="drugSystem">当前容器的药品系统</param>
        /// <param name="changeVolume">变化量(正为增，负为减)</param>
        /// <param name="drugName">需要增减量的药品名</param>
        /// <param name="time">变化时间（为0时突变）</param>
        /// <returns>变化后液体量</returns>
        public void ChangeLiquid(DrugSystem drugSystem, float changeVolume, string drugName = "", float time = 0.5f, Action<string, float> actionTrans = null)
        {
            if (string.IsNullOrEmpty(drugName))
            {
                drugName = drugSystem.FirstName;
            }

            if (!string.IsNullOrEmpty(drugName) && !drugSystem.IsHaveDrugForName(drugName))
            {
                drugSystem.AddDrug(drugName, 0);
            }

            if (actionTrans != null)
            {
                liquidTrans = actionTrans;
            }

            var drug = drugSystem.GetDrug(drugName);

            float startV = drug.Volume;

            //Debug.Log(this.name + "液面变化量为" + changeVolume);

            if (time < 0.0f)
            {
                //Debug.LogError("时间不能为负");
                return;
            }

            //若改变量趋近于0，则不变--防止误差
            if (Mathf.Abs(changeVolume) < 0.001f)
            {
                return;
            }

            float endV = startV + changeVolume;

            //防止溢出
            //startV = DrugSystemIns.AllDrugs[DrugName].Volume;
            //endV = startV + changeVolume;

            if (endV > ContainerVolume)
            {
                //Debug.Log("液体溢出" + endV + "容器已满");
                endV = ContainerVolume;
                hasSpill = true;
            }

            //耗完液体
            if (endV < 0.0f)
            {
                isOver = true;
                //Debug.Log("液体已耗完" + endV);
                endV = 0.0f;
            }


            //Debug.Log("开始值" + startV);
            //Debug.Log("结束值" + endV);

            //突变
            if (time == 0.0f)
            {
                LiquidEffect.SetValue(endV, ContainerVolume);
                drug.Volume = endV;
                if (isOver && liquidOver != null)
                {
                    //Debug.Log("液体耗尽的回调被调用");
                    liquidOver.Invoke();
                }
                if (hasSpill && liquidSpill != null)
                {
                    //Debug.Log("液体溢出的回调被调用");
                    liquidSpill.Invoke();
                }

                if (liquidTrans != null)
                {
                    liquidTrans.Invoke(drugName, changeVolume);
                }
                return;
            }

            //是否正在变化--防止冲突
            if (changeCoroutine != null)
            {
                //Debug.Log("当前最终值" + startV);
                startV = endV;

                //Debug.Log("重设最终值" + endV);

                Keyframe tmpKey = LiquidEffect.animationCurve[0];

                LiquidEffect.StopCoroutine(changeCoroutine);

                changeCoroutine = LiquidEffect.StartCoroutine(OnLiquidChange(tmpKey.value, endV, time, drugSystem, drugName));

                return;
            }


            changeCoroutine = LiquidEffect.StartCoroutine(OnLiquidChange(startV, endV, time, drugSystem, drugName));

            return;
        }



        /// <summary>
        /// 液面改变协程
        /// </summary>
        /// <param name="_startV">开始体积</param>
        /// <param name="_endV">结束体积</param>
        /// <param name="time">变化时间</param>
        /// <returns></returns>
        IEnumerator OnLiquidChange(float _startV, float _endV, float time, DrugSystem drugSystem = null, string drugName = "", Action<string, float> actionTrans = null)
        {
            //Debug.Log(this.name);

            //突变
            if (time <= 0.0f)
            {
                Debug.Log("time <=0.0f");
            }

            //初始键值
            LiquidEffect.animationCurve.MoveKey(0, new Keyframe(0.0f, _startV));

            //最终键值
            LiquidEffect.animationCurve.MoveKey(1, new Keyframe(time + 0.1f, _endV));

            float tmpV = _startV;
            float tmpLiquidV = _startV;
            while (true)
            {
                animationCurveTime += Time.deltaTime;

                if (animationCurveTime < time)
                {
                    tmpLiquidV = LiquidEffect.animationCurve.Evaluate(animationCurveTime);
                    
                    LiquidEffect.SetValue(tmpLiquidV, ContainerVolume);

                    //发送事件
                    if (liquidTrans!=null&&!string.IsNullOrEmpty(drugName))
                    {
                        liquidTrans.Invoke(drugName, tmpLiquidV - tmpV);
                    }
                    tmpV = tmpLiquidV;

                    //实时更新药品量
                    if (drugSystem != null && !string.IsNullOrEmpty(drugName))
                    {
                        var drug = drugSystem.GetDrug(drugName);
                        drug.Volume = tmpLiquidV;
                    }
                }
                else
                {
                    //最后一帧
                    if (liquidTrans != null && !string.IsNullOrEmpty(drugName))
                    {
                        liquidTrans.Invoke(drugName, _endV - tmpV);
                    }
                    tmpV = _endV;

                    LiquidEffect.SetValue(_endV, ContainerVolume);

                    //实时更新药品量
                    if (drugSystem != null && !string.IsNullOrEmpty(drugName))
                    {
                        var drug = drugSystem.GetDrug(drugName);
                        drug.Volume = _endV;
                    }

                    LiquidEffect.SetValue(_endV, ContainerVolume);

                    animationCurveTime = 0.0f;


                    if (isOver && liquidOver != null)
                    {
                        //Debug.Log("液体耗尽的回调被调用");
                        liquidOver.Invoke();
                    }
                    if (hasSpill && liquidSpill != null)
                    {
                        //Debug.Log("液体溢出的回调被调用");
                        liquidSpill.Invoke();
                    }

                    if (changeCoroutine != null)
                    {
                        LiquidEffect.StopCoroutine(changeCoroutine);
                        changeCoroutine = null;
                    }

                }

                yield return null;
            }
        }


        private void OnDestroy()
        {
            if (changeCoroutine != null && LiquidEffect != null)
            {
                LiquidEffect.StopCoroutine(changeCoroutine);
                changeCoroutine = null;
            }
        }

    }
}