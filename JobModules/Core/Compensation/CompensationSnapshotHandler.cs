using Core.EntityComponent;
using Core.Playback;
using Core.Replicaton;

namespace Core.Compensation
{
    class CompensationSnapshotHandler : EntityMapDummyCompareHandler
    {
        private ISnapshot _snapshot;
        private IInterpolationInfo _interpolationInfo;

        public CompensationSnapshotHandler(
            IInterpolationInfo interpolationInfo)
        {
            _snapshot = Snapshot.Allocate();
            _interpolationInfo = interpolationInfo;
        }

        public ISnapshot TheSnapshot
        {
            get { return _snapshot; }
        }

        public override void OnDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity)
        {
            var e = CompensatioSnapshotGameEntity.Allocate(leftEntity.EntityKey);
            _snapshot.AddEntity(e);
            e.ReleaseReference();
        }

        public override void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent,
            IGameEntity rightEntity, IGameComponent rightComponent)
        {
            var localEntity = _snapshot.GetEntity(leftEntity.EntityKey);
            var localComponent = localEntity.AddComponent(rightComponent.GetComponentId());

            var comp = localComponent as IInterpolatableComponent;
            // ReSharper disable once PossibleNullReferenceException
            comp.Interpolate(leftComponent, rightComponent,
                _interpolationInfo);

        }



        public override bool IsExcludeComponent(IGameComponent component)
        {
            return !(component is ICompensationComponent);
        }
    }
}