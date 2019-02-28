using System;
using App.Shared.Components.Player;
using Shared.Scripts;
using Utils.Appearance;
using Utils.AssetManager;
using Utils.CharacterState;

namespace App.Shared.GameModules.Player.Appearance.WardrobeControllerPackage
{
    public class WardrobeController : WardrobeControllerBase, ICharacterLoadResource
    {
        public WardrobeController(Action bagChanged) : base(bagChanged)
        { }

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
            SetLatestWardrobeValue(Wardrobe.Cap, value.Cap);
            SetLatestWardrobeValue(Wardrobe.PendantFace, value.PendantFace);
            SetLatestWardrobeValue(Wardrobe.Inner, value.Inner);
            SetLatestWardrobeValue(Wardrobe.Armor, value.Armor);
            SetLatestWardrobeValue(Wardrobe.Outer, value.Outer);
            SetLatestWardrobeValue(Wardrobe.Glove, value.Glove);
            SetLatestWardrobeValue(Wardrobe.Waist, value.Waist);
            SetLatestWardrobeValue(Wardrobe.Trouser, value.Trouser);
            SetLatestWardrobeValue(Wardrobe.Foot, value.Foot);
            SetLatestWardrobeValue(Wardrobe.Bag, value.Bag);
            SetLatestWardrobeValue(Wardrobe.Entirety, value.Entirety);
            SetLatestWardrobeValue(Wardrobe.CharacterHair, value.CharacterHair);
            SetLatestWardrobeValue(Wardrobe.CharacterHead, value.CharacterHead);
            SetLatestWardrobeValue(Wardrobe.CharacterGlove, value.CharacterGlove);
            SetLatestWardrobeValue(Wardrobe.CharacterInner, value.CharacterInner);
            SetLatestWardrobeValue(Wardrobe.CharacterTrouser, value.CharacterTrouser);
            SetLatestWardrobeValue(Wardrobe.CharacterFoot, value.CharacterFoot);
        }

        private void CopyToLatestWardrobeComponent(LatestAppearanceComponent value)
        {
            if(null == value) return;
            value.Cap = GetLatestWardrobeValue(Wardrobe.Cap);
            value.PendantFace = GetLatestWardrobeValue(Wardrobe.PendantFace);
            value.Inner = GetLatestWardrobeValue(Wardrobe.Inner);
            value.Armor = GetLatestWardrobeValue(Wardrobe.Armor);
            value.Outer = GetLatestWardrobeValue(Wardrobe.Outer);
            value.Glove = GetLatestWardrobeValue(Wardrobe.Glove);
            value.Waist = GetLatestWardrobeValue(Wardrobe.Waist);
            value.Trouser = GetLatestWardrobeValue(Wardrobe.Trouser);
            value.Foot = GetLatestWardrobeValue(Wardrobe.Foot);
            value.Bag = GetLatestWardrobeValue(Wardrobe.Bag);
            value.Entirety = GetLatestWardrobeValue(Wardrobe.Entirety);
            value.CharacterHair = GetLatestWardrobeValue(Wardrobe.CharacterHair);
            value.CharacterHead = GetLatestWardrobeValue(Wardrobe.CharacterHead);
            value.CharacterGlove = GetLatestWardrobeValue(Wardrobe.CharacterGlove);
            value.CharacterInner = GetLatestWardrobeValue(Wardrobe.CharacterInner);
            value.CharacterTrouser = GetLatestWardrobeValue(Wardrobe.CharacterTrouser);
            value.CharacterFoot = GetLatestWardrobeValue(Wardrobe.CharacterFoot);
        }

        #endregion
        
    }
}
