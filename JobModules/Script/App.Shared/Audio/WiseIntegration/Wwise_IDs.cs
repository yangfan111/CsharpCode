using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.Audio
{
    public class Wwise_IDs
    {
        /// <summary>
        ///     自己/多人玩家音频播放选择
        /// </summary>
        public const uint Different_player_effect = 678245580;

        /// <summary>
        ///     换弹速度选择
        /// </summary>
        public const uint Gun_magazine_speed = 3930419550;
        
        public const uint FlashBomb = 527817437;


        public static void GetMapAmb(int id,out MapAmbInfo ambInfo)
        {
           MapAmbDict.TryGetValue(id, out ambInfo);
        }
        
        private static Dictionary<int, MapAmbInfo> MapAmbDict = new Dictionary<int, MapAmbInfo>()
        {
            { 1003,new MapAmbInfo(1003,"S003_Amb_Wind_soft_loop_V1","")},
            { 2001,new MapAmbInfo(2001,"M001_Amb_BackNoise_V1","")},
            { 1001,new MapAmbInfo(1001,"S001_Amb_Wind_hard_loop","")},
            { 1002,new MapAmbInfo(1002,"S002_Amb_Crowd_cheer_loop","S002_Amb_Crowd_cheer_loop_stop")}

        };

    }
    
    public struct MapAmbInfo
    {
        public int Id;
        public string PlayerAmb;
        public string PlayerAmbStop;

        public string EnvironmentAmb;
        public const string AssetName = "sound/common";

        public MapAmbInfo(int id, string playerAmb, string playerAmbStop)
        {
            Id = id;
            PlayerAmb = playerAmb;
            EnvironmentAmb = string.Format("{0}_Audio_Amb",id);
            PlayerAmbStop = playerAmbStop;
        }

        public void PlayAmb()
        {
            if (Id > 0)
            {
                var audioMgr = AkSoundEngineController.AudioMgrGetter;
                if(audioMgr != null)
                    AkSoundEngine.PostEvent(PlayerAmb, audioMgr.battleListener.ThdViewEmitter.gameObject);
            }
        }

        public void StopAmb()
        {
            if (Id > 0 && !string.IsNullOrEmpty(PlayerAmbStop))
            {
                var audioMgr = AkSoundEngineController.AudioMgrGetter;
                if(audioMgr != null)
                    AkSoundEngine.PostEvent(PlayerAmb, audioMgr.battleListener.ThdViewEmitter.gameObject);
            }
        }
    }
} // namespace AK