using Core.Utils;
using System;
using System.Collections.Generic;
using Utils.Utils;
using XmlConfig;

namespace Utils.Configuration
{
    public interface IStateTransitionConfigManager
    {
        Dictionary<EPlayerState, HashSet<EPlayerInput>> GetTransitons();
    }

    public class StateInteruptConfigManager : AbstractConfigManager<StateInteruptConfigManager>
    {
        private Dictionary<EPlayerState, StateInterruptItem> configs = new Dictionary<EPlayerState, StateInterruptItem>(CommonIntEnumEqualityComparer<EPlayerState>.Instance);

        public StateInterruptItem GetItem(EPlayerState playerState)
        {
            StateInterruptItem interruptItem;
            configs.TryGetValue(playerState, out interruptItem);
            return interruptItem;
        }
        
        public override void ParseConfig(string xml)
        {
            configs.Clear();
            StateInterruptConfig cfg = null;
            try
            {
                cfg = XmlConfigParser<StateInterruptConfig>.Load(xml);
            }
            catch (Exception e)
            {
                Logger.Error("error : " + e.ToString());
            }
            finally
            {
                foreach(var item in cfg.Items)
                {
                    configs[item.State] = item;
                }
            }
         
        }
    }
    public class StateTransitionConfigManager : AbstractConfigManager<StateTransitionConfigManager>, IStateTransitionConfigManager
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(StateTransitionConfigManager));
        private Dictionary<EPlayerState, StateTransitionConfigItem> _configs = new Dictionary<EPlayerState, StateTransitionConfigItem>(CommonIntEnumEqualityComparer<EPlayerState>.Instance);
        private Dictionary<EPlayerState, HashSet<EPlayerInput>> _transitions = new Dictionary<EPlayerState, HashSet<EPlayerInput>>(CommonIntEnumEqualityComparer<EPlayerState>.Instance);
        public override void ParseConfig(string xml)
        {
            _configs.Clear();
            _transitions.Clear();
            StateTransitionConfig cfg = null;
            try
            {
                cfg  = XmlConfigParser<StateTransitionConfig>.Load(xml);
            }
            catch (Exception e)
            {
                Logger.Error("error : " + e.ToString());
            }
            foreach(var item in cfg.Items)
            {
                item.vsTransition = new bool[(int)Transition.Length];

                foreach (var transition in item.Transitions) {
                    item.vsTransition[(int)transition] = true;
                }

                _configs[item.State] = item;
            }
            InitTransitions();
        }

        private HashSet<EPlayerInput> GetPlayerInputs(StateTransitionConfigItem item)
        {
            var set = new HashSet<EPlayerInput>();
            var itemType = item.GetType();
            for(var e = EPlayerInput.None + 1; e < EPlayerInput.Length; e ++)
            {
                try
                {
                    Transition field = (Transition)Enum.Parse(typeof(Transition), e.ToString());
                    /*var field = itemType.GetField(e.ToString());*/
                    if (null != field)
                    {
                        /*var val = field.GetValue(item);*/
                        var val = item.vsTransition[(int)field];
                        if ((bool)val)
                        {
                            set.Add(e);
                        }
                    }
                }
                catch (Exception excep)
                {
                    Logger.Debug("error : " + excep.ToString() + " e : " + e.ToString());
                }
            }
            return set;
        } 

        public StateTransitionConfigItem GetConditionByState(EPlayerState state)
        {
            StateTransitionConfigItem inputs;
            _configs.TryGetValue(state, out inputs);
            return inputs;
        }

        private void InitTransitions()
        {
            foreach(var pair in _configs)
            {
                _transitions[pair.Key] = GetPlayerInputs(pair.Value);
            }
        }

        public Dictionary<EPlayerState, HashSet<EPlayerInput>> GetTransitons()
        {
            return _transitions;
        }

   }
}