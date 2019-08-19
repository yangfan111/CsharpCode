using App.Shared;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core;
using Core.Free;
using Free.framework;
using System.Linq;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameModules.GamePlay.Free.Entity
{
    public class ChangeAvatarHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.ChangeAvatar;
        }

        public void Handle(SimpleProto data)
        {
            var playerEntity = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;
            if (playerEntity != null)
            {
                var weaponController = playerEntity.WeaponController();
                if (data.Ks[0] == 3)
                {
                    weaponController.SetWeaponPart( (EWeaponSlotType)data.Ins[0], data.Ins[1]);
                }
                if (data.Ks[0] == 4)
                {
                    weaponController.DeleteWeaponPart( (EWeaponSlotType)data.Ins[0], (EWeaponPartType)data.Ins[1]);
                }
            }
        }
    }
}
