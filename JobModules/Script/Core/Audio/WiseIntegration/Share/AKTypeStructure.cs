using System.Collections.Generic;

namespace   Core.Audio
{
 
    public class AKType 
    {

        public string Name { get; protected set; }
        //如果使用，就作为配置id
        public int ID { get; protected set; }

        public virtual bool IsUsable()
        {
            return true;
            // return TriggerList != null;
        }
        public AKType(int id, List<int> triggerList)
        {

            ID = id;
            this.TriggerList = triggerList;
        }
        public AKType(List<int> triggerList)
        {
            this.TriggerList = triggerList;
        }
        public AKType() { }
        //用于trigger触发加载
        public List<int> TriggerList { get; protected set; }
    }
    public class AkBankRes : AKType
    {
        public const AudioDataIdentifyType Identify = AudioDataIdentifyType.Name;
        //用于同步加载选项
        public AudioBankLoadType LoadType { get; set; }
        //   public const AudioBankIdentifyType IndexType = AudioBankIdentifyType.Name;
        //是否后台加载，用于trigger触发加载
        public bool LoadAsynchronous { get; set; }


        //用于trigger触发卸载
        public List<int> UnLoadTriggerList { get; protected set; }
        public AkBankRes(string name, AudioBankLoadType loadType, List<int> triggerList,
            List<int> unloadTriggerList) : base(triggerList)
        {
            this.Name = name;
            LoadType = loadType;
            this.UnLoadTriggerList = unloadTriggerList;
        }
    }
    public class AKStateGroup : AKType
    {
        public const AudioDataIdentifyType Identify = AudioDataIdentifyType.ID;
        public string CurrState { get; private set; }

        private AKGroupCfg source;
       


        public AKStateGroup(AKGroupCfg cfg) 
        {
            source = cfg;
            ID = cfg.id;
            TriggerList = cfg.triggerList;
            CurrState = cfg.defaultState;
        }
        public string GetGroupName() { return source.name; }
        public string GetDefaultState() { return source.defaultState; }
        
        public bool IsDefault
        {
            get
            {
                return CurrState == source.defaultState;
            }
        }
        public virtual bool CanVaryState(string state)
        {
            return (state != CurrState && source.states.Contains(state));
        }
        public void Reset()
        {
            CurrState = source.defaultState;
        }
        public void Change(string state)
        {
            CurrState = state;
        }
     


    }
    public class AKSwitchGroup : AKStateGroup
    {
        public static AKTypesController.GroupCfgFinder CfgFinder { private get; set; }
        
       public static AKSwitchGroup Allocate(int id,UnityEngine.GameObject target)
        {
            return new AKSwitchGroup(CfgFinder(id),target);

        }
        public UnityEngine.GameObject BindObject{ get; private set; }
        public AKSwitchGroup(AKGroupCfg cfg,UnityEngine.GameObject target) : base(cfg)
        {
            BindObject = target;
        }
        public override bool CanVaryState(string state)
        {
            return BindObject && base.CanVaryState(state);
        }
        public bool Dismiss()
        {
            return BindObject==null;
        }
    

    }
    public class AKRTPC : AKType
    {
        public readonly float defaultValue;
        public float Value { get; protected set; }
        public AKRTPC(int id, List<int> triggerList, float value) : base(id, triggerList)
        {
            defaultValue = value;
            Value = value;
        }
        public bool Verify(float value)
        {
            return Value != value && value > 0;
        }
        public void Reset(float value) { Value = value; }

        public void Restore()
        {
            Value = defaultValue;
        }

    }
    //public class AKEvent : AKElement
    //{

    //}
    //public class RTPC : AKElement
    //{

    //}

}
