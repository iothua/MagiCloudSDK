using MagiCloud.Core.MInput;
using UnityEngine;
using MagiCloud.Core.Events;
using MagiCloud.Core.UI;
using System;
using MagiCloud;

public class TestUIRay : MonoBehaviour {

    private IButton currentButton;
    public GameObject currentObject;//当前抓取物体
    public GameObject rayObject;//射线照射物体
    private GameObject uiObject; //只要照射UI

    private bool isEnter;
    public bool IsButtonPress;

    public LayerMask layerMask;

    public int HandIndex;

    public void Start()
    {
        EventHandGrip.AddListener(OnButtonPress);
        EventHandIdle.AddListener(OnButtonRelease);
        EventHandUIRay.AddListener(OnUIRay);
    }

    void OnUIRay(Ray ray, int handIndex)
    {
        if (HandIndex != handIndex) return;

        RaycastHit hit;

        if (IsButtonPress && currentButton != null)
        {
            currentButton.OnDownStay(handIndex);
        }

        if (Physics.Raycast(ray, out hit, 10000, layerMask))
        {
            //1、如果照射到了，将此碰撞体加0.5f
            //2、如果是握拳时，碰撞体范围还是以0.5f为算，如果默认，则以0.5为计算

            if (hit.collider.gameObject.CompareTag("button"))
            {
                if (!IsButtonPress)
                {
                    if (rayObject == null)
                    {
                        //移入
                        currentButton = hit.collider.GetComponent<IButton>();
                        if (currentButton == null) return;

                        OnButtonEnter(hit.collider.gameObject);

                    }
                    else if (currentObject == hit.collider.gameObject)
                    {
                        //如果相等，计算下

                        currentButton.Collider.IsShake = false;

                        RaycastHit detectionHit;
                        //如果在扫描时，不是这个物体则进行清空
                        if (Physics.Raycast(ray, out detectionHit, 10000, layerMask))
                        {
                            if (currentObject != detectionHit.collider.gameObject)
                            {
                                ClearButton(HandIndex);
                                //rayObject = null;
                            }
                            else
                            {
                                currentButton.Collider.IsShake = true;
                                return;
                            }
                        }
                        else
                        {
                            NotUIRay();
                        }
                    }
                    else
                    {

                        ClearButton(HandIndex);

                        //移入
                        currentButton = hit.collider.GetComponent<IButton>();
                        if (currentButton == null) return;

                        OnButtonEnter(hit.collider.gameObject);
                    }
                }
                else
                {
                    rayObject = hit.collider.gameObject;
                    //如果握拳，则进行处理
                }
            }
            else
            {
                NotUIRay();

            }
        }
        else
        {
            NotUIRay();
        }
    }



    /// <summary>
    /// Not UI射线处理
    /// </summary>
    void NotUIRay()
    {
        if (uiObject != null)
        {
            EventHandUIRayExit.SendListener(uiObject, HandIndex);

            uiObject = null;
        }

        rayObject = null;
        if (IsButtonPress) return;

        ClearButton(HandIndex);
    }

    private void OnButtonRelease(int handIndex)
    {
        if (HandIndex != handIndex) return;

        if (!IsButtonPress)
        {
            return;
        }

        IsButtonPress = false;

        if (currentButton != null)
            currentButton.OnUp(handIndex);

        if (currentObject != rayObject)
        {
            if (currentButton != null)
                currentButton.OnUpRange(handIndex, false);

            ClearButton(handIndex);

            currentButton = null;
            currentObject = null;
            rayObject = null;
            return;
        }

        if (currentButton != null)
            currentButton.OnUpRange(handIndex, true);

        if (currentButton != null && currentObject == rayObject)
        {
            currentButton.OnClick(handIndex);
        }

    }

    private void OnButtonPress(int handIndex)
    {
        if (HandIndex != handIndex) return;

        if (currentButton != null)
        {
            currentButton.OnDown(handIndex);
            IsButtonPress = true;
        }
    }

    void OnButtonEnter(GameObject hitObject)
    {
        currentButton.Collider.IsShake = true;

        rayObject = hitObject;

        currentButton.OnEnter(HandIndex);

        currentObject = rayObject;
    }

    /// <summary>
    /// 清空Button信息
    /// </summary>
    public void ClearButton(int handIndex)
    {
        if (currentButton == null) return;

        currentButton.OnExit(handIndex);
        //清除原来的大小
        currentButton.Collider.IsShake = false;

        currentButton = null;
        currentObject = null;
        rayObject = null;
    }
}
