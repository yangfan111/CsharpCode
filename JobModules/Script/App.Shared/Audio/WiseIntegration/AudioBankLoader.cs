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
                if(bankName.StartsWith("Hall_")|| bankName.StartsWith("Map_"))
                    continue;
                AKBankAtom atom = bankAtomSet.Register(bankName, AudioBank_LoadMode.Aync);
                bankAtomSet.DoLoadBank(atom);
            }

            IsInitialized = true;
            return AKRESULT.AK_Success;
        }
        
        public void LoadAtom(string bankName, bool ignoreIfAssetNotExist, WiseReusltHandler handler)
        {
            LoadAtom(bankName, ignoreIfAssetNotExist, AudioBank_LoadMode.Normal, handler);
        }

        public void LoadAtom(string               bankName,   bool               ignoreIfAssetNotExist,
                             AudioBank_LoadMode loadMode,  WiseReusltHandler handler)
        {
            AKBankAtom atom = bankAtomSet.Get(bankName);
            if (atom == null)
            {
                if (ignoreIfAssetNotExist)
                {
                    if (handler != null)
                        handler(AKRESULT.AK_BankNotLoadYet);
                    return;
                }

                atom = bankAtomSet.Register(bankName, loadMode);
            }
            bankAtomSet.DoLoadBank(atom, handler);
        }
    }
}