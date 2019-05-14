using App.Shared.Util;
using Core;
using Core.Utils;
using System.Collections.Generic;
using App.Shared.Player.Events;
using UnityEngine;
using XmlConfig;

namespace App.Shared.Audio
{
    internal class AKTypesController
    {

        private Dictionary<int, AudioEmitter> EmitterDict = new Dictionary<int, AudioEmitter>();

        
        internal AudioEmitter GetEmitter(AkGameObj target)
        {
           AudioEmitter emitter;
           var          instanceId = target.gameObject.GetInstanceID();
           if (!EmitterDict.TryGetValue(instanceId, out emitter))
           {
               emitter                 = new AudioEmitter(target);
               EmitterDict[instanceId] = emitter;
           }

           return emitter;
        }

    }
    internal class AudioEmitter
    {
        internal AudioEmitter(AkGameObj target)
        {
            this.target = target;
        }

        private Dictionary<int, AKSwitchAtom> switchDict = new Dictionary<int, AKSwitchAtom>();
        private AkGameObj target;

        public bool NeedDiscard
        {
            get { return target == null; }
        }

        private AKSwitchAtom RegisterGetSwitch(int switchId)
        {
            AKSwitchAtom atom;
            if (!switchDict.TryGetValue(switchId, out atom))
            {
                atom = new AKSwitchAtom(switchId);
                switchDict[switchId] = atom;
            }

            return atom;
        }

        internal void PostEvent(AudioEventItem eventItem, bool skipSwitchSetting)
        {
            if (!skipSwitchSetting && eventItem.SwitchGroup != null && eventItem.SwitchGroup.Count > 0)
            {
                eventItem.SwitchGroup.ForEach(
                    (id) =>
                    {
                        var atom = RegisterGetSwitch(id);
                        atom.SetSwitch(target.gameObject);
                    });
            }

            var playerId = AkSoundEngine.PostEvent(eventItem.Event, target.gameObject,(uint) AkCallbackType.AK_EndOfEvent,GameAudioMedia.OnEventCallback, target);
            AudioUtil.LogPostEventResult(playerId, eventItem.Event);
        }

        internal void SetSwitch(int switchId, int index)
        {
            var switchAtom = RegisterGetSwitch(switchId);
            switchAtom.SetSwitch(index, target.gameObject);
        }
    }


}