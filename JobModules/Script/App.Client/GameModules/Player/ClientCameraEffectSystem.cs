using App.Shared.Components;
using Core.CameraControl.NewMotor;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using Shared.Scripts.Effect;
using UnityEngine;
using XmlConfig;

namespace App.Client.GameModules.Player
{
    public class ClientCameraEffectSystem : AbstractSelfPlayerRenderSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientCameraEffectSystem));
        private IGroup<PlayerEntity> _playerGroup;
        private Transform _scopeP1Trans;
        private Transform _scopeP3Trans;
        private bool _isGunSightEffect;
        private const string DofMaskName = "Aim_dofmask";

        public ClientCameraEffectSystem(Contexts contexts) : base(contexts)
        {
        }

        protected override bool Filter(PlayerEntity playerEntity)
        {
            return playerEntity.hasCameraStateNew && playerEntity.hasAppearanceInterface;
        }

        public override void OnRender(PlayerEntity player)
        {
            CheckCameraGunSights(player);
        }
        
        private void CheckCameraGunSights(PlayerEntity player)
        {
            if ((ECameraViewMode) player.cameraStateNew.ViewNowMode == ECameraViewMode.GunSight
                && (ECameraViewMode) player.cameraStateNew.ViewLastMode != ECameraViewMode.GunSight && !_isGunSightEffect)
            {
                _isGunSightEffect = true;
                StartSetSights(player);
            }else if ((ECameraViewMode) player.cameraStateNew.ViewNowMode != ECameraViewMode.GunSight
                      && (ECameraViewMode) player.cameraStateNew.ViewLastMode == ECameraViewMode.GunSight && _isGunSightEffect)
            {
                _isGunSightEffect = false;
                CancelSights(player);
            }
        }

        private void StartSetSights(PlayerEntity player)
        {
            player.appearanceInterface.Appearance.ResetP1ObjShader();
            //没有判断是否有配件等条件，因为有可能有默认带镜的武器
            StartSetModelScope(player.appearanceInterface.Appearance.GetWeaponP1InHand(), ref _scopeP1Trans, true);
            CancelModelScope(ref _scopeP3Trans, false);
        }
        
        private void CancelSights(PlayerEntity player)
        {
            player.appearanceInterface.Appearance.SetP1ObjTopLayerShader();
            CancelModelScope(ref _scopeP1Trans, true);
            StartSetModelScope(player.appearanceInterface.Appearance.GetWeaponP3InHand(), ref _scopeP3Trans, false);
        }

        private void StartSetModelScope(GameObject hand, ref Transform scopeTrans, bool isFirstModel)
        {
            if (null == hand)
            {
                //丢枪的时候，逻辑在服务端处理，客户端同步之前可能会没有武器
                Logger.Warn("no weapon find in hand");
                return;
            }

            foreach (var child in hand.transform.GetComponentsInChildren<Transform>())
            {
                if (child.name != DofMaskName) continue;
                scopeTrans = child;
                var renderer = child.GetComponent<Renderer>();
                if (null != renderer && isFirstModel)
                {
                    renderer.enabled = false;
                }
                
                //三人称不调用景深脚本
                if (isFirstModel)
                {
                    child.SendMessage("EnableEffect", true, SendMessageOptions.DontRequireReceiver);
                    SetScope(child, true);
                }
               
                break;
            }
        }

        private void CancelModelScope(ref Transform scopeTrans, bool isFirstModel)
        {
            if (null == scopeTrans) return;
            if (isFirstModel)
            {
                scopeTrans.SendMessage("EnableEffect", false, SendMessageOptions.DontRequireReceiver);
                SetScope(scopeTrans, false);
            }
            scopeTrans = null;
        }


        private void SetScope(Transform scope, bool enable)
        {
            if (scope.childCount > 0)
            {
                var inner = scope.GetChild(0);
                if (null != inner)
                {
                    inner.gameObject.SetActive(enable);
                    var innerRender = inner.GetComponent<AbstractEffectMonoBehaviour>();
                    if (innerRender != null)
                    {
                        innerRender.SetParam("Enable", enable);
                    }
                }
            }
        }
        
    }
}