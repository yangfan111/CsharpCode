namespace Core.EntityComponent
{
    public class GameComponentComparatorHandler : IEnumeratableComparatorHandler<IGameComponent>
    {
        public GameComponentComparatorHandler()
        {
          
        }
        
        public void Init(IEntityMapCompareHandler handler)
        {
            _handler = handler;
            _leftEntity = null;
            _rightEntity = null;
        }

        private IEntityMapCompareHandler _handler;
        private IGameEntity _leftEntity;
        private IGameEntity _rightEntity;

        public IGameEntity LeftEntity
        {
            get { return _leftEntity; }
            set { _leftEntity = value; }
        }

        public IGameEntity RightEntity
        {
            get { return _rightEntity; }
            set { _rightEntity = value; }
        }

        public int CompareItem(IGameComponent leftItem, IGameComponent rightItem)
        {
            return leftItem.GetComponentId() - rightItem.GetComponentId();
        }

        public void OnLeftItemMissing(IGameComponent rightItem)
        {
            _handler.OnLeftComponentMissing(_leftEntity, _rightEntity, rightItem);
        }

        public void OnRightItemMissing(IGameComponent leftItem)
        {
            _handler.OnRightComponentMissing(_leftEntity, _rightEntity, leftItem);
        }

        public void OnItemSame(IGameComponent leftItem, IGameComponent rightItem)
        {
            _handler.OnDiffComponent(LeftEntity, leftItem, RightEntity, rightItem);
        }

        public bool IsBreak()
        {
            return _handler.IsBreak();
        }
    }
}