using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.Audio
{
    public interface IRef<T>
    {
        bool Register(T   target);
        bool UnRegister(T target);
        bool Has(T        target);
    }


    public class AudioRefCounter : IRef<GameObject>
    {
        private readonly HashSet<GameObject> refs = new HashSet<GameObject>();

        public bool Has(GameObject target)
        {
            return refs.Contains(target);
        }

        public int Count
        {
            get { return refs.Count; }
        }

        public virtual bool Register(GameObject target)
        {
            return refs.Add(target);
        }

        public virtual bool UnRegister(GameObject target)
        {
            return refs.Remove(target);
        }
    }

    public class AKSwitchAtom
    {
        public readonly AudioGroupItem config;
        public          int            stateIndex = -1;

        public AKSwitchAtom(int in_grpId)
        {
            config = SingletonManager.Get<AudioGroupManager>().FindById(in_grpId);
        }

        public void InitSwitch(GameObject target)
        {
            if (this.stateIndex < 0)
            {
                AKRESULT akresult = AkSoundEngine.SetSwitch(config.Group, config.StateArr[0], target);
                if (AudioUtil.VerifyAKResult(akresult, string.Format("AKSwitch state:{0}-{1}", config.Group,config.StateArr[0])))
                {
                    this.stateIndex = 0;
                }
            }
        }

        public void SetSwitch(int stateIndex, GameObject target)
        {
            if (stateIndex < 0) return;
            if (stateIndex != this.stateIndex || this.stateIndex < 0)
            {
                AKRESULT akresult;
                if (config.StateArr.Length > stateIndex)
                {
                    akresult = AkSoundEngine.SetSwitch(config.Group, config.StateArr[stateIndex], target);
                }
                  
                else
                {
                    AudioUtil.Logger.ErrorFormat("wise group:{0},state {1}", config.Group, stateIndex);  
                    akresult = AKRESULT.AK_Fail;
                }
                  
                if (AudioUtil.VerifyAKResult(akresult, "AKSwitch state:"+config.Group))
                {
                    this.stateIndex = stateIndex;
                }
            }
        }
    }

    public class AKBankAtom : AudioRefCounter
    {
        public string               BankName   { get; private set; }
        public AudioBank_LoadAction LoadAction { get; private set; }
        public AudioBank_LoadMode   LoadMode   { get; private set; }


        //public BankLoadStage LoadStage { get; private set; }

        public AKBankAtom(string bnkName, AudioBank_LoadAction loadAction, AudioBank_LoadMode loadMode)
        {
            BankName   = bnkName;
            LoadAction = loadAction;
            LoadMode   = loadMode;
            //  LoadStage = BankLoadStage.Unload;
        }

        public AKBankAtom(string bnkName) : this(bnkName,
            AudioBank_LoadAction.Normal, AudioBank_LoadMode.Sync)
        {
        }


        //public class AKEvent : AKElement
        //{

        //}
        //public class RTPC : AKElement
        //{

        //}
    }
}