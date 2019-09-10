using App.Client.GameModules.Ui.Logic;
using UserInputManager.Lib;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class PickUpUiAdapter : UIAdapter, IPickUpUiAdapter
    {
        private PlayerContext player;
        private SceneObjectContext sceneObject;
        private MapObjectContext mapObject;
        private SessionContext session;
        private VehicleContext vehicle;
        private FreeMoveContext freeMove;
        private UserInputManager.Lib.UserInputManager userInputManager;
        private UiContext uiContext;

        public PickUpUiAdapter(PlayerContext player,  SceneObjectContext sceneObject, MapObjectContext mapObject, SessionContext session, VehicleContext vehicle, FreeMoveContext freeMove, UserInputManager.Lib.UserInputManager userInputManager, UiContext uiContext)
        {
            this.player = player;
            this.sceneObject = sceneObject;
            this.mapObject = mapObject;
            this.session = session;
            this.vehicle = vehicle;
            this.freeMove = freeMove;
            this.userInputManager = userInputManager;
            this.uiContext = uiContext;
        }

        public void RegisterKeyhandler(KeyHandler handler)
        {
            userInputManager.RegisterKeyhandler(handler);
        }
        public void RegisterPointerhandler(PointerKeyHandler handler)
        {
            userInputManager.RegisterPointerhandler(handler);
        }

        public SceneObjectCastLogic GetSceneObjectCastLogic()
        {
            return new SceneObjectCastLogic(
                player,
                sceneObject,
                session.clientSessionObjects,
                session.clientSessionObjects.UserCmdGenerator,
                SingletonManager.Get<RaycastActionConfigManager>().Distance);
        }

        public MapObjectCastLogic GetMapObjectCastLogic()
        {
            return new MapObjectCastLogic(
                player,
                mapObject,
                session.clientSessionObjects,
                session.clientSessionObjects.UserCmdGenerator,
                SingletonManager.Get<RaycastActionConfigManager>().Distance);
        }
        
        public VehicleCastLogic GetVehicleCastLogic()
        {
            return new VehicleCastLogic(
                vehicle,
                player,
                session.clientSessionObjects.UserCmdGenerator,
                SingletonManager.Get<RaycastActionConfigManager>().Distance);
        }
        public FreeObjectCastLogic GetFreeObjectCastLogic()
        {
            return new FreeObjectCastLogic(
                freeMove, player, userInputManager,
                SingletonManager.Get<RaycastActionConfigManager>().Distance);
        }
        public DoorCastLogic GetDoorCastLogic()
        {
            return new DoorCastLogic(
                mapObject,
                player,
                session.clientSessionObjects.UserCmdGenerator,
                SingletonManager.Get<RaycastActionConfigManager>().Distance);
        }
        public PlayerCastLogic GetPlayerCastLogic()
        {
            return new PlayerCastLogic(
                player,
                session.clientSessionObjects.UserCmdGenerator,
                SingletonManager.Get<RaycastActionConfigManager>().Distance);
        }
        public PlayerStateTipLogic GetPlayerStateTipLogic()
        {
            return new PlayerStateTipLogic(
                player,
                session.clientSessionObjects.UserCmdGenerator);
        }

        public CommonCastLogic GetCommonCastLogic()
        {
            return new CommonCastLogic(player, 
                sceneObject, 
                SingletonManager.Get<RaycastActionConfigManager>().Distance);
        }

        public bool IsCountDown()
        {
            return uiContext.uI.CountingDown;
        }

        public BuffTipLogic GetBuffTipLogic()
        {
            return new BuffTipLogic(
                player,
                uiContext,
                session.clientSessionObjects.UserCmdGenerator);
        }
    }
}