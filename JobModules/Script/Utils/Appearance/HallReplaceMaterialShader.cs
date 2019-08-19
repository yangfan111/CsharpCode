using Utils.AssetManager;

namespace Utils.Appearance
{
    public class HallReplaceMaterialShader : ReplaceMaterialShaderBase
    {
        protected override AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler loadedHanlder)
        {
            return LoadRequestFactory.Create<object>(assetInfo, loadedHanlder.OnLoadSuccess);
        }
    }
}
