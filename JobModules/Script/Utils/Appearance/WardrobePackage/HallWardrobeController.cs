using System;
using Utils.AssetManager;

namespace Utils.Appearance.WardrobePackage
{
    public class HallWardrobeController : WardrobeControllerBase
    {
        public HallWardrobeController(Action bagChanged) : base(bagChanged)
        {
        }
        
        protected override AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler loadedHanlder)
        {
            return LoadRequestFactory.Create<object>(assetInfo, loadedHanlder.OnLoadSucc);
        }
    }
}
