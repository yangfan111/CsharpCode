using App.Shared;
using Core.GameModule.Interface;
using UnityEngine;

namespace App.Client.GameModules.Player
{
    public class ClientPlayerGUISystem : IOnGuiSystem
    {
        private PlayerContext _playerContext;
        private GUIStyle usableStyle;

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
        }

        public void OnGUI()
        {
            if (!SharedConfig.ShowShootText || !_playerContext.flagSelfEntity.hasStatisticsServerData ||
                SharedConfig.IsOffline)
                return; 
            var staticsServerData = _playerContext.flagSelfEntity.statisticsServerData;
            if (SharedConfig.CleanShowShootText)
            {
                SharedConfig.CleanShowShootText = false;
                staticsServerData.GUIClean();
                
            }
            GUI.Label(new Rect(0, 210, Screen.width * 0.5f, Screen.height * 0.2f), staticsServerData.GetMatchStr(), usableStyle);
        }

        void GetStaticsServer()
        {
        }
    }
}