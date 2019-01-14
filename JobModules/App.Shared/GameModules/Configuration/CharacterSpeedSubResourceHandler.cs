using System.Collections.Generic;
using System.Linq;
using Core.CharacterController;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Configuration
{
    public class CharacterSpeedSubResourceHandler: AbstractSubResourceLoadHandler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(CharacterSpeedSubResourceHandler));

        public CharacterSpeedSubResourceHandler(ILoadRequestManager loadRequestManager) : base(loadRequestManager)
        {
        }

        protected override void LoadSubResourcesImpl(List<Tuple<AssetInfo, Object>> subResources)
        {
            var obj = subResources[0].Item2;
            var assetInfos = GetAssetInfos(obj);
            if (assetInfos != null)
            {
                foreach (AssetInfo assetInfo in assetInfos)
                {
                    AddLoadRequest(assetInfo);
                }
            }
        }

        private List<AssetInfo> GetAssetInfos(UnityEngine.Object obj)
        {
            var asset = obj as TextAsset;
            if (null != asset)
            {
                var config = XmlConfigParser<CharacterStateConfig>.Load(asset.text);
                HashSet<AssetInfo> assetInfos = new HashSet<AssetInfo>(AssetInfo.AssetInfoComparer.Instance);
                assetInfos.Add(new AssetInfo(config.JumpCurveInfo.BundleName, config.JumpCurveInfo.AssetName));

                return assetInfos.ToList();
            }

            return null;
        }

        protected override void OnLoadSuccImpl(AssetInfo assetInfo, Object obj)
        {
            var asset = obj as TextAsset;
            if (null != asset)
            {
                var config = XmlConfigParser<SerializableCurve>.Load(asset.text);
                SingletonManager.Get<CharacterStateConfigManager>().AirMoveCurve = config.toCurve();
            }
            
            ForceExit();
        }
    }
}