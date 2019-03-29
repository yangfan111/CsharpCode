using Utils.Singleton;

namespace Assets.Sources.Free.Data
{
    public class UIDataManager : Singleton<UIDataManager>, IUIDataManager
    {
        public SimpleUIData SimpleUIData { get; private set; }
        public SimpleFreeUIData SFreeUIata { get; private set; }
        public AllObserverNameData AllObserverNameData { get; private set; }
        public string FreeType { get; set; }

        public UIDataManager()
        {
            SFreeUIata = new SimpleFreeUIData();
            AllObserverNameData = new AllObserverNameData();
        }
    }
}
