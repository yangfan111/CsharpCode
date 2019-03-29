using Entitas;

namespace App.Shared.Components.Sound
{
    [Sound]
    public class AssetInfoComponent : IComponent
    {
        public string AssetName;
        public string BundleName;
    }
}
