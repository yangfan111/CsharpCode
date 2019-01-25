using System.Collections.Generic;
using System;
namespace   App.Shared.Audio
{
    public class AudioTriggerList
    {


        private readonly Dictionary<uint, AudioTriggerBase> triggerCompSet = new Dictionary<uint, AudioTriggerBase>();
        private static readonly Dictionary<uint, string> allTriggerTypes = AudioTriggerBase.GetAllDerivedTypes();
        private UnityEngine.GameObject target;

        public void BindTarget(UnityEngine.GameObject target)
        {
            this.target = target;
        }
        internal void Subscribe(uint triggerID, AudioTriggerBase.Trigger handler)
        {
            if (!target) return;
            string triggerClassName = string.Empty;
            if (!allTriggerTypes.TryGetValue(triggerID, out triggerClassName)) return;
            AudioTriggerBase triggerComp = null;
            if (!triggerCompSet.TryGetValue(triggerID, out triggerComp))
            {
                UnityEngine.Debug.Log(triggerClassName);
                triggerComp = (AudioTriggerBase)target.gameObject.AddComponent(Type.GetType(triggerClassName, true, true));
                triggerCompSet.Add(triggerID, triggerComp);
            }
            triggerComp.Register(handler);

        }
        internal void Subscribe(AudioTriggerEventType triggerType, AudioTriggerBase.Trigger handler)
        {
            Subscribe(AkUtilities.ShortIDGenerator.Compute(triggerType.ToString()), handler);
        }
        internal void Cancel(string triggerName, AudioTriggerBase.Trigger in_delegate = null)
        {
            Cancel(AkUtilities.ShortIDGenerator.Compute(triggerName), in_delegate);
        }
        internal void Cancel(uint triggerID, AudioTriggerBase.Trigger in_delegate = null)
        {
            if (!target) return;
            AudioTriggerBase triggerCmp = null;
            if (triggerCompSet.TryGetValue(triggerID, out triggerCmp))
            {
                if (in_delegate != null)
                    triggerCmp.UnRegister(in_delegate);
                else
                    triggerCmp.triggerDelegate = null;
            }

        }
        internal void Recycle(bool forceReflush = false)
        {
            if (!target) return;

            foreach (var comp in triggerCompSet.Values)
            {
                if (!comp) continue;
                if (forceReflush)
                {
                    UnityEngine.Object.Destroy(comp);
                }
                else
                {
                    comp.triggerDelegate = null;
                }

            }
            if (forceReflush)
                triggerCompSet.Clear();

        }

    }
}
