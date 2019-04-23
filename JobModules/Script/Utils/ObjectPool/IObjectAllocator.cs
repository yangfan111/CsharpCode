using System;
using System.Text;

namespace Core.ObjectPool
{
    public interface IObjectAllocator
    {
        void PrintDebugInfo(StringBuilder sb);
        object Allocate();
        void Free(object t);
        IObjectFactory Factory { get; set; }
    }

    public class DummyObjectAllocator : IObjectAllocator
    {
        private Type _type;

        public DummyObjectAllocator(Type type)
        {
            _type = type;
        }

        public void PrintDebugInfo(StringBuilder sb)
        {
        }

        public object Allocate()
        {
            return Activator.CreateInstance(_type, true);
        }

        public void Free(object t)
        {
        }

        public IObjectFactory Factory { get; set; }
    }
}