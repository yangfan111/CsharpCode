using Assets.XmlConfig;
using Core.Bag;
using Core.Utils;

namespace App.Shared.GameModeLogic.BagSlotLogic
{
    public class DoublePrimeWeaponBagSlotLogic : AbstractBagSlotLogic 
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DoublePrimeWeaponBagSlotLogic));
        public override EWeaponSlotType GetSlotByIndex(int index)
        {
            if(index <= 0 || index > (int)EWeaponSlotType.Length)
            {
                return EWeaponSlotType.None;
            }
            return (EWeaponSlotType)index;
        }
    }
}
