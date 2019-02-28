using App.Shared.Util;
using Core.Utils;
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

        public bool Has(GameObject target)
        {
            return refs.Contains(target);
        }
        public int Count { get { return refs.Count; } }
        public virtual bool Register(GameObject target)
        {
            return refs.Add(target);
        }

        public virtual bool UnRegister(GameObject target)
        {
            return refs.Remove(target);
        }
    }
    public class AKBankAtom : AudioRefCounter
    {
        public string BankName { get; private set; }
        public AudioBank_LoadAction LoadAction { get; private set; }
        public AudioBnk_LoadModel LoadModel { get; private set; }

        public static event Action<BankLoadResponseStruct> onLoadFinish;
        public static event Action<AKBankAtom> onLoadBefore;


        //public BankLoadStage LoadStage { get; private set; }

        public AKBankAtom(string bnkName, AudioBank_LoadAction loadAction, AudioBnk_LoadModel loadModel)
        {
            BankName = bnkName;
            LoadAction = loadAction;
            LoadModel = loadModel;
            //  LoadStage = BankLoadStage.Unload;
        }
        public AKBankAtom(string bnkName, Action<BankLoadResponseStruct> in_loadFinishEvt) : this(bnkName, AudioBank_LoadAction.Normal, AudioBnk_LoadModel.Sync)
        {
        }
        //TODO:异步加载
        public void Load(BankResultHandler callback, GameObject target, object userData = null)
        {
            onLoadBefore(this);
            AKRESULT result = AkBankManager.LoadBankRes(BankName, false, false);
            BankLoadResponseStruct callbackData = new BankLoadResponseStruct();
            callbackData.loadResult = result;
            callbackData.callback = callback;
            callbackData.userData = userData;
            callbackData.target = target;
            callbackData.atom = this;
            callbackData.bnkName = BankName;
            onLoadFinish(callbackData);
        }


    }
    public class AKSwitchAtom
    {
        public readonly AudioGroupItem config;
        public string currState;
        public static event Action<AKSwitchAtom, GameObject> onImplentment;
        public AKSwitchAtom(int in_grpId, int stateIndex, GameObject target)
        {
            config = SingletonManager.Get<AudioGroupManager>().FindById(in_grpId);
            
            AssertUtility.Assert(config != null);
            if (stateIndex < 0)
                currState = config.DefaultState;
            else
                currState = config.States[stateIndex];
            if (onImplentment != null)
                onImplentment(this, target);
        }
        public void SetSwitch(int stateIndex, GameObject target)
        {
            if (stateIndex < 0) return;
            string newState = config.States[stateIndex];
            if (newState != currState)
            {
                currState = newState;
                if (onImplentment != null)
                    onImplentment(this, target);
            }
        }
        public void Restore(GameObject target)
        {
            string newState = config.DefaultState;
            if (newState != currState)
            {
                newState = currState;
                if (onImplentment != null)
                    onImplentment(this, target);
            }


        }

    }
    public class AKEventAtom : AudioRefCounter
    {
        //private readonly Dictionary<GameObject, List<AKSwitchAtom>> gameobjSwitchs = new Dictionary<GameObject, List<AKSwitchAtom>>();
        public static event Action<AKEventAtom, GameObject, bool> onImplentment;
        public readonly List<int> attachedGrps;
        public readonly string evtName;
        public AKEventAtom(AudioEventItem config)
        {
            attachedGrps = config.SwitchGroup;
            evtName = config.Event;
        }

        public void PostEvent(GameObject target)
        {
            bool firstPlay = Register(target);
            if (onImplentment != null)
                onImplentment(this, target, firstPlay);

        }

    }


    //public class AKEvent : AKElement
    //{

    //}
    //public class RTPC : AKElement
    //{

    //}

}
