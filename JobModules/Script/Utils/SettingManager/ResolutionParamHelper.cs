using System;
using Core.Utils;
using UnityEngine;

namespace Utils.SettingManager
{
    public class ResolutionParamHelper
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(ResolutionParamHelper));

        public static void SetParamToSettingManager()
        {
            
            var manager = SettingManager.GetInstance();
            if (manager == null)
            {
                Logger.Error("Can't found settingManager");
                return;
            }

            manager.UseBootParam = true;
            manager.WidthParam = Screen.width;
            manager.HeightParam = Screen.height;
            manager.IsFullScreen = Screen.fullScreen;

        }
    }
}
