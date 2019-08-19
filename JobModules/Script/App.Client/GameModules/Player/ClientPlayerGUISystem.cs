using App.Shared;
using App.Shared.Audio;
using Core;
using Core.GameModule.Interface;
using Core.SessionState;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.Player
{
    public class ClientPlayerGUISystem : IOnGuiSystem
    {
        private PlayerContext _playerContext;
        private GUIStyle usableStyle;
        private GUIStyle usableStyle1;

        public ClientPlayerGUISystem(PlayerContext playerContext)
        {
            _playerContext = playerContext;
            usableStyle = new GUIStyle
            {
                            normal = new GUIStyleState
                            {
                                            textColor = Color.red
                            },
                            fontSize = 15
            };
            usableStyle1 = new GUIStyle
            {
                            normal = new GUIStyleState
                            {
                                            textColor = Color.blue
                            },
                            fontSize = 15
            };
        }

        private void GUIShootText()
        {
            if (!GMVariable.ShowShootText || !_playerContext.flagSelfEntity.hasStatisticsServerData ||
                SharedConfig.IsOffline)
                return; 
            var staticsServerData = _playerContext.flagSelfEntity.statisticsServerData;
            if (GMVariable.CleanShowShootText)
            {
                GMVariable.CleanShowShootText = false;
                staticsServerData.GUIClean();
                
            }
            GUI.Label(new Rect(0, 210, Screen.width * 0.5f, Screen.height * 0.2f), staticsServerData.GetMatchStr(), usableStyle);
        }

        private void GUISessionText()
        {
            if (GMVariable.ShowSessionTimer)
            {
                GUI.Label(new Rect(0, 210, Screen.width * 0.5f, Screen.height * 0.4f), SingletonManager.Get<SessionStateTimer>().ToString(), usableStyle1);
            }
        }

        private void GUIAudioListenerText()
        {
            if (GMVariable.AudioListenerLog)
            {
                if (AkSoundEngineController.AudioMgrGetter != null)
                {
                    AkGameObj listenerObj = AkSoundEngineController.AudioMgrGetter.battleListener.DefaultListenerObj;
                    var listenerComp = AkSoundEngineController.AudioMgrGetter.battleListener.DefaultListener;
                    if (!listenerObj)
                        return;
                    //Transform lstenerParent = _playerContext.flagSelfEntity.appearanceInterface.Appearance.CharacterP1.transform;
                    string s = "";
                    GetTransIterationStr(listenerObj.transform, ref s);
                    Vector3 vec = listenerObj != null ? listenerObj.transform.position : Vector3.zero;
                    GUI.Label(new Rect(0, 210, Screen.width * 0.5f, Screen.height * 0.2f), string.Format("hierarchy:{0}\nvec:{1} enable:{2}-{3}",s,vec,listenerObj.isActiveAndEnabled,listenerComp.isActiveAndEnabled), usableStyle);
                }
            }

            if (GMVariable.AudioFootstepLog)
            {
                AudioGrp_FootMatType matType = (AudioGrp_FootMatType) _playerContext.flagSelfEntity.playerClientUpdate.LastMatType;
                GUI.Label(new Rect(0, 210, Screen.width * 0.5f, Screen.height * 0.2f),matType.ToString(), usableStyle);
            }
        }

        private void GetTransIterationStr(Transform trans,ref string s)
        {
            s += trans.name+"/";
            if(trans.parent)
                GetTransIterationStr(trans.parent, ref s);
        }

        public static LoggerAdapter Logger = new LoggerAdapter(typeof(BattleAudioManager));
        public void OnGUI()
        {
            GUIShootText();
            GUIAudioListenerText();
            GUISessionText();
        }
        
        void GetStaticsServer()
        {
        }
    }
}