using System.Collections.Generic;

namespace Core.Audio
{

    public class AudioComponent : UnityEngine.MonoBehaviour
    {

    }
    public class AudioConst
    {
        //AKAudioEngineDriver
        public const AkCurveInterpolation DefualtCurveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear;//默认插值函数
        public const float DefaultTransitionDuration = 0.0f; //默认转换过渡值
        public const uint PlayingId = AkSoundEngine.AK_INVALID_PLAYING_ID;
        public const uint EndEventFlag = (uint)AkCallbackType.AK_EndOfEvent;
        public const float DefualtVolumeRate = 1.0f;
        public const string PluginName = "Wise";//默认音频插件
        public static event System.Action<bool> onForbiddenOptionVary;
        /// <summary>
        /// 启动默认音频加载方式
        /// </summary>
        private static string audioLoadTypeWhenStarup = "Sync";

        public static string AudioLoadTypeWhenStarup
        {
            get { return audioLoadTypeWhenStarup; }
#if UNITY_EDITOR
            set
            {
                audioLoadTypeWhenStarup = value;
            }
#endif
        }

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
                    IsForbidden = value;
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
        public AudioFrameworkException() : base()
        { }
        public AudioFrameworkException(string message)
            : base("AudioFrame Exception=>" + message)
        {

        }
    }


    //public class AudioRunTimePoolParams
    //{
    //    private static int CustomPoolSize = AkSoundEngineController.s_DefaultPoolSize;
    //    private static int pooIterator = AudioConst.CustomPoolOriginCounter;
    //    static readonly List<int> usedPoolList = new List<int>();
    //    public static bool IsUsed(int poolId)
    //    {
    //        return usedPoolList.Contains(poolId);
    //    }

    //}



    public enum AudioAmbientEmitterType
    {
        ActionOnCustomEventType,
        UseCallback
    }

    public enum AudioBankLoadType
    {
        DecodeOnLoad,
        DecodeOnLoadAndSave,
        Normal,

    }
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

    public enum AudioDataIdentifyType
    {
        Name,
        ID
    }
    public enum AudioGroupType
    {
        SwitchGroup = 1,
        StateGroup = 2,

    }




}
