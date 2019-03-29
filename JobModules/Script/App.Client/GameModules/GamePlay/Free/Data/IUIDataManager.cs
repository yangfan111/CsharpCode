namespace Assets.Sources.Free.Data
{
    public interface IUIDataManager
    {
        SimpleUIData SimpleUIData { get; }
        SimpleFreeUIData SFreeUIata { get; }
        AllObserverNameData AllObserverNameData { get; }
        string FreeType { get; set; }
    }
}
