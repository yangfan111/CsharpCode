#if UNITY_EDITOR
using UnityEngine;
using Core.Audio;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class AudioCoreSettingsComponent : MonoBehaviour
{

    public AudioCoreSetingData coreSettingData = new AudioCoreSetingData();


    private void Awake()
    {
        if(!Application.isPlaying)
        {
            //TODO:加载,保存本地xml忽略文件
            coreSettingData.isForbiden = false;
            coreSettingData.audioLoadTypeWhenStarup = "Sync";
            coreSettingData.usePicker = false;
            coreSettingData.wiseInstallationPath = @"E:\Wwise 2017.2.8.6698\";
            coreSettingData.wiseProjectPath = @"E:\MyWwise\ShengSiJuJi\ShengSiJuJi\ShengSiJuJi.wproj";
        }
      
    }
}
#endif