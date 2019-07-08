using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Configuration;
using Utils.SettingManager;
using Utils.Singleton;
namespace App.Shared.GameModules.Configuration
{
    public class SettingInitSystem : IInitializeSystem
    {

        public void Initialize()
        {
            
            var ins = SettingManager.GetInstance();
            if (ins.IsInitialized())
                return;
            var config = SingletonManager.Get<SettingConfigManager>();
            var video = SingletonManager.Get<SettingVideoConfigManager>();
            var videosetting = SingletonManager.Get<VideoSettingConfigManager>();

            ins.InitConfig(config.ConfigDic.Values.ToList(), video.ConfigDic.Values.ToList(), videosetting.Configs);
            SettingManager.GetInstance().SetSettingFromLocal();
            SettingManager.GetInstance().FlushData();
            ins.SetQualityByCustomLevel();
            
        }


    }
}
