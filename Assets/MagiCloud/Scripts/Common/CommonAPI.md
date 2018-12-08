#计时功能 TimerController:
virtualTime:用于设置指定的虚拟时间
realTime:   设置对应虚拟时间的真实时间，单位 秒
playEvent:  开启事件
pauseEvent: 暂停事件
stop:       结束事件
playingEvent:帧执行事件，返回时间0~1
Playing:    开启状态
Pausing:    暂停状态
Stopping:   结束状态（默认状态）
Play()      开始计时
Pause()     暂停
Connitue()  继续

---
#运动轨迹 MotionTrack
activeRecord:   激活记录轨迹
interval:       记录时间间隔
target:         记录的目标
activeShowLine: 激活显示轨迹,当关闭时，正在显示的部分不会自动消失
lifeTime:       显示时长
delay:          延迟显示
width:          轨迹粗细
color:          轨迹颜色
material:       轨迹材质
line:           轨道渲染组件
SetLine()       设置轨道线属性
PlayRecord()    开启记录
StopRecord()    停止记录
PlayShowLine()  开启轨道显示
StopShowLine()  停止轨道显示
ClearTrack()    清空记录的轨道
ClearLine()     清除显示的线
