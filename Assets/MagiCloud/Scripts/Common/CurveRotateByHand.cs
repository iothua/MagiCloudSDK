using UnityEngine;
using MagiCloud.Equipments;
namespace MagiCloud.Common
{
    /// <summary>
    /// 跟随手曲线旋转
    /// </summary>
    public class CurveRotateByHand :EquipmentBase
    {
        private Vector2 recordPos;
        private int recordHand;
        private bool isGrad;
        [SerializeField, Header("旋转速率")]
        private float rate = 10;
        [SerializeField, Header("手势有效移动角度")]
        private float range = 45;
        [SerializeField, Header("可旋转范围")]
        private Vector2 limitRange = new Vector2(0,180);
        [SerializeField, Header("旋转中心，不能和抓取物体坐标相等")]
        private Transform center;
        private Transform grad;
        private bool isInit = false;
        protected override void Start()
        {
            if (center==null) center=transform;
            grad =FeaturesObject.Customize.grabObject.transform;
            if (grad.position==center.position) return;
            base.Start();
            FeaturesObject.Customize.OnCustomizeUpdate.AddListener(OnUpdate);
            isInit=true;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (isInit)
                FeaturesObject.Customize.OnCustomizeUpdate.RemoveListener(OnUpdate);
        }
        private void OnUpdate(GameObject arg0,Vector3 arg1,int arg2)
        {
            recordPos=MUtility.MainWorldToScreenPoint(grad.position);
            Vector2 curPos = MOperateManager.GetHandScreenPoint(arg2);
            var handVector = curPos-recordPos;
            var handDir = handVector.normalized;
            var handDis = handVector.magnitude;
            if (handDis<=0f) return;
            Vector2 vector = grad.position-center.position;
            var dir = vector.normalized;

            var right = Vector2.Dot(handDir,dir);

            //切线
            var normal = right>0 ? new Vector2(dir.y,-dir.x) : new Vector2(-dir.y,dir.x);

            //判读手移动方向是否在切线范围内
            float angle = Vector2.Angle(normal,handDir);
            ////30度内为有效
            float r = right>0 ? -rate : rate;
            if (angle<=range)
            {

                float z = center.localEulerAngles.z+r;
                z=Mathf.Clamp(z,limitRange.x,limitRange.y);
                var q = Quaternion.Euler(0,0,z);
                center.localRotation=q;
            }

        }
    }
}
