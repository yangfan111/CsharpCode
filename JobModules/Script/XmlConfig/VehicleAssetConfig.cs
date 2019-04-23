using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace XmlConfig
{
    public enum EVehicleType
    {
        Car = 0,
        Ship,
    }

    public class VehicleAssetConfigItem
    {
        public int Id { get; set; }
        public EVehicleType Type { get; set; }
        public string BundleName { get; set; }
        public string AssetName { get; set; }
        public int PostureId { get; set; }
        public string TipName { get; set; }

        public bool HasRadio { get; set; }

        public Vector3 CameraAnchorOffset { get; set; }
        public float   CameraDistance { get; set; }
        public float   CameraRotationDamping { get; set; }
    }

    public class VehicleAssetConfig
    {
        public VehicleAssetConfigItem[] Items;
    }
}
