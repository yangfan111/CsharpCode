using App.Client.GameModules.Attack;
using App.Client.GameModules.ClientEffect;
using App.Client.GameModules.GamePlay;
using App.Client.GameModules.GamePlay.Free.Entitas;
using App.Client.GameModules.OC;
using App.Client.GameModules.Player;
using App.Client.GameModules.Player.Robot;
using App.Client.GameModules.SceneManagement;
using App.Client.GameModules.SceneObject;
using App.Client.GameModules.Sound;
using App.Client.GameModules.Terrain;
using App.Client.GameModules.Throwing;
using App.Client.GameModules.Ui;
using App.Client.GameModules.Ui.System;
using App.Client.GameModules.Vehicle;
using App.Shared;
using App.Shared.Configuration;
using App.Shared.GameModules;
using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Configuration;
using Assets.App.Shared.GameModules.Camera.Utils;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Compensation;
using Core.Configuration;
using Core.Free;
using Core.GameModule.Module;
using Free.framework;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.SessionStates
{
    public class GameModuleFactory
    {
        public static CompositeGameModule CreatePreparePlayerGameModule(Contexts contexts)
        {
            var gameModule = new CompositeGameModule();
            gameModule.AddModule(new ClientPlayerModule(contexts));
            return gameModule;
        }

        public static CompositeGameModule CreateCompositeGameModule(Contexts contexts)
        {
            var motors = MotorsFactory.CraeteMotors(contexts, SingletonManager.Get<CameraConfigManager>().Config);
            var sessionObjects = contexts.session.commonSession;

            var gameModule = new CompositeGameModule();

            IHitBoxEntityManager hitBoxEntityManager = new HitBoxEntityManager(contexts, false);
            ICompensationWorldFactory factory =
                            new ClientCompensationWorldFactory(contexts.session.commonSession.GameContexts,
                            hitBoxEntityManager);
            GameModule cmdModule = new UserCmdGameModule(contexts, factory,
                new BulletHitHandler(contexts, sessionObjects.EntityIdGenerator, null), new MeleeHitHandler(null),
                new ThrowingHitHandler(null), contexts.session.commonSession, motors);
//            cmdModule.AddSystem(new PlayerAddMarkSystem(contexts));
            cmdModule.AddSystem(new PlayerSprayPaintSystem(contexts));

            gameModule.AddModule(cmdModule);

            gameModule.AddModule(new BulletModule(contexts));
            gameModule.AddModule(new ThrowingModule(contexts));
            gameModule.AddModule(new ClientEffectModule(contexts));
            gameModule.AddModule(new ClientVehicleModule(contexts));
            gameModule.AddModule(new ClientPlayerModule(contexts));
            gameModule.AddModule(new ClientSoundModule(contexts));


            if (SharedConfig.IsRobot)
            {
                gameModule.AddModule(new ClientRobotModule(contexts));
            }

            gameModule.AddSystem(new VisionCenterUpdateSystem(contexts));
            gameModule.AddSystem(new TerrainDataLoadSystem(contexts));
            if (SingletonManager.Get<MapConfigManager>().SceneParameters is SceneConfig
                || SingletonManager.Get<MapConfigManager>().SceneParameters.Id == 0)
            {
                gameModule.AddSystem(new ClientAutoWorldShiftRenderSystem(contexts));
            }

            //gameModule.AddSystem(new OcclusionCullingSystem(contexts));

            gameModule.AddModule(new ClientSceneObjectModule(contexts, sessionObjects));
            //  gameModule.AddModule(new ClientSoundModule(contexts));
            gameModule.AddModule(new ClientGamePlayModule(contexts));
            gameModule.AddModule(new ClientFreeMoveModule(contexts));
            gameModule.AddModule(new UiModule(contexts));

            gameModule.AddModule(new ConfigurationRefreshModule(contexts));
            SingletonManager.Get<FreeUiManager>().Contexts1 = contexts;
            SimpleMessageManager.Instance.Init(contexts.session.entityFactoryObject.SoundEntityFactory);

            return gameModule;
        }

        public static CompositeGameModule CreateRobotCompositeGameModule(Contexts contexts)
        {
            var motors =
                            MotorsFactory.CraeteMotors(contexts, SingletonManager.Get<CameraConfigManager>().Config);
            var sessionObjects = contexts.session.commonSession;

            var gameModule = new CompositeGameModule();

            IHitBoxEntityManager hitBoxEntityManager = new HitBoxEntityManager(contexts, false);
            ICompensationWorldFactory factory =
                            new ClientCompensationWorldFactory(contexts.session.commonSession.GameContexts,
                            hitBoxEntityManager);
            GameModule cmdModule = new UserCmdGameModule(contexts, factory,
            new BulletHitHandler(contexts, sessionObjects.EntityIdGenerator, null), new MeleeHitHandler(null),
            new ThrowingHitHandler(null), contexts.session.commonSession, motors);
            //     cmdModule.AddSystem(new PlayerAddMarkSystem(contexts));

            gameModule.AddModule(cmdModule);
            //      gameModule.AddModule(new BulletModule(contexts));
            //      gameModule.AddModule(new ThrowingModule(contexts));
            //      gameModule.AddModule(new ClientEffectModule(contexts));
            //  gameModule.AddModule(new ClientVehicleModule(contexts));
            // gameModule.AddModule(new ClientPlayerModule(contexts));
            if (SharedConfig.IsRobot)
            {
                gameModule.AddModule(new ClientRobotModule(contexts));
            }

            //     gameModule.AddModule(new ClientSceneObjectModule(contexts, sessionObjects));
            //     gameModule.AddModule(new ClientSoundModule(contexts));
            gameModule.AddModule(new ClientGamePlayModule(contexts));
            gameModule.AddModule(new ClientFreeMoveModule(contexts));


            gameModule.AddModule(new ConfigurationRefreshModule(contexts));

            SimpleProto loginSucess = FreePool.Allocate();
            loginSucess.Key = FreeMessageConstant.ClientLoginSucess;
            if (contexts.session.clientSessionObjects.NetworkChannel != null)
            {
                contexts.session.clientSessionObjects.NetworkChannel.SendReliable((int) EClient2ServerMessage.FreeEvent,
                loginSucess);
            }

            SingletonManager.Get<FreeUiManager>().Contexts1 = contexts;
            SimpleMessageManager.Instance.Init(contexts.session.entityFactoryObject.SoundEntityFactory);

            return gameModule;
        }
    }
}