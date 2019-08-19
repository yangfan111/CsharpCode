

using App.Shared.Components;
using Core;
using Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.SettingManager;
using XmlConfig;

namespace App.Shared
{
    public class SharedConfig
    {

        static SharedConfig()
        {

            CreateSnapshotThreadCount = LoginServerThreadCount = Environment.ProcessorCount>MaxThreadCount?MaxThreadCount:Environment.ProcessorCount;

        }
#if UNITY_EDITOR
        public static bool UserTerrain = false;
#else
        public static bool UserTerrain = true;
#endif
#if UNITY_EDITOR
        public const string IsLocalServer = "IsLocalServer";
        public static bool IsUsingLocalServer 
        {
            get { return PlayerPrefs.GetInt(IsLocalServer, 0) > 0; }
            set { PlayerPrefs.SetInt(IsLocalServer, value ? 1 : 0); }
        }

       


#endif
        public static string RecodFile = "123";
        public static bool IsReplay = false;
        public static bool IsRecord = false;
        
        public static bool DisableDoor = false;

        public static bool CollectTriggerObjectDynamic = true;

        public static bool ServerAuthorative = false;

        public static bool DynamicPrediction = true;//whether use dynamic prediction for vehicle prediction

        public static bool CalcVehicleCollisionDamageOnClient = true;

        public static bool DisableVehicleCull = false;

        public static int VehicleActiveUpdateRate = 1;

        public static bool IsServer;

        public static bool IsOffline;

  
        public static bool isMute;

        public static bool IsMute
        {
            get { return isMute; }
            set
            {
                if (isMute != value)
                {
                    isMute = value;
                    AkSoundEngineController.Instance.ActivateAudio(!value);
                }
            }
        }

        /**
         * Only for client
         */
        public static bool IsRobot;

        public static string runTestRame;

        public static EGameMode CurrentGameMode = EGameMode.Survival;

        public static string femaleModelName = "basicfemale";
        public static string maleModelName = "basicmale";

        public static bool IsLittleEndian = false;

        public static bool MutilThread = true;

        public static bool IsShowTerrainTrace = false;
        
        public static int BulletSimulationIntervalTime = 50;

        public static int BulletLifeTime = 5000;
        public static int CreateSnapshotThreadCount = 8;
        public static int LoginServerThreadCount = 8;
        public static int MaxThreadCount = 8;

        public static float MaxSaveDistance = 2;
        public static float MaxSaveAngle = 60;
        public static int ChickenSaveNeedTime = 10000;
        public static int CommonSaveNeedTime = 3000;

        public static int CullingInterval = 2000;
        public static int CullingRange = 3000;
        public static int CullingRangeMid = 800;
        public static int CullingRangeSmall = 50;
        public static bool CullingOn = false;
        public static bool StopSampler = false;
        public static bool HaveFallDamage = true;
        public static bool InSamplingMode = false;
        public static bool InLegacySampleingMode = false;
        public static bool needConfigLODTree = false;
        public static int ModeId = 1002;

        /// <summary>
        /// 是否显示self角色包围盒
        /// </summary>
        public static bool ShowCharacterBoundingBox = false;
        public static bool ShowGroundInfo = false;
        // 是否关闭下滑
        public static bool EnableSlide = true;
        public static bool DebugAnimation = false;
        public static bool EnableGpui = true;

        public static bool EnableCustomOC = true;

        //大厅传过来的模式，加载需要提前知道
        public static int GameRule = GameRules.Offline;

        public static float WorldShiftDistance = 1000;
        public static float TestWorldShiftDistance = 10;
        
        public static bool WorldShiftEnable = true;
        public static bool WorldShiftDisableTerrainDetail = false;
        public static float WorldShiftTerrainDetailDistance = 60;

        public static bool EnableAnimator = false;
        public static bool DisableAnimator = false;

        public static bool ChangeRole = false;
        public static bool ShowAnimationClipEvent = false;
        public static int ServerFrameRate = 20;

        public static bool IgnoreProp = false;

        public static bool RestoreMultiTag = true;

        public static bool IsHXMod = false;
        public static bool DisableRecycleSetramingGo = false;

        // [PPAN] 是否以MD5方式加载AssetBundles
        public static string ABMD5Manifest = null;

        public static bool GPUSort = false;

        public static bool EnableDepthPrepass = true;
        public static bool EnableHZBCull = true; 

#if HAVE_DEPTH_PREPASS
        public static int GrassQueue = 1300;
#else
        public static int GrassQueue = -1;
#endif

        public static UnityEngine.Vector3 GetPlayerBirthPosition(int entityId)
        {
            
                if (UserTerrain)
                {
                    return new UnityEngine.Vector3(4000, 200, 4000);
                }
                else
                {
                    return new UnityEngine.Vector3(10, 50, 10);
                }
            
        }
        public static UnityEngine.Vector3 GetTerrainInitPosition()
        {
            return new UnityEngine.Vector3(4000, 200, 4000);
        }
        public static UnityEngine.Vector3 GetVehicleBirthPosition(EVehicleType type, int entityId)
        {

            if (UserTerrain)
            {
                return type == EVehicleType.Car ? new UnityEngine.Vector3(4000, 200, 4005): new UnityEngine.Vector3(3950, 85, 3395);
            }
            else
            {
                return type == EVehicleType.Car ? new UnityEngine.Vector3(15, 50, 10) : new UnityEngine.Vector3(0, 2, 103);
            }

        }


        public static bool isFouces = true;

        public static void  InitConfigBootArgs(Dictionary<string,string> bootCmd)
        {
            if (bootCmd.ContainsKey("Offline"))
            {
                SharedConfig.IsOffline = true;
            }

            if (bootCmd.ContainsKey("DisableDoor"))
            {
                SharedConfig.DisableDoor = true;
            }

            if (bootCmd.ContainsKey("SampleFps"))
            {
                SharedConfig.InSamplingMode = true;
                SharedConfig.DisableDoor = true;
            }

            if(bootCmd.ContainsKey("ConfigLODTree"))
            {
                SharedConfig.needConfigLODTree = true;
            }
			
	        if (bootCmd.ContainsKey("LegacySampleFps"))
            {
            	SharedConfig.InLegacySampleingMode = true;
	            SharedConfig.DisableDoor = true;
        	}


            if (bootCmd.ContainsKey("DisableProfileFile"))
            {
                SharedConfig.DisableProfileFile = true;
            }

            if (bootCmd.ContainsKey("ProfilerDebug"))
            {
                DurationHelp.Debug = true;
            }
            else
            {
                DurationHelp.Debug = false;
            }

            if (bootCmd.ContainsKey("DisableGPUI"))
            {
                EnableGpui = false;
            }
            
            if (bootCmd.ContainsKey("DisableGc"))
            {
                SharedConfig.DisableGc = true;
            }
            else
            {
                SharedConfig.DisableGc = false;
            } 
            if (bootCmd.ContainsKey("Token"))
            {
                TestUtility.TestToken = bootCmd["Token"];
            }

            if (bootCmd.ContainsKey("RoleModelId"))
            {
                TestUtility.RoleModelId = int.Parse(bootCmd["RoleModelId"]);
            }

            if (bootCmd.ContainsKey("HXMod"))
            {
                SharedConfig.IsHXMod = true;
            }

            if (bootCmd.ContainsKey("NoIgnoreProp"))
            {
                SharedConfig.IgnoreProp = false;
            }

            if (bootCmd.ContainsKey("IgnoreProp"))
            {
                SharedConfig.IgnoreProp = true;
            }

            if (bootCmd.ContainsKey("DepthPrepass"))
            {
                SharedConfig.EnableDepthPrepass = true;
            }

            if (bootCmd.ContainsKey("HZBCull"))
            {
                SharedConfig.EnableHZBCull = true;
            }

            if (bootCmd.ContainsKey("DisableRecycleSetramingGo"))
            {
                SharedConfig.DisableRecycleSetramingGo = true;
            }
            if (bootCmd.ContainsKey("IsRecord"))
            {
                SharedConfig.IsRecord = true;
            }

            if (bootCmd.ContainsKey("IsReplay"))
            {
                SharedConfig.IsReplay = true;
            }

            if (bootCmd.ContainsKey("RecodFile"))
            {
                SharedConfig.RecodFile = bootCmd["RecodFile"];
            }
			// [PPAN] Add AssetBundles MD5 Support
            if (bootCmd.ContainsKey("ABMD5Support"))
            {
                SharedConfig.ABMD5Manifest = bootCmd["ABMD5Support"];
            }

            if (bootCmd.ContainsKey("quality"))
            {
                SettingManager.GetInstance().SetBootQualityParm(bootCmd["quality"]);
            }

            if (bootCmd.ContainsKey("DisableMT"))
            {
                SharedConfig.MutilThread = false;
            }

        }

        public static bool DisableGc;

        public static bool DisableProfileFile = false;

        public static bool ShowHitFeedBack;
        public static string RobotActionName = "test2";

    }
}


