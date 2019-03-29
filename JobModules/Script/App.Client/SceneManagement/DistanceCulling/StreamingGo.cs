using System;
using System.Collections.Generic;
using App.Client.SceneManagement.DistanceCulling.Factory;
using App.Shared.SceneManagement.Streaming;
using Shared.Scripts.SceneManagement;
using UnityEngine;
using Utils.AssetManager;

namespace App.Client.SceneManagement.DistanceCulling
{
    class StreamingGo : ICacheableElement
    {
        public StreamingGoKey Key;
        public StreamingGoStatus Status;
        public DistCullingCat Cat;
        
        public UnityObject Go { get; private set; }
        public CullingHandler[] Handlers = new CullingHandler[Constants.DistCullingCatCount];

        private Action<StreamingGo> _reuseHandler;

        #region Debug

        public string AssetName;
        public Vector3 Position;
        public Vector3 Size;

        #endregion

        public StreamingGo(Action<StreamingGo> reuseHandler)
        {
            _reuseHandler = reuseHandler;
        }

        public bool SetGo(UnityObject go)
        {
            if (Status == StreamingGoStatus.Loading)
            {
                Status = StreamingGoStatus.Loaded;
                Go = go;

                return true;
            }
            
            return false;
        }

        public void AddHandler(CullingHandler handler)
        {
            var index = (int) handler.Category;
            
            if (Handlers[index] != null)
                handler.Sibling = Handlers[index];

            Handlers[index] = handler;
        }

        private void RemoveHandlers()
        {
            for (int i = 0; i < Constants.DistCullingCatCount; i++)
            {
                var itor = Handlers[i];
                while (itor != null)
                {
                    itor.Free();
                    itor = itor.Sibling;
                }

                Handlers[i] = null;
            }
        }

        public void SetHandlerActivation(int index, bool value, IStreamingResourceHandler handler)
        {
            if (Status == StreamingGoStatus.Loaded)
            {
                var itor = Handlers[index];
                while (itor != null)
                {
                    itor.StateChanged(value);
                    itor = itor.Sibling;
                }
            }
        }

        public bool SetGoActivatiton(DistCullingCat cat, bool isActivated, IStreamingResourceHandler handler)
        {
            bool ret = false;

            if (cat == Cat)
            {
                switch (Status)
                {
                    case StreamingGoStatus.NotLoaded:
                        if (isActivated)
                        {
                            handler.LoadGo(Key.SceneIndex, Key.GoIndex);
                            Status = StreamingGoStatus.Loading;
                        }

                        break;
                    case StreamingGoStatus.Loading:
                        if (!isActivated)
                            Status = StreamingGoStatus.NotLoaded;
                        
                        break;
                    case StreamingGoStatus.Loaded:
                        if (!isActivated)
                        {
                            handler.UnloadGo(Go, Key.SceneIndex);
                            
                            Status = StreamingGoStatus.NotLoaded;
                            RemoveHandlers();
                            Go = null;
                            ret = true;
                        }

                        break;
                }
            }

            return ret;
        }

        public object Clone()
        {
            return new StreamingGo(_reuseHandler);
        }

        public void Reset()
        {
            Go = null;
            RemoveHandlers();
            Status = StreamingGoStatus.NotLoaded;
        }

        public void Free()
        {
            _reuseHandler(this);
        }
    }

    enum StreamingGoStatus
    {
        NotLoaded,
        Loading,
        Loaded
    }

    struct StreamingGoKey
    {
        public int SceneIndex;
        public int GoIndex;

        public override string ToString()
        {
            return string.Format("[{0}:{1}]", SceneIndex, GoIndex);
        }

        public bool Equals(StreamingGoKey obj)
        {
            return SceneIndex == obj.SceneIndex && GoIndex == obj.GoIndex;
        }

        public int GetHashCode()
        {
            return SceneIndex << 24 + GoIndex;
        }
        
        class StreamingGoKeyComparer : IEqualityComparer<StreamingGoKey>
        {
            public bool Equals(StreamingGoKey x, StreamingGoKey y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(StreamingGoKey obj)
            {
                return obj.GetHashCode();
            }
        }
        
        public static readonly IEqualityComparer<StreamingGoKey> Instance = new StreamingGoKeyComparer();
    }
}