using Core.Bag;
using Core.GameModeLogic;
using Core.Utils;
using Entitas;
using System;
using System.Collections.Generic;

namespace App.Shared.WeaponLogic
{
    public class DefaultWeaponSlotController : IWeaponSlotController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultWeaponSlotController));
        private readonly EWeaponSlotType[] _slots = new EWeaponSlotType[]
        {
            EWeaponSlotType.PrimeWeapon1,
            EWeaponSlotType.PrimeWeapon2,
            EWeaponSlotType.SubWeapon,
            EWeaponSlotType.MeleeWeapon,
            EWeaponSlotType.GrenadeWeapon,
        };

        public virtual EWeaponSlotType[] AvaliableSlots
        {
            get
            {
                return _slots;
            }
        }

        protected Dictionary<EWeaponSlotType, Action<PlayerEntity>> AllAddWeaponComponentDic = new Dictionary<EWeaponSlotType, Action<PlayerEntity>>()
        {
            {EWeaponSlotType.PrimeWeapon1, (player)=>  player.AddPrimeWeapon() },
            {EWeaponSlotType.PrimeWeapon2, (player)=>  player.AddSubWeapon() },
            {EWeaponSlotType.SubWeapon, (player)=>  player.AddPistol() },
            {EWeaponSlotType.MeleeWeapon, (player)=>  player.AddMelee() },
            {EWeaponSlotType.GrenadeWeapon, (player)=>  {
                player.AddGrenade();
                player.AddGrenadeInventoryData(0, 0, 0, 0); } },
            {EWeaponSlotType.TacticWeapon, (player)=>  player.AddTacticWeapon() },
        };

        public DefaultWeaponSlotController()
        {
        }

        public EWeaponSlotType GetWeaponSlotByIndex(int index)
        {
            if(index > AvaliableSlots.Length - 1 || index < 0)
            {
                return EWeaponSlotType.None;
            }
            return AvaliableSlots[index];
        }

        public bool IsSlotValid(EWeaponSlotType slot)
        {
            for(int i = 0; i < AvaliableSlots.Length; i++)
            {
                if(slot == AvaliableSlots[i])
                {
                    return true;
                }
            }
            return false;
        }

        public void InitPlayerWeaponBySlotInfo(Entity entity)
        {
            var playerEntity = entity as PlayerEntity;
            for(int i = 0; i < AvaliableSlots.Length; i++)
            {
                Action<PlayerEntity> addWeaponComponent;
                if (AllAddWeaponComponentDic.TryGetValue(AvaliableSlots[i], out addWeaponComponent))
                {
                    addWeaponComponent(playerEntity);
                }
                else
                {
                    Logger.ErrorFormat("cant add weapon component for {0}", AvaliableSlots[i]);
                }
            }
        }
    }
}
