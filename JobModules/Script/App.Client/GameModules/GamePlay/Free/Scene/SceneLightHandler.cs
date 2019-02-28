using Core.Free;
using Free.framework;

namespace Assets.Sources.Free.Scene
{
    public class SceneLightHandler : ISimpleMesssageHandler
    {


        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.LIGHTMAP;
        }

        public void Handle(SimpleProto simpleUI)
        {
        }
    }
}
