using Core.Utils;
using Utils.Configuration;

namespace Core.Configuration
{
    public class AssetConfigManager: AbstractConfigManager<AssetConfigManager>
    {
        public override void ParseConfig(string xml)
        {
            Version.Instance.LocalAsset = xml;
        }
    }
}