**MFrameworkCore**
* * *
* [简介](#简介)
* [框架思路介绍](#框架思路介绍)
* [API目录](#api目录)

# 简介
MFrameworkCore是作者根据多年的开发项目经验、结合Kinect的操作方式以及实际项目开发情况，随着现在VR设备、鼠标/触摸、android、Kinect等平台的频繁使用，在开发项目过程中，更多的希望开发一次项目，那么可以很方便的适配所有的平台，其中适配一般有输入端、UI、物体等，适合**做实验类**项目。

**注意：该开源项目不提供任何Kinect方面的资料，目前只支持鼠标的输入端，后期会加入android、VR设备。**

* * *

MFrameworkCore基于以上核心需求，采用最原始的方式**发射线**和**屏幕坐标**，对Unity的UGUI和Input进行了扩展，从而将输入端和UI彻底分离出来，去实现各个项目常用的项目需求。内部集成InputHand(输入端)、KGUI、MBehaviour(时间周期)
Events(事件)、Features(功能)、Interactive(距离交互)、Operates(操作)、仪器(Equipments)。

* * *

## [MInputHand](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.MInput#MInputHand)
MInputHand是针对输入端的控制端每一个光标的关联，提供相关事件、属性、状态等。
* * *
**注意：该MInputHand不是说每一个鼠标中的左键、右键、滚轮等三个键的状态。 而是对外部设备输入端中一个“光标”一个MInputHand。 这里的光标可以理解成以屏幕坐标点为基准，比如鼠标在unity中就只有一个屏幕坐标，那么他就代表着只有一个MInputHand。比如Kinect有两只手，每一手都有握拳-释放以及相应的屏幕坐标，那么它应该有两个MInputHand。andorid五个手指，每一个手指都有它的状态、屏幕坐标等。那么它就应该有5个MInputHand。**

## [Operate](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Operate)
Operate是操作模块，根据输入端相关信息实例化MInputHand，根据MInputHand提供的相关数据状态、对物体、UI进行处理，并且发送出一些事件、以及提供一些外部的静态方法。

## [KGUI](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.KGUI)
KGUI是基于UGUI中的Image、Canvas、BoxCollider碰撞体进行二次封装与扩展，形成一套适合作者项目开发的KGUI插件，它采用的是射线Ray和屏幕坐标的方式，开发出Button(按钮)、Backpack(背包)、Table(表格)、Scrollbar(滚动条)、ScrollView(滚动视图)、Toggle(开关)、Dropdown(下拉框)、Label(物体标签)等控件。
![KGUI控件](https://github.com/iothua/MFrameworkCore/blob/develop/WikiPng/kgui/kgui_control.jpg)

## [MBehaviour](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.MBehaviour)
MBehaviour是根据在实际项目开发中，很多时候unity自带的Awake、OnEnable、Start等时间执行顺序，并不按照标准的进行，在有些时候会有些混乱，基于这个需求，框架提供MBehaviour时间轴模式，内部集成Awake、OnEnable、Start等初始化方法，保证所有物体之间的初始化方法，严格按照执行顺序进行。

```cs
behaviour = new MBehaviour(ExecutionPriority.Highest, -1000, enabled);

behaviour.OnAwake(() =>
{
    Debug.Log("OnAwake");
});

behaviour.OnDestroy(() =>
{
    Debug.Log("OnDestroy");
});

behaviour.OnEnable(()=>{
    Debug.Log("OnEnable");
});

behaviour.OnStart(()=>{
  Debug.Log("OnStart");
});

MBehaviourController.AddBehaviour(behaviour);
```

## [Events](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#Events静态)
Event相关事件是基于摄像机camera以及输入端坐标点在unity中发出一条射线，然后根据这条射线对unity中的gameObject进行相关的操作，在这个过程中延伸出很多事件，比如物体抓取、摄像机缩放、围绕物体旋转、物体的移动距离交互、物体的标签相关UI等。
##  [Features](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Features)
Features模块基于项目开发中，一些常用的功能的集成端，比如物体之间交互虚影、高亮、标签、限制移动等功能，在框架中有一个功能控制端脚本，只需要在编辑器上配置好，即可方便使用。
![featureObjectController](https://github.com/iothua/MFrameworkCore/blob/develop/WikiPng/featureObjectController.jpg)
##  [Interactives](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Interactive)
Interactive模块是距离交互模块，物体与物体之间的距离检测，并且执行相应的通知。
![](https://github.com/iothua/MFrameworkCore/blob/develop/WikiPng/distanceIntecation.jpg)
针对仪器Equipment交互的距离检测InteractionEquipment是基于DistanceInteraction扩展而来。
![](https://github.com/iothua/MFrameworkCore/blob/develop/WikiPng/interactionEquipment.jpg)
## [Equipment仪器](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Features#%E4%BB%AA%E5%99%A8%E7%AB%AFequipmentbase)
Equipment模块，该框架适合实验类项目开发，每个实验器材其实都是一个仪器，仪器与仪器之间可进行交互处理，Equipment模块基于Interactive距离模块，进行衍生。集成了相关距离交互后的仪器信息通知、以及公用属性等。
![](https://github.com/iothua/MFrameworkCore/blob/develop/WikiPng/equipmentBase.png)

* * *

# 框架思路介绍
在框架中需要注意到的三个比较核心的类MInputHand、MOperate、Controller(各个设备的输入端)，比如鼠标为(MouseControll)、KinectController(Kinect输入端不开源)、AndroidController(开发中)。
该框架最初是为Kinect设备而研发的，在Kinect设备中，人有两个手，每一手在unity中又有世界坐标、屏幕坐标、手势的握拳、手势释放等动作。基于对Kinect的日渐熟悉，慢慢的发现其实鼠标本质上就是一只手，鼠标有一个屏幕坐标点然后在根据不同的按键结合点延伸出一些其他的操作，Android的触点操作本质上就是五个手指点的相互之间操作。

* * *

所以理论上我们只需要知道每一个外部设备在Unity所属的空间点（屏幕坐标、世界坐标都行），在配合Unity相机发射出一条射线。在结合外部设备的按键（比如某键按下、释放等操作）。在结合时间、坐标偏移等参数，基本可以延伸出常用的所有手势动作，比如双手操作缩放、握拳移动旋转、拖拽等等。在android平台、鼠标、外部VR设备等等，原理都基本一致，当然这些设备肯定有一套属于自己的动作事件以及GUI判定。

* * *

如果我们开发一个项目，单纯的去用每一个平台最初的事件和相关方法的话，那么在兼容其他平台就会出现很多的问题。所以MFrameworkCore应运而生，MFrameworkCore提供一套属于自己的全部事件，所有的物体操作、摄像机操作、UI操作都基于它而来。 MFrameworkCore在去做兼容不同的平台做处理。从而保证项目在PC端测试开发后，只需要选择VR平台或者Kinect平台绑定相关的SDK事件，即可无缝衔接，最快速的去测试兼容。
![](https://github.com/iothua/MFrameworkCore/blob/develop/WikiPng/%E7%BB%93%E6%9E%84%E5%9B%BE.jpg)

API目录
=================
* [MagiCloud.Core.Events](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events)
   * [ExecutionPriority](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#executionpriority)
   * [Handlers](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#handlers) 
   * [Events基础](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#events基础) 
   * [Events静态](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#events静态)
      * [EventHandGrip](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandgrip)
      * [EventHandIdle](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandidle)
      * [EventHandRay](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandray)
      * [EventHandRays](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandrays)
      * [EventHandStart](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandstart)
      * [EventHandStop](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandstop)
      * [EventHandUIRay](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhanduiray)
      * [EventCameraRotate](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventcamerarotate)
      * [EventCameraZoom](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventcamerazoom)
      * [EventHandGrabObject](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandgrabobject)
      * [EventHandGrabObjectKey](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandgrabobjectkey)
      * [EventHandReleaseObject](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandreleaseobject)
      * [EventHandReleaseObjectKey](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandreleaseobjectkey)
      * [EventHandRayTarget](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandraytarget)
      * [EventHandRayTargetEnter](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandraytargetenter)
      * [EventHandRayTargetExit](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhandraytargetexit)
      * [EventHandUIRayEnter](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhanduirayenter)
      * [EventHandUIRayExit](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#eventhanduirayexit)
   * [Events扩展方法](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#events扩展方法)
      * [AddGrabObject()](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#addgrabobject)
      * [RemoveGrabObject()](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#removegrabobject)
      * [RemoveGrabObjectAll()](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#removegrabobjectall)
      * [AddReleaseObject()](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#addreleaseobject)
      * [RemoveReleaseObject()](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#removereleaseobject)
      * [RemoveReleaseObjectAll()](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#removereleaseobjectall)
      * [AddRayTargetEnter()](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#addraytargetenter)
      * [RemoveRayTargetEnter()](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#removeraytargetenter)
      * [AddRayTargetExit()](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#addraytargetexit)
      * [RemoveRayTargetExit()](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.Events#removeraytargetexit)
* [MagiCloud.Core.MBehaviour](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.MBehaviour)
  * [MBehaviourController行为控制端](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.MBehaviour#mbehaviourcontroller)
  * [MBehaviour行为类](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.MBehaviour#mbehaviour)
  * [MBehaviourExpansion行为扩展方法](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.MBehaviour#mbehaviourexpansion)
* [MagiCloud.Core.MInput](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.MInput)
  * [MInputHandStatus手状态](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.MInput#minputhandstatus)
  * [MInputHand手](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.MInput#minputhand)
  * [IHandUI 手具体实现接口](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.MInput#minputhand)
  * [IHandController 输入端控制接口](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Core.MInput#ihandcontroller)
* [MagiCloud.Features](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Features)
* [MagiCloud.Interactive](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Interactive)
* [MagiCloud.KGUI](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.KGUI)
* [MagiCloud.Operate](https://github.com/iothua/MFrameworkCore/wiki/MagiCloud.Operate)
