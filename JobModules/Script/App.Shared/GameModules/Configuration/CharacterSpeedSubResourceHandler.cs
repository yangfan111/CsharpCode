using System.Collections.Generic;
using System.Linq;
using Core.CharacterController;
using Core.Utils;
using Sharpen;
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

        protected override void OnLoadSuccImpl(UnityObject unityObj)
        {
            var asset = unityObj.As<TextAsset>();
            if (null != asset)
            {
                var config = XmlConfigParser<SpeedCurveConfig>.Load(asset.text);
                SingletonManager.Get<CharacterStateConfigManager>().AirMoveCurve = config.AirMoveCurve.toCurve();
                List<MovementCurveInfo> movementCurve = new List<MovementCurveInfo>();
                foreach (var info in config.MovementCurveInfos)
                {
                    movementCurve.Add(info.ToMovementCurveInfo());
                }
                SingletonManager.Get<CharacterStateConfigManager>().MovementCurve = movementCurve;
            }
        }
    }
}