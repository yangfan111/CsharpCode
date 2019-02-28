namespace Assets.Sources.Free.UI
{
    public interface IUIUpdater
    {
        bool IsDisabled { get; set; }
        void UIUpdate(int frameTime);
    }
}
