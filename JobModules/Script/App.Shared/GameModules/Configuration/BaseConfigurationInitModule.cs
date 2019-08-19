using App.Shared.Configuration;
using Assets.Core.Configuration;
using Assets.Utils.Configuration;
using Core.Configuration;
using Core.Configuration.Sound;
using Core.Configuration.Terrains;
using Core.SessionState;
using Entitas;
using Shared.Scripts.SceneManagement;
using System.Collections.Generic;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.SettingManager;
using Utils.Singleton;

namespace App.Shared.GameModules.Configuration
{
    public class BaseConfigurationInitModule : Systems
    {
        private readonly IUnityAssetManager _assetManager;

        const string ASSETBUNDLENAME = "i18nprefab";
        const string ASSETNAME = "ssjjLanguage";

        private ISessionCondition _sessionState;
        /// <summary>
        ///
        /// 注意：不能加载特定模式地图需要的配置文件
        /// </summary>
        /// <param name="sessionState"></param>
        public BaseConfigurationInitModule(ISessionCondition sessionState, IUnityAssetManager assetManager)
        {
            _assetManager = assetManager;
            AddConfigSystem<AssetConfigManager>(sessionState, "svn.version");
            AddConfigSystem<CharacterStateConfigManager>(sessionState, "SpeedConfig");
            AddConfigSystem<AvatarAssetConfigManager>(sessionState, "role_avator_res");
            AddConfigSystem<AvatarSkinConfigManager>(sessionState, "AvatarSkin");
            AddConfigSystem<MeleeAttackCDConfigManager>(sessionState, "MeleeAttackCDConfig");

            AddConfigSystem<FirstPersonOffsetConfigManager>(sessionState, "FirstPersonOffset");
            AddConfigSystem<RoleConfigManager>(sessionState, "role");
            AddConfigSystem<CharacterInfoManager>(sessionState, "CharacterInfo");
            AddConfigSystem<KillFeedBackConfigManager>(sessionState, "killfeedback");

            AddConfigSystem<CameraConfigManager>(sessionState, "Camera");

            AddConfigSystem<SoundConfigManager>(sessionState, "Sound");
            AddConfigSystem<PlayerSoundConfigManager>(sessionState, "PlayerSound");
            AddConfigSystem<BulletDropConfigManager>(sessionState, "BulletDrop");
            AddConfigSystem<ClientEffectCommonConfigManager>(sessionState, "ClientEffectCommon");
            AddConfigSystem<WeaponConfigManagement>(sessionState, "WeaponData");
            AddConfigSystem<WeaponResourceConfigManager>(sessionState, "weapon");
            AddConfigSystem<ClipDropConfigManager>(sessionState, "ClipDrop");
            AddConfigSystem<WeaponPartsConfigManager>(sessionState, "weapon_parts");
            AddConfigSystem<WeaponPartSurvivalConfigManager>(sessionState, "weapon_parts_survival");
            AddConfigSystem<GameItemConfigManager>(sessionState, "gameitem");
            AddConfigSystem<RoleAvatarConfigManager>(sessionState, "role_avator");
            AddConfigSystem<CardConfigManager>(sessionState, "card");
            AddConfigSystem<TypeForDeathConfigManager>(sessionState, "TypeForDeath");
            AddConfigSystem<ChatConfigManager>(sessionState, "chat");
            AddConfigSystem<DropAreaConfigManager>(sessionState, "droparea");
            AddConfigSystem<DropPoolConfigManager>(sessionState, "droppool");
            AddConfigSystem<DropItemConfigManager>(sessionState, "dropitem");

            AddConfigSystem<TerrainSoundConfigManager>(sessionState, "TerrainSound");
            AddConfigSystem<TerrainEffectConfigManager>(sessionState, "TerrainEffect");
            AddConfigSystem<TerrainMaterialConfigManager>(sessionState, "TerrainMaterial");
            AddConfigSystem<TerrainTextureConfigManager>(sessionState, "TerrainTexture");
            AddConfigSystem<TerrainVehicleFrictionConfigManager>(sessionState, "TerrainFriction");
            AddConfigSystem<TerrainTextureTypeConfigManager>(sessionState, "TerrainTextureType");

            AddConfigSystem<DynamicPredictionErrorCorrectionConfigManager>(sessionState,
                "DynamicPredictionErrorCorrectionConfig");
            AddConfigSystem<VehicleAssetConfigManager>(sessionState, "VehicleConfig");
            AddConfigSystem<VehicleSoundConfigManager>(sessionState, "VehicleSound");
            AddConfigSystem<StateTransitionConfigManager>(sessionState, "StateTransition");
            AddConfigSystem<StateInteruptConfigManager>(sessionState, "StateInterrupt");
            AddConfigSystem<RaycastActionConfigManager>(sessionState, "RaycastAction");
            AddConfigSystem<LadderRankConfigManager>(sessionState, "ladderrank");

            AddConfigSystem<WeaponPropertyConfigManager>(sessionState, "weapon_property");
            AddConfigSystem<PropConfigManager>(sessionState, "prop");
            AddConfigSystem<EnvironmentTypeConfigManager>(sessionState, "EnvironmentType");
            AddConfigSystem<ClientEffectConfigManager>(sessionState, "ClientEffect");
            AddConfigSystem<GameModeConfigManager>(sessionState, "gamemode");
            AddConfigSystem<GameRuleConfigManager>(sessionState, "gamerule");

            AddConfigSystem<WeaponAvatarConfigManager>(sessionState, "weapon_avator");
            
            //AddConfigSystem<StreamingLevelStructure>(sessionState, "Map002" + StreamingConfig.DataXMLName, "002" + StreamingConfig.StreamingDataABName);
            //AddConfigSystem<ScenesLightmapStructure>(sessionState, "Map002" + StreamingConfig.LightXMLName, "002" + StreamingConfig.StreamingLightDataABName);
            //AddConfigSystem<ScenesIndoorCullStructure>(sessionState, "Map002" + StreamingConfig.InDoorXMLName, "002" + StreamingConfig.StreamingInDoorABName);
            
            AddConfigSystem<MapsDescription>(sessionState, "mapConfig");
            AddConfigSystem<AudioWeaponManager>(sessionState, "WeaponAudio");
            AddConfigSystem<AudioEventManager>(sessionState, "AudioEvent");
            AddConfigSystem<AudioGroupManager>(sessionState, "AudioGroup");
            if (!SettingManager.GetInstance().IsInitialized())
            {
                AddConfigSystem<SettingConfigManager>(sessionState, "setting");
                AddConfigSystem<SettingVideoConfigManager>(sessionState, "settingVideoPartial");
            }

            AddConfigSystem<VideoSettingConfigManager>(sessionState, "video_setting");
            AddConfigSystem<LoadingTipConfigManager>(sessionState, "loadingtips");
            AddConfigSystem<IndividuationConfigManager>(sessionState, "individuation");
            AddConfigSystem<WeaponWorkShopConfigManager>(sessionState, "weapon_workshop");
            AddConfigSystem<WeaponUpdateConfigManager>(sessionState, "weapons_update");
            AddConfigSystem<GlobalDataConfigManager>(sessionState, "global_data");
            _sessionState = sessionState;

        }





        private void AddConfigSystem<T>(ISessionCondition sessionState, string asset,
            string bundleName = "tables")
            where T : AbstractConfigManager<T>, IConfigParser, new()
        {

            this.Add(new DefaultConfigInitSystem<T>(_assetManager, sessionState, new AssetInfo(bundleName, asset),
                SingletonManager.Get<T>()));
        }
        List<IExecuteSystem> _systems = new List<IExecuteSystem>();

    }
}