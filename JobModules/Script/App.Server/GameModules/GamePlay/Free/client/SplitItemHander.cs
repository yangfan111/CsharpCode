using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.chicken;
using App.Server.GameModules.GamePlay.Free.item;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Shared;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.item;
using com.wd.free.skill;
using Core.Free;
using Free.framework;
using gameplay.gamerule.free.item;
using gameplay.gamerule.free.rule;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class SplitItemHandler : ParaConstant, IFreeMessageHandler
    {
        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.SplitItem;
        }

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            FreeData fd = (FreeData)player.freeData.FreeData;

            room.FreeArgs.TempUse(PARA_PLAYER_CURRENT, fd);

            string key = message.Ss[0];
            int count = message.Ins[0];

            if (key.StartsWith(ChickenConstant.BagDefault))
            {
                ItemPosition ip = FreeItemManager.GetItemPosition(room.FreeArgs, key, fd.GetFreeInventory().GetInventoryManager());
                FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());

                if (ip.GetCount() > count)
                {
                    if (info.cat == (int) ECategory.Weapon && SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(info.id).Type == (int) EWeaponType_Config.ThrowWeapon)
                    {
                        CarryClipUtil.DeleteGrenade(count, info.id, fd, room.FreeArgs);
                        for (int i = 0; i < count; i++)
                        {
                            player.WeaponController().RemoveGreande(info.id);
                        }
                    }
                    else
                    {
                        ip.SetCount(ip.GetCount() - count);
                        ip.GetInventory().GetInventoryUI().ReDraw((ISkillArgs)room.FreeArgs, ip.GetInventory(), true);
                    }
                }
                else
                {
                    ip.GetInventory().RemoveItem((ISkillArgs)room.FreeArgs, ip);
                }

                room.RoomContexts.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleObjectEntity(
                        (ECategory) info.cat, info.id, count, fd.Player.position.Value);

                if (info.cat == (int) ECategory.GameItem && SingletonManager.Get<GameItemConfigManager>().GetConfigById(info.id).Type == (int) GameItemType.Bullet)
                {
                    player.WeaponController().SetReservedBullet((EBulletCaliber) info.id, CarryClipUtil.GetClipCount(info.id, fd, room.FreeArgs));
                }
            }

            room.FreeArgs.Resume(PARA_PLAYER_CURRENT);
        }

    }
}
