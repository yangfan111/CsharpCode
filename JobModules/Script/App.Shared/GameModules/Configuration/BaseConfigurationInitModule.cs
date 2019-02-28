using App.Shared.Configuration;
using Assets.Core.Configuration;
using Assets.Utils.Configuration;
using Core.Configuration;
using Core.Configuration.Sound;
using Core.Configuration.Terrains;
using Core.GameModule.Module;
using Core.SessionState;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.SettingManager;
using Utils.Singleton;

namespace App.Shared.GameModules.Configuration
{
    public class BaseConfigurationInitModule : GameModule
    {
      

        public BaseConfigurationInitModule(Contexts context, ISessionState sessionState)
        {
            AddConfigSystem<AssetConfigManager>(sessionState, "svn.version");
            AddConfigSystem<CharacterStateConfigManager>(sessionState, "SpeedConfig");
            AddConfigSystem<AvatarAssetConfigManager>(sessionState, "role_avator_res");
            AddConfigSystem<MeleeAttackCDConfigManager>(sessionState, "MeleeAttackCDConfig");

            AddConfigSystem<FirstPersonOffsetConfigManager>(sessionState, "FirstPersonOffset");
            AddConfigSystem<RoleConfigManager>(sessionState, "role");
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
            AddConfigSystem<MapPositionConfigManager>(sessionState, "temp");
            AddConfigSystem<WeaponPartSurvivalConfigManager>(sessionState, "weapon_parts_survival");
            AddConfigSystem<GameItemConfigManager>(sessionState, "gameitem");
            AddConfigSystem<RoleAvatarConfigManager>(sessionState, "role_avator");
            AddConfigSystem<CardConfigManager>(sessionState, "card");
            AddConfigSystem<TypeForDeathConfigManager>(sessionState, "TypeForDeath");
            AddConfigSystem<ChatConfigManager>(sessionState, "chat");

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
            AddConfigSystem<RaycastActionConfigManager>(sessionState, "RaycastAction");
            AddConfigSystem<LadderRankConfigManager>(sessionState, "ladderrank");

            AddConfigSystem<WeaponPropertyConfigManager>(sessionState, "weapon_property");
            AddConfigSystem<PropConfigManager>(sessionState, "prop");
            AddConfigSystem<EnvironmentTypeConfigManager>(sessionState, "EnvironmentType");
            AddConfigSystem<ClientEffectConfigManager>(sessionState, "ClientEffect");
            AddConfigSystem<GameModeConfigManager>(sessionState, "gamemode");

            AddConfigSystem<WeaponAvatarConfigManager>(sessionState, "weapon_avator");
            AddConfigSystem<StreamingLevelStructure>(sessionState, "streaminglevel", "tablesfrombuilding");
            AddConfigSystem<MapsDescription>(sessionState, "mapConfig");
            AddConfigSystem<AudioWeaponManager>(sessionState, "WeaponAudio");
            AddConfigSystem<AudioEventManager>(sessionState, "AudioEvent");
            AddConfigSystem<AudioGroupManager>(sessionState, "AudioGroup");
            if (!SettingManager.GetInstance().IsInitialized())
            {
                AddConfigSystem<SettingManager>(sessionState, "setting");
            }

            AddConfigSystem<VideoSettingConfigManager>(sessionState, "video_setting");
            AddConfigSystem<LoadingTipConfigManager>(sessionState, "loadingtips");
        }

        private void AddConfigSystem<T>(ISessionState sessionState, string asset,
            string bundleName = "tables")
            where T : AbstractConfigManager<T>, IConfigParser, new()
        {
           
            AddSystem(new DefaultConfigInitSystem<T>(sessionState, new AssetInfo(bundleName, asset),
                SingletonManager.Get<T>()));
        }
        
    }
}