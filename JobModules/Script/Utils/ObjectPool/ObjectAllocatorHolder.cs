using Core.Utils;

namespace Core.ObjectPool
{
    public class ObjectAllocatorHolder<T> 
    {
        private static IObjectAllocator _allocator;
        
        static ObjectAllocatorHolder()
        {
            
            if (_allocator == null)
            {
                _allocator = ObjectAllocators.GetAllocator(typeof(T));
            }
            
        }

        public static IObjectAllocator GetAllocator()
        {
            return _allocator;
        }

        public static T Allocate()
        {
            var rc = _allocator.Allocate();
            AssertUtility.Assert(rc.GetType() == typeof(T));
            return (T)rc;
        }
        public static void Free(T t)
        {
            _allocator.Free(t);
        }
        
    }
}