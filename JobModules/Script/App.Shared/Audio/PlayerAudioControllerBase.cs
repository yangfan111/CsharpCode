using App.Shared.Configuration;
using App.Shared.Terrains;
using Core;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Utils.AssetManager;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.Audio
{
    public class PlayerAudioControllerBase:ModuleLogicActivator<PlayerAudioControllerBase>
    {
        protected PlayerEntity entity;
        public void Initialize(PlayerEntity entity,int mapId)
        {
            this.entity = entity;
            this.mapId = mapId;
        }

        public virtual void Update(IUserCmd cmd)
        {
            if (cmd.FilteredInput.IsInput(EPlayerInput.IsCrouch))
            {
                var states = entity.StateInteractController().GetCurrStates();
                if (states.Contains(EPlayerState.Stand) || states.Contains(EPlayerState.Prone))
                    PlaySimpleAudio(EAudioUniqueId.Crouch, true);
                else
                    PlaySimpleAudio(EAudioUniqueId.CrouchToStand, true);
            }
            
        }

        public virtual void LoadMapAmbient(IUnityAssetManager assetManager)
        {
            
        }

        public virtual void PlayJumpStepAudio()
        {
        }

        public virtual void PlayEmptyFireAudio()
        {
            
        }
        public virtual void PlayFootstepAudioC(AudioGrp_Footstep stepState)
        {
        }
        public virtual void PlayFootstepAudioS(AudioGrp_Footstep stepState)
        {
        }
        public virtual void PlaySimpleAudio(EAudioUniqueId uniqueId, bool sync = false)
        {
           throw new System.NotImplementedException();
        }

        public virtual void PlayDeadAudio()
        {
            
        }
        
        public virtual void StopFireTrigger()
        {
            
        }

        public virtual void PlayFireAudio(int weaponId, AudioGrp_ShotMode shotMode)
        {
            throw new System.NotImplementedException();
        }

        public virtual void PlayMeleeAttackAudio(int weaponId, int attackType)
        {
            throw new System.NotImplementedException();
        }
        public void PlayPullBoltAudio(int weaponId)
        {
            PlayReloadAuido(weaponId, AudioGrp_Magazine.PullboltOnly,0);
        }

        public void PlayReloadBulletAudio(int weaponId)
        {
            PlayReloadAuido(weaponId, AudioGrp_Magazine.FillBulletOnly,0);
        }

        public void PlayReloadAudio(int weaponId, bool emptyReload,float MagizineSpeed)
        {
            if (emptyReload)
                PlayReloadAuido(weaponId, AudioGrp_Magazine.MagizineAndPull,MagizineSpeed);
            else
                PlayReloadAuido(weaponId, AudioGrp_Magazine.MagizineOnly,MagizineSpeed);
        }

        public virtual void PlayReloadAuido(int weaponId, AudioGrp_Magazine magizineOnly, float magizineSpeed)
        {
            throw new System.NotImplementedException();

        }
        
        protected IMyTerrain terrainConfig;
        protected int mapId;

        protected IMyTerrain GetTerrainConfig(int sceneId)
        {
            if (terrainConfig == null || terrainConfig._mapId != sceneId)
                terrainConfig = SingletonManager.Get<TerrainManager>().GetTerrain(sceneId);
            return terrainConfig;
        }

        protected AudioGrp_FootMatType FootMatType
        {
            get
            {
                int sceneId = SingletonManager.Get<MapConfigManager>().SceneParameters.Id;
                TerrainMatOriginType matType = (TerrainMatOriginType) GetTerrainConfig(sceneId)
                    .GetTerrainPositionMatType(entity.position.Value);
                return matType.ToAudioMatGrp();
            }
        }

        public virtual void StopPullBoltAudio(int configId)
        {
            
        }

        public virtual void StopSwimAudio()
        {
            
        }
    }
}