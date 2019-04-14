using Core;
using Core.Utils;
using System.Collections.Generic;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class PlayerStateInputsDataMap 
    {
        private readonly  Dictionary<EPlayerState,PlayerStateInputData> playerStatesDict = 
            new Dictionary<EPlayerState, PlayerStateInputData>((int)EPlayerState.Length,
                CommonIntEnumEqualityComparer<EPlayerState>.Instance);

        private static PlayerStateInputsDataMap instance;
        public static PlayerStateInputsDataMap Instance
        {
            get
            {
                if(instance == null)
                    instance = new PlayerStateInputsDataMap(SingletonManager.Get<StateTransitionConfigManager>().GetTransitons());
                return instance;
            }
        }
        
        public PlayerStateInputsDataMap(Dictionary<EPlayerState, HashSet<EPlayerInput>> datas)
        {
            Reload(datas);
        }

        public void Reload(Dictionary<EPlayerState, HashSet<EPlayerInput>> datas)
        {
            playerStatesDict.Clear();
            foreach(var data in datas)
                playerStatesDict[data.Key] = new PlayerStateInputData(data.Key, data.Value);
        }

        public PlayerStateInputData GetState(EPlayerState state)
        {
            PlayerStateInputData stateVal;
            playerStatesDict.TryGetValue(state, out stateVal);
            return stateVal;
        }
    }
}
