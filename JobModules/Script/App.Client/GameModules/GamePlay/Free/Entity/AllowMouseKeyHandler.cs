using App.Protobuf;
using Core.Free;
using Free.framework;

namespace Assets.Sources.Free.Entity
{

    public class AllowMouseKeysHandler : ISimpleMesssageHandler
    {


    public bool CanHandle(int key)
    {
        return key == FreeMessageConstant.CONFIG_ALLOW_MOUSE_KEYS;
    }

    public void Handle(SimpleProto simpleUI)
    {
//        var battleModel:BattleModel = GameModelLocator.getInstance().gameModel;
        var v = simpleUI.Ins[0];
			
//        GlobalVars.allowMouseKeys = v;
    }
		
    }
}
