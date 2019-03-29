using App.Protobuf;
using Assets.Sources.Free.Effect;
using Free.framework;

namespace Assets.Sources.Free.UI
{
    public interface IShowStyle
    {
        IShowStyle Parse(SimpleProto simpleProto);

        void Show(SimpleFreeUI simpleFreeUI, int currentTime, int totalTime);

        void ShowEffect(FreeRenderObject renderObject, int currentTime, int totalTime);
    }
}
