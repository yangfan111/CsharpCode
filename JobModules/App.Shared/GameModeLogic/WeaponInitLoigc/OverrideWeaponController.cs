using Core.Room;

namespace App.Shared.GameModeLogic.WeaponInitLoigc
{
    public class OverrideWeaponController
    {
        private PlayerWeaponBagData _playerWeaponBagData;
        public PlayerWeaponBagData GetOverridedBagData(PlayerEntity playerEntity, PlayerWeaponBagData playerWeaponBagData)
        {
            if(!playerEntity.hasOverrideBag)
            {
                return playerWeaponBagData;
            }
            if(playerEntity.overrideBag.TacticWeapon > 0)
            {
                if(null == _playerWeaponBagData)
                {
                    _playerWeaponBagData = new PlayerWeaponBagData();
                }
                playerWeaponBagData.CopyTo(_playerWeaponBagData);
                bool replace = false;
                foreach(var weapon in _playerWeaponBagData.weaponList)
                {
                    var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);
                    if(slot == Core.Bag.EWeaponSlotType.TacticWeapon)
                    {
                        weapon.WeaponTplId = playerEntity.overrideBag.TacticWeapon;
                        replace = true;
                    }
                }
                if(!replace)
                {
                    _playerWeaponBagData.weaponList.Add(new PlayerWeaponData
                    {
                        Index = PlayerWeaponBagData.Slot2Index(Core.Bag.EWeaponSlotType.TacticWeapon),
                        WeaponTplId = playerEntity.overrideBag.TacticWeapon,
                    });
                }
                return _playerWeaponBagData;
            }
            return playerWeaponBagData;
        }
    }
}
