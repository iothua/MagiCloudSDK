using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace MagiCloud.UIFrame
{
    /// <summary>
    /// Loading界面
    /// </summary>
    public class UI_Loading : UI_MainInterface
    {
        public Data.LoadData LoadData;

        public Image backgroundImage;
        public SpriteRenderer background; //背景精灵

        public override void OnInitialize()
        {
            base.OnInitialize();

            Operate = UIOperate.Loading;
        }

        public void AddSpriteData(string key,Sprite sprite)
        {
            if (LoadData.SpriteDatas.Any(obj => obj.key.Equals(key))) return;

            LoadData.SpriteDatas.Add(new Data.LoadSpriteData() { key = key, Value = sprite });
        }

        public void RemoveSpriteData(string key)
        {
            var result = LoadData.SpriteDatas.Find(obj => obj.key.Equals(key));

            if (result == null) return;
            LoadData.SpriteDatas.Remove(result);
        }

        public void AddObjectData(string key,GameObject target)
        {
            if (LoadData.ObjectDatas.Any(obj => obj.key.Equals(key))) return;

            LoadData.ObjectDatas.Add(new Data.LoadObjectData() { key = key, Value = target });
        }

        public void RemoveObject(string key)
        {
            var result = LoadData.ObjectDatas.Find(obj => obj.key.Equals(key));
            if (result == null) return;

            LoadData.ObjectDatas.Remove(result);
        }

        
        IEnumerator Loading(Action start = default(Action),Action end = default(Action))
        {

            //KinectHandStartStatus status = KinectConfig.GetHandStartStatus();

            //KinectConfig.SetHandStartStatus(KinectHandStartStatus.None);

            int[] nums = Utility.Utilitys.GetRandomSequence(LoadData.SpriteDatas.Count, LoadData.loadCount);

            for (int i = 0; i < nums.Length; i++)
            {

                SetBackground(LoadData.SpriteDatas[nums[i]].Value);

                yield return new WaitForSeconds(LoadData.loadTime);

                if (i == 0 && start != null)
                {
                    start();
                }
            }

            //KinectConfig.SetHandStartStatus(status);


            if (end != null)
                end();

            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator RandomLoading(float time, float startTime, Action start = default(Action), float endTime = 0.5f, Action end = default(Action))
        {
            //KinectHandStartStatus status = KinectConfig.GetHandStartStatus();
            //KinectConfig.SetHandStartStatus(KinectHandStartStatus.None);

            var value = Utility.Utilitys.GetRandomSequence(LoadData.SpriteDatas.Count, 1)[0];

            SetBackground(LoadData.SpriteDatas[value].Value);

            yield return new WaitForSeconds(startTime);

            if (start != null)
                start();

            yield return new WaitForSeconds(time);

            //KinectConfig.SetHandStartStatus(status);


            if (end != null)
                end();

            yield return new WaitForSeconds(endTime);
        }

        IEnumerator LoadingKey(string key, Action start = default(Action), Action end = default(Action))
        {
            //KinectHandStartStatus status = KinectConfig.GetHandStartStatus();
            //KinectConfig.SetHandStartStatus(KinectHandStartStatus.None);

            SetBackground(LoadData.SpriteDatas.Find(obj => obj.key.Equals(key)).Value);

            if (start != null)
                start();

            yield return new WaitForSeconds(LoadData.loadTime);

            //KinectConfig.SetHandStartStatus(status);


            if (end != null)
                end();

            yield return new WaitForSeconds(0.5f);
        }

        void SetBackground(Sprite sprite)
        {
            switch (type)
            {
                case UIType.Canvas:
                    backgroundImage.sprite = sprite;
                    break;
                case UIType.SpriteRender:
                    background.sprite = sprite;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 启动Loading
        /// </summary>
        /// <param name="end"></param>
        public void OnStart(Action start = default(Action), Action end = default(Action))
        {
            OnOpen();

            UIManager.Instance.StartCoroutine(Loading(start, end));
        }

        /// <summary>
        /// 指定key
        /// </summary>
        /// <param name="key"></param>
        public void OnStart(string key, Action start = default(Action),Action end = default(Action))
        {
            OnOpen();
            UIManager.Instance.StartCoroutine(LoadingKey(key, start, end));
        }

        /// <summary>
        /// 随机显示
        /// </summary>
        public void OnRandomShow(float startTime = 0.1f,Action start = default(Action), float endTime = 0.5f,Action end = default(Action))
        {
            OnOpen();
            UIManager.Instance.StartCoroutine(RandomLoading(LoadData.loadTime, startTime, start, endTime, end));
        }
    }
}
