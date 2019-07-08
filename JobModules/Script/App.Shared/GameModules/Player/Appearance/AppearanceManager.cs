using System.Collections.Generic;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player.Appearance.PropControllerPackage;
using App.Shared.GameModules.Player.Appearance.WardrobeControllerPackage; 
using App.Shared.GameModules.Player.Appearance.WeaponControllerPackage;
using Core.Appearance;
using Utils.Appearance;
using Core.EntityComponent;
using UnityEngine;
using Utils.Appearance.ManagerPackage;
using Utils.Appearance.WardrobePackage;

namespace App.Shared.GameModules.Player.Appearance
{
    public class AppearanceManager : AppearanceManagerBase, ICharacterAppearance
    {
        private readonly WeaponController _weaponController;
        private readonly WardrobeController _wardrobeController;
        private readonly PropController _propController;
        private readonly RagDollController _ragDollController = new RagDollController();
        public AppearanceManager()
        {
            WeaponControllerBaseImpl = new WeaponController();
            _weaponController = (WeaponController) WeaponControllerBaseImpl;
            
            WardrobeControllerBaseImpl = new WardrobeController(_weaponController.RemountWeaponInPackage);
            _wardrobeController = (WardrobeController) WardrobeControllerBaseImpl;
            
            PropControllerBaseImpl = new PropController();
            _propController = (PropController) PropControllerBaseImpl;
            
            ReplaceMaterialShaderBaseImpl = new ReplaceMaterialShader();
        }

        public override void Execute()
        {
            _weaponController.Execute();
            _wardrobeController.Execute();
            if (!SharedConfig.IsServer)
                _wardrobeController.TryRewind();
            _propController.Execute();
            if (!SharedConfig.IsServer)
                _propController.TryRewind();
        }

        public override void UpdateAvatar()
        {
            if(!SharedConfig.IsServer)
                _wardrobeController.Update();
        }
        
        public void ClearThirdPersonCharacter()
        {
            WardrobeControllerBaseImpl.ClearThirdPersonCharacter();
            WeaponControllerBaseImpl.ClearThirdPersonCharacter();
        }

        public void CheckP3HaveInit(bool value)
        {
            if(value) return;
            WardrobeControllerBaseImpl.P3HaveInit = false;
            WeaponControllerBaseImpl.P3HaveInit = false;
        }

        #region Sync

        public void SyncLatestFrom(IGameComponent playerLatestAppearance)
        {
            _weaponController.SyncFromLatestComponent((LatestAppearanceComponent)playerLatestAppearance);
            _wardrobeController.SyncFromLatestComponent((LatestAppearanceComponent)playerLatestAppearance);
            _propController.SyncFromLatestComponent((LatestAppearanceComponent)playerLatestAppearance);
        }

        public void SyncPredictedFrom(IGameComponent playerPredictedAppearance)
        {
            _weaponController.SyncFromPredictedComponent((PredictedAppearanceComponent)playerPredictedAppearance);
        }

        public void SyncClientFrom(IGameComponent playerClientAppearance)
        {
            _weaponController.SyncFromClientComponent((ClientAppearanceComponent)playerClientAppearance);
        }

        public void SyncLatestTo(IGameComponent playerLatestAppearance)
        {
            _weaponController.SyncToLatestComponent((LatestAppearanceComponent)playerLatestAppearance);
            _wardrobeController.SyncToLatestComponent((LatestAppearanceComponent)playerLatestAppearance);
            _propController.SyncToLatestComponent((LatestAppearanceComponent)playerLatestAppearance);
        }

        public void SyncPredictedTo(IGameComponent playerPredictedAppearance)
        {
            _weaponController.SyncToPredictedComponent((PredictedAppearanceComponent)playerPredictedAppearance);
        }

        public void SyncClientTo(IGameComponent playerClientAppearance)
        {
            _weaponController.SyncToClientComponent((ClientAppearanceComponent)playerClientAppearance);
        }

        public void TryRewind()
        {
            _weaponController.TryRewind();
            if(!SharedConfig.IsServer) //服务器不需穿戴装扮
            {
                _wardrobeController.TryRewind();
                _propController.TryRewind();
            }
        }

        #endregion

        private readonly List<KeyValuePair<int, string>> _needUpdateActionField = new List<KeyValuePair<int, string>>();
        public void SetInputSchemeActionField(int schemeIndex, string actionName)
        {
            _needUpdateActionField.Add(new KeyValuePair<int, string>(schemeIndex, actionName));
        }

        public List<KeyValuePair<int, string>> GetInputSchemeActionFieldToUpdate()
        {
            return _needUpdateActionField;
        }

        public WardrobeControllerBase GetWardrobeController()
        {
            return _wardrobeController;
        }
        
        public WeaponControllerBase GetController<TPlayerWeaponController>()
        {
            return _weaponController;
        }

        public override void SetThirdPersonCharacter(GameObject obj)
        {
            _ragDollController.SetThirdPersonCharacter(obj);
            base.SetThirdPersonCharacter(obj);
        }
        
        public void SetRagDollComponent(IGameComponent component)
        {
            _ragDollController.SetRagDollComponent((RagDollComponent)component);
        }

        public override void PlayerReborn()
        {
            _ragDollController.ControlRagDoll(false);
            base.PlayerReborn();
        }

        public override void PlayerDead()
        {
            _ragDollController.ControlRagDoll(true);
            base.PlayerDead();
        }
    }
}
