using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using Core;
using XmlConfig;
using Utils.Configuration;
using Utils.CharacterState;
using Utils.Singleton;
using App.Shared.GameModules.Weapon;
using App.Shared;
using Shared.Scripts;
using UnityEngine.AI;

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

            var contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            var playerEntity = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;
            var weaponController = playerEntity.WeaponController();
            if (playerEntity != null)
            {
                if (data.Ks[0] == 1)
                {
                    var config = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(data.Ins[0]);
                    if (null != config)
                    {
                        var roleId = playerEntity.playerInfo.RoleModelId;
                        var role = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleId);
                        if(null != role)
                        {
                            switch ((Sex)role.Sex)
                            {
                                case Sex.Male:
                                    playerEntity.appearanceInterface.Appearance.ChangeAvatar(config.Res);
                                    break;
                                case Sex.Female:
                                    playerEntity.appearanceInterface.Appearance.ChangeAvatar(config.FRes);
                                    break;
                            }
                        }
                   } 
                }
                else if (data.Ks[0] == 2)
                {
                    if (data.Ins[1] < 0)
                    {
                        weaponController.PickUpWeapon(WeaponUtil.CreateScan(data.Ins[0]));
                    }
                    else
                    {
                        weaponController.DropWeapon((EWeaponSlotType)data.Ins[1]);

                        var scanVal = WeaponUtil.CreateScan(data.Ins[0], (val)=> val.Bullet =30);
                        weaponController.ReplaceWeaponToSlot( (EWeaponSlotType)data.Ins[1], scanVal);
                        if (playerEntity.stateInterface.State.CanDraw()&& weaponController.HeldSlotType == EWeaponSlotType.None)
                        { 
                            weaponController.TryArmWeapon( (EWeaponSlotType)data.Ins[1]);
                        }
                    }
                }
                else if(data.Ks[0] == 6)
                {
                    weaponController.DropWeapon( (EWeaponSlotType)data.Ins[0]);
                }
                else if (data.Ks[0] == 3)
                {
                    weaponController.SetWeaponPart( (EWeaponSlotType)data.Ins[0], data.Ins[1]);
                }
                else if (data.Ks[0] == 4)
                {
                    weaponController.DeleteWeaponPart( (EWeaponSlotType)data.Ins[0], (EWeaponPartType)data.Ins[1]);
                }
                else if (data.Ks[0] == 5)
                {
                    playerEntity.appearanceInterface.Appearance.ClearAvatar((Wardrobe)data.Ins[0]);
                }else if(data.Ks[0] == 8)
                {
                    bool add = data.Bs[0];
                    if (add)
                    {
                        weaponController.TryHoldGrenade(data.Ins[0]);
                    }
                    else
                    {
                        playerEntity.WeaponController().RemoveGreande(data.Ins[0]);
                    }
                }
                else if (data.Ks[0] == 7)
                {
                    int ani = data.Ins[0];
                    if(ani > 100)
                    {
                        switch (ani)
                        {
                            case 101:
                                playerEntity.stateInterface.State.PickUp();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        playerEntity.stateInterface.State.UseProps(ani);
                        weaponController.ForceUnArmHeldWeapon();
                        playerEntity.autoMoveInterface.PlayerAutoMove.StopAutoMove();
                    }
                }
            }
        }
    }
}
