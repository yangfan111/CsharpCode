using System;
using System.Collections.Generic;
using App.Shared.Audio;
using App.Shared.Util;
using Common;
using Core;
using Core.Configuration;
using Core.ObjectPool;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;
using AssetInfo = Utils.AssetManager.AssetInfo;
using Object = UnityEngine.Object;

namespace App.Shared
{
    public enum EEffectStageType
    {
        Sleep,
        WaitCreate,
        AsynLoading,
        Playing,
        WaitFinish
    }


    public interface IClientObjectEmitter
    {
        void Recycle();
    }

    public class ClientEffectEmitter : BaseRefCounter, IClientObjectEmitter
    {
        public static IComparer<ClientEffectEmitter> SequenceIndexComparer = new SequenceIndexRelationalComparer();
        private LoggerAdapter _loggerAdapter = new LoggerAdapter("IClientObjectBehavior");
        private EffectObject _nodeObject;
        private Dictionary<AssetInfo, UnityObject> attachedEffectsObjects = new Dictionary<AssetInfo, UnityObject>();


        public Transform PoolFolder
        {
            get { return LocalObjectGenerator.EffectLocal[classify].poolFolder; }
        }

        public bool IsPreload;
        private EEffectStageType stageType;
        private float timeFinishStamp;

        public EEffectObjectClassify classify { set; get; }

        public EffectObject nodeObject
        {
            get
            {
                if (!_nodeObject)
                {
                    var go = new GameObject(SequenceIndex.ToString());
                    _nodeObject                     = go.AddComponent<EffectObject>();
                    _nodeObject.AudioMono           = go.AddComponent<AkGameObj>();
                    _nodeObject.clientEffectEmitter = this;
                    Enable = false;
                    go.transform.SetParent(PoolFolder);
                }

                return _nodeObject;
            }
        }

        
        public AssetInfo Asset    { get; private set; }

        public float Duration { get; set; }

        public float LoadAsyncTimestamp { get; private set; }

        public EEffectStageType StageType
        {
            get { return stageType; }
            set
            {
                
                stageType = value;
                if (stageType == EEffectStageType.AsynLoading)
                {
                    LoadAsyncTimestamp = MyGameTime.time;
                }
            }
        }

        public uint SequenceIndex { get; set; }

        public bool IsStatic
        {
            get { return Duration < 0; }
        }

        public bool IsValid
        {
            get { return Asset != AssetInfo.EmptyInstance; }
        }

        public bool CanPlayRightNow
        {
            get
            {
                UnityObject unityObject;
                attachedEffectsObjects.TryGetValue(Asset, out unityObject);
                return unityObject != null;
            }
        }

        public bool Enable
        {
            set
            {
                nodeObject.gameObject.SetActive(value);
                if (!value)
                {
                    nodeObject.transform.InactiveSelf(false);
                }
                else
                {
                    nodeObject.gameObject.SetActive(value);
                }
            }
        }


        public void Recycle()
        {
            Enable          = false;
            StageType       = EEffectStageType.Sleep;
            Asset           = AssetInfo.EmptyInstance;
            nodeObject.transform.SetParent(LocalObjectGenerator.EffectLocal[classify].poolFolder);
            Duration        = 0;
            timeFinishStamp = 0;
            IsPreload       = false;
            if (effectBehavior != null)
            {
                effectBehavior.Recycle(this);
                effectBehavior = null;
            }
            
        }

        public static ClientEffectEmitter Allocate()
        {
            return ObjectAllocatorHolder<ClientEffectEmitter>.Allocate();
        }

        protected override void OnCleanUp()
        {
            ObjectAllocatorHolder<ClientEffectEmitter>.Free(this);
        }

        public void Preload(int effectId)
        {
            LoadAsset(effectId);
            IsPreload = true;
        }


        private IEffectBehavior effectBehavior;
        public void Initialize(int effectId, IEffectBehavior behavior)
        {
            effectBehavior = behavior;
            LoadAsset(effectId);
        }

        void LoadAsset(int effectId)
        {
            var effectConfig = SingletonManager.Get<ClientEffectConfigManager>().GetConfigItemById(effectId);
            if (effectConfig != null && null != effectConfig.Asset)
                Asset = new AssetInfo(effectConfig.Asset.BundleName, effectConfig.Asset.AssetName);
            else
                Asset = AssetInfo.EmptyInstance;
            StageType = EEffectStageType.WaitCreate;
        }

        public void OnPreLoadSucess(string s, UnityObject unityObject)
        {
            if (StageType != EEffectStageType.AsynLoading)
            {
                return;
            }

            if (!unityObject.AsGameObject)
            {
                StageType = EEffectStageType.WaitFinish;
                return;
            }

            UnityObject uo;
            if (attachedEffectsObjects.TryGetValue(Asset, out uo))
            {
                _loggerAdapter.ErrorFormat("effect Multi Asset {0} loaded to same effect object {1}", Asset,
                    nodeObject);
                Object.Destroy(uo.AsObject);
            }

            attachedEffectsObjects[Asset] = unityObject;
            StageType                     = EEffectStageType.WaitFinish;
            unityObject.AsGameObject.transform.SetParent(nodeObject.transform);
            _loggerAdapter.InfoFormat("effect preload {0} sucess", unityObject.AsGameObject.name);
        }

        public void OnLoadSucess(string s, UnityObject unityObject)
        {
            if (StageType != EEffectStageType.AsynLoading)
            {
                return;
            }

            if (!unityObject.AsGameObject)
            {
                StageType = EEffectStageType.WaitFinish;
                return;
            }

            UnityObject uo;
            if (attachedEffectsObjects.TryGetValue(Asset, out uo))
            {
                _loggerAdapter.ErrorFormat("effect Multi Asset {0} loaded to same effect object {1}", Asset,
                    nodeObject);
                Object.Destroy(uo.AsObject);
            }

            attachedEffectsObjects[Asset] = unityObject;
            
            _loggerAdapter.InfoFormat("effect load {0} sucess", unityObject.AsGameObject.name);
            DoRealPlay(unityObject);
        }

        public void DoRealPlay(UnityObject unityObject = null)
        {
            unityObject = unityObject ?? attachedEffectsObjects[Asset];

            unityObject.AsGameObject.transform.SetParentWithInitialize(nodeObject.transform);
            Enable = true;
            if (effectBehavior != null)
                effectBehavior.PlayEffect(this, unityObject.AsGameObject);
            timeFinishStamp = MyGameTime.time + Duration;
            StageType = EEffectStageType.Playing;
        }


        public void DoFrameUpdate()
        {
            if (effectBehavior != null)
                effectBehavior.FrameUpdate(this);
            if ((!IsStatic  && timeFinishStamp < MyGameTime.time) || effectBehavior.NeedRecycle)
            {
                StageType = EEffectStageType.WaitFinish;
            }
        }


        private sealed class SequenceIndexRelationalComparer : IComparer<ClientEffectEmitter>
        {
            public int Compare(ClientEffectEmitter x, ClientEffectEmitter y)
            {
                return x.SequenceIndex.CompareTo(y.SequenceIndex);
            }
        }
    }
}