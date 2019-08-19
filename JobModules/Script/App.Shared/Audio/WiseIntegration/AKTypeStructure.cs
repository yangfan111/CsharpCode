using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.Audio
{
    public interface IRef<T>
    {
        bool Register(T target);
        bool UnRegister(T target);
        bool Has(T target);
    }


    public class AudioRefCounter : IRef<GameObject>
    {
        private readonly HashSet<GameObject> refs = new HashSet<GameObject>();

        public int Count
        {
            get { return refs.Count; }
        }

        public bool Has(GameObject target)
        {
            return refs.Contains(target);
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
        public int stateIndex = -1;

        public AKSwitchAtom(int in_grpId)
        {
            config = SingletonManager.Get<AudioGroupManager>().FindById(in_grpId);
        }

        public void InitSwitch(GameObject target)
        {
            if (stateIndex < 0)
            {
                AKRESULT akresult = AkSoundEngine.SetSwitch(config.GetConvertedGroupId(), config.GetConvertedGroupStateId(0), target);
                if (AudioUtil.VerifyAKResult(akresult, "AKSwitch state:{0}-{1}", config.Group, config.StateArr[0]))
                {
                    stateIndex = 0;
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
                    akresult = AkSoundEngine.SetSwitch(config.GetConvertedGroupId(), config.GetConvertedGroupStateId(stateIndex), target);
                }

                else
                {
                    AudioUtil.Logger.ErrorFormat("wise group:{0},state {1}", config.Group, stateIndex);
                    akresult = AKRESULT.AK_Fail;
                }

                if (AudioUtil.VerifyAKResult(akresult, "AKSwitch state:{0}", config.Group))
                {
                    this.stateIndex = stateIndex;
                }
            }
        }
    }

    public class AKBankAtom
    {
        private event WiseReusltHandler lastReulstHandler;
        //public BankLoadStage LoadStage { get; private set; }

        public AKBankAtom(string bnkName, AudioBank_LoadMode loadMode)
        {
            BankName   = bnkName;
            LoadMode = loadMode;
            //  LoadStage = BankLoadStage.Unload;
        }

        public readonly string BankName;
        public readonly AudioBank_LoadMode LoadMode;

        public bool Execute(WiseReusltHandler handler, out AKRESULT akresult)
        {
            switch (LoadMode)
            {
                case AudioBank_LoadMode.Aync:
                    akresult = AkBankManager.LoadBank(BankName, DefaultAyncHandler);
                    if(akresult == AKRESULT.AK_BankAlreadyLoaded)
                        return true;
                    if(handler != null)
                        lastReulstHandler += handler;
                    return false;
                case AudioBank_LoadMode.DecodeOnLoad:
                    akresult = AkBankManager.LoadBank(BankName, true, false);
                    break;

                case AudioBank_LoadMode.DecodeOnLoadAndSave:
                    akresult = AkBankManager.LoadBank(BankName, true, true);
                    break;


                case AudioBank_LoadMode.Normal:
                    akresult = AkBankManager.LoadBank(BankName);
                    break;
                default:
                    akresult = AKRESULT.AK_UnHandled;
                    break;
            }

            return true;
        }

        private void DefaultAyncHandler(uint inBankId, IntPtr inInMemoryBankPtr, AKRESULT inELoadResult, uint inMemPoolId,
                                   object inCookie)
        {
            AudioUtil.Logger.InfoFormat("[Wise] Aync Loading {0} {1}", BankName, inELoadResult);
            if (lastReulstHandler != null)
            {
                lastReulstHandler(inELoadResult);
                lastReulstHandler = null;
                
            }
        }

        //public class AKEvent : AKElement
        //{

        //}
        //public class RTPC : AKElement
        //{

        //}
    }
}