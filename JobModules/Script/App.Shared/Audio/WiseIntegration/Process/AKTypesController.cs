using App.Shared.Util;
using Core.Utils;
using System.Collections.Generic;
using UnityEngine;
using XmlConfig;

namespace App.Shared.Audio
{
    public class AKTypesController
    {
        /// <summary>
        /// eventId ==>event
        /// </summary>
        private readonly Dictionary<int, AKEventAtom> events = new Dictionary<int, AKEventAtom>();
        /// <summary>
        /// gameobject=>switchStates
        /// </summary>
        private readonly Dictionary<GameObject, HashSet<AKSwitchAtom>> gameobjectSwitchGrps = new Dictionary<GameObject, HashSet<AKSwitchAtom>>();

        public AKTypesController()
        {
            AKEventAtom.onImplentment += PostEventFinalHandler;
            AKSwitchAtom.onImplentment += SwitchStateFinalHandler;
        }


        private void SwitchStateFinalHandler(AKSwitchAtom atom, GameObject target)
        {
            AKRESULT result = AkSoundEngine.SetSwitch(atom.config.Group, atom.currState, target);
            AudioUtil.AssertProcessResult(result, "set switch {1} {0}", atom.currState,target.name);
        }
        private void PostEventFinalHandler(AKEventAtom atom, GameObject target, bool firstPlayInObject)
        {
            //get switch 
            if (firstPlayInObject && atom.attachedGrps.Count > 0)
            {
                foreach (int grp in atom.attachedGrps)
                {
                    RegisterGetSwitch(target, grp);
                }
            }
            AkSoundEngine.PostEvent(atom.evtName, target);
        }
        /// <summary>
        /// switch获取
        /// </summary>
        /// <param name="target"></param>
        /// <param name="grpId"></param>
        /// <returns></returns>
        public AKSwitchAtom GetSwitch(GameObject target, int grpId)
        {
            HashSet<AKSwitchAtom> switchAtoms;
            if (gameobjectSwitchGrps.TryGetValue(target, out switchAtoms))
            {
                foreach (var atom in switchAtoms)
                {
                    if (atom.config.Id == grpId)
                        return atom;
                }
            }
            return null;
        }
        /// <summary>
        /// 新switch注册获取
        /// </summary>
        /// <param name="target"></param>
        /// <param name="grpId"></param>
        /// <param name="stateIndex"></param>
        /// <returns></returns>
        public AKSwitchAtom RegisterGetSwitch(GameObject target, int grpId, int stateIndex = -1)
        {
            AssertUtility.Assert(target != null);
            HashSet<AKSwitchAtom> switchAtoms;
            AKSwitchAtom ret;
            if (gameobjectSwitchGrps.TryGetValue(target, out switchAtoms))
            {
                foreach (var atom in switchAtoms)
                {
                    if (atom.config.Id == grpId)
                        return atom;
                }
                ret = new AKSwitchAtom(grpId, stateIndex, target);
                switchAtoms.Add(ret);
            }
            else
            {
                ret = new AKSwitchAtom(grpId, stateIndex, target);
                switchAtoms = new HashSet<AKSwitchAtom>() { ret };
                gameobjectSwitchGrps.Add(target, switchAtoms);
            }
            return ret;
        }
        public AKEventAtom RegisterGetEvt(AudioEventItem config)
        {
            AKEventAtom evt;
            if (!events.TryGetValue(config.Id, out evt))
            {
                evt = new AKEventAtom(config);
                events.Add(config.Id, evt);
            }
            return evt;
        }


    }
}
