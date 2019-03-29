using Core.Free;
using Free.framework;

namespace Assets.Sources.Free.Scene
{
    public class ClosePvsHandler : ISimpleMesssageHandler
    {

        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PVS;
        }

        public void Handle(SimpleProto simpleUI)
        {
            var close = simpleUI.Bs[0];

//            GlobalVars.scene.world.worldHelp.setEnablePvsByRule(!close);
        }
    }
}
