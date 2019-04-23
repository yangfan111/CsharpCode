using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlType("child")]
    public class TerrainVehicleFrictionConfigItem
    {
        public int VehicleId;
        public int TextureType;
        public float Grip;
        public float Drag;
    }

    public struct FrictionKey
    {
        public int VehicleId;
        public int TextureType;

        public FrictionKey(int vehicleId, int textureType)
        {
            this.VehicleId = vehicleId;
            this.TextureType = textureType;
        }
    }

    [XmlRoot("root")]
    public class TerrainVehicleFrictionConfig
    {
        public TerrainVehicleFrictionConfigItem[] Items;
    }
}
