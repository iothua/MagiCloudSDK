namespace MagiCloud.Core.UI
{
    public interface IToggle :IButton
    {
        ToggleEvent OnValueChanged { get; }
    }
}
