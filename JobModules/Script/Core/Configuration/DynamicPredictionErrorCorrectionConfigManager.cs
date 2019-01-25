using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using XmlConfig;

namespace Core.Configuration
{
    public class DynamicPredictionErrorCorrectionConfigManager : AbstractConfigManager<DynamicPredictionErrorCorrectionConfigManager>
    {
        public float LinearDeltaThresholdSq = 5.0f;
        public float LinearInterpAlpha = 0.2f;
        public float LinearRecipFixTime = 1.0f;
        public float AngularDeltaThreshold = (float) (0.2f * Math.PI);
        public float AngularInterpAlpha = 0.1f;
        public float AngularRecipFixTime = 0.5f;
        public float BodySpeedThresholdSq = 0.2f;
        public float BodyPositionThresholdSq = 0.1f;

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.ErrorFormat("dynamic prediction error correction config is Empty");
                return;
            }

            var cfg = XmlConfigParser<DynamicPredictionErrorCorrectionConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("dynamic prediction error correction config is illegal content:{0}", xml);
                return;
            }

            LinearDeltaThresholdSq = cfg.LinearDeltaThresholdSq;
            LinearInterpAlpha = cfg.LinearInterpAlpha;
            LinearRecipFixTime = cfg.LinearRecipFixTime;
            AngularDeltaThreshold = cfg.AngularDeltaThreshold;
            AngularInterpAlpha = cfg.AngularInterpAlpha;
            AngularRecipFixTime = cfg.AngularRecipFixTime;
            BodySpeedThresholdSq = cfg.BodySpeedThresholdSq;
            BodyPositionThresholdSq = cfg.BodyPositionThresholdSq;
        }
    }
}
