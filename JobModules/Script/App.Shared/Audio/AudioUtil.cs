using App.Shared.Audio;
using App.Shared.Sound;
using Core.Utils;
using System.IO;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using Core;
using UnityEngine;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared
{
    public static class AudioUtil
    {
        public static readonly LoggerAdapter AudioLogger = new LoggerAdapter(typeof(AKAudioDispatcher));
        public static void AssertProcessResult(AKRESULT result, string s, params object[] args)
        {
            if (!SharedConfig.IsServer)
            {
                s = string.Format(s, args);
                if (result != AKRESULT.AK_Success && result != AKRESULT.AK_BankAlreadyLoaded)
                {
                 //   DebugUtil.MyLog(s + string.Format(" {0} ", result), DebugUtil.DebugColor.Grey);
                    AudioLogger.Info(string.Format("[Audio Result Exception]{0}  {1}", s, result));
                }
                else
                {
                    AudioLogger.Info(s + string.Format(" {0} ", result));
                }
            }
        }
        public static bool Sucess(this AKRESULT result)
        {
            return result == AKRESULT.AK_Success || result == AKRESULT.AK_BankAlreadyLoaded;
        }

        public static AudioGrp_ShotMode ToAudioGrpShotMode(this EFireMode fireModel)
        {

            switch (fireModel)
            {
                case EFireMode.Auto:
                    return AudioGrp_ShotMode.Continue;
                case EFireMode.Burst:
                    return AudioGrp_ShotMode.Trriple;
                default:
                    return AudioGrp_ShotMode.Single;
            }

        }
        public static AudioGrp_HitMatType ToAudioGrpHitMatType(this EEnvironmentType enviromentType)
        {

            switch (enviromentType)
            {
                case EEnvironmentType.Wood:
                    return AudioGrp_HitMatType.Wood;
                case EEnvironmentType.Glass:
                    
                case EEnvironmentType.Stone:
                case EEnvironmentType.Concrete:
                case EEnvironmentType.Soil:
                    return AudioGrp_HitMatType.Concrete;
                case EEnvironmentType.Water:
                    return AudioGrp_HitMatType.Water;
                default:
                    return AudioGrp_HitMatType.Concrete;
            }
        }
        
       // [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void NLog(string s, params object[] args)
        {
            if (!SharedConfig.IsServer)
            {
                s = string.Format(s, args);
        //       DebugUtil.MyLog(s, DebugUtil.DebugColor.Blue);
                AudioLogger.Info("[Audio Log] " + s);
            }

        }
        public static void ELog(string s, params object[] args)
        {
            if(!SharedConfig.IsServer)
            {
            s = string.Format(s, args);
        //    DebugUtil.MyLog(s, DebugUtil.DebugColor.Grey);
                AudioLogger.Info("[Audio Error] " + s);

            }
        }

        public static string[] GetBankAssetNamesByFolder(string folder)
        {
            try
            {
                string assetFolder = (string.IsNullOrEmpty(folder)) ? AkUtilities.GetWiseBankFolder_Full() : folder;
                var paths = Directory.GetFiles(assetFolder, "*.bnk", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < paths.Length; i++)
                    paths[i] = Path.GetFileName(paths[i]);
                return paths;
            }
            catch (System.Exception e)
            {

            }
            return null;

        }
       public static AudioGrp_MatIndex ToAudioMatGrp(this TerrainMatOriginType matType)
        {
            switch(matType)
            {
          
                case TerrainMatOriginType.Dirt:
                    return AudioGrp_MatIndex.Concrete;
                case TerrainMatOriginType.Grass:
                    return AudioGrp_MatIndex.Grass;
                case TerrainMatOriginType.Rock:
                    return AudioGrp_MatIndex.Rock;
                case TerrainMatOriginType.Sand:
                    return AudioGrp_MatIndex.Sand;
                default:
                    return AudioGrp_MatIndex.Default;
            }
        }

        public static AudioGrp_Footstep ToAudioFootGrp(this PostureInConfig posture)
        {
            var step = AudioGrp_Footstep.None;
            switch (posture)
            {
                case PostureInConfig.Crouch:
                        step = AudioGrp_Footstep.Squat;
                    break;
                case PostureInConfig.Prone:
                        step = AudioGrp_Footstep.Crawl;
                    break;
                case PostureInConfig.Stand:
                        step = AudioGrp_Footstep.Walk;
                    
                    break;
                case PostureInConfig.Swim:
                  //  player.soundManager.Value.PlayOnce(EPlayerSoundType.Swim);
                    break;
                case PostureInConfig.Dive:
                   // player.soundManager.Value.PlayOnce(EPlayerSoundType.Dive);
                    break;
            }

            return step;
        }

        public static float GetFootstepPlayInterval(AudioGrp_Footstep stepState)
        {
            switch (stepState)
            {
               case AudioGrp_Footstep.Crawl:
                   return AudioInfluence.StepPlayInfo.CrawlStepPlayInterval;
               case AudioGrp_Footstep.Land:
                   return AudioInfluence.StepPlayInfo.WalkStepPlayInterval;
               case AudioGrp_Footstep.Squat:
                   return AudioInfluence.StepPlayInfo.SquatStepPlayInterval;
                case AudioGrp_Footstep.Walk:
                    return AudioInfluence.StepPlayInfo.WalkStepPlayInterval;
            }

            return GlobalConst.DefaultAudioFootstepInterval;
        }


   
    }
}
