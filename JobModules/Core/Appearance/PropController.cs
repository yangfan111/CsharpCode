using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Appearance;
using Utils.Appearance.PropItem;

namespace Core.Appearance
{
    public class PropController : PropControllerBase, ILastestPropState, ICharacterLoadResource
    {
        public void SyncFromLatestPropState(ILastestPropState state)
        {
            AppearanceUtils.CopyLatestPropState(state, this);
        }

        public void SyncToLatestPropState(ILastestPropState state)
        {
            AppearanceUtils.CopyLatestPropState(this, state);
        }
    }
}
