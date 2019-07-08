using ArtPlugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.SettingManager;

namespace Utils.SettingManager
{
    public interface SettingHandler
    {
        void OnApplySetting(int settingId,string value);
    }


    public class VideoSettingHandler : SettingHandler
    {

        string customQuality = "5";

        public void OnApplySetting(int settingId, string value)
        {
            
            ApplyVideoSetting(settingId,value);
        }



        int MapVideoIdToQualityType(int settingId)
        {
            switch (settingId)
            {
                case 102: return 1;
                case 103: return 2;
                case 104: return 3;
                case 105: return 4;
                case 106: return 5;
                case 107: return 6;
                case 108: return 7;
                default:
                    break;
            }
            return 0;
        }

        void ApplyVideoSetting(int settingId, string value)
        {
            if(settingId == 97)
            {
                SetResolutionAndWindow();
            }
            else if(settingId == 98)
            {
                SetResolutionAndWindow();
            }
            else
            {
                if (settingId== SettingManager.VIDEO_CONTROLID)
                {
                    if(value!=SettingManager.CUSTOM_OPTION_INDEX.ToString())
                    foreach (var item in SettingManager.videoEffectRange)
                    {
                        SetQualitySubOption(item, value);
                    }
                }
                else
                {
                    SetQualitySubOption(settingId, value);
                }
                

            }

        }

        private void SetQualitySubOption(int settingId, string value)
        {
            var type = MapVideoIdToQualityType(settingId);
            if (type != 0)
            {
                if (SettingManager.videoEffectRange.Contains(settingId))
                {
                    var quality = SettingManager.GetInstance().GetSettingValue((int)VideoSettingId.Quality);
                    if (quality != "5")
                    {
                        value = quality;
                    }
                }

                GameQualitySettingManager.SetSettingByType(type, int.Parse(value));
                GameQualitySettingManager.ApplyTypeSetting(type);
            }

           
        }

        float lastEffectTime = 0;
        public  void SetResolutionAndWindow()
        {
            if (Time.time - lastEffectTime < 0.1f) return;
            lastEffectTime = Time.time;


            var res = SettingManager.GetInstance().GetConfigById(98);
            var isfull = SettingManager.GetInstance().GetConfigById(97) == "0";
            string[] xy = res.Split('x');

            Screen.SetResolution(int.Parse(xy[0]), int.Parse(xy[1]), isfull);
        }

    }

}
