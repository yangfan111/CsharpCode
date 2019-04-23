using System.Collections.Generic;
using Utils.Utils;
using XmlConfig;

namespace Utils.Configuration
{
    public interface IStateTransitionConfigManager
    {
        Dictionary<EPlayerState, HashSet<EPlayerInput>> GetTransitons();
    }

    public class StateTransitionConfigManager : AbstractConfigManager<StateTransitionConfigManager>, IStateTransitionConfigManager
    {
        private Dictionary<EPlayerState, StateTransitionConfigItem> _configs = new Dictionary<EPlayerState, StateTransitionConfigItem>(CommonIntEnumEqualityComparer<EPlayerState>.Instance);
        private Dictionary<EPlayerState, HashSet<EPlayerInput>> _transitions = new Dictionary<EPlayerState, HashSet<EPlayerInput>>(CommonIntEnumEqualityComparer<EPlayerState>.Instance);
        public override void ParseConfig(string xml)
        {
            _configs.Clear();
            _transitions.Clear();
            var cfg = XmlConfigParser<StateTransitionConfig>.Load(xml);
            foreach(var item in cfg.Items)
            {
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
                var field = itemType.GetField(e.ToString());
                if(null != field)
                {
                    var val = field.GetValue(item);
                    if((bool)val)
                    {
                        set.Add(e);
                    }
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