using App.Shared.Components;
using Core.CameraControl.NewMotor;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using UnityEngine;
using XmlConfig;

namespace App.Client.GameModules.Player
{
    public class ClientCameraEffectSystem : AbstractSelfPlayerRenderSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientCameraEffectSystem));
        private IGroup<PlayerEntity> _playerGroup;
        private Transform _scopeTrans;
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
            var hand = player.appearanceInterface.Appearance.GetWeaponP1InHand();
            if (null == hand)
            {
                //丢枪的时候，逻辑在服务端处理，客户端同步之前可能会没有武器
                Logger.Warn("no weapon find in hand");
                return;
            }

            foreach (var child in hand.transform.GetComponentsInChildren<Transform>())
            {
                if (child.name != DofMaskName) continue;
                _scopeTrans = child;
                var renderer = child.GetComponent<Renderer>();
                if (null != renderer)
                {
                    renderer.enabled = false;
                }

                child.SendMessage("EnableEffect", true, SendMessageOptions.DontRequireReceiver);
                if (child.childCount > 0)
                {
                    var inner = child.GetChild(0);
                    if (null != inner)
                    {
                        inner.gameObject.SetActive(true);
                        var innerRenderer = inner.GetComponent<MeshRenderer>();
                        if (null != innerRenderer)
                        {
                            innerRenderer.enabled = true;
                        }
                    }
                }

                break;
            }
        }

        private void CancelSights(PlayerEntity player)
        {
            player.appearanceInterface.Appearance.SetP1ObjTopLayerShader();
            if (null == _scopeTrans) return;
            _scopeTrans.SendMessage("EnableEffect", false, SendMessageOptions.DontRequireReceiver);
            if (_scopeTrans.childCount > 0)
            {
                var inner = _scopeTrans.GetChild(0);
                if (null != inner)
                {
                    inner.gameObject.SetActive(false);
                    var renderer = inner.GetComponent<MeshRenderer>();
                    if (null != renderer)
                    {
                        renderer.enabled = false;
                    }
                }
            }

            _scopeTrans = null;
        }
    }
}