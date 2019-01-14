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

        public WeaponAvatarAnimSubResourceHandler(ILoadRequestManager loadRequestManager, Action loadedDoneCallBack = null):
            base(loadRequestManager)
        {

        }

        protected override void LoadSubResourcesImpl(List<Tuple<AssetInfo, UnityEngine.Object>> subResources)
        {
            var obj = subResources[0].Item2;
            var assetInfos = GetAssetInfos(obj);
            if (assetInfos != null)
            {
                foreach (var asset in assetInfos)
                {
                    AddLoadRequest(asset);
                }
            }
        }

        protected override void OnLoadSuccImpl(AssetInfo assetInfo, UnityEngine.Object obj)
        {
            if (obj == null)
            {
                _logger.ErrorFormat("preload animator controller asset:{0} is loaded failed, the asset is not preload, please check the weapon_avator.xml is correctly config and asset:{0} is exist in assetbundle", assetInfo);
            }
            else
            {
                SingletonManager.Get<WeaponAvatarConfigManager>().AddToAssetPool(assetInfo, obj);
                ForceExit();
            }
        }

        private List<AssetInfo> GetAssetInfos(UnityEngine.Object obj)
        {
            var asset = obj as TextAsset;
            if (null != asset)
            {
                var config = XmlConfigParser<WeaponAvatarConfig>.Load(asset.text);

                HashSet<AssetInfo> assetInfos = new HashSet<AssetInfo>(AssetInfo.AssetInfoComparer.Instance);
                foreach (var item in config.Items)
                {
                    if (!string.IsNullOrEmpty(item.AnimBundle))
                    {
                        if (!string.IsNullOrEmpty(item.AnimFemaleP1))
                        {
                            assetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimFemaleP1));
                        }
                        if (!string.IsNullOrEmpty(item.AnimFemaleP3))
                        {
                            assetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimFemaleP3));
                        }
                        if (!string.IsNullOrEmpty(item.AnimMaleP1))
                        {
                            assetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimMaleP1));
                        }
                        if (!string.IsNullOrEmpty(item.AnimMaleP3))
                        {
                            assetInfos.Add(new AssetInfo(item.AnimBundle, item.AnimMaleP3));
                        }
                    }


                }
                return assetInfos.ToList();
            }

            return null;
        }
    }
}
