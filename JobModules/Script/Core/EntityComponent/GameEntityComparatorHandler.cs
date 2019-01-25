using System.Collections.Generic;

namespace Core.EntityComponent
{
    class GameEntityComparatorHandler : IEnumeratableComparatorHandler<KeyValuePair<EntityKey, IGameEntity>>
    {
        public int ComponentCount;

        public GameEntityComparatorHandler(IEntityMapCompareHandler handler)
        {
            _handler = handler;
            _comparator = new GameEntityComparator(handler);
        }

        private IEntityMapCompareHandler _handler;
        private EntityKeyComparer _comparer = new EntityKeyComparer();
        private GameEntityComparator _comparator;

        public int CompareItem(KeyValuePair<EntityKey, IGameEntity> leftItem,
            KeyValuePair<EntityKey, IGameEntity> rightItem)
        {
            return _comparer.Compare(leftItem.Key, rightItem.Key);
        }

        public void OnLeftItemMissing(KeyValuePair<EntityKey, IGameEntity> rightItem)
        {
            _handler.OnLeftEntityMissing(rightItem.Value);
        }

        public void OnRightItemMissing(KeyValuePair<EntityKey, IGameEntity> leftItem)
        {
            _handler.OnRightEntityMissing(leftItem.Value);
        }

        public void OnItemSame(KeyValuePair<EntityKey, IGameEntity> leftItem,
            KeyValuePair<EntityKey, IGameEntity> rightItem)
        {
            IGameEntity leftEntity = leftItem.Value;
            IGameEntity rightEntity = rightItem.Value;
            ComponentCount += _comparator.Diff(leftEntity, rightEntity,false);


        }

        public bool IsBreak()
        {
            return _handler.IsBreak();
        }
    }
}