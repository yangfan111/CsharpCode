using App.Shared.Components.Player;
using App.Shared.EntityFactory;
using App.Shared.GameMode;
using App.Shared.GameModules;
using Core;
using Core.Prediction.UserPrediction.Cmd;
using Core.Room;
using Core.Utils;
using System;
using System.Collections.Generic;
using Core.EntityComponent;
using WeaponConfigNs;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="GameModeControllerBase" />
    /// </summary>
    public class GameModeControllerBase : ModuleLogicActivator<GameModeControllerBase>, ISessionMode, IPickupHandler,
                                          IReservedBulletHandler, IWeaponSlotsLibrary
    {
        public IModeProcessListener ProcessListener { get; protected set; }

        public IPickupHandler PickupHandler { get; protected set; }

        public IReservedBulletHandler ReservedBulletHandler { get; protected set; }

        public IWeaponSlotsLibrary SlotLibary { get; protected set; }


        public virtual EWeaponSlotType GetSlotByIndex(int index)
        {
            return EWeaponSlotType.None;
        }

        public void ExchangePlayerWeapon(PlayerEntity player)
        {
            var controller     = player.WeaponController();
            var primeHoldAgent = controller[EWeaponSlotType.PrimeWeapon];
            var scdHoldAgent   = controller[EWeaponSlotType.SecondaryWeapon];
            if (!primeHoldAgent.IsValid() && !scdHoldAgent.IsValid())
                return;
            EntityKey primeKey = primeHoldAgent.WeaponKey;
            EntityKey scdKey   = scdHoldAgent.WeaponKey;
            controller.SyncBagWeapon(EWeaponSlotType.PrimeWeapon, scdKey);
            controller.SyncBagWeapon(EWeaponSlotType.SecondaryWeapon, primeKey);
            player.playerWeaponServerUpdate.UpdateCmdType = (byte) EWeaponUpdateCmdType.ExchangePrimaryAppearance;
        }

        public virtual void RecoverPlayerWeapon(PlayerEntity player, int index = -1)
        {
            if (index > -1)
            {
                player.WeaponController().InitBag(index);
                return;
            }

            if (player.hasPlayerInfo)
                player.WeaponController().InitBag(GetDefaultBagIndex(player.playerInfo));
            else
                player.WeaponController().InitBag(0);
        }

        public virtual void Initialize(Contexts contexts, int modeId)
        {
        }

        public virtual bool CanModeSwitchBag
        {
            get { return true; }
        }

        public virtual bool CanModeAutoReload
        {
            get { return true; }
        }

        /// <summary>
        /// 当前模式Id
        /// </summary>
        public int ModeId { get; private set; }
        //List<PlayerWeaponData> ServerRoomWeaponPreLog(PlayerWeaponBagData[] originList)
        //{
        //    for (int i = 0; i < originList.Length; i++)
        //    {
        //        if (originList[i] == null || originList[i].weaponList == null) continue;
        //        foreach (PlayerWeaponData weapon in originList[i].weaponList)
        //        {
        //            Logger.InfoFormat("Server Origin Data =====> BagIndex:{0}|In:{1}", originList[i].BagIndex, weapon.ToString());

        //        }
        //    }
        //}
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GameModeControllerBase));

        public List<PlayerWeaponBagData> FilterSortedWeaponBagDatas(
            Components.Player.PlayerInfoComponent playerInfoComponent)
        {
            var list = new List<PlayerWeaponBagData>();
            if (playerInfoComponent.JobAttribute == (int) EJobAttribute.EJob_Variant ||
                playerInfoComponent.JobAttribute == (int) EJobAttribute.EJob_Matrix)
            {
                var originList = PlayerEntityFactory.MakeVariantWeaponBag();
                for (int i = 0; i < originList.Length; i++)
                {
                    list.Add(originList[i]);
                }
            }
            else
            {
                var originList = playerInfoComponent.WeaponBags;
                for (int i = 0; i < originList.Length; i++)
                {
                    if (originList[i] == null) continue;
                    Logger.InfoFormat("Server Origin Data =====>add BagIndex:{0}", originList[i].BagIndex);
                    list.Add(originList[i]);
                    //    ServerRoomWeaponPreLog(playerInfoComponent.WeaponBags);
                    //    if ((EGameMode)ModeId == EGameMode.Survival) return null;
                    //    if (playerInfoComponent.WeaponBags == null ||
                    //playerInfoComponent.WeaponBags.Length == 0) return null;
                    //    var valuableBagDatas = new List<PlayerWeaponBagData>(playerInfoComponent.WeaponBags);
                    //    for (int i = valuableBagDatas.Count - 1; i >= 0; i--)
                    //    {
                    //        if (valuableBagDatas[i] == null || valuableBagDatas[i].weaponList.Count == 0 ||
                    //          valuableBagDatas[i].BagIndex > GlobalConst.WeaponBagMaxCount)
                    //        {
                    //            valuableBagDatas.RemoveAt(i);
                    //            continue;
                    //        }
                    //   //     DebugUtil.LogInUnity("Server init bag item:"+valuableBagDatas[i].ToString(),DebugUtil.DebugColor.Blue);
                    //    }
                    //    valuableBagDatas.Sort(ModeUtil.RoomWeaponCompareCmd);

                    //    return valuableBagDatas;
                }
            }

            return list;
        }

        public List<PlayerWeaponBagData> FilterSortedWeaponBagDatas(PlayerEntity player)
        {
            if ((EGameMode) ModeId == EGameMode.Survival || !player.hasPlayerInfo) return null;
            return FilterSortedWeaponBagDatas(player.playerInfo);
        }

        public int GetUsableWeapnBagLength(PlayerEntity player)
        {
            if ((EGameMode) ModeId == EGameMode.Survival || !player.hasPlayerInfo) return 1;
            return GetUsableWeapnBagLength(player.playerInfo);
        }

        public int GetUsableWeapnBagLength(Components.Player.PlayerInfoComponent playerInfoComponent)
        {
            if ((EGameMode) ModeId == EGameMode.Survival || playerInfoComponent.WeaponBags == null ||
                playerInfoComponent.WeaponBags.Length == 0) return 1;
            return playerInfoComponent.WeaponBags.Length;
        }

        public int GetDefaultBagIndex(Components.Player.PlayerInfoComponent playerInfoComponent)
        {
            if (playerInfoComponent == null || playerInfoComponent.WeaponBags == null) return 0;
            int ret = 0;
            foreach (var bagData in playerInfoComponent.WeaponBags)
            {
                if (bagData == null || bagData.weaponList == null) continue;
                if (bagData.weaponList != null && bagData.weaponList.Count > 0)
                    ret = Math.Min(ret, bagData.BagIndex);
            }

            return ret;
        }

        public void SetMode(int modeId)
        {
            ModeId = modeId;
        }

        //public void OnPickup(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        //{
        //    ProcessListener.OnPickup(controller, slot);
        //}

        //public void OnExpend(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        //{
        //    ProcessListener.OnExpend(controller, slot);
        //}

        //public void OnDrop(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        //{
        //    ProcessListener.OnDrop(controller, slot);
        //}


        public int SetReservedBullet(IPlayerWeaponProcessor controller, EWeaponSlotType slot, int count)
        {
            return ReservedBulletHandler.SetReservedBullet(controller, slot, count);
        }

        public int SetReservedBullet(IPlayerWeaponProcessor controller, EBulletCaliber caliber, int count)
        {
            return ReservedBulletHandler.SetReservedBullet(controller, caliber, count);
        }

        public int GetReservedBullet(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
            return ReservedBulletHandler.GetReservedBullet(controller, slot);
        }

        public int GetReservedBullet(IPlayerWeaponProcessor controller, EBulletCaliber caliber)
        {
            return ReservedBulletHandler.GetReservedBullet(controller, caliber);
        }


        public EWeaponSlotType GetWeaponSlotByIndex(int index)
        {
            return SlotLibary.GetWeaponSlotByIndex(index);
        }

        public bool IsSlotValid(EWeaponSlotType slot)
        {
            return SlotLibary.IsSlotValid(slot);
        }

        public EWeaponSlotType[] AvaliableSlots
        {
            get { return SlotLibary.AvaliableSlots; }
        }

        public void SendPickup(IPlayerWeaponProcessor weaponProcessor, int entityId, int itemId, int category,
                               int                    count)
        {
            PickupHandler.SendPickup(weaponProcessor, entityId, itemId, category, count);
            ProcessListener.OnItemPickup(weaponProcessor, itemId, category, count);
        }

        public void SendAutoPickupWeapon(int entityId)
        {
            PickupHandler.SendAutoPickupWeapon(entityId);
        }

        public void AutoPickupWeapon(PlayerEntity player, List<int> sceneEntityValues)
        {
            PickupHandler.AutoPickupWeapon(player, sceneEntityValues);
        }

        public void DoPickup(PlayerEntity player, int sceneEntityValue)
        {
            PickupHandler.DoPickup(player, sceneEntityValue);
        }

        public void Drop(PlayerEntity player, EWeaponSlotType slot, IUserCmd cmd)
        {
            PickupHandler.Drop(player, slot, cmd);
        }

        public void OnSwitch(IPlayerWeaponProcessor controller, int weaponId, EInOrOff op)
        {
            ProcessListener.OnSwitch(controller, weaponId, op);
        }


        //        public void CallBeforeAction(IPlayerWeaponProcessor controller, EPlayerActionType actionType)
        //        {
        //        }

        //        private void HandleRollbackInterrupt(IPlayerWeaponProcessor controller, EPlayerActionType actionType)
        //        {
        //            switch (actionType)
        //            {
        //                case EPlayerActionType.Drive:
        //                    controller.AddVehicleInterrupt();
        //                    break;
        //                case EPlayerActionType.Climp:
        //                    controller.AddClimbInterrupt();
        //                    break;
        //                case EPlayerActionType.Swim:
        //                    controller.AddSwimInterrupt();
        //                    break;
        //                case EPlayerActionType.Prone:
        //                    controller.AddProneInterrupt();
        //                    break;
        //                case EPlayerActionType.PullBolt:
        //                    controller.AddPullboltInterrupt();
        //                    break;
        //            }
        //        }
        //
        //        private void HandleSightInterrupt(IPlayerWeaponProcessor controller, EPlayerActionType actionType)
        //        {
        //                controller.AddSightViewInterrupt();
        //        }
    }
}