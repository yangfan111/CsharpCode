using Core;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="PlayerWeaponController" />
    /// </summary>
    public partial class PlayerWeaponController
    {
        private readonly WeaponComponentsAgent[,] slotWeaponAgents;

        private WeaponComponentsAgent heldWeaponAgent;

        public WeaponComponentsAgent HeldWeaponAgent
        {
            get
            {
                if (heldWeaponAgent == null)
                    UpdateHeldWeaponAgent();
                return heldWeaponAgent;
            }
        }

        private void UpdateHeldWeaponAgent()
        {
            heldWeaponAgent = CreateGetWeaponAgent(HeldBagPointer, HeldSlotType);
        }

        public PlayerWeaponController()
        {

            slotsAux = new WeaponSlotsAux();
            slotWeaponAgents = new WeaponComponentsAgent[GameGlobalConst.WeaponBagMaxCount, GameGlobalConst.WeaponSlotMaxLength];
        }

        public WeaponComponentsAgent GetWeaponAgent(EWeaponSlotType slotType = EWeaponSlotType.Pointer, int bagIndex = -1)
        {
            if (bagIndex < 0) bagIndex = HeldBagPointer;
            if (slotType == EWeaponSlotType.Pointer) slotType = HeldSlotType;
            return CreateGetWeaponAgent(bagIndex, slotType);
        }

        private WeaponComponentsAgent CreateGetWeaponAgent(int bagIndex, EWeaponSlotType slotType)
        {
            if (slotWeaponAgents[bagIndex, (int)slotType] == null)
            {
                var newAgent = new WeaponComponentsAgent();
                playerWeaponAgent.AddSlotWeaponListener(slotType, bagIndex, newAgent.Sync);
                slotWeaponAgents[bagIndex, (int)slotType] = newAgent;

            }
            return slotWeaponAgents[bagIndex, (int)slotType];
        }
    }
}
