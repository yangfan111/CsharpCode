using System;

namespace App.Client.SceneManagement.DistanceCulling.Factory
{
    abstract class CullingHandler : ICacheableElement
    {
        public CullingHandler Sibling;
        public DistCullingCat Category;

        public abstract void StateChanged(bool value);

        public abstract object Clone();

        public virtual void Reset()
        {
            Sibling = null;
        }

        public abstract void Free();
    }
}