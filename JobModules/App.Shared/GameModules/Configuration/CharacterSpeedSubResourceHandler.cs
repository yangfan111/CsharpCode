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

        public CharacterSpeedSubResourceHandler() : base()
        {
        }

        protected override bool LoadSubResourcesImpl()
        {
            var config = SingletonManager.Get<CharacterStateConfigManager>();
            AddLoadRequest(config.AirMoveCurveAssetInfo);
            return true;
        }

        protected override void OnLoadSuccImpl(AssetInfo assetInfo, Object obj)
        {
            var asset = obj as TextAsset;
            if (null != asset)
            {
                var config = XmlConfigParser<SerializableCurve>.Load(asset.text);
                SingletonManager.Get<CharacterStateConfigManager>().AirMoveCurve = config.toCurve();
            }
        }
    }
}