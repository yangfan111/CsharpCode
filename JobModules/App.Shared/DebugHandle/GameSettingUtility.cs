using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using UnityEngine;

namespace App.Shared.DebugHandle
{
    public static class GameSettingUtility
    {
        private static bool IsMatch(string target)
        {
            if ((SharedConfig.IsServer && target.Equals("s")) ||
                (!SharedConfig.IsServer && target.Equals("c")))
            {
                return true;
            }

            return false;
        }

        public static void SetFrameRate(string target, int frameRate)
        {
            if (IsMatch(target))
            {
                Application.targetFrameRate = frameRate;
                Debug.LogError(Application.targetFrameRate);
            }
        }

        public static string GetQualityName()
        {
            return QualitySettings.names[QualitySettings.GetQualityLevel()];
        }

        public static string[] GetQualityNameList()
        {
            return SharedConfig.IsServer ? null : QualitySettings.names;
        }

        public static void SetQuality(int levelIndex)
        {
            if (!SharedConfig.IsServer)
            {
                var names = QualitySettings.names;
                if (levelIndex >= 0 && levelIndex < names.Length)
                {
                    QualitySettings.SetQualityLevel(levelIndex, true);
                }
            }
            
        }
    }
}
