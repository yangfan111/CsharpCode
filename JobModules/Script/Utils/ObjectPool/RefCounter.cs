using System;
using System.Threading;
using Core.Utils;

namespace Core.ObjectPool
{

    public delegate void CleanUpHandler();

    [Serializable]
    public sealed class RefCounter : BaseRefCounter
    {
      
        private Action _handler;
        private IRefCounter _parent;
        public RefCounter(Action handler, IRefCounter parent)
        {
            _handler = handler;
            _parent = parent;
        }

        protected override void OnCleanUp()
        {
            if (_handler != null)
                _handler();
        }

        public override string ToString()
        {
            return string.Format("{0}, Parent: {1}", base.ToString(), _parent.GetType());
        }
    }

    [Serializable]
    public abstract class BaseRefCounter : IRefCounter
    {
        private const int MaxRef = 1000;
        private int _refCount;
#if UNITY_EDITOR
        private volatile int _currentThread;
        private string _msg;
#endif


        private void SetCurrentThread()
        {
#if UNITY_EDITOR
            //AssertUtility.Assert(_currentThread == 0, _msg);
            _currentThread = Thread.CurrentThread.ManagedThreadId;
#endif
        }

        private void ClearCurrentThread()
        {
#if UNITY_EDITOR
            _currentThread = 0;
#endif
        }

        protected BaseRefCounter()
        {
#if UNITY_EDITOR
            _msg = string.Format("concurrent usage of type {0}" , GetType());
#endif
            AcquireReference();
        }

        public void ReInit()
        {

#if UNITY_EDITOR
            _currentThread = 0;
#endif
            _refCount = 1;
            OnReInit();
        }

        protected virtual void OnReInit()
        {
            
        }

      
        public virtual void AcquireReference()
        {
            SetCurrentThread();
            Interlocked.Increment(ref _refCount);
            AssertUtility.Assert(_refCount < MaxRef);
            ClearCurrentThread();
        }

        public virtual void ReleaseReference()
        {
            SetCurrentThread();
            if (Interlocked.Decrement(ref _refCount) == 0)
            {
                OnCleanUp();
            }
            AssertUtility.Assert(_refCount >= 0);
            ClearCurrentThread();
        }

        protected abstract void OnCleanUp();

        public int RefCount
        {
            get { return _refCount; }
        }

        public override string ToString()
        {
            return string.Format("RefCount: {0}, type {1}", _refCount, GetType());
        }
    }
}