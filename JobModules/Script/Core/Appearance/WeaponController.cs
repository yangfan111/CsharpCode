using Core.CharacterState;
using Core.Compare;
using Core.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;
using Core.Utils;
using XmlConfig;
using Utils.Appearance;

namespace Core.Appearance
{
    public class WeaponController : WeaponControllerBase, ILatestWeaponState, IPredictedPlaybackWeaponState, ICharacterLoadResource
    {
        public WeaponController()
        {
        }

        public void SyncFromLatestWeaponState(ILatestWeaponState state)
        {
            AppearanceUtils.CopyLatestWeaponState(state, this);
        }

        public void SyncToLatestWeaponState(ILatestWeaponState state)
        {
            AppearanceUtils.CopyLatestWeaponState(this, state);
        }

        public void SyncFromPredictedWeaponState(IPredictedPlaybackWeaponState state)
        {
            AppearanceUtils.CopyPredictedWeaponState(state, this);
        }

        public void SyncToPredictedWeaponState(IPredictedPlaybackWeaponState state)
        {
            AppearanceUtils.CopyPredictedWeaponState(this, state);
        }

        public void SetWeaponChangedCallBack(Action<GameObject, GameObject> callBack)
        {
            _weaponChangedCallBack = callBack;
        }

        public void SetCacheChangeAction(Action cacheChangeAction)
        {
            _cacheChangeAction = cacheChangeAction;
        }
    }
}
