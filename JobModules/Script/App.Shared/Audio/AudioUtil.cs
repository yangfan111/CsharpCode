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
using Assets.XmlConfig;
using Utils.Singleton;
using Utils.Configuration;

namespace App.Shared
{
    public static class AudioUtil
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AudioDispatcher));

        public static void AssertProcessResult(AKRESULT result, string s, params object[] args)
        {
            if (!SharedConfig.IsServer)
            {
                s = string.Format(s, args);
                if (result != AKRESULT.AK_Success && result != AKRESULT.AK_BankAlreadyLoaded)
                {
                    //   DebugUtil.MyLog(s + string.Format(" {0} ", result), DebugUtil.DebugColor.Grey);
                    Logger.Info(string.Format("[Audio Result Exception]{0}  {1}", s, result));
                }
                else
                {
                    Logger.Info(s + string.Format(" {0} ", result));
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

        public static AudioGrp_HitMatType ToAudioGrpHitMatType(this EBodyPart bodyPart)
        {
            switch (bodyPart)
            {
                case EBodyPart.Head:
                    return AudioGrp_HitMatType.Head;
                default:
                    return AudioGrp_HitMatType.Body;
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

        public static string[] GetBankAssetNamesByFolder(string folder)
        {
            try
            {
                string assetFolder = (string.IsNullOrEmpty(folder)) ? AkUtilities.GetWiseBankFolder_Full() : folder;
                var    paths       = Directory.GetFiles(assetFolder, "*.bnk", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < paths.Length; i++)
                    paths[i] = Path.GetFileName(paths[i]);
                return paths;
            }
            catch (System.Exception e)
            {
            }

            return null;
        }

        public static AudioGrp_FootMatType ToAudioMatGrp(this TerrainMatOriginType matType)
        {
            switch (matType)
            {
                case TerrainMatOriginType.Dirt:
                    return AudioGrp_FootMatType.Concrete;
                case TerrainMatOriginType.Grass:
                    return AudioGrp_FootMatType.Grass;
                case TerrainMatOriginType.Rock:
                    return AudioGrp_FootMatType.Rock;
                case TerrainMatOriginType.Sand:
                    return AudioGrp_FootMatType.Sand;
                default:
                    return AudioGrp_FootMatType.Default;
            }
        }

        public static EAudioUniqueId ToUseItemAudioUniqueId(int itemId)
        {
            var gameItemConfig = SingletonManager.Get<GameItemConfigManager>().GetConfigById(itemId);
            EAudioUniqueId audioUniqueId =EAudioUniqueId.None ;
            switch ((EItemAudioType) gameItemConfig.Type)
            {
                case EItemAudioType.Bandage:
                    audioUniqueId = EAudioUniqueId.UseBandage;
                    break;
                case EItemAudioType.Doping:
                    audioUniqueId = EAudioUniqueId.UseDoping;
                    break;
                case EItemAudioType.EngeryDrink:
                    audioUniqueId = EAudioUniqueId.UseEngeryDrink;
                    break;
                case EItemAudioType.Gasoline:
                    break;
                case EItemAudioType.MedicalPackage:
                    audioUniqueId = EAudioUniqueId.UseMedicalPackage;
                    break;
                case EItemAudioType.AidPackage:
                    audioUniqueId = EAudioUniqueId.UseAidPackage;
                    break;
                case EItemAudioType.Pill:
                    audioUniqueId = EAudioUniqueId.UsePill;
                    break;
                default:
                    break;
            }

            return audioUniqueId;
        }
        public static EAudioUniqueId ToAudioUniqueId(ECategory itemCategory, int itemId)
        {
            EAudioUniqueId audioUniqueId = EAudioUniqueId.None;
            switch (itemCategory)
            {
                case ECategory.Weapon:
                    break;
                case ECategory.WeaponPart:

                    var partId  = SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetDefaultPartBySetId(itemId);
                    var partCfg = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(partId);
                    switch ((EWeaponPartType) partCfg.Type)
                    {
                        case EWeaponPartType.UpperRail:
                            audioUniqueId = EAudioUniqueId.PickupSightPart;
                            break;
                        case EWeaponPartType.Magazine:
                            audioUniqueId = EAudioUniqueId.PicupMagazinePart;
                            break;
                        default:
                            audioUniqueId = EAudioUniqueId.PickupWeaponPart;
                            break;
                    }

                    break;
                case ECategory.GameItem:
                    var gameItemConfig = SingletonManager.Get<GameItemConfigManager>().GetConfigById(itemId);
                    switch ((EItemAudioType) gameItemConfig.Type)
                    {
                        case EItemAudioType.Bullet:
                            audioUniqueId = EAudioUniqueId.PickupBullet;
                            break;
                        case EItemAudioType.AidPackage:
                            audioUniqueId = EAudioUniqueId.PickupAidPackage;
                            break;
                        case EItemAudioType.Bandage:
                            audioUniqueId = EAudioUniqueId.PickupBandage;
                            break;
                        case EItemAudioType.Doping:
                            audioUniqueId = EAudioUniqueId.PickupDoping;
                            break;
                        case EItemAudioType.EngeryDrink:
                            audioUniqueId = EAudioUniqueId.PickupEngeryDrink;
                            break;
                        case EItemAudioType.Gasoline:
                            audioUniqueId = EAudioUniqueId.PickupGasoline;
                            break;
                        case EItemAudioType.MedicalPackage:
                            audioUniqueId = EAudioUniqueId.PickupMedicalPackage;
                            break;
                        case EItemAudioType.Pill:
                            audioUniqueId = EAudioUniqueId.PickupPill;
                            break;
                        default:
                            break;
                    }

                    break;
                case ECategory.Avatar:
                    audioUniqueId = EAudioUniqueId.PickupCloth;
                    break;
            }

            return audioUniqueId;
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

        //        public static float GetFootstepPlayInterval(AudioGrp_Footstep stepState)
        //        {
        //            switch (stepState)
        //            {
        //                case AudioGrp_Footstep.Crawl:
        //                    return AudioInfluence.StepPlayInfo.CrawlStepPlayInterval;
        //                case AudioGrp_Footstep.Land:
        //                    return AudioInfluence.StepPlayInfo.WalkStepPlayInterval;
        //                case AudioGrp_Footstep.Squat:
        //                    return AudioInfluence.StepPlayInfo.SquatStepPlayInterval;
        //                case AudioGrp_Footstep.Walk:
        //                    return AudioInfluence.StepPlayInfo.WalkStepPlayInterval;
        //            }
        //
        //            return GlobalConst.DefaultAudioFootstepInterval;
        //        }


        public static void LogPostEventResult(uint playingId, string atomEvtName)
        {
            if (playingId == AkSoundEngine.AK_INVALID_PLAYING_ID && AkSoundEngine.IsInitialized())
            {
                if (GlobalConst.EnableAudioLog)
                    DebugUtil.MyLog("Audio Post event failed:{0}", atomEvtName);
                Logger.ErrorFormat("Audio Post event failed:{0}", atomEvtName);
            }
            else if (GlobalConst.EnableAudioLog)
            {
                DebugUtil.MyLog("Audio Post event {0} sucess", atomEvtName);
            }
        }

        public static bool VerifyAKResult(AKRESULT akresult, object contexts)
        {
            var result = akresult.Sucess();
            if (!result)
            {
                Logger.ErrorFormat("Audio process {1} failed:{0}", akresult, contexts);
                if (GlobalConst.EnableAudioLog)
                {
                    DebugUtil.MyLog("Audio process {1} failed:{0}", akresult, contexts);
                }
            }
            else if (GlobalConst.EnableAudioLog)
            {
                DebugUtil.MyLog("Audio process {0} sucess", contexts);
            }

            return result;
        }
    }
}