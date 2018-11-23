namespace MagiCloud.Core.UI
{
    public interface IButton
    {
        bool IsEnable { get; set; }

        void OnClick(int handIndex);
        void OnEnter(int handIndex);
        void OnExit(int handIndex);
        void OnDown(int handIndex);
        void OnUp(int handIndex);

        void OnUpRange(int handIndex, bool isRange);

        void OnDownStay(int handIndex);
    }
}
