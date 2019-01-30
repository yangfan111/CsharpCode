using UnityEngine;
using System.Collections.Generic;
using System;
using XmlConfig;

namespace App.Shared.Audio
{


    public class AKAudioDispatcher
    {
        public  AudioRegulator Regulator { get; private set; }
        //    public AKAudioEngineDriver Driver { get; private set; }
        private AKAudioBankLoader bankLoader;

        private readonly AKTypesController typesController= new AKTypesController();
        public  AKAudioDispatcher(AKAudioBankLoader loader)
        {
            typesController = new AKTypesController();
            bankLoader = loader;
            loader.handlerAgent.BroadcastAsync_LoadSucessEvt += OnAsyncBnkLoadSucess;
            loader.handlerAgent.BroadcastAsync_LoadFailEvt += OnAsyncBnkALoadFail;
            loader.handlerAgent.BroadcastAsync_UpdateRefEvt += OnAsyncBnkLoadSucess;
        }
        ///全局状态控制
        public AKRESULT VaryGlobalState(int groupId, string state)
        {
        
            AKRESULT ret = typesController.VaryGlobalState(groupId, state);
            HandleAKGroupResult(ret, groupId, state,null);
            return ret;
        }
        public AKRESULT VarySwitchState(int groupId, string state, GameObject target)
        {
            AKRESULT ret = typesController.VarySwitchState(groupId, target, state);
            HandleAKGroupResult(ret, groupId, state, target);
            return ret;
        }
        //事件发声之前之前调用
        public AKRESULT SetSwitchDefaultState(int groupId, GameObject target)
        {
            AKRESULT ret = typesController.VarySwitchState(groupId, target, string.Empty);
            HandleAKGroupResult(ret, groupId,string.Empty, target);
            return ret;
        }
        //void OnConfigAssetLoadFinish()
        //{
        //    typesController = new AKTypesController();
        //}
        public AKAudioDispatcher()
        {
            
            //OnConfigAssetLoadFinish+= config load finish
        }
        private static Dictionary<AudioTriggerEventType, string> triggerTypeBehaviors;
        public static Dictionary<AudioTriggerEventType, string> TriggerTypeBehaviors
        {
            get
            {
                if (triggerTypeBehaviors == null)
                {
                    triggerTypeBehaviors = new Dictionary<AudioTriggerEventType, string>();
                    Type et = typeof(AudioTriggerEventType);
                    Array enumArr = System.Enum.GetValues(et);
                    foreach (AudioTriggerEventType etype in enumArr)
                    {
                        string strName = Enum.GetName(et, etype);
                        triggerTypeBehaviors.Add(etype, "On" + strName);
                    }
                }
                return triggerTypeBehaviors;
            }
        }
        
        public void PrepareEvent(int eventId,GameObject target)
        {
            //typesController.VarySwitchState()
        }
        public AKRESULT PostEvent(AudioEventItem econfig, GameObject target)
        {
            AKRESULT ret = bankLoader.TryLoadBnk(econfig.BankRef);
            if (ret == AKRESULT.AK_BankAlreadyLoaded || ret == AKRESULT.AK_Success)
            {
                if (econfig.SwitchGroup.Count > 0)
                {
                    //为当前物体做音频组初始化
                    foreach(var group in econfig.SwitchGroup)
                    {
                        if (!typesController.IsGameObjectGroupVailed(group, target))
                            SetSwitchDefaultState(group, target);
                    }
                }
                //TODO:重载函数的多重应用
                uint playingId = AkSoundEngine.PostEvent(econfig.Event, target);
                return AKRESULT.AK_Success;
            }
            return ret;
        }
        public void PostEvent(int eventId, GameObject target)
        {
            AKEventCfg evtCfg = AKEventCfg.FindById(eventId);
            AKRESULT ret = bankLoader.TryLoadBnk(evtCfg.bankRef);
            if (ret == AKRESULT.AK_BankAlreadyLoaded || ret == AKRESULT.AK_Success)
            {
                //设置Switch初始化
                if (evtCfg.switchGroup > 0 && !typesController.IsGameObjectGroupVailed(evtCfg.switchGroup,target))
                {
                    SetSwitchDefaultState(evtCfg.switchGroup, target);
                  //  ret = typesController.IsGameObjectSwitchGroupVailed(evtCfg.switchGroup, target);
                }
                uint playingId = AkSoundEngine.PostEvent(evtCfg.name, target);
            }
            else
            {
                AudioUtil.AssertInProcess(ret,"Perform bank Load Sync fail,result:{0},bankName:{1},target:{2}", ret, evtCfg.bankRef, target);
            }
        }
        public void PrepareEvent(int eventId)
        {

        }
        
        public void PrepareBank(string bankName)
        {

        }
        void OnAsyncBnkLoadSucess(string bankName)
        {

        }
        void OnAsyncBnkALoadRefUpdate(string bankName)
        {

        }
        void OnAsyncBnkALoadFail(string bankName)
        {

        }
        void HandleAKGroupResult(AKRESULT result,int group,string state,GameObject target )
        {
            //if(result!=AKRESULT.AK_Success)
            //{
            //    AKAudioEntry.AudioLogger.ErrorFormat("[Audio=>Dispather]Perform AKGroup fail,result:{0},errgroup:{1},errState:{2},target:{3}", result, group, string.IsNullOrEmpty(state) ?"default": state, target);
            //}
            //else
            //{
            //    AKAudioEntry.AudioLogger.Info("[Audio=>Dispather]Perform AKGroup sucess");
            //}
        }
    }
}
