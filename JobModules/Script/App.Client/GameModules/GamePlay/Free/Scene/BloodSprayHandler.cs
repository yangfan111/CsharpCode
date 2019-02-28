using Core.Free;
using Free.framework;

namespace Assets.Sources.Free.Scene
{
    public class BloodSprayHandler : ISimpleMesssageHandler
    {


        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.BLOOD_SPAY;
        }

        public void Handle(SimpleProto simpleUI)
        {
            var close = simpleUI.Bs[0];

            //        EffectActivationModel.getInstance().activationWallBlood = !close;
        }
    }
}
