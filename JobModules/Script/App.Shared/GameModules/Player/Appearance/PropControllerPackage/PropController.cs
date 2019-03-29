using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using Utils.Appearance;
using Utils.Appearance.PropItem;
using Utils.AssetManager;

namespace App.Shared.GameModules.Player.Appearance.PropControllerPackage
{
    public class PropController : PropControllerBase, ICharacterLoadResource
    {
        public void SyncFromLatestComponent(LatestAppearanceComponent value)
        {
            CopyFromLatestWardrobeComponent(value);
        }

        public void SyncToLatestComponent(LatestAppearanceComponent value)
        {
            CopyToLatestWardrobeComponent(value);
        }

        protected override AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler mountHandler)
        {
            return LoadRequestFactory.Create<PlayerEntity>(assetInfo, mountHandler.OnLoadSucc);
        }

        #region Helper

        private void CopyFromLatestWardrobeComponent(LatestAppearanceComponent value)
        {
            if(null == value) return;
            SetPropIdValue(value.PropId);
        }

        private void CopyToLatestWardrobeComponent(LatestAppearanceComponent value)
        {
            if(null == value) return;
            value.PropId = GetPropIdValue();
        }

        #endregion


    }
}
