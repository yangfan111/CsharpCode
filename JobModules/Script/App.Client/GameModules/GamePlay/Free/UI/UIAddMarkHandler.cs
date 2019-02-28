using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class UIAddMarkHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.MarkPos;
        }

        public void Handle(SimpleProto simpleProto)
        {
            long playerId = simpleProto.Ls[0];
            int num = simpleProto.Ins[0];
            float posX = simpleProto.Fs[0];
            float posY = simpleProto.Fs[1];
            int flag = (int)simpleProto.Fs[2];

            var data = SingletonManager.Get<FreeUiManager>().Contexts1.ui.map;

            if (flag == 1)
            {
                //add mark
                data.AddMapMark(playerId, num, posX, posY);
            }
            else
            {
                //remove mark
                data.RemoveMapMark(playerId);
            }
        }
    }
}
