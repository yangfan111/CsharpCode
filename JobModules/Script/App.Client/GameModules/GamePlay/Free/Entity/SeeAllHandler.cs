using Core.Free;
using Free.framework;

namespace Assets.Sources.Free.Entity
{
    public class SeeAllHandler : ISimpleMesssageHandler
    {

    public const int INVISIBLE = 1;
		
    public bool CanHandle(int key)
    {
        return key == FreeMessageConstant.MSG_SEE_ALL;
    }

    public void Handle(SimpleProto simpleUI)
    {
        var v = simpleUI.Bs[0];
        //Contexts.sharedInstance.battleRoom.gameStateEntity.isOutline = v;
    }
		
    }
}
