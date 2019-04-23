#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Core.Compare;
using UnityEngine;
using XmlConfig;
using CompareUtility = Utils.Compare.CompareUtility;

[Serializable]
public class AudioSourceEditorData
{
    public  int                  eventArrIndex = -1; //id or eventName
    public  EAudioEventClassify  EventClassify;
    private List<AudioGroupItem> switchGroups = new List<AudioGroupItem>();

    
    public string[] EventClassifyArr
    {
        get { return EventGrpNames[EventClassify]; }
    }

    public AudioEventItem EventCfg
    {
        get
        {
            eventArrIndex = CompareUtility.LimitBetween<int>(eventArrIndex, 0, EventClassifyArr.Length - 1);
            string evtName = EventClassifyArr[eventArrIndex];
            return nameItemsMap[evtName];
        }
    }

    public int EventId
    {
        get { return EventCfg.Id; }
    }

    public List<AudioGroupItem> EventSwitchGroups
    {
        get
        {
            Initialize();
            switchGroups.Clear();
            if (EventCfg != null)
            {
                var groupIds = EventCfg.SwitchGroup;
                foreach (var id in groupIds)
                {
                    switchGroups.Add(switchGroupMap[id]);
                }
            }

            return switchGroups;
        }
    }

    private static List<AudioGroupItem> stateGroups = new List<AudioGroupItem>();
    public static List<AudioGroupItem> StateGroups
    {
        get
        {
            Initialize();
            return stateGroups;
        }
    }
    private static Dictionary<int, AudioGroupItem>           switchGroupMap;

    public static Dictionary<int, AudioGroupItem> SwitchGroupMap
    {
        get
        {
            Initialize();
            return SwitchGroupMap;
        }
    }
 

    private static Dictionary<EAudioEventClassify, string[]> eventClassifyNames;

    private static List<string> TryGetList(EAudioEventClassify                           eventClassify,
                                           Dictionary<EAudioEventClassify, List<string>> dict)
    {
        List<string> list;
        if (!dict.TryGetValue(eventClassify, out list))
        {
            list                = new List<string>(30);
            dict[eventClassify] = list;
        }

        return list;
    }

    public static  bool                               initialize   = false;
    private static Dictionary<string, AudioEventItem> nameItemsMap = new Dictionary<string, AudioEventItem>();

    public static Dictionary<EAudioEventClassify, string[]> EventGrpNames
    {
        get
        {
            Initialize();
            return eventClassifyNames;
        }
    }

    public static void Initialize()
    {
        if (!initialize)
        {
            switchGroupMap = new Dictionary<int, AudioGroupItem>();
            var              path = Path.Combine(Application.dataPath, "Assets/CoreRes/template/AudioGroup.xml");
            var              text = File.ReadAllText(path);
            AudioGroupConfig data = XmlConfigParser<AudioGroupConfig>.Load(text);
            stateGroups.Clear();
            foreach (var item in data.Items)
            {
                switchGroupMap[item.Id] = item;
                if (item.GroupType == (int) EAudioGroupType.StateGroup)
                {
                    stateGroups.Add(item);
                }
                
            }

            eventClassifyNames = new Dictionary<EAudioEventClassify, string[]>();

            var tmpEventClassifyNames = new Dictionary<EAudioEventClassify, List<string>>();
            path = Path.Combine(Application.dataPath, "Assets/CoreRes/template/AudioEvent.xml");
            text = File.ReadAllText(path);
            AudioEventConfig data1 = XmlConfigParser<AudioEventConfig>.Load(text);
            foreach (var item in data1.Items)
            {
                nameItemsMap[item.Event] = item;
            }

            foreach (var evtItem in nameItemsMap.Values)
            {
                if (evtItem.Event.EndsWith("_stop"))
                {
                    continue;
                }

                EAudioEventClassify classifyType;
                if (evtItem.Id < 1000)
                {
                    classifyType = EAudioEventClassify.Action;
                }
                else if (evtItem.Id < 4000)
                {
                    if (evtItem.Event.Contains("magazine"))
                        classifyType = EAudioEventClassify.Magazine;
                    else if (evtItem.Event.Contains("pullbolt"))
                        classifyType = EAudioEventClassify.Pullbolt;
                    else
                        classifyType = EAudioEventClassify.Weapon;
                }
                else
                {
                    classifyType = EAudioEventClassify.Other;
                }

                List<string> list = TryGetList(classifyType, tmpEventClassifyNames);
                list.Add(evtItem.Event);
            }

            foreach (var keyPair in tmpEventClassifyNames)
            {
                eventClassifyNames.Add(keyPair.Key, keyPair.Value.ToArray());
            }

            initialize = true;
            Debug.Log("XML Reload Sucess");
        }
    }
}
#endif