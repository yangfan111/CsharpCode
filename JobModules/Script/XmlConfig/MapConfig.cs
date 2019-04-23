using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using UnityEngine;

namespace XmlConfig
{
    public enum SpecialZone
    {
        Water
    }

    public class SpecialZoneScope
    {
        public SpecialZone Type;
        public Vector3 MaxPoint;
        public Vector3 MinPoint;
        public Vector3 Rotation;
    }

    public class VehicleBirthPoint
    {
        public int VId;
        public Vector3 Position;
    }

    [XmlInclude(typeof(SceneConfig))]
    [XmlInclude(typeof(LevelConfig))]
    public abstract class AbstractMapConfig
    {
        public int Id;
        public Vector3 PlayerBirthPosition;
        public List<VehicleBirthPoint> VehicleBirthPoints;

        public List<SpecialZoneScope> SpecialZones;

        [XmlElement(ElementName = "AssetBundleName", IsNullable = true)] public string BundleName;
        [XmlElement("MapName")][DefaultValue("")] public string MapName;

        //地图原点坐标，海拔，位置便宜的基准点
        public Vector3 OriginPosition;
        //地图的可见区域大小，一般是大小地图上的显示的区域大小
        public Vector3 Size;
        //大小地图剪影
        public string MapLarge;
        public string MapLargeBundlename;
        public Rect MapOffSet;
        //地图的大小等级等义
        public int MapLevel;
        //小地图上的显示大小
        public float MiniMapShowSize;

        public string Name;
        public string IconBundle;
        public string Icon;
        public string Description;
        public int NewMap;
        public int DefaultPlayerNum;
    }

    [XmlRoot("root")]
    public class MapConfig
    {
        [XmlArray(ElementName = "MapInfo", IsNullable = false)] public AbstractMapConfig[] Items;
    }

    public class LevelConfig: AbstractMapConfig
    {
        [XmlElement("AssetName")] public string AssetName;
        public Vector3 TerrainMin = Vector3.zero;
        public int TerrainSize = 0;
        public int TerrainDimension = 0;
    }

    public class SceneConfig: AbstractMapConfig
    {
        public Vector3 TerrainMin = Vector3.zero;
        public int TerrainSize = 0;
        public int TerrainDimension = 0;
        public string TerrainNamePattern = string.Empty;
        public List<string> AdditiveSceneName = new List<string>();
    }
}
