using System.Collections;
using System.Collections.Generic;
using Core.ObjectPool;

namespace Core.Utils
{
    public class ReusableList<TItem> : AbstractReferenceCountedObject<ReusableList<TItem>, List<TItem>>
    {
        public static ReusableList<TItem> Allocate()
        {
            return ObjectAllocatorHolder<ReusableList<TItem>>.Allocate();
        }
        protected ReusableList() : base(new List<TItem>())
        {
        }

        protected override void ResetObject(List<TItem> obj)
        {
            obj.Clear();
        }
    }
}