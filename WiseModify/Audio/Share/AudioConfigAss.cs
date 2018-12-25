using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace   App.Shared.Audio
{
    public class AKGroupCfg
    {
        public int id;
        public string name;
        public AudioGroupType type;
        public List<string> states;
        public string defaultState;
        public List<int> triggerList;
        public static List<AKGroupCfg> Gather { get { return new List<AKGroupCfg>(); } }
        public static AKGroupCfg FindById(int id) { return new AKGroupCfg(); }
    }

    public class AKEventCfg
    {
        public int id;
        public string name;
        public int switchGroup;
        public string bankRef;
        public static AKEventCfg FindById(int id) { return new AKEventCfg(); }

    }
    public static class AudioConfigSimulator
    {
        public static AkBankRes SimulateBankConfig()
        {
            var co = new AkBankRes("Test",AudioBankLoadType.Normal,new List<int>(),new List<int>());
            return co;

        }
        
    }
}
