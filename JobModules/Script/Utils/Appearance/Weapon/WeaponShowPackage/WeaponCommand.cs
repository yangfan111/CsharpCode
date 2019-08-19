using System;
using XmlConfig;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public class WeaponCommand
    {
        private WeaponInPackage _weaponPosition;
        private int _attachment;
        private WeaponPartLocation _attachmentLocation;
        private int _id;
        private int _reloadState;

        private Action _remountWeaponInPackage;
        private Action _changeWeaponOnLocator;
        private Action<int> _reload;
        private Action<WeaponInPackage> _changeWeaponInHand;
        private Action<WeaponInPackage, int> _changeWeaponInPackage;
        private Action<WeaponInPackage, WeaponPartLocation, int> _changeWeaponAttachment;

        public void SetChangeWeaponInHand(Action<WeaponInPackage> action, WeaponInPackage pos)
        {
            _changeWeaponInHand = action;
            _weaponPosition = pos;
        }

        public void SetChangeWeaponInPackage(Action<WeaponInPackage, int> action, WeaponInPackage pos, int id)
        {
            _changeWeaponInPackage = action;
            _weaponPosition = pos;
            _id = id;
        }

        public void SetChangeAttachment(Action<WeaponInPackage, WeaponPartLocation, int> action,
            WeaponInPackage pos,
            WeaponPartLocation location,
            int id)
        {
            _changeWeaponAttachment = action;
            _weaponPosition = pos;
            _attachmentLocation = location;
            _attachment = id;
        }

        public void SetRemountWeaponInPackage(Action action)
        {
            _remountWeaponInPackage = action;
        }

        public void SetChangeWeaponOnLocator(Action action)
        {
            _changeWeaponOnLocator = action;
        }

        public void SetReload(Action<int> action, int reloadState)
        {
            _reload = action;
            _reloadState = reloadState;
        }

        public void Execute()
        {
            if (_changeWeaponInHand != null)
            {
                _changeWeaponInHand.Invoke(_weaponPosition);
                _changeWeaponInHand = null;
            }

            if (_changeWeaponInPackage != null)
            {
                _changeWeaponInPackage.Invoke(_weaponPosition, _id);
                _changeWeaponInPackage = null;
            }

            if (_changeWeaponAttachment != null)
            {
                _changeWeaponAttachment.Invoke(_weaponPosition, _attachmentLocation, _attachment);
                _changeWeaponAttachment = null;
            }

            if (_remountWeaponInPackage != null)
            {
                _remountWeaponInPackage.Invoke();
                _remountWeaponInPackage = null;
            }

            if (null != _changeWeaponOnLocator)
            {
                _changeWeaponOnLocator.Invoke();
                _changeWeaponOnLocator = null;
            }

            if (null != _reload)
            {
                _reload.Invoke(_reloadState);
                _reload = null;
            }
        }
    }
}
