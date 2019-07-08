using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.SettingManager;
using XmlConfig;

namespace ArtPlugins
{
    public class GameQualitySettingBaseMB : MonoBehaviour
    {


        Dictionary<EVideoSettingId, List<Action<float>>> cacheSetters = new Dictionary<EVideoSettingId, List<Action<float>>>();
        Dictionary<EVideoSettingId, Func<string>> cacheGetters = new Dictionary<EVideoSettingId, Func<string>>();


        private void OnDestroy()
        {
            UnRegisterVideoSettingCallback();
        }

        public void RegisterVideoSettingCallback(EVideoSettingId id, Action<float> setter, Func<string> getter)
        {
            List<Action<float>> list = null;
            if (cacheSetters.TryGetValue(id, out list))
            {

            }
            else
            {
                list = new List<Action<float>>();
                cacheSetters[id] = list;
            }
            list.Add(setter);

            cacheGetters[id] = getter;

            GameQualitySettingManager.RegSettingChangeCallback(id, setter, getter);
        }

        public void UseVideoSettingCallbacCallback(EVideoSettingId id, float value)
        {
            GameQualitySettingManager.ApplyVideoEffect(id, value);
        }

        public void UnRegisterVideoSettingCallback()
        {
            foreach (var list in cacheSetters)
            {
                foreach (var item in list.Value)
                {
                    GameQualitySettingManager.UnRegSettingChangeCallback(list.Key, item, null);
                }
            }

            foreach (var item in cacheGetters)
            {
                GameQualitySettingManager.UnRegSettingChangeCallback(item.Key, null, item.Value);
            }

        }

    }
}