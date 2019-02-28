using System.Collections.Generic;
using System;
using UnityEngine;

namespace App.Shared.Audio
{

    ///TODO List:
    ///异常处理
    ///配置化
    ///结果广播
    public partial class AudioBankLoader
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
            switch (AudioInfluence.LoadTactics)
            {
                case AudioBnk_LoadTactics.LoadEntirely:
                    break;
                default:
                    break;
            }
            string[] assetNames = AudioUtil.GetBankAssetNamesByFolder(null);
            foreach (string bankName in assetNames)
            {
                bankAtomSet.Register(new BankLoadRequestStruct(bankName), true);
            }
            IsInitialized = true;
            return AKRESULT.AK_Success;

        }
        public void LoadAtom(string bankName)
        {
            LoadAtom(new BankLoadRequestStruct(bankName));
        }
        public void LoadAtom(string bankName, BankResultHandler handler, GameObject target, object userData = null)
        {
            LoadAtom(new BankLoadRequestStruct(bankName), handler, target, userData);
        }
        public void LoadAtom(BankLoadRequestStruct requrest)
        {
            LoadAtom(requrest, DefaultLoadHandler, null);
        }

        public void LoadAtom(BankLoadRequestStruct requrest, BankResultHandler handler, GameObject target, object userData = null)
        {
            AKRESULT result;
            AKBankAtom atom = bankAtomSet.Get(requrest.bnkName);
            if (atom == null)
            {
                if (requrest.ignoreIfAssetNotExist)
                {
                    if (handler != null)
                        handler(BankLoadResponseStruct.Create(requrest.bnkName, AKRESULT.AK_BankNotLoadYet));
                    return;
                }
                atom = bankAtomSet.Register(requrest, false);

            }
            else
            {
                result = bankAtomSet.Vertify(atom);
                if (result != AKRESULT.AK_Success)
                {
                    if (handler != null)
                        handler(BankLoadResponseStruct.Create(requrest.bnkName, result, target, userData));
                    return;
                }

            }
            atom.Load(handler, target, userData);


        }
        private void DefaultLoadHandler(BankLoadResponseStruct response)
        {
            AudioUtil.AssertProcessResult(response.loadResult, "load {0}", response.atom.BankName);
        }

        //public AKRESULT TryUnloadBnk(int cfgId)
        //{
        //    return TryUnloadBnk(GetBankName(cfgId));
        //}
        //public AKRESULT TryUnloadBnk(string bankName)
        //{
        //    var result = bankAtomSet.VertifyBankUnloadLicense(bankName);
        //    if (result != AKRESULT.AK_Success)
        //        return result;
        //    result = bankAtomSet.Unload(bankName);
        //    //   handlerAgent.BroadcastBankUnloadResult(bankName, result);
        //    return result;
        //}

        //public AKRESULT TryLoadBnkAsync(string bankName)
        //{
        //    var result = bankAtomSet.VertifyBankLoadLicense(bankName);
        //    if (result != AKRESULT.AK_Success)
        //        return result;
        //    return bankAtomSet.LoadAsync(bankName, OnAsyncBnkLoadHandler,
        //        null);
        //}
        //private void InternalLoadBnkHandler(AKBankAtom atom)
        //{
        //    if (atom.InternalAsyncLoad)
        //        TryLoadBnkAsync(atom.GetName());
        //    else
        //        TryLoadBnk(atom.GetName());
        //}
        //private void InteranlUnloadBnkHandler(AKBankAtom atom)
        //{
        //    TryUnloadBnk(atom.GetName());
        //}

        //public AKRESULT TryLoadBnkAsync(string bankName, LoadResultStackDelegate loadResultHandler, System.Object customArgs = null)
        //{
        //    var result = bankAtomSet.VertifyBankLoadLicense(bankName);
        //    if (result != AKRESULT.AK_Success)
        //        return result;
        //    return bankAtomSet.LoadAsync(bankName, OnAsyncBnkLoadHandler,
        //        new LoadResultCallback_Data(bankName, loadResultHandler, customArgs));
        //}

        ////全局异步回调处理
        //private void OnAsyncBnkLoadHandler(uint in_bankID, System.IntPtr in_InMemoryBankPtr, AKRESULT in_eLoadResult,
        //uint in_memPoolId, object in_Cookie)
        //{
        //    LoadResultCallback_Data callbackData = in_Cookie != null ? (LoadResultCallback_Data)in_Cookie : null;
        //    handlerAgent.BroadcastBankLoadResult(callbackData.Name, in_eLoadResult);
        //    if (callbackData != null)
        //    {
        //        callbackData.Call(in_eLoadResult);
        //    }
        //}


        //public AKAudioBankLoader(List<AkBankRes> bankResList)
        //{
        //    handlerAgent = new BankLoadHandlerAgent(InternalLoadBnkHandler, InteranlUnloadBnkHandler);
        //    bankAtomSet = new AKBankAtomSet(handlerAgent);
        //    Recycle(bankResList);
        //}
        //public void Recycle(List<AkBankRes> bankResList)
        //{
        //    if (!IsInitialized)
        //    {
        //        IsInitialized = true;
        //    }
        //    else
        //    {
        //        triggerList.Recycle(false);
        //        var unloadFailList = bankAtomSet.UnloadAll();

        //        if (unloadFailList.Count > 0)
        //        {
        //            //TODO:异常处理
        //        }
        //        bankAtomSet.Recycle();
        //    }
        //    triggerList.BindTarget(AKAudioEntry.PluginsDriver.gameObject);
        //    AKBankAtom atom = null;
        //    foreach (AkBankRes bankData in bankResList)
        //    {
        //        bankAtomSet.Add(bankData, handlerAgent);
        //        List<int> list = atom.GetTriggerList();
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            triggerList.Subscribe((AudioTriggerEventType)list[i], atom.LoadHandler);
        //        }
        //        list = atom.GetUnLoadTriggerList();
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            triggerList.Subscribe((AudioTriggerEventType)list[i], atom.UnloadHandler);
        //        }

        //    }

        //}


    }
}




