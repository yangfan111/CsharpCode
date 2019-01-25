using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace   App.Shared.Audio
{
    public class AKTypesController
    {

        private readonly Dictionary<int, AKSwitchGroup> typeSwitch_GameObjHash_Datas = new Dictionary<int, AKSwitchGroup>();
        private readonly Dictionary<int, AKStateGroup> typeState_ConfigId_Datas = new Dictionary<int, AKStateGroup>();

        public delegate AKGroupCfg GroupCfgFinder(int groupId);
        public AKTypesController()
        {
            var groupList = AKGroupCfg.Gather();
            IEnumerator<AKGroupCfg> enumerator = groupList.GetEnumerator();
            AKGroupCfg cfg;
            while (enumerator.MoveNext())
            {
                cfg = enumerator.Current;
                if (cfg.type == AudioGroupType.StateGroup)
                {
                    typeState_ConfigId_Datas.Add(cfg.id, new AKStateGroup(cfg));
                }
            }
            AKSwitchGroup.CfgFinder = AKGroupCfg.FindById;

        }
       
        public bool IsGameObjectGroupVailed(int groupId,UnityEngine.GameObject target)
        {
            AKAudioEntry.AudioAssert(target != null);
            int instanceId = target.GetInstanceID();
            return typeSwitch_GameObjHash_Datas.ContainsKey(instanceId);
        }


        public AKRESULT VarySwitchState(int groupId, UnityEngine.GameObject target, string state = "")
        {
            AKAudioEntry.AudioAssert(target != null);
            int instanceId = target.GetInstanceID();
            AKSwitchGroup group;
            AKRESULT ret;
            if (typeSwitch_GameObjHash_Datas.TryGetValue(instanceId, out group))
            {
                if (string.IsNullOrEmpty(state))
                {
                    ret = AkSoundEngine.SetSwitch(group.GetGroupName(), group.GetDefaultState(), target);
                }
                else if (!group.CanVaryState(state))
                {
                    return AKRESULT.AK_InvalidStateGroupElement;
                }
                else
                {
                    ret = AkSoundEngine.SetSwitch(group.GetGroupName(), state, target);

                }
            }
            else
            {
                group = AKSwitchGroup.Allocate(groupId, target);
                if (group.CanVaryState(state))
                {
                    ret = AkSoundEngine.SetSwitch(group.GetGroupName(), state, target);
                }
                else
                {
                    ret = AkSoundEngine.SetSwitch(group.GetGroupName(), group.GetDefaultState(), target);
                }
                typeSwitch_GameObjHash_Datas.Add(instanceId, group);
            }
            VaryGroupIfSucess(group, ret, state);
            return ret;

        }
        private void VaryGroupIfSucess(AKStateGroup group, AKRESULT result, string state)
        {
            if (result == AKRESULT.AK_Success)
            {
                group.Change(state);
            }
        }
        public AKRESULT VaryGlobalState(int groupId, string state)
        {
            AKStateGroup group;
            if (!typeState_ConfigId_Datas.TryGetValue(groupId, out group))
            {
                return AKRESULT.AK_InvalidStateGroup;
            }
            if (!group.CanVaryState(state))
            {
                return AKRESULT.AK_InvalidStateGroupElement;
            }
            AKRESULT ret = AkSoundEngine.SetState(group.GetGroupName(), state);
            VaryGroupIfSucess(group, ret, state);
            return ret;
        }
        public void Recycle()
        {
            foreach (var current in typeState_ConfigId_Datas.Values)
            {
                if (!current.IsDefault)
                {
                    AKRESULT result = AkSoundEngine.SetState
                        (current.GetGroupName(), current.GetDefaultState());
                    VaryGroupIfSucess(current, result, current.GetDefaultState());

                }
            }
            foreach (KeyValuePair<int, AKSwitchGroup> pairs in typeSwitch_GameObjHash_Datas)
            {
                if (pairs.Value.Dismiss())
                {
                    typeSwitch_GameObjHash_Datas.Remove(pairs.Key);
                }
                else
                {
                    if (!pairs.Value.IsDefault)
                    {
                        AKRESULT result = AkSoundEngine.SetState
                            (pairs.Value.GetGroupName(), pairs.Value.GetDefaultState());
                        VaryGroupIfSucess(pairs.Value, result, pairs.Value.GetDefaultState());

                    }
                }
            }
        }

    }
}
