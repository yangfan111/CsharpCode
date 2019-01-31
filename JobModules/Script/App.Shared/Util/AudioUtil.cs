using App.Shared.Audio;
using App.Shared.Sound;
using Core.Utils;
using WeaponConfigNs;

public static class AudioUtil
{
    public static readonly LoggerAdapter AudioLogger = new LoggerAdapter(typeof(AKAudioDispatcher));
    public static void AssertProcessResult(AKRESULT result, string s, params object[] args)
    {
        s = string.Format(s, args);
        if (result != AKRESULT.AK_Success && result != AKRESULT.AK_BankAlreadyLoaded)
        {
            DebugUtil.LogInUnity(s + string.Format(" {0} ", result), DebugUtil.DebugColor.Grey);
            AudioLogger.Error(string.Format("[Audio Result Exception]{0}  {1}", s, result));
        }
        else
        {
            DebugUtil.LogInUnity(s + string.Format(" {0} ", result), DebugUtil.DebugColor.Default);
        }
    }
    public static bool Sucess(this AKRESULT result)
    {
        return result == AKRESULT.AK_Success || result == AKRESULT.AK_BankAlreadyLoaded;
    }
  
    public static AudioGrp_ShotModelIndex ToAudioGrpIndex(this EFireMode fireModel)
    {
        
        switch (fireModel)
        {
            case EFireMode.Auto:
                return AudioGrp_ShotModelIndex.Continue;
            case EFireMode.Burst:
                return AudioGrp_ShotModelIndex.Trriple;
            default:
                return AudioGrp_ShotModelIndex.Single;
        }
        
    }
    public static void NLog(string s, params object[] args)
    {
        s = string.Format(s, args);
        DebugUtil.LogInUnity(s, DebugUtil.DebugColor.Default);
        AudioLogger.Info("[Audio Log] " + s);
    }
    public static void ELog(string s, params object[] args)
    {
        s = string.Format(s, args);
        DebugUtil.LogInUnity(s, DebugUtil.DebugColor.Grey);
        AudioLogger.Error("[Audio Error] " + s);
    }
}