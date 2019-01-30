using Core;
using Core.GameModeLogic;
using Entitas;
using System;
using System.Collections.Generic;

namespace App.Shared.GameModules.Weapon
{
    public class WeaponSlotsLibrary : IWeaponSlotsLibrary
    {

        private static readonly EWeaponSlotType[] DefaultSlotIndices = new EWeaponSlotType[]
{
            EWeaponSlotType.PrimeWeapon1,
            EWeaponSlotType.PrimeWeapon2,
            EWeaponSlotType.SubWeapon,
            EWeaponSlotType.MeleeWeapon,
            EWeaponSlotType.GrenadeWeapon,
};
        private static readonly EWeaponSlotType[] GroupSlotIndices = new EWeaponSlotType[]
{
            EWeaponSlotType.PrimeWeapon1,
            EWeaponSlotType.SubWeapon,
            EWeaponSlotType.GrenadeWeapon,
            EWeaponSlotType.MeleeWeapon,
            EWeaponSlotType.TacticWeapon
};
        public static WeaponSlotsLibrary Allocate(EWeaponSlotsGroupType groupType)
        {
            //EWeaponSlotType[] indices = null;
            switch (groupType)
            {
                case EWeaponSlotsGroupType.Default:
                    return new WeaponSlotsLibrary(DefaultSlotIndices);
                default:
                    return new WeaponSlotsLibrary(GroupSlotIndices);
            }
        }
        public static readonly Dictionary<EWeaponSlotType, Action<PlayerEntity>> AllAddWeaponComponentDic = new Dictionary<EWeaponSlotType, Action<PlayerEntity>>()
            {
                {EWeaponSlotType.PrimeWeapon1, (player)=>  player.AddPrimeWeapon() },
                {EWeaponSlotType.PrimeWeapon2, (player)=>  player.AddSubWeapon() },
                {EWeaponSlotType.SubWeapon, (player)=>  player.AddPistol() },
                {EWeaponSlotType.MeleeWeapon, (player)=>  player.AddMelee() },
                {EWeaponSlotType.GrenadeWeapon, (player)=>  {
                    player.AddGrenade();
                    player.AddGrenadeCacheData(0, 0, 0, 0); } },
                {EWeaponSlotType.TacticWeapon, (player)=>  player.AddTacticWeapon() },
            };
        private EWeaponSlotType[] availableIndices;

        public EWeaponSlotType[] AvaliableSlots { get { return availableIndices; } }

        public WeaponSlotsLibrary(EWeaponSlotType[] in_indices)
        {
            availableIndices = in_indices;
        }
        public bool IsSlotValid(EWeaponSlotType slot)
        {
            for (int i = 0; i < availableIndices.Length; i++)
            {
                if (slot == availableIndices[i])
                {
                    return true;
                }
            }
            return false;
        }
        public EWeaponSlotType GetWeaponSlotByIndex(int index)
        {
            if (index > availableIndices.Length - 1 || index < 0)
            {
                return EWeaponSlotType.None;
            }
            return availableIndices[index];
        }

        public void AllocateWeaponComponents(Entity entity)
        {
            var playerEntity = entity as PlayerEntity;
            for (int i = 0; i < AvaliableSlots.Length; i++)
            {
                Action<PlayerEntity> addWeaponComponent;
                if (!AllAddWeaponComponentDic.TryGetValue(AvaliableSlots[i], out addWeaponComponent))
                    continue;
                addWeaponComponent(playerEntity);
            }
        }
    }
}