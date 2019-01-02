#if UNITY_EDITOR
using UnityEngine;
using Core.Audio;
[ExecuteInEditMode]
public class AudioCustomizeSettingsComponent : MonoBehaviour
{
    
    public bool isForbiden = false;
    public string audioLoadTypeWhenStarup = "Sync";
    public bool usePicker = false;
    public string wiseInstallationPath = @"E:\Wwise 2017.2.8.6698\";
    public string wiseProjectPath = @"E:\MyWwise\ShengSiJuJi\ShengSiJuJi\ShengSiJuJi.wproj";



    private void Awake()
    {
        if (!Application.isPlaying)
            return;
        AudioConst.IsForbidden = isForbiden;
        AudioConst.AudioLoadTypeWhenStarup = audioLoadTypeWhenStarup;
        AudioCustomizeSettings.SetCreatePacker(usePicker);
        AudioCustomizeSettings.DeveloperWwiseInstallationPath = wiseInstallationPath;
        AudioCustomizeSettings.DeveloperWwiseProjectPath = wiseProjectPath;

    }
    private void Update()
    {
        
    }
    
}

    #endif