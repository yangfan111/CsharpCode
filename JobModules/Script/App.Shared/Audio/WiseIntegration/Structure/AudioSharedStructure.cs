using App.Shared.Audio;
using Core.Utils;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace App.Shared
{

    public class AudioComponent : UnityEngine.MonoBehaviour
    {

    }
    public delegate void BankResultHandler(BankLoadResponseStruct data);
    public class AudioInfluence
    {
        //AKAudioEngineDriver

        public static AudioStepPlayInfo stepPlayInfo;

        public static AudioStepPlayInfo StepPlayInfo
        {
            get
            {
                if(stepPlayInfo == null)
                    stepPlayInfo = new AudioStepPlayInfo(GlobalConst.DefaultAudioFootstepInterval);
                return stepPlayInfo;
            }
            set
            {
                if (value != null)
                    stepPlayInfo =value ;
            }
        }
        
        public const AkCurveInterpolation DefualtCurveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear;//默认插值函数
        public const float DefaultTransitionDuration = 0.0f; //默认转换过渡值
        public const uint PlayingId = AkSoundEngine.AK_INVALID_PLAYING_ID;
        public const uint EndEventFlag = (uint)AkCallbackType.AK_EndOfEvent;
        public const float DefualtVolumeRate = 1.0f;
        public const string PluginName = "Wise";//默认音频插件
        public static event System.Action<bool> onForbiddenOptionVary;
        public readonly static AudioBnk_LoadTactics LoadTactics = AudioBnk_LoadTactics.LoadEntirely;
    

        private static bool isForbidden = false;
        //public const uint CustomPoolMaxNum = 1;
        //public const int CustomPoolOriginCounter = 1001;

    }
    public class AudioFrameworkException : System.Exception
    {

        public AudioFrameworkException(string message, params string[] args) : base("AudioFrame Exception=>" + string.Format(message, args))
        {
        }
    }

    public struct BankLoadRequestStruct
    {
        public string bnkName;
        public AudioBank_LoadAction actionType;
        public AudioBnk_LoadModel modelType;
        public bool ignoreIfAssetNotExist;
        public BankLoadRequestStruct(string in_name)
        {
            bnkName = in_name;
            actionType = AudioBank_LoadAction.Normal;
            modelType = AudioBnk_LoadModel.Sync;
            ignoreIfAssetNotExist = false;
        }
        public BankLoadRequestStruct(string in_name, AudioBank_LoadAction action, AudioBnk_LoadModel model, bool in_ignoreIfAssetNotExist)
        {
            bnkName = in_name;
            actionType = action;
            modelType = model;
            ignoreIfAssetNotExist = in_ignoreIfAssetNotExist;
        }

    }
    public struct BankLoadResponseStruct
    {
        public AKRESULT loadResult;
        public AKBankAtom atom;
        public string bnkName;
        public System.Object userData;
        public UnityEngine.GameObject target;
        public BankResultHandler callback;
        public static BankLoadResponseStruct Create(string bnkName, AKRESULT loadReuslt)
        {

            return Create(bnkName, loadReuslt, null, null);
        }
        public static BankLoadResponseStruct Create(string bnkName, AKRESULT loadReuslt, GameObject target, System.Object userData)
        {
            var response = new BankLoadResponseStruct();
            response.bnkName = bnkName;
            response.loadResult = loadReuslt;
            response.target = target;
            response.userData = userData;
            return response;
        }

    }
    [System.Serializable]
    public class AudioStepPlayInfo
    {

        public float WalkStepPlayInterval;
        public float CrawlStepPlayInterval;
        public float SquatStepPlayInterval;
        public float SwimStepPlayInterval;
        public float DiveStepPlayInterval;
        public AudioStepPlayInfo(float commonInterval)
        {
            WalkStepPlayInterval = commonInterval;
            CrawlStepPlayInterval = commonInterval;
            SquatStepPlayInterval = commonInterval;
            SwimStepPlayInterval = commonInterval;
            DiveStepPlayInterval = commonInterval;
        }

    }

    public enum AudioBank_LoadAction
    {
        DecodeOnLoad,
        DecodeOnLoadAndSave,
        Normal,

    }

    public enum AudioBnk_LoadTactics
    {
        LoadEntirely,
    }
    public enum AudioBnk_LoadModel
    {
        Sync,
        Async,
        Prepare,
    }
    public enum AudioBank_LoadStage
    {
        Unload,
        Loaded,
        Loading,
    }
    //AudioGrp_FootstepState
    public enum AudioGrp_Footstep
    {
        None = -1,
        Walk = 0,
        Squat = 1,
        Crawl = 2,
        //unused
        Land = 3,
        Id = 122,
    }
    public enum AudioGrp_HitMatType
    {
        None = -1,
        Armor = 0,
        Body = 1,
        Helmet = 4,
        Head = 3,
        Concrete = 2,
        //unused
        Metal = 5,
        Water = 6,
        Wood = 7,
        Id = 148,
    }

    public enum AudioClientEffectType
    {
        BulletHit=1,
        BulletDrop=2,
    }
    public enum AudioGrp_Magazine
    {
        None =-1,
        FillBulletOnly=0,//只装弹
        PullboltOnly =1,//只拉栓
        MagizineAndPull=2,//只拉栓
        MagizineOnly=3,//只换弹夹
        Id = 144,
    }
    public enum TerrainMatOriginType
    {
        Default = 0,
        Dirt = 1,
        Grass = 2,
        Rock = 3,
        Sand = 4,

    }
    public enum AudioGrp_ShotMode
    {
        Single = 0,
        Trriple = 1,
        Continue = 2,
        Silencer = 3,
        Id = 102,
    }
    public enum AudioEnvironmentSourceType
    {
        UseBandage,
        UseAidKt,
        UseDrink,
        UseTablet,
        UseEpinephrine,
        UseGasoline,
        OpenParachute,
        OnGliding,
        OnParachute,
        GetDown,
        GetUp,
        ChangeWeapon,
        ChangeMode,
        OpenDoor,
        CloseDoor,
        Walk,//footStep
        Squat,//footStep
        Crawl,//footStep
        WalkSwamp,
        SquatSwamp,
        DropWater,
        Swim,
        Dive,
        CrawlSwamp,
        Land,//footStep
        Length,
    }



    public enum AudioGrp_FootMatType
    {
        Default = 0,
        Grass = 0,
        Concrete = 1,
        Wood = 2,
        Sand = 3,
        Rock = 4,
        Metal = 5,
        Rug = 6,
        Wetland = 7,
        Id = 121,
    }
    public enum AudioGrp_BulletType
    {
        Default =0,
        Lv1  = 0,
        Lv2    = 1,
        Lv3 = 2,
        Id       = 142,
    }

    [System.Obsolete]
    public enum AudioAmbientEmitterType
    {
        ActionOnCustomEventType,
        UseCallback
    }
    [System.Obsolete]
    public enum AudioTriggerEventType
    {

        SceneLoad = 1,
        ColliderEnter = 2,
        CollisionExist = 3,
        MouseDown = 4,
        MouseEnter = 5,
        MouseExist = 6,
        MouseUp = 7,
        GunSimple = 33,
        GunContinue = 34,
        CarStar = 35,
        CarStop = 36,
        Default = 99,

    }

}
