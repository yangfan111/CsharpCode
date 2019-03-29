using System.Collections.Generic;
using Utils.Configuration;
using XmlConfig;

namespace Core.Configuration.Terrains
{
    public class TerrainVehicleFrictionConfigManager : AbstractConfigManager<TerrainVehicleFrictionConfigManager>
    {

        private class FrictionKeyComparer : IEqualityComparer<FrictionKey>
        {
            public bool Equals(FrictionKey x, FrictionKey y)
            {
                return x.VehicleId == y.VehicleId && x.TextureType == y.TextureType;
            }

            public int GetHashCode(FrictionKey obj)
            {
                return obj.VehicleId ^ obj.TextureType;
            }
        }

        private Dictionary<FrictionKey, TerrainVehicleFrictionConfigItem> _dictFrictions = new Dictionary<FrictionKey, TerrainVehicleFrictionConfigItem>(new FrictionKeyComparer());

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("terrain vehicle friction config xml is empty !");
                return;
            }
            _dictFrictions.Clear();
            var cfg = XmlConfigParser<TerrainVehicleFrictionConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("terrain vehicle friction config is illegal content : {0}", xml);
                return;
            }
            foreach (var item in cfg.Items)
            {
                var key = new FrictionKey(item.VehicleId, item.TextureType);
                _dictFrictions[key] = item;
            }
        }

        public TerrainVehicleFrictionConfigItem GetFrictionById(int vehicleId, int textureType)
        {
            var key = new FrictionKey(vehicleId, textureType);
            TerrainVehicleFrictionConfigItem configItem;

            if (_dictFrictions.TryGetValue(key, out configItem))
            {
                return configItem;
            }

            return null;
        }
    }
}
