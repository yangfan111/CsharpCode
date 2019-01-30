using System;
using Utils.Appearance;

namespace Core.Appearance
{
    public class WardrobeController : WardrobeControllerBase , ILatestWardrobeState, ICharacterLoadResource
    {
        public WardrobeController(Action bagChanged) : base(bagChanged)
        { }

        public void SyncFromLatestWardrobeState(ILatestWardrobeState state)
        {
            AppearanceUtils.CopyLatestWardrobeState(state, this);
        }

        public void SyncToLatestWardrobeState(ILatestWardrobeState state)
        {
            AppearanceUtils.CopyLatestWardrobeState(this, state);
        }
    }
}
