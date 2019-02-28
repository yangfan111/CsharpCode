using Utils.Appearance;
using Utils.AssetManager;

namespace App.Shared.GameModules.Player.Appearance
{
    public class ReplaceMaterialShader : ReplaceMaterialShaderBase
    {
        protected override AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler mountHandler)
        {
            return LoadRequestFactory.Create<PlayerEntity>(assetInfo, mountHandler.OnLoadSucc);
        }
    }
}