using System.Collections.Generic;
using System;
using UnityEngine;

namespace App.Shared.Audio
{
    public  class AudioBankLoader
    {
        public bool IsInitialized { get; private set; }

        //   public readonly BankLoadHandlerAgent handlerAgent;
        //        private readonly AudioTriggerList triggerList = new AudioTriggerList();
        private readonly AKBankAtomSet bankAtomSet;

        public AudioBankLoader()
        {
            //handlerAgent = new BankLoadHandlerAgent(InternalLoadBnkHandler, InteranlUnloadBnkHandler);
            bankAtomSet = new AKBankAtomSet();
        }

        public AKRESULT Initialize()
        {
            if (IsInitialized) return AKRESULT.AK_Success;
            if (!AkSoundEngine.IsInitialized())
                return AKRESULT.AK_Fail;
            string[] assetNames = AudioUtil.GetBankAssetNamesByFolder(null);
            foreach (string bankName in assetNames)
            {
                if(bankName.Contains("hall_"))
                    continue;
                AKBankAtom atom = bankAtomSet.Register(bankName, AudioBank_LoadAction.Normal, AudioBank_LoadMode.Sync);
                bankAtomSet.DoLoadBank(atom, null);
            }

            IsInitialized = true;
            return AKRESULT.AK_Success;
        }

        public void LoadAtom(string bankName, bool ignoreIfAssetNotExist, Action<AKRESULT> handler)
        {
            LoadAtom(bankName, ignoreIfAssetNotExist, AudioBank_LoadAction.Normal, AudioBank_LoadMode.Sync, handler);
        }

        public void LoadAtom(string               bankName,   bool               ignoreIfAssetNotExist,
                             AudioBank_LoadAction loadAction, AudioBank_LoadMode loadMode, Action<AKRESULT> handler)
        {
            AKRESULT   result;
            AKBankAtom atom = bankAtomSet.Get(bankName);
            if (atom == null)
            {
                if (ignoreIfAssetNotExist)
                {
                    if (handler != null)
                        handler(AKRESULT.AK_BankNotLoadYet);
                    return;
                }

                atom = bankAtomSet.Register(bankName, loadAction, loadMode);
            }
            else
            {
                result = bankAtomSet.Vertify(atom);
                if (result != AKRESULT.AK_Success)
                {
                    if (handler != null)
                        handler(result);
                    return;
                }
            }

            bankAtomSet.DoLoadBank(atom, handler);
        }
    }
}