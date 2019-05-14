using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Room;
using Core.Utils;
using System.Collections.Generic;
using App.Shared.Components.Weapon;
using Core.EntityComponent;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameMode
{
    /// <summary>
    /// Defines the <see cref="ServerWeaponInitHandler" />
    /// </summary>
    public class ServerWeaponInitHandler
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerWeaponInitHandler));

        /*/// <summary>
        /// Defines the <see cref="OverrideBagTaticsCacheData" />
        /// </summary>
        private class OverrideBagTaticsCacheData
        {
            private readonly PlayerWeaponBagData _playerWeaponBagData = new PlayerWeaponBagData();

            public PlayerWeaponBagData CombineOverridedBagData(IPlayerWeaponSharedGetter getter, PlayerWeaponBagData playerWeaponBagData)
            {
                playerWeaponBagData.CopyTo(_playerWeaponBagData);
                if (getter.OverrideBagTactic < 1)
                {
                    return _playerWeaponBagData;
                }
                bool replace = false;
                foreach (var weapon in playerWeaponBagData.weaponList)
                {
                    var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);
                    if (slot == EWeaponSlotType.TacticWeapon)
                    {
                        weapon.WeaponTplId = getter.OverrideBagTactic;
                        replace = true;
                    }
                }
                if (!replace)
                {
                    _playerWeaponBagData.weaponList.Add(new PlayerWeaponData
                    {
                        Index = PlayerWeaponBagData.Slot2Index(EWeaponSlotType.TacticWeapon),
                        WeaponTplId = getter.OverrideBagTactic,
                    });
                }
                return _playerWeaponBagData;
            }
        }

        private readonly OverrideBagTaticsCacheData BagTaticsCache = new OverrideBagTaticsCacheData();*/
        

        [System.Obsolete]
        private void TrashOldWeapons(PlayerEntity player)
        {
            var controller = player.WeaponController();
            var bagSetCmp = player.FindBagSetComponent();
                for (EWeaponSlotType j = EWeaponSlotType.None + 1; j < EWeaponSlotType.Length; j++)
                    controller.DestroyWeapon(j, 0);
        }

       
        public void RecoverPlayerWeapon(PlayerEntity player, List<PlayerWeaponBagData> sortedWeaponList, int pointer)
        {
            if (sortedWeaponList == null)
                return;
            //丢弃武器数据
            //      TrashOldWeapons(player);
            //重新初始化武器数据
            GenerateInitialWeapons(player, sortedWeaponList, pointer);
        }
        private List<PlayerWeaponData> CollectGrenadeList(List<PlayerWeaponBagData> sortedList)
        {
            List<PlayerWeaponData> list = new List<PlayerWeaponData>();
            foreach (var value in sortedList)
            {
                foreach (var weapon in value.weaponList)
                {
                    var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);
                    if (slot == EWeaponSlotType.ThrowingWeapon)
                    {
                        list.Add(weapon);
                    }
                }
            }
            //       DebugUtil.MyLog(list.ToString(), DebugUtil.DebugColor.Blue);
            return list;
        }
        [System.Obsolete]
        private void GenerateGrenadeList(List<PlayerWeaponBagData> sortedList,IPlayerWeaponProcessor processor)
        {
            processor.GrenadeHandler.ClearCache();
            foreach (var value in sortedList)
            {
                foreach (var weapon in value.weaponList)
                {
                    var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);
                    if (slot == EWeaponSlotType.ThrowingWeapon)
                    {
                        processor.GrenadeHandler.AddCache(weapon.WeaponTplId);
                    }
                }
            }
        }


        private void GenerateInitialWeapons(PlayerEntity player, List<PlayerWeaponBagData> sortedWeaponList,int pointer)
        {
            
         //   GenerateGrenadeList(sortedWeaponList, player.WeaponController());
            for (int i = 0; i < sortedWeaponList.Count; i++)
            {
                if (pointer == sortedWeaponList[i].BagIndex)
                {
                    GenerateBagWeaponByIndex(sortedWeaponList[i], player.WeaponController(), player.playerWeaponServerUpdate.ReservedWeaponSubType);
                    break;
                }
            }
            player.playerWeaponServerUpdate.UpdateCmdType = (byte)EWeaponUpdateCmdType.UpdateHoldAppearance;
             
            //defaultBagFstSlot =processor.PollGetLastSlotType();
            //DebugUtil.MyLog("defaultBagFstSlot:" + defaultBagFstSlot,DebugUtil.DebugColor.Blue);
            //processor.TryArmWeapon(defaultBagFstSlot);
        }
        

        private void GenerateBagWeaponByIndex(PlayerWeaponBagData srcBagData, IPlayerWeaponProcessor controller, List<int> reservedSubType)
        {
            var removedList = new List<EWeaponSlotType>();
            for (EWeaponSlotType j = EWeaponSlotType.None + 1; j < EWeaponSlotType.Length; j++)
            {
                removedList.Add(j);
            }
            controller.GrenadeHandler.ClearCache();
            foreach (PlayerWeaponData weapon in srcBagData.weaponList)
            {
                Logger.InfoFormat("[[[[[ServerInitialize data]]]]] BagIndex:{0}|In:{1}" , srcBagData.BagIndex,weapon.ToString());
                var weaponAllConfig =SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weapon.WeaponTplId);
                bool needReserved = true;
                if (null != reservedSubType && reservedSubType.Count != 0)
                {
                    if (weaponAllConfig.NewWeaponCfg.Type == (int) EWeaponType_Config.ThrowWeapon)
                        needReserved = reservedSubType.Contains((int) EWeaponSubType.Throw);
                    else
                        needReserved = reservedSubType.Contains(weaponAllConfig.NewWeaponCfg.SubType);
                }
                if(!needReserved)
                    continue;

                var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);
          
                var weaponType = (EWeaponType_Config)weaponAllConfig.NewWeaponCfg.Type;
                if (weaponType != EWeaponType_Config.ThrowWeapon)
                {
                    var orient = WeaponUtil.CreateScan(weapon);
                    orient.Bullet = weaponAllConfig.PropertyCfg.Bullet;
                    orient.ReservedBullet = weaponAllConfig.PropertyCfg.Bulletmax;
                    if (orient.Magazine > 0)
                    {
                        orient.Bullet += SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(orient.Magazine).Bullet;
                    }
                    controller.ReplaceWeaponToSlot(slot, srcBagData.BagIndex, orient);
                }
                else
                {
                    controller.GrenadeHandler.AddCache(weapon.WeaponTplId);
                }
                removedList.Remove(slot);
            }
            foreach (var i in removedList)
            {
                controller.DestroyWeapon(i, 0);
            }
        }
    }
}
//var controller = player.WeaponController();

//playerWeaponAgent.HeldBagPointer = index;
//            List<PlayerWeaponBagData> bagDatas = ModeController.FilterSortedWeaponBagDatas(RelatedPlayerInfo);
//PlayerWeaponBagData tarBag = null;
//            foreach (var bagData in bagDatas)
//            {
//                if (bagData.BagIndex == index)
//                {
//                    tarBag = bagData;
//                    break;
//                }
//            }
//            if (tarBag == null) return;
//            var removedList = new List<EWeaponSlotType>();
//            for (EWeaponSlotType j = EWeaponSlotType.None + 1; j<EWeaponSlotType.Length; j++)
//            {
//                if(j!= EWeaponSlotType.ThrowingWeapon && j!= EWeaponSlotType.TacticWeapon)
//                    removedList.Add(j);
//            }
//            foreach (var weapon in tarBag.weaponList)
//            {
//                var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);

//var weaponAllConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weapon.WeaponTplId);
//var weaponType = (EWeaponType_Config)weaponAllConfig.NewWeaponCfg.Type;
//                if (slot != EWeaponSlotType.ThrowingWeapon && slot != EWeaponSlotType.TacticWeapon)
//                {
//                    var orient = WeaponUtil.CreateScan(weapon);
//orient.Bullet = weaponAllConfig.PropertyCfg.Bullet;
//                    orient.ReservedBullet = weaponAllConfig.PropertyCfg.Bulletmax;
//                    if (orient.Magazine > 0)
//                    {
//                        orient.Bullet += SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(orient.Magazine).Bullet;
//                    }
//                    ReplaceWeaponToSlot(slot, 0, orient);
//                }
//                removedList.Remove(slot);
//            }
//            foreach(var i in removedList)
//            {
//               DestroyWeapon(i,0);
//            }
//            EWeaponSlotType newSlot = PollGetLastSlotType(false);
//TryArmWeapon(newSlot);