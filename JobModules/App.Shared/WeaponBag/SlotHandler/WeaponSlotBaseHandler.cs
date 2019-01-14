using Core.Bag;
using Core.Utils;
using App.Shared.Components.Bag;
using Core.GameModeLogic;
using Core.Sound;
using Core;
using App.Shared.Util;

namespace App.Shared.WeaponLogic
{
    /// <summary>
    /// 4 location : ground body hand pacakge
    /// </summary>
    [WeaponSpecies(EWeaponSlotType.PrimeWeapon1)]
    [WeaponSpecies(EWeaponSlotType.PrimeWeapon2)]
    [WeaponSpecies(EWeaponSlotType.SubWeapon)]

    public class WeaponSlotBaseHandler 
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponSlotBaseHandler));
        protected WeaponComponent _weapon;
        protected IPlayerWeaponActionLogic _playerWeaponActionLogic;
        protected IReservedBulletLogic _reservedBulletController;
        protected int lastStuffedId;
        public WeaponSlotBaseHandler()
        {
        }
        public virtual void OnCost(WeaponBagLogic bagLogic,System.Action<EWeaponSlotType> callback)
        {
            var bullet = bagLogic.GetWeaponBullet();
            bagLogic.SetWeaponBullet(bullet - 1);
        }
        /// <summary>
        /// 装备槽位填充完成
        /// </summary>
        /// <returns></returns>
        public virtual Err_WeaponLogicErrCode OnStuffEnd(int lastId)
        {
            lastStuffedId = lastId;
            return Err_WeaponLogicErrCode.Sucess;
        }
        public virtual int PickNextInstance()
        {
            return -1;
        }

        /// <summary>
        /// weapon from body, hand to ground
        /// </summary>
        public virtual Err_WeaponLogicErrCode OnDrop()
        {
            _playerWeaponActionLogic.RemoveWeapon(_weapon.GetSlotType());
            return Err_WeaponLogicErrCode.Sucess;
        }

        /// <summary>
        /// wewapon from body to hand
        /// </summary>
        public virtual Err_WeaponLogicErrCode OnSwitchIn(EWeaponSlotType from,System.Action<EWeaponSlotType,int> onSameSpeciesSwitch)
        {
            var slot = _weapon.GetSlotType();
            if (_weapon.Id < 1)
            {
                if(Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("no weapon in slot {0}", slot);
                }
                return Err_WeaponLogicErrCode.Err_IdNotExisted;
            }
         
            if (slot == from)
            {
                // 如果要切换到的位置和当前位置一致不作处理
                return Err_WeaponLogicErrCode.Err_SameWeaponPosition;
            }

            // 如果切换到的位置有枪，设置当前武器
            if (_weapon.Id > 0)
            {
                _playerWeaponActionLogic.DrawWeapon(slot);
                return Err_WeaponLogicErrCode.Sucess;
           //     Core.Audio.GameAudioMedium.PerformOnGunSwitch(_weapon.Id);
               // _playerSoundManager.PlayOnce(XmlConfig.EPlayerSoundType.ChangeWeapon);
            }
            return Err_WeaponLogicErrCode.Err_Default;
        }
    }
}
