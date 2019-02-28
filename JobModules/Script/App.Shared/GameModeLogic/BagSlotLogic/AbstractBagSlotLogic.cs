using Core;
using Core;
using Core.Utils;

namespace App.Shared.GameModeLogic.BagSlotLogic
{
    public abstract class AbstractBagSlotLogic : IBagSlotLogic
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(AbstractBagSlotLogic));

        public virtual EWeaponSlotType GetSlotByIndex(int index)
        {
            if (index < 0 && index >= (int)EWeaponSlotType.Length)
            {
                return EWeaponSlotType.None;
            }   
            switch (index)
            {
                case 2:
                    return EWeaponSlotType.None;
                default:
                    return (EWeaponSlotType)index;
            }
        }
    }
}
