using Core.WeaponLogic;
using System.Collections.Generic;
using Core.CharacterState;
using Core.Utils;
using Core.WeaponLogic.Bullet;
using App.Shared.WeaponLogic.Bullet;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using App.Shared.WeaponLogic.Tactic;
using Core.Free;
using Utils.Singleton;

namespace App.Shared.WeaponLogic
{
    public class WeaponFactory : IWeaponFactory
    {
        private class WeaponAssembly
        {
            public IWeaponLogic BaseLogic;
            public IWeaponSoundLogic Sound;
            public IWeaponEffectLogic Effect;
        }

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponFactory));
        private IWeaponLogicFactory _weaponLogicFactory;
        private IPlayerWeaponState _playerWeaponState;
        private IBulletFireInfoProviderDispatcher _bulletInfoProviderDispatcher;
        private WeaponAssembly _current;
        private IFreeArgs _freeArgs;

        private Dictionary<int, WeaponAssembly> _cache = new Dictionary<int, WeaponAssembly>();

        public void ClearCache()
        {
            _cache.Clear();
        }

        public WeaponFactory(IPlayerWeaponState playerWeaponState, 
            CharacterStateManager characterState, 
            IWeaponLogicFactory weaponLogicFactory,
            IFreeArgs freeArgs)
        {
            _playerWeaponState = playerWeaponState;
            _weaponLogicFactory = weaponLogicFactory;
            _bulletInfoProviderDispatcher = new BulletFireInfoProviderDispatcher(playerWeaponState);
            _freeArgs = freeArgs;
        }

        public IWeaponLogic GetWeaponLogic()
        {
            if(null == _current)
            {
                return null;
            }
            return _current.BaseLogic;
        }

        public IWeaponSoundLogic GetWeaponSoundLogic()
        {
            if(null == _current)
            {
                return null;
            }
            return _current.Sound;
        }

        public IWeaponEffectLogic GetWeaponEffectLogic()
        {
            if(null == _current)
            {
                return null;
            }
            return _current.Effect;
        }

        public IPlayerWeaponState GetPlayerWeaponState()
        {
            return _playerWeaponState;
        }

        public void Prepare(int weaponId)
        {
            if(_cache.ContainsKey(weaponId))
            {
                _current = _cache[weaponId];
                return;
            }
            var weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponId);
            if(null == weaponCfg)
            {
                return;
            }
            _current = new WeaponAssembly();
            if(weaponCfg.Type == (int)EWeaponType.TacticWeapon)
            {
                _current.Sound = null;
                _current.Effect = null;
                _current.BaseLogic = new TacticWeaponLogic(weaponId, _freeArgs);
            }
            else
            {
                _current.Sound = _weaponLogicFactory.CreateWeaponSoundLogic(weaponId, _playerWeaponState);
                _current.Effect = _weaponLogicFactory.CreateWeaponEffectLogic(weaponId);
                _current.BaseLogic = _weaponLogicFactory.CreateWeaponLogic(weaponId, _current.Sound, _current.Effect, _bulletInfoProviderDispatcher);
            }
            _cache[weaponId] = _current;
        }
    }
}
