using UnityEngine;
using System.Collections;

namespace MagiCloud.Kinect
{
    /// <summary>
    /// 手势识别管理
    /// </summary>
    public class KinectHandIdentifyManager : MonoBehaviour
    {
        private GameObject twoHandObject;
        private GameObject oneHandObject;

        private OneHandControl oneHand;
        private TwoHandControl twoHand;

        public KinectUserManager userManager { get; set; }

        private void Initialize()
        {
            twoHandObject = new GameObject("TwoIdentify");
            twoHand = twoHandObject.AddComponent<TwoHandControl>();
            twoHandObject.transform.SetParent(this.transform);
            twoHandObject.SetActive(false);

            oneHandObject = new GameObject("OneHandObject");
            oneHand = oneHandObject.AddComponent<OneHandControl>();
            oneHandObject.transform.SetParent(this.transform);
            oneHandObject.SetActive(false);
        }

        private void Start()
        {
            Initialize();
            twoHand.handIdentifyManager = this;
        }

        //判断用户是否激活操控
        public void StartJointRay(long userID, KinectUserManager kinectUser)
        {
            if (KinectConfig.GetHandStartStatus() == KinectActiveHandStadus.Two)
            {
                if (!twoHandObject.activeSelf)
                {
                    twoHandObject.SetActive(true);
                    oneHandObject.SetActive(false);
                }
                twoHand.StartJointRay2(userID, kinectUser);
            }
            else if (KinectConfig.GetHandStartStatus() == KinectActiveHandStadus.One)
            {
                if (KinectConfig.GetKinectHandActiveStatus() == KinectHandStatus.Identify)
                {
                    if (!oneHandObject.activeSelf)
                    {
                        oneHandObject.SetActive(true);
                        twoHandObject.SetActive(false);
                    }

                    oneHand.StartDetectHand(userID, kinectUser);
                }
                else if (KinectConfig.GetKinectHandActiveStatus() == KinectHandStatus.Enable)
                {
                    oneHand.StartDetectHand(userID, kinectUser);
                }
                else if (KinectConfig.GetKinectHandActiveStatus() == KinectHandStatus.Disable)
                {
                    oneHandObject.SetActive(false);
                }
            }
        }
    }
}

