using System;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.Core;

namespace Chemistry.Chemicals
{
    /// <summary>
    /// 反应控制器
    /// </summary>
    public class ReactionControl
    {
        private bool isStopProduct; //标记是否暂停反应

        private float reactionSpeed = 1f; //反应速度
        private float adjustSpeed = 0.01f; //调节整体速度

        /// <summary>
        /// 药品系统（获取反应物）
        /// </summary>
        private DrugSystem _drugSystem;

        /// <summary>
        /// 反应条件
        /// </summary>
        private List<ConditionBase> _lstConditionBases;

        /// <summary>
        /// 一次反应的数据
        /// </summary>
        private ReactionInfo _reactionInfo;

        /// <summary>
        /// 反应速度
        /// </summary>
        public float ReactionSpeed
        {
            get { return reactionSpeed; }
        }

        public ReactionInfo ReactionInfo
        {
            get { return _reactionInfo; }
        }

        /// <summary>
        /// 开始反应
        /// </summary>
        public event Action<ReactionInfo> EventStart; //开始反应
        /// <summary>
        /// 开始反应（从暂停中过来）
        /// </summary>
        public event Action<ReactionInfo> EventRestore; //开始反应(从暂停中过来)
        /// <summary>
        /// 开始反应中
        /// </summary>
        public event Action<ReactionInfo> EventUpdate; //开始反应中
        /// <summary>
        /// 暂停反应时
        /// </summary>
        public event Action<ReactionInfo> EventPause; //暂停反应时
        /// <summary>
        /// 暂停反应中
        /// </summary>
        public event Action<ReactionInfo> EventPausing; //暂停反应中
        /// <summary>
        /// 结束反应
        /// </summary>
        public event Action<ReactionInfo> EventEnd; //结束反应


        /// <summary>
        /// 是否开始反应
        /// </summary>
        public bool IsStartPlaying { get; set; }

        /// <summary>
        /// 是否反应
        /// </summary>
        public bool IsPlaying { get; set; }

        private MBehaviour behaviour;

        /// <summary>
        /// 实例化反应控制器
        /// </summary>
        /// <param name="drugSystem"></param>
        public ReactionControl(DrugSystem drugSystem)
        {
            _drugSystem = drugSystem;
            _lstConditionBases = new List<ConditionBase>();
        }

        #region 公有方法

        /// <summary>
        /// 反应系统执行
        /// </summary>
        public void OnUpdateListen()
        {
            if (IsPlaying)
            {
                InvokeUpdate(_reactionInfo);
            }
            if (isStopProduct)
            {
                InvokePausing(_reactionInfo);
            }
        }

        /// <summary>
        /// 开始反应
        /// </summary>
        /// <param name="drugInfoF"></param>
        public ReactionInfo StartProduct(float startTime = 0)
        {
            if (_reactionInfo == null)
            {
                //第一次反应，需要查找对应的反应类型获取反应ID
                var reaction = ReactionManager.GetReaction(_drugSystem, _lstConditionBases);
                if (reaction!=null)
                {
                    _reactionInfo = new ReactionInfo(reaction, _drugSystem);

                    behaviour = new MBehaviour();
                    float sumTime = 0;

                    behaviour.OnUpdate_MBehaviour(() =>
                    {
                        sumTime = Time.deltaTime;

                        if (!IsStartPlaying)
                        {
                            if (sumTime > startTime)
                            {
                                IsStartPlaying = true;
                                IsPlaying = true;

                                InvokeStartFirst(_reactionInfo);
                            }
                        }
                        else
                        {
                            OnUpdateListen(); //一直执行
                        }
                    });

                    return _reactionInfo;
                }
                else
                {
                    Debug.Log("没有找到对应的反应类型，开启反应失败...");

                    return null;
                }
            }
            else
            {
                //从暂停状态回来，继续反应
                InvokeRestore(_reactionInfo);

                IsPlaying = true;
                isStopProduct = false;

                return _reactionInfo;
            }
        }

        /// <summary>
        /// 暂停反应
        /// </summary>
        public void StopProduct()
        {
            if (!isStopProduct && IsPlaying)
            {
                isStopProduct = true;
                IsPlaying = false;
                InvokePause(_reactionInfo);
            }

        }

        /// <summary>
        /// 结束反应
        /// </summary>
        public void EndProduct()
        {
            if (IsPlaying || isStopProduct)
            {
                IsPlaying = false;
                isStopProduct = false;
                IsStartPlaying = false;

                InvokeEnd(_reactionInfo);
                _reactionInfo = null;
            }
        }

        /// <summary>
        /// 设置反应速度
        /// </summary>
        /// <param name="speed"></param>
        public void SetReactionSpeed(float speed)
        {
            reactionSpeed = speed;
        }

        /// <summary>
        /// 添加反应条件
        /// </summary>
        /// <param name="name"></param>
        public void AddReactionCondition(string name)
        {
            _lstConditionBases.Add(new ConditionBase(name));
        }

        /// <summary>
        /// 删除反应条件
        /// </summary>
        /// <param name="name"></param>
        public void RemoveReactionCondition(string name)
        {
            if (_lstConditionBases.Contains(new ConditionBase(name)))
                _lstConditionBases.Remove(new ConditionBase(name));
        }

        #endregion


        private void InvokeStartFirst(ReactionInfo reactionInfo)
        {
            //开始反应
            if (EventStart != null)
                EventStart.Invoke(reactionInfo);
            //Debug.Log("InvokeStartFirst...");
        }

        /// <summary>
        /// 从暂停反应中恢复过来
        /// </summary>
        /// <param name="reactionInfo"></param>
        private void InvokeRestore(ReactionInfo reactionInfo)
        {
            //从暂停后又开始反应
            reactionInfo.lastStopTime = reactionInfo.startTime;
            reactionInfo.startTime = 0f;
            reactionInfo.sumProductAmount = 0f;

            if (EventRestore != null)
                EventRestore.Invoke(reactionInfo);
            //Debug.Log("InvokeStartOne...");

        }

        private void InvokeUpdate(ReactionInfo reactionInfo)
        {
            reactionInfo.startTime += Time.deltaTime;
            reactionInfo.sumDeltaProduct = 0f;
            reactionInfo.sumProductAmount = 0f;

            //反应物赋值
            foreach (ResponseDrugInfo item in reactionInfo.LstReactionDrugInfos)
            {
                item.deltaProduct = Time.deltaTime * item.speed * ReactionSpeed * adjustSpeed;
                item.sumProduct += item.deltaProduct;
                item.drugInfo.ReduceDrug(item.deltaProduct);
                if (item.drugInfo.Volume <= 0)
                    EndProduct();
            }

            //产物赋值
            foreach (ProductDrugInfo item in reactionInfo.LstProductDrugInfos)
            {
                //反应数据添加数据
                item.deltaProduct = Time.deltaTime * item.speed * ReactionSpeed * adjustSpeed;
                item.sumProduct += item.deltaProduct;
                reactionInfo.sumDeltaProduct += item.deltaProduct;
                reactionInfo.sumProductAmount += item.sumProduct;

                //药品系统添加数据
                _drugSystem.AddDrug(item.drugInfo.Name, item.deltaProduct);
            }

            if (EventUpdate != null)
                EventUpdate.Invoke(reactionInfo);
            //Debug.Log("InvokeStartUpdate...");

        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="reactionInfo"></param>
        private void InvokePause(ReactionInfo reactionInfo)
        {
            reactionInfo.lastStartTime = reactionInfo.startTime;
            reactionInfo.stopTime = 0f;
            if (EventPause != null)
                EventPause.Invoke(reactionInfo);
            //Debug.Log("InvokeEventStopOne...");

        }

        /// <summary>
        /// 暂停反应中……
        /// </summary>
        /// <param name="reactionInfo"></param>
        private void InvokePausing(ReactionInfo reactionInfo)
        {
            reactionInfo.stopTime += Time.deltaTime;

            if (EventPausing != null)
                EventPausing.Invoke(reactionInfo);
            //Debug.Log("InvokeEventStopUpdate...");

        }

        /// <summary>
        /// 执行停止反应
        /// </summary>
        /// <param name="reactionInfo"></param>
        private void InvokeEnd(ReactionInfo reactionInfo)
        {
            if (EventEnd != null)
                EventEnd.Invoke(reactionInfo);
            //Debug.Log("InvokeEventEndOne...");

            reactionInfo.ResetInfo();
        }
    }

}