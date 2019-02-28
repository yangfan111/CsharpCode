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
            if ((ECameraViewMode) player.cameraStateNew.ViewNowMode == ECameraViewMode.GunSight
                && (ECameraViewMode) player.cameraStateNew.ViewLastMode != ECameraViewMode.GunSight)
            {
                if (!_isGunSightEffect)
                {
                    //没有判断是否有配件等条件，因为有可能有默认带镜的武器
                    var hand = player.appearanceInterface.Appearance.GetWeaponP1InHand();
                    if (null == hand)
                    {
                        //丢枪的时候，逻辑在服务端处理，客户端同步之前可能会没有武器
                        Logger.Warn("no weapon find in hand");
                        return;
                    }

                    var childs = hand.transform.GetComponentsInChildren<Transform>();
                    foreach (var child in childs)
                    {
                        if (child.name == DofMaskName)
                        {
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

                    _isGunSightEffect = true;
                }
            }
            else if ((ECameraViewMode) player.cameraStateNew.ViewNowMode != ECameraViewMode.GunSight
                     && (ECameraViewMode) player.cameraStateNew.ViewLastMode == ECameraViewMode.GunSight)
            {
                if (_isGunSightEffect)
                {
                    if (null != _scopeTrans)
                    {
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

                    _isGunSightEffect = false;
                }
            }
        }
    }
}