using Assets.Sources.Free.Data;
using Assets.Sources.Free.Render;

namespace Assets.Sources.Free.UI
{
    public interface IFreeComponent
    {

        int Type { get; }

        int ValueType { get; }

        string Key { get; }

        void SetPos(IComponentGroup freeUI, float x, float y, int width, int height, int relative, int parent);

        void RefreshPosition();

        void SetValue(params object[] value);

        void Initial(params object[] init);

        void Frame(IUIDataManager uiDataManager, int frameTime);

        IUiObject ToUI ();

        IFreeComponent Clone();

        event AddComponent ComponentAdded;

        FreeUIEvent FreeUIEvent { get; }

        string EventKey { get; set; }

        bool IsNoMouse { get; }

        void Destroy();

    }
}
