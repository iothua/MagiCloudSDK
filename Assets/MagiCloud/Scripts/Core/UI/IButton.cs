namespace MagiCloud.Core.UI
{
    public interface IButton
    {
        IBoxCollider BoxCollider { get; }
        bool IsEnable { get; set; }
        ButtonEvent onClick { get; }  //鼠标点击

        ButtonEvent onEnter { get; }  //鼠标移入
        ButtonEvent onExit { get; }    //鼠标移出
        ButtonEvent onDown { get; }    //鼠标按下
        ButtonEvent onUp { get; }      //鼠标抬起
        ButtonEvent onDownStay { get; }  //按下持续
        PanelEvent onUpRange { get; }
        void OnClick(int handIndex);
        void OnEnter(int handIndex);
        void OnExit(int handIndex);
        void OnDown(int handIndex);
        void OnUp(int handIndex);

        void OnUpRange(int handIndex,bool isRange);

        void OnDownStay(int handIndex);
    }
    public interface IPanel
    {

    }
}
