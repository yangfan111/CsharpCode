using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Configuration
{

    public class WeaponAvatarAnimSubResourceHandler : AbstractSubResourceLoadHandler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(WeaponAvatarAnimSubResourceHandler));

        public WeaponAvatarAnimSubResourceHandler()
        {

        }

        protected override bool LoadSubResourcesImpl()
        {
            var config = SingletonManager.Get<WeaponAvatarConfigManager>();
            
            bool hasAsset = false;
            foreach (var asset in config.AnimSubResourceAssetInfos)
            {
                if(AddLoadRequest(asset))
                    _logger.InfoFormat("Add WeaponAvatarAnimSubResource {0}", asset);
                hasAsset = true;
            }

            return hasAsset;
        }

        protected override void OnLoadSuccImpl(UnityObject unityObj)
        {
            _logger.InfoFormat("WeaponAvatarAnimSubResource {0} Loaded Has Object: {1}", unityObj.Address, unityObj.AsObject != null);
            if (unityObj.AsObject == null)
            {
                _logger.ErrorFormat("preload animator controller asset:{0} is loaded failed, the asset is not preload, please check the weapon_avator.xml is correctly config and asset:{0} is exist in assetbundle", unityObj.Address);
            }
            else
            {
                SingletonManager.Get<WeaponAvatarConfigManager>().AddToAssetPool(unityObj.Address, unityObj);
            }
        }
    }
}
