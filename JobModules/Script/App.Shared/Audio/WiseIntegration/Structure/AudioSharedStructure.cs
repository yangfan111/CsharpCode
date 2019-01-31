using Core.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.Audio
{

    public class AudioComponent : UnityEngine.MonoBehaviour
    {

    }
    public delegate void BankResultHandler(BankLoadResponseData data);
    public class AudioInfluence
    {
        //AKAudioEngineDriver
        public const AkCurveInterpolation DefualtCurveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear;//默认插值函数
        public const float DefaultTransitionDuration                = 0.0f; //默认转换过渡值
        public const uint PlayingId                                 = AkSoundEngine.AK_INVALID_PLAYING_ID;
        public const uint EndEventFlag                              = (uint)AkCallbackType.AK_EndOfEvent;
        public const float DefualtVolumeRate                        = 1.0f;
        public const string PluginName                              = "Wise";//默认音频插件
        public static event System.Action<bool> onForbiddenOptionVary;
        public readonly static AudioBnk_LoadTactics LoadTactics     = AudioBnk_LoadTactics.LoadEntirely;
        /// <summary>
        /// 全局音频禁用常量
        /// </summary>
        public static bool IsForbidden
        {
            get { return isForbidden; }
            set
            {
                if (isForbidden != value)
                {
                    isForbidden = value;
                    if (onForbiddenOptionVary != null)
                        onForbiddenOptionVary(isForbidden);
                }
            }
        }

        private static bool isForbidden = false;
        //public const uint CustomPoolMaxNum = 1;
        //public const int CustomPoolOriginCounter = 1001;

    }
    public class AudioFrameworkException : System.Exception
    {
      
        public AudioFrameworkException(string message,params string[]args) :base("AudioFrame Exception=>" + string.Format(message,args))
        {
        }
    }

    public struct BankLoadRequestData
    {
        public string bnkName;
        public AudioBank_LoadAction actionType;
        public AudioBnk_LoadModel modelType;
        public bool ignoreIfAssetNotExist;
        public BankLoadRequestData(string in_name)
        {
            bnkName = in_name;
            actionType = AudioBank_LoadAction.Normal;
            modelType = AudioBnk_LoadModel.Sync;
            ignoreIfAssetNotExist = false;
        }
        public BankLoadRequestData(string in_name, AudioBank_LoadAction action,AudioBnk_LoadModel model,bool in_ignoreIfAssetNotExist)
        {
            bnkName = in_name;
            actionType = action;
            modelType = model;
            ignoreIfAssetNotExist = in_ignoreIfAssetNotExist;
        }

    }
    public struct BankLoadResponseData
    {
        public AKRESULT loadResult;
        public AKBankAtom atom;
        public string bnkName;
        public System.Object userData;
        public UnityEngine.GameObject target;
        public BankResultHandler callback;
        public static BankLoadResponseData Create(string bnkName,AKRESULT loadReuslt)
        {
            
            return Create(bnkName,loadReuslt,null,null);
        }
        public static BankLoadResponseData Create(string bnkName, AKRESULT loadReuslt, GameObject target,System.Object userData)
        {
            var response = new BankLoadResponseData();
            response.bnkName = bnkName;
            response.loadResult = loadReuslt;
            response.target = target;
            response.userData = userData;
            return response;
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

    public enum AudioGrp_ShotModelIndex
    {
        Single = 0,
        Trriple = 1,
        Continue = 2,
        Silencer = 3,
        Id = 102,
    }
    public enum AudioGrp_FootstepMatIndex
    {
        Grass =0,
        Concrete =1,
        Wood =2,
        Sand =3,
        Rock =4,
        Metal =5,
        Rug =6,
        Wetland =7,
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
