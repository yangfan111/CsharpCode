using Core.Components;
using Core.EntityComponent;

namespace Core.Replicaton
{
    public struct ReplicationFilter
    {
        private bool _isSelf;

        public bool IsSelf
        {
            get { return _isSelf; }
            set { _isSelf = value; }
        }

        public bool IsSyncSelfOrThird(IGameEntity entity, EntityKey self)
        {
            _isSelf = CalcIsSelf(entity, self);

            if (IsSyncNonSelf(entity, _isSelf))
                return true;

            if (IsSyncSelf(entity, _isSelf))
                return true;
            

            return false;
        }

        public bool IsSyncNonSelf(IGameEntity entity, EntityKey self)
        {
            _isSelf = CalcIsSelf(entity, self);

            return IsSyncNonSelf(entity, _isSelf);
        }

        private bool IsSyncNonSelf(IGameEntity entity, bool isSelf)
        {
            if (!isSelf && entity.IsSyncNonSelf && !entity.IsDestroy)
            {
                return true;
            }
            return false;

        }

        public bool IsSyncSelf(IGameEntity entity, bool isSelf)
        {

            if (isSelf && entity.IsSyncSelf && !entity.IsDestroy)
            {
                return true;
            }
            return false;
        }
        public bool IsSyncSelf(IGameEntity entity, EntityKey self)
        {
            _isSelf = CalcIsSelf(entity, self);
            return IsSyncSelf(entity, _isSelf);
        }

        public bool IsCompensation(IGameEntity entity)
        {
            return entity.IsCompensation
                   && !entity.IsDestroy;
        }

        public bool CalcIsSelf(IGameEntity entity, EntityKey self)
        {
            bool isSelf = false;
            if (entity.EntityKey == self)
            {
                isSelf = true;
            }
            else
            {
                if (entity.HasOwnerIdComponent)
                {
                    if (self == entity.OwnerIdComponent.Value)
                        isSelf = true;
                }
                

             
            }
            return isSelf;
        }

    
    }
}