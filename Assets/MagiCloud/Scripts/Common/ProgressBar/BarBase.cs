using MagiCloud.KGUI;
using System;
using UnityEngine;

namespace MagiCloud.Common
{
    public abstract class BarBase :IBar
    {
        protected ButtonType barType;
        protected bool isHorizontal = false;           //横向还是纵向
        protected bool isReverse = false;              //是否反向  
        [Range(0f,1f)]
        protected float value;
        protected Transform bgRoot;
        protected Transform frontRoot;

        public ButtonType BarType => barType;

        protected BarBase(bool isHorizontal,bool isReverse,Transform parent)
        {
            this.isHorizontal=isHorizontal;
            this.isReverse=isReverse;
            InitRoot(parent);
        }
        public virtual void InitRoot(Transform parent)
        {
            if (bgRoot==null)
                bgRoot=parent.Find("bg");
            if (bgRoot==null)
            {
                bgRoot=new GameObject("bg").transform;
                bgRoot.ResetTransform(parent);
            }
            if (frontRoot==null)
                frontRoot=parent.Find("front");
            if (frontRoot==null)
            {
                frontRoot=new GameObject("front").transform;
                frontRoot.ResetTransform(parent);
            }
        }



        public virtual float GetValue()
        {
            return value;
        }

        public virtual void SetValue(float value)
        {

        }
        public virtual bool Remove()
        {
            return true;
        }

        public virtual void Init(Sprite bgSprite,Sprite frontSprite,Vector2 bgSize,Vector2 frontSize)
        {
        }
    }


}