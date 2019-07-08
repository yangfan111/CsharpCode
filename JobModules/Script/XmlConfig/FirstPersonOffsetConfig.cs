using System.Xml.Serialization;
using WeaponConfigNs;
using System.Collections.Generic;

namespace XmlConfig
{
    [XmlType("OffsetItems")]
    public class FirstPersonOffsetItem
    {
        public int Id;
        public List<float> Offset1;
        public List<float> Offset2;
        public List<float> Offset3;
        public List<int> OffsetRatios1;
        public List<int> OffsetRatios2;
        public List<int> OffsetRatios3;
        public List<float> FirstPersonRotationOffset1;
        public List<float> FirstPersonRotationOffset2;
        public List<float> FirstPersonRotationOffset3;
        public List<int> FirstPersonRotationOffsetRatios1;
        public List<int> FirstPersonRotationOffsetRatios2;
        public List<int> FirstPersonRotationOffsetRatios3;
        public List<float> SightOffset1;
        public List<float> SightOffset2;
        public List<float> SightOffset3;
        public List<int> SightOffsetRatio1;
        public List<int> SightOffsetRatio2;
        public List<int> SightOffsetRatio3;

        public float HorizontalUpperLimit;
        public float HorizontalRestoreVel;
        public float HorizontalVelCoefficient;
        public float VerticalUpperLimit;
        public float VerticalRestoreVel;
        public float VerticalVelCoefficient;

        public float SightHorizontalUpperLimit;
        public float SightHorizontalRestoreVel;
        public float SightHorizontalVelCoefficient;
        public float SightVerticalUpperLimit;
        public float SightVerticalRestoreVel;
        public float SightVerticalVelCoefficient;
    }

    [XmlRoot("root")]
    public class FirstPersonOffsetConfig
    {
        public FirstPersonOffsetItem[] Items;
        public readonly static float[] ScreenRatios = {
            0,  //
            1024f / 768f,
            1152f / 864f,
            1280f / 720f,
            1280f / 768f,
            1280f / 800f,
            1280f / 960f,
            1280f / 1024f,
            1360f / 768f,
            1440f / 900f,
            1600f / 900f,
            1600f / 1024f,
            1680f / 1050f,
            1920f / 1080f
        };
    }
}
