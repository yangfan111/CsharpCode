using Core.Configuration;
using Utils.AssetManager;
using UnityEngine;
using Core.Utils;
using Utils.CharacterState;

namespace App.Client.GameModules.ClientEffect.EffectLogic
{
    internal class MuzzleSparkClientEffect : AbstractClientEffect
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MuzzleSparkClientEffect));

        public override void Initialize(ClientEffectEntity entity)
        {
            base.Initialize(entity);
            var go = (GameObject)entity.assets.LoadedAssets[Asset];
            if(null != go && entity.hasAttachParent)
            {
                var owner = AllContexts.player.GetEntityWithEntityKey(entity.attachParent.ParentKey);
                if(null == owner)
                {
                    Logger.ErrorFormat("on player with entity key {0} !", entity.attachParent.ParentKey);
                    return;
                }
                if(!owner.hasAppearanceInterface)
                {
                    Logger.Error("owner has no appearance interface !");
                    return;
                }
                var appearance = owner.appearanceInterface.Appearance;
                var characterBone = owner.characterBoneInterface.CharacterBone;
                var muzzleTrans = characterBone.GetLocation(SpecialLocation.MuzzleEffectPosition, appearance.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
                if (null != muzzleTrans)
                {
                    go.transform.parent = muzzleTrans;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.forward = muzzleTrans.forward;
                }
                else
                {
                    Logger.Error("muzzle effect anchor transform doesn't exist !");
                }
            }
            else
            {
                if(null == go)
                {
                    Logger.ErrorFormat("no {0} in loadedassets : load fialed ?", Asset);
                }
                else
                {
                    Logger.Error("entity has no parent component");
                }
            }
        }
    }
}
