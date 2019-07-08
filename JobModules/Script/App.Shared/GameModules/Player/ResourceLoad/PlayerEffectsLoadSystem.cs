using System.Collections.Generic;
using App.Shared.Player;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using Shared.Scripts;
using Shared.Scripts.Effect;
using UnityEngine;
using Utils.Appearance;
using Utils.Appearance.Effects;
using Utils.AssetManager;
using Utils.AssetManager.Converter;

namespace App.Shared.GameModules.Player.ResourceLoad
{
    public class PlayerEffectsLoadSystem : ReactiveResourceLoadSystem<PlayerEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEffectsLoadSystem));
        public static readonly string ModeloffsetName = "ModelOffset";
        private readonly PlayerContext _player;
        
        public PlayerEffectsLoadSystem(Contexts context) : base(context.player)
        {
            _player = context.player;
        }

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.RecycleableAsset.Added());
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        public override void SingleExecute(PlayerEntity player)
        {
            foreach (var asset in _assetInfos)
            {
                AssetManager.LoadAssetAsync(player, asset, OnLoadSucc);
            }
        }

        public void OnLoadSucc(PlayerEntity player,UnityObject unityObj)
        {
            GameObject go = unityObj.AsGameObject;
            var parentTrans = GetParentTransform(player);
            go.transform.SetParent(parentTrans);
            go.transform.localPosition = new Vector3(0, 0, 0);
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            
            var addEffectCurFrame = !player.hasEffects;
            if (addEffectCurFrame)
                player.AddEffects();
            player.effects.AddLocalEffect(unityObj);
            EffectUtility.RegistEffect(player.RootGo().gameObject, unityObj);

            if (addEffectCurFrame)
            {
                RegistAllGodModeEffects(player);
                RegistAllAbstractMono(player);
            }
            
            Logger.InfoFormat("{0} addEffect {1}", player.RootGo().name, unityObj.AsGameObject.name);
        }
        
        private readonly string EffectLoadSite = "Effects";
        private Transform GetParentTransform(PlayerEntity player)
        {
            var root = player.RootGo().transform;
            for (int i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (child.name == EffectLoadSite)
                    return child;
            }
            return null;
        }

        private void RegistAllGodModeEffects(PlayerEntity player)
        {
            player.characterBoneInterface.CharacterBone.HandleAllWeapon(
                o => player.effects.GetEffect(GodModeName).AddGameObject(o) );
            player.characterBoneInterface.CharacterBone.HandleAllAttachments(
                o=>player.effects.GetEffect(GodModeName).AddGameObject(o));
            player.characterBoneInterface.CharacterBone.HandleAllWardrobe(
                o => { player.effects.GetEffect(GodModeName).AddGameObject(o); });
        }

        private void RegistAllAbstractMono(PlayerEntity player)
        {
            player.characterBoneInterface.CharacterBone.HandleSingleWardrobe(Wardrobe.CharacterHead,
                o => { player.effects.AddLocalEffect(o); });
        }
        
        private readonly string GodModeName = "GodModeEffect";
        
        private readonly AssetInfo[] _assetInfos = new AssetInfo[]
        {
            new AssetInfo("effect/common", "GodModeEffect".ToLower()), 
            new AssetInfo("effect/common", "DepthOfFieldEffect".ToLower()), 
        };
    }
}