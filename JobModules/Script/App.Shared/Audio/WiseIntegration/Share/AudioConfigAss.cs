 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.Audio
{
    public class AKGroupCfg
    {
        public int id;
        public string name;
        public AudioGroupType type;
        public List<string> states;
        public string defaultState;
        public List<int> triggerList = new List<int>();
        private static Dictionary<int, AKGroupCfg> dict = new Dictionary<int, AKGroupCfg>();

        public static List<AKGroupCfg> Gather()
        {
            if (dict.Count == 0)
            {
                dict.Add(11, AudioConfigSimulator.SimAKGroup1());

                dict.Add(12, AudioConfigSimulator.SimAKGroup2());
            }

            return dict.Values.ToList<AKGroupCfg>();
        }

        public static AKGroupCfg FindById(int id) { Gather(); return dict[id]; }
    }

    public class AKEventCfg
    {
        public int id;
        public string name;
        public int switchGroup;
        public string bankRef;
        private static Dictionary<int, AKEventCfg> dict = new Dictionary<int, AKEventCfg>();
        public static AKEventCfg FindById(int id) { Gather(); return dict[id]; }
        public static List<AKEventCfg> Gather()
        {
            if(dict.Count==0)
            {
                dict.Add(1, AudioConfigSimulator.SimAKEventCfg1());

                dict.Add(2, AudioConfigSimulator.SimAKEventCfg2());
            }
          
            return dict.Values.ToList<AKEventCfg>();
        }
    }
 
    public static class AudioConfigSimulator
    {
        public static AkBankRes SimAKBankCfg()
        {
            var co = new AkBankRes("Weapon_Footstep", AudioBankLoadType.Normal, new List<int>(), new List<int>());
            return co;

        }
        public static AKEventCfg SimAKEventCfg1()
        {
            var co = new AKEventCfg();
            co.id = 1; 
            co.name = "Gun_56_shot";
            co.bankRef = "Weapon_Footstep";
            co.switchGroup = 11;
            return co;
        }
        public static AKEventCfg SimAKEventCfg2()
        {
            var co = new AKEventCfg();
            co.id = 2;
            co.name = "Gun_P1911_shot";
            co.bankRef = "Weapon_Footstep";
            co.switchGroup = 11;
            return co;
        }
        public static AKGroupCfg SimAKGroup1()
        {
            var co = new AKGroupCfg();
            co.id = 11;
            co.type = AudioGroupType.SwitchGroup;
            co.name = "Gun_shot_mode_type";
            co.states = new List<string>() { "Gun_shot_mode_type_single", "Gun_shot_mode_type_triple" };
            co.defaultState = "Gun_shot_mode_type_single";
            return co;
        }
        public static AKGroupCfg SimAKGroup2()
        {
            var co = new AKGroupCfg();
            co.id = 12;
            co.type = AudioGroupType.StateGroup;
            co.name = "Global_camera";
            co.states = new List<string>() { "Global_camera_overwater", "Global_camera_underwater" };
            co.defaultState = "Global_camera_overwater";
            return co;
        }

    }
}
