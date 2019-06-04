namespace MagiCloud.Core.UI
{
    public interface IButton
    {
        IBoxCollider BoxCollider { get; }
        bool IsEnable { get; set; }
        ButtonEvent Click { get; }  //鼠标点击

        ButtonEvent Enter { get; }  //鼠标移入
        ButtonEvent Exit { get; }    //鼠标移出
        ButtonEvent Down { get; }    //鼠标按下
        ButtonEvent Up { get; }      //鼠标抬起
        ButtonEvent DownStay { get; }  //按下持续
        PanelEvent UpRange { get; }
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
