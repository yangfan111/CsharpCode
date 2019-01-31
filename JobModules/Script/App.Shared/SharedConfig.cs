

using System;
using System.Collections.Generic;
using App.Shared.Components;
using App.Shared.Components.Vehicle;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared
{
    public class SharedConfig
    {

        static SharedConfig()
        {

            CreateSnapshotThreadCount = LoginServerThreadCount = Environment.ProcessorCount;

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
        public static bool DisableDoor = false;

        public static bool CollectTriggerObjectDynamic = true;

        public static bool ServerAuthorative = false;

        public static bool DynamicPrediction = true;//whether use dynamic prediction for vehicle prediction

        public static bool CalcVehicleCollisionDamageOnClient = true;

        public static bool DisableVehicleCull = false;

        public static bool IsServer;

        public static bool IsOffline;

        /**
         * Only for client
         */
        public static bool IsRobot;

        public static string runTestRame;

        public static GameMode CurrentGameMode = GameMode.Survival;

        public static string femaleModelName = "basicfemale";
        public static string maleModelName = "basicmale";

        public static bool IsLittleEndian = false;

        public static bool MutilThread = true;

        public static bool IsShowTerrainTrace = false;
        
        public static int BulletSimulationIntervalTime = 50;

        public static int BulletLifeTime = 1200;
        public static int CreateSnapshotThreadCount = 8;
        public static int LoginServerThreadCount = 8;

        public static float MaxSaveDistance = 2;
        public static float MaxSaveAngle = 60;
        public static int SaveNeedTime = 10000;

        public static int CullingInterval = 2000;
        public static int CullingRange = 3000;
        public static int CullingRangeMid = 800;
        public static int CullingRangeSmall = 50;
        public static bool CullingOn = false;
        public static bool StopSampler = false;
        public static bool HaveFallDamage = true;
        public static bool InSamplingMode = false;
        public static bool InLegacySampleingMode = false;
        public static int ModeId = 1002;

        /// <summary>
        /// 是否显示self角色包围盒
        /// </summary>
        public static bool ShowCharacterBoundingBox = false;
        public static bool ShowGroundInfo = false;
        // Disable Occlusion Culling
        public static bool EnableOC = true;
        // Disable Distance Culling
        public static bool EnableDC = true;

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
			
	        if (bootCmd.ContainsKey("LegacySampleFps"))
            {
            	SharedConfig.InLegacySampleingMode = true;
	            SharedConfig.DisableDoor = true;
        	}


            if (bootCmd.ContainsKey("DisableOC"))
            {
                SharedConfig.EnableOC = false;
            }

            if (bootCmd.ContainsKey("DisableDC"))
            {
                SharedConfig.EnableDC = false;
            }

            if (bootCmd.ContainsKey("ProfilerDebug"))
            {
                DurationHelp.Debug = true;
            }
            else
            {
                DurationHelp.Debug = false;
            }
        }

        public static bool ShowHitFeedBack;
        public static string RobotActionName = "test2";

    }
}


