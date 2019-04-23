using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class DynamicPredictionErrorCorrectionConfig
    {
        public float LinearDeltaThresholdSq;
        public float LinearInterpAlpha;
        public float LinearRecipFixTime;
        public float AngularDeltaThreshold;
        public float AngularInterpAlpha;
        public float AngularRecipFixTime;
        public float BodySpeedThresholdSq;
        public float BodyPositionThresholdSq;
    }
}
