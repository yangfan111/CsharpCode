using App.Shared;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    class PlayerObserveHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlayerObserveTrigger;
        }

        public void Handle(SimpleProto data)
        {
            var contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            var SelfPlayer = contexts.player.flagSelfEntity;
            if (data.Bs[0])
            {
                var PlayerCamera = SelfPlayer.cameraObj.MainCamera.gameObject;

                if (PlayerCamera.GetComponent<AkAudioListener>() == null)
                {
                    AkAudioListener Listener = PlayerCamera.AddComponent<AkAudioListener>();
                    AkSpatialAudioListener SpatialListener = PlayerCamera.AddComponent<AkSpatialAudioListener>();
                    Listener.enabled = true;
                    SpatialListener.enabled = true; 
                }
                else
                {
                    PlayerCamera.GetComponent<AkAudioListener>().enabled = true;
                    PlayerCamera.GetComponent<AkSpatialAudioListener>().enabled = true;
                }
                
                SelfPlayer.AudioController().StopDefaultListener();
            }
            else
            {
                var CameraListener = SelfPlayer.cameraObj.MainCamera.gameObject.GetComponent<AkAudioListener>();
                var CameraSpatialListener = SelfPlayer.cameraObj.MainCamera.gameObject.GetComponent<AkSpatialAudioListener>();
                if (CameraListener!=null)
                {
                    CameraListener.enabled = false;
                    CameraSpatialListener.enabled = false;
                    UnityEngine.MonoBehaviour.Destroy(CameraListener);
                    UnityEngine.MonoBehaviour.Destroy(CameraSpatialListener);
                    
                    SelfPlayer.AudioController().OpenDefaultListener();
                }
            }
        }
    }
}
