using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.Audio
{
    public class AKBankAtomSet
    {
        private readonly Dictionary<string, AKBankAtom> bankAtomContenter = new Dictionary<string, AKBankAtom>();

        //完整加载的bank列表
        private readonly HashSet<AKBankAtom> loadedBanks = new HashSet<AKBankAtom>();

        //加载中的bank列表
        private readonly HashSet<AKBankAtom> loadingBanks = new HashSet<AKBankAtom>();
        //  private readonly HashSet<string> bankOnLoadIdList = new HashSet<string>();


        public AKBankAtom Register(string               bnkName,
                                   AudioBank_LoadAction actionType,
                                   AudioBank_LoadMode   modeType)
        {
            AKBankAtom atom;
            if (!bankAtomContenter.TryGetValue(bnkName, out atom))
            {
                atom = new AKBankAtom(bnkName, actionType, modeType);
                bankAtomContenter.Add(bnkName, atom);
            }

            return atom;
        }

        public AKBankAtom Get(string bnk)
        {
            AKBankAtom atom;
            bankAtomContenter.TryGetValue(bnk, out atom);
            return atom;
        }

        public AKRESULT Vertify(AKBankAtom atom)
        {
            if (loadedBanks.Contains(atom))
                return AKRESULT.AK_BankAlreadyLoaded;
            if (loadingBanks.Contains(atom))
                return AKRESULT.AK_BankInLoadingQueue;
            return AKRESULT.AK_Success;
        }
        //TODO:支持多种加载方式
        public void DoLoadBank(AKBankAtom atom,System.Action<AKRESULT> handler)
        {
            LoadPrepare(atom);
            
            AKRESULT akresult = AkBankManager.LoadBankRes(atom.BankName, false, false);
            LoadFinish(atom, akresult);
            if (handler != null)
                handler(akresult);
            else
                AudioUtil.VerifyAKResult(akresult,"Audio load atom:"+atom.BankName);
        }
         void LoadPrepare(AKBankAtom atom)
        {
            loadingBanks.Add(atom);
        }

        void LoadFinish(AKBankAtom atom,AKRESULT akresult)
        {
            loadingBanks.Remove(atom);
            if (akresult.Sucess())
            {
                loadedBanks.Add(atom);
            }
        }
    }
}