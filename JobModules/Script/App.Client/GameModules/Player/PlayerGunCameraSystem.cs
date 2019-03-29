using Core.CameraControl.NewMotor;
using Core.GameModule.Interface;
using UnityEngine;
using XmlConfig;

namespace App.Client.GameModules.Player
{
    public class PlayerGunCameraSystem : IPlaybackSystem
    {
        private PlayerContext _playerContext;
        private GameObject _weapon;
        public PlayerGunCameraSystem(PlayerContext playerContext)
        {
            _playerContext = playerContext;
        }

        public void OnPlayback()
        {
            var player = _playerContext.flagSelfEntity;
            if(null == player)
            {
                return;
            }
            if(player.cameraStateNew.ViewNowMode == (byte)ECameraViewMode.GunSight)
            {
                if(null == _weapon)
                {
                    _weapon = player.appearanceInterface.Appearance.GetWeaponP1InHand();
                    var hand = player.firstPersonModel.Value;
                    EnableGunCam(hand, true);
                }
            }
            else
            {
                if(null != _weapon)
                {
                    var hand = player.firstPersonModel.Value;
                    EnableGunCam(hand, false);
                    EnableGunCam(_weapon, false);
                    _weapon = null; 
                }
           }
        }

        private void EnableGunCam(GameObject go, bool enabled)
        {
            if(null == go)
            {
                return;
            }
            foreach (var rd in go.GetComponentsInChildren<Renderer>())
            {
                foreach (var mat in rd.materials)
                {
                    if (mat.shader.name == "Standard (MSAO)")
                    {
                        if(enabled)
                        {
                            mat.EnableKeyword("_GUN_CAMERA");
                        }
                        else
                        {
                            mat.DisableKeyword("_GUN_CAMERA");
                        }
                    }
                }
            }
        }
    }
}
