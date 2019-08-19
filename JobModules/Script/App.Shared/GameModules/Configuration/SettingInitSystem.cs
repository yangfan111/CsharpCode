using Core.SessionState;
using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Configuration;
using Utils.SettingManager;
using Utils.Singleton;
namespace App.Shared.GameModules.Configuration
{
    public class SettingInitSystem : IExecuteSystem
    {

        bool init = false;


        ISessionCondition sessionState;

        public SettingInitSystem(ISessionCondition sessionState)
        {
            this.sessionState = sessionState;
            this.sessionState.CreateExitCondition(this.GetType().Name);
        }

        public void Execute()
        {
            
            var ins = SettingManager.GetInstance();
            if (ins.IsInitialized())
            {
                this.sessionState.FullfillExitCondition(this.GetType().Name);
                return;
            }
               

            
            var config = SingletonManager.Get<SettingConfigManager>();
            var video = SingletonManager.Get<SettingVideoConfigManager>();
            var videosetting = SingletonManager.Get<VideoSettingConfigManager>();

            if(!init&&config.IsInitialized&&video.IsInitialized&&videosetting.IsInitialized)
            {
                ins.InitConfig(config.ConfigDic.Values.ToList(), video.ConfigDic.Values.ToList(), videosetting.Configs);
                SettingManager.GetInstance().SetSettingFromLocal();
                SettingManager.GetInstance().FlushData();
                ins.SetQualityByCustomLevel();
                init = true;
                this.sessionState.FullfillExitCondition(this.GetType().Name);
            }
        }

       
    }
}
