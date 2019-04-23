namespace Core.ObjectPool
{
    public abstract class AbstractReferenceCountedObject<TChild, TObject> : BaseRefCounter where TChild : AbstractReferenceCountedObject<TChild, TObject>
    {
        protected AbstractReferenceCountedObject(TObject item)
        {
            _value = item;
        }

        private TObject _value;

        public TObject Value
        {
            get { return _value; }
        }

        public static TChild Allocate()
        {
            return ObjectAllocatorHolder<TChild>.Allocate();
        }

        protected abstract void ResetObject(TObject obj);

        protected override void OnCleanUp()
        {
            ResetObject(_value);
            ObjectAllocatorHolder<TChild>.Free(this as TChild);
        }


    }
}