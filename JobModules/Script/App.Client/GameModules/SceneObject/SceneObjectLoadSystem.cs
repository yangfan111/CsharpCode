using App.Client.GameModules.Effect;
using App.Shared;
using App.Shared.Components.SceneObject;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using System;
using System.Collections.Generic;
using Utils.AssetManager;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;
using AssetInfo = Utils.AssetManager.AssetInfo;

namespace App.Client.GameModules.SceneObject
{
    /// <summary>
    ///     Defines the <see cref="SceneObjectLoadSystem" />
    /// </summary>
    public class SceneObjectLoadSystem : ReactiveResourceLoadSystem<SceneObjectEntity>
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SceneObjectLoadSystem));

        private readonly SimpleLoadRespondHander simpleLoadHandler;

        private readonly LoadWeaponRespondHandler weaponLoadHandler;

        private IUnityAssetManager _assetManager;

        private PlayerContext _playerContext;

        // private static readonly AssetInfo GroundProbFlashAsset = new AssetInfo("effect/common", "GroundPropFlash");
        public SceneObjectLoadSystem(Contexts contexts, ICommonSessionObjects sessionObjects) : base(
        contexts.sceneObject)
        {
            _assetManager     = sessionObjects.AssetManager;
            _playerContext    = contexts.player;
            simpleLoadHandler = new SimpleLoadRespondHander(contexts);
            weaponLoadHandler = new LoadWeaponRespondHandler(contexts);
        }

        protected override ICollector<SceneObjectEntity> GetTrigger(IContext<SceneObjectEntity> context)
        {
            return context.CreateCollector(SceneObjectMatcher.Position.Added());
        }

        protected override bool Filter(SceneObjectEntity entity)
        {
            return entity.hasSimpleItem || entity.hasWeaponObject;
        }

        public override void SingleExecute(SceneObjectEntity sceneObjectEntity)
        {
            if (!sceneObjectEntity.hasEffects)
                sceneObjectEntity.AddEffects();
            
            if (sceneObjectEntity.hasWeaponObject)
                LoadWeaponObject(sceneObjectEntity);
            else if (sceneObjectEntity.hasSimpleItem)
                LoadSimpleItem(sceneObjectEntity);
        }

        private void LoadWeaponObject(SceneObjectEntity sceneObjectEntity)
        {
            var weapon = sceneObjectEntity.weaponObject;
            
            var allConfigs = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weapon.ConfigId);
            if (allConfigs == null)
                return;

            if(WeaponUtil.IsC4p(weapon.ConfigId))
                sceneObjectEntity.AddBombSound(DateTime.Now.Ticks / 10000L, 0);

            var avatarSize = allConfigs.WeaponDefaultAvartar.Size;
            if (avatarSize != 1)
                sceneObjectEntity.AddSize(avatarSize);
            weapon.ApplyParts(allConfigs.DefaultParts);
            Logger.InfoFormat("***Load weapon {0} from object***",allConfigs.S_Id);
            LoadWeaponObject(sceneObjectEntity, allConfigs.NewWeaponCfg);
        }

        private void LoadSimpleItem(SceneObjectEntity sceneObjectEntity)
        {
            var category = sceneObjectEntity.simpleItem.Category;
            var id       = sceneObjectEntity.simpleItem.Id;
            if (id < 1)
            {
                Logger.ErrorFormat("itemid is illegal for item {0}", sceneObjectEntity.entityKey);
                return;
            }

            switch ((ECategory) category)
            {
                case ECategory.Avatar:
                    var avatarCfg = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(id);
                    if (null != avatarCfg)
                    {
                        if (null != _playerContext.flagSelfEntity && _playerContext.flagSelfEntity.hasPlayerInfo)
                        {
                            LoadAvatar(sceneObjectEntity, _playerContext.flagSelfEntity, avatarCfg);
                        }
                    }
                    break;
                case ECategory.WeaponPart:
                    var partId    = SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetDefaultPartBySetId(id);
                    var partCfg   = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(partId);
                    var partAsset = new AssetInfo(partCfg.Bundle, partCfg.Res);
                    if (string.IsNullOrEmpty(partAsset.AssetName) || string.IsNullOrEmpty(partAsset.BundleName))
                    {
                        break;
                    }

                    var partSize = SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetSizeById(id);
                    if (partSize != 1)
                    {
                        sceneObjectEntity.AddSize(partSize);
                    }

                    _assetManager.LoadAssetAsync(sceneObjectEntity, partAsset,
                    simpleLoadHandler.OnLoadUnityObjectSucess);

                    sceneObjectEntity.effects.AddGlobalEffect(GlobalEffectManager.GlobalGroundPropFlash);

                    break;
                case ECategory.GameItem:
                    var itemSsset = SingletonManager.Get<GameItemConfigManager>().GetAssetById(id);
                    if (string.IsNullOrEmpty(itemSsset.AssetName) || string.IsNullOrEmpty(itemSsset.BundleName))
                    {
#if UNITY_EDITOR
                        UnityEngine.Debug.LogError(string.Format("item is not config for id {0}", id));
#else
                        Logger.ErrorFormat("item is not config for id {0}", id);
#endif
                        return;
                    }

                    var itemSize = SingletonManager.Get<GameItemConfigManager>().GetSizeById(id);
                    if (itemSize != 1)
                    {
                        sceneObjectEntity.AddSize(itemSize);
                    }

                    _assetManager.LoadAssetAsync(sceneObjectEntity, itemSsset,
                    simpleLoadHandler.OnLoadUnityObjectSucess);
                    sceneObjectEntity.effects.AddGlobalEffect(GlobalEffectManager.GlobalGroundPropFlash);
                    break;
            }
        }

        private void LoadPartAssetAsync(SceneObjectEntity sceneObjectEntity, AssetInfo assetInfo,
                                        PartAssetData partAssetData)
        {
            sceneObjectEntity.weaponAttachment.PartAssetDict[assetInfo] = partAssetData;
            if (sceneObjectEntity.multiUnityObject.AddLoadingAssets(assetInfo))
            {
                _assetManager.LoadAssetAsync(sceneObjectEntity, assetInfo, weaponLoadHandler.OnLoadPartObjectSucess);
            }
            else
            {
                weaponLoadHandler.OnLoadPartObjectSucess(sceneObjectEntity,
                    sceneObjectEntity.multiUnityObject.LoadedAssets[assetInfo]);
            }
         
        }
        private void LoadNecessaryAssetAsync(SceneObjectEntity sceneObjectEntity,AssetInfo assetInfo)
        {
            if (sceneObjectEntity.multiUnityObject.AddLoadingAssets(assetInfo))
            {
                _assetManager.LoadAssetAsync(sceneObjectEntity, assetInfo, weaponLoadHandler.OnLoadNecessaryObjectSucess);
            }
            else
            {
                weaponLoadHandler.OnLoadNecessaryObjectSucess(sceneObjectEntity,
                    sceneObjectEntity.multiUnityObject.LoadedAssets[assetInfo]);
            }

        }
        private void LoadEffectAssetAsync(SceneObjectEntity sceneObjectEntity,AssetInfo assetInfo)
        {
            if (sceneObjectEntity.multiUnityObject.AddLoadingAssets(assetInfo))
            {
                _assetManager.LoadAssetAsync(sceneObjectEntity, assetInfo, weaponLoadHandler.OnLoadEffectObjetSucess);
            }
            else
            {
                weaponLoadHandler.OnLoadEffectObjetSucess(sceneObjectEntity,
                    sceneObjectEntity.multiUnityObject.LoadedAssets[assetInfo]);
            }

        }
        private void LoadWeaponObject(SceneObjectEntity sceneObjectEntity, WeaponResConfigItem weaponCfg)
        {
            var avatarId = weaponCfg.AvatorId;
            if (sceneObjectEntity.weaponObject.WeaponAvatarId > 0)
                avatarId = sceneObjectEntity.weaponObject.WeaponAvatarId;
            AssetInfo thdPersonAsset =SingletonManager.Get<WeaponAvatarConfigManager>().GetThirdPersonWeaponModel(avatarId);
            if (!sceneObjectEntity.hasMultiUnityObject)
            {
                sceneObjectEntity.AddMultiUnityObject();
            }
            else
            {
                sceneObjectEntity.multiUnityObject.Prepare();
            }
            if(!sceneObjectEntity.hasWeaponAttachment)
                sceneObjectEntity.AddWeaponAttachment();
            else
                sceneObjectEntity.weaponAttachment.Reset();
            LoadNecessaryAssetAsync(sceneObjectEntity, thdPersonAsset);
            List<int> weaponParts = sceneObjectEntity.weaponObject.CollectParts();
            for (int i = 0; i < weaponParts.Count; i++)
            {
                AssetInfo partAssetInfo = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset( weaponParts[i]);
                var partAssetData = new PartAssetData();
                partAssetData.PartId       = weaponParts[i];
                partAssetData.PartSlotType = (int) SingletonManager.Get<WeaponPartsConfigManager>().GetPartType(weaponParts[i]);
                LoadPartAssetAsync(sceneObjectEntity,partAssetInfo,partAssetData);
            }

            //加载特效
            List<AssetInfo> effectAssetInfo =
                            SingletonManager.Get<WeaponAvatarConfigManager>().GetEffectAsset(avatarId);
            foreach (var effect in effectAssetInfo)
            {
                LoadEffectAssetAsync(sceneObjectEntity,effect);
            }
            
            sceneObjectEntity.effects.AddGlobalEffect(GlobalEffectManager.GlobalGroundPropFlash);
        }

        private void LoadAvatar(SceneObjectEntity sceneObjectEntity, PlayerEntity playerEntity,
                                RoleAvatarConfigItem avatarCfg)
        {
            var role = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(playerEntity.playerInfo.RoleModelId);
            if (null != role)
            {
                switch ((Sex) role.Sex)
                {
                    case Sex.Female:
                        var fAvatarAsset = new AssetInfo(avatarCfg.Bundle, avatarCfg.FPrefab);
                        _assetManager.LoadAssetAsync(sceneObjectEntity, fAvatarAsset,
                        simpleLoadHandler.OnLoadUnityObjectSucess);
                        break;
                    case Sex.Male:
                        var mAvatarAsset = new AssetInfo(avatarCfg.Bundle, avatarCfg.MPrefab);
                        _assetManager.LoadAssetAsync(sceneObjectEntity, mAvatarAsset,
                        simpleLoadHandler.OnLoadUnityObjectSucess);
                        break;
                }

                sceneObjectEntity.effects.AddGlobalEffect(GlobalEffectManager.GlobalGroundPropFlash);
            }
        }
    }
}