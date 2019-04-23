using Utils.AssetManager;

namespace Utils.Appearance.PropItem
{
    public class HallPropController : PropControllerBase
    {
        protected override AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler mountHandler)
        {
            return LoadRequestFactory.Create<object>(assetInfo, mountHandler.OnLoadSucc);
        }
    }
}
