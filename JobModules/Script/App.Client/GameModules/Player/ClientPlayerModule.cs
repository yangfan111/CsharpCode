using App.Client.ClientSystems;
using App.Client.GameModules.ClientPlayer;
using App.Client.GameModules.Player.PlayerShowPackage;
using App.Shared.Configuration;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Player.ResourceLoad;
using App.Shared.SceneManagement;
using Assets.App.Client.GameModules.GamePlay.Free.Entitas;
using Assets.App.Client.GameModules.Player;
using Core.GameModule.Module;
using Utils.Singleton;
using XmlConfig;


namespace App.Client.GameModules.Player
{
    public class ClientPlayerModule : GameModule
    {
        public ClientPlayerModule(Contexts contexts)
        {
            AddSystem(new PlayerPlayBackFilterSystem(contexts));
            AddSystem(new PlayerEntityInitSystem(contexts));

            AddSystem(new PlayerChangeRoleSystem(contexts));
            AddSystem(new PlayerResourceLoadSystem(contexts));
            AddSystem(new ClientPlayerCameraInitSystem(contexts.player));

            AddSystem(new PlayerPlaybackSystem(contexts));
            AddSystem(new PlayerFirstPersonHandRotationSystem());
            AddSystem(new InputSchemeUpdateSystem());

            AddSystem(new PlayerAutoMoveSystem(contexts));
            
            //player playBack show
            AddSystem(new CreatePlaybackPlayerLifeStateDataSystem(contexts));
            AddSystem(new PlayerAnimationPlaybackSystem(contexts));
            AddSystem(new PlayerAppearancePlaybackSystem(contexts));
            AddSystem(new PlayerBonePlaybackSystem(contexts));
            AddSystem(new PlayerAvatarPlaybackSystem(contexts));
            AddSystem(new WeaponAnimationPlaybackSystem(contexts));

            AddSystem(new CameraFxInitSystem(contexts.player));
            AddSystem(new ClientCameraEffectSystem(contexts));

            AddSystem(new PlayerDebugDrawSystem(contexts));

            AddSystem(new PlayerDeadAnimSystem(contexts));
            AddSystem(new PlayerStateChangeSystem(contexts));
            AddSystem(new PlayerEquipPickAndDropSystem(contexts.session.clientSessionObjects.UserCmdGenerator));
            AddSystem(new ClientCameraFinalRenderSystem(contexts));

            if (SingletonManager.Get<MapConfigManager>().SceneParameters is SceneConfig)
                AddSystem(new PositionRelatedEffectUpdateSystem(contexts,
                    SingletonManager.Get<DynamicScenesController>().GetPositionRelatedEffectUpdater()));
            else
                AddSystem(new PositionRelatedEffectUpdateSystem(contexts,
                    SingletonManager.Get<LevelController>().GetPositionRelatedEffectUpdater()));

            AddSystem(new ClientPlayerSaveSystem(contexts));
            AddSystem(new PlayerRaycastInitSystem(contexts));
            AddSystem(new PingSystem(contexts));
            AddSystem(new RaycastTestSystem(contexts));
            AddSystem(new ClientPlayerWeaponSystem(contexts));
            AddSystem(new PlayerUpdateRotationRenderSystem(contexts));
            AddSystem(new ClientPlayerEntityInitSystem(contexts.player));
            AddSystem(new ClientPlayerTipShowSystem(contexts));
            AddSystem(new ClientPlayerDebugDrawBoxSystem(contexts));
            AddSystem(new ClientPlayerDebugAnimationSystem());
            //AddSystem(new PlayerGunCameraSystem(contexts.player));
        }
    }

   
}
