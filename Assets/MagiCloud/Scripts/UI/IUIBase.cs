namespace MagiCloud.UISystem
{
    public interface IUIBase
    {
        string Name { get; }
        IUIGroup OwnerGroup { get; }
        int Priority { get; set; }
        bool Pause { get; set; }
        bool IsCovered { get; set; }
        bool IsAvailable { get; }
        bool IsVisible { get; set; }
        bool PauseCoveredUI { get; }
        void OnInit(int id,IUIGroup group,bool pauseCoveredUI);
        void OnOpen();
        void OnClose();
        void OnCovered();
        void OnReveal();
        void OnEnableUI();
        void OnDisableUI();
        void OnDepthChanged(int groupDepth,int depth);
    }
}
