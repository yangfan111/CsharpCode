
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

        public AKBankAtomSet()
        {
            AKBankAtom.onLoadBefore += OnLoadPrepare;
            AKBankAtom.onLoadFinish += OnLoadResult;
        }

        public AKBankAtom Register(BankLoadRequestStruct requestData, bool loadIfSucess)
        {
            AKBankAtom atom;
            if (!bankAtomContenter.TryGetValue(requestData.bnkName, out atom))
            {
                atom = new AKBankAtom(requestData.bnkName, requestData.actionType, requestData.modelType);
                bankAtomContenter.Add(requestData.bnkName, atom);
                if (loadIfSucess)
                {
                    atom.Load(null, null);
                }
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

        void OnLoadPrepare(AKBankAtom atom)
        {
            loadingBanks.Add(atom);
        }

        void OnLoadResult(BankLoadResponseStruct response)
        {
            loadingBanks.Remove(response.atom);
            if (response.loadResult.Sucess())
            {
                loadedBanks.Add(response.atom);
            }
            if (response.callback != null)
                response.callback(response);
            else
                AudioUtil.AssertProcessResult(response.loadResult, "load {0}", response.atom.BankName);
        }
    }


}
