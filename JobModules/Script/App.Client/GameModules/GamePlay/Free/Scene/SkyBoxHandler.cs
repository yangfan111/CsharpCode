using Core.Free;
using Free.framework;

namespace Assets.Sources.Free.Scene
{
    public class SkyBoxHandler : ISimpleMesssageHandler
    {


        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.CHANGE_SKYBOX;
        }

        public void Handle(SimpleProto simpleUI)
        {
            var skyBoxId = simpleUI.Ins[0];
            
        }
    }
}
