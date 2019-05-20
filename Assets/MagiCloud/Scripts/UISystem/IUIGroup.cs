namespace MagiCloud.UISystem
{
    public enum EUIType
    {
        Canvas = 0,
        Sprite = 1,
    }
    public interface IUIGroup
    {
        EUIType UIType { get; }
        string Name { get; }
        int Depth { get; set; }
        IUIBase GetUI(string name);
        IUIBase[] GetUIs(string name);
        void AddUI(IUIBase ui);
        void RemoveUI(string name);
        void RemoveUI(IUIBase ui);
        bool HasUI(string name);
        void OnOpen(IUIBase ui);
        void OnClose(IUIBase ui);
    }
}
