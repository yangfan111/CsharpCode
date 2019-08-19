using Core.EntityComponent;
using Core.Playback;
using Core.Replicaton;

namespace Core.Compensation
{
    class CompensationMapDiffHandler : EntityMapDiffHandlerAdapter
    {
        private ISnapshot snapshot;
        private IInterpolationInfo interpolationInfo;

        public CompensationMapDiffHandler(IInterpolationInfo interpolationInfo)
        {
            snapshot = Snapshot.Allocate();
            this.interpolationInfo = interpolationInfo;
        }

        public ISnapshot TheSnapshot
        {
            get { return snapshot; }
        }

        public override void DoDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity)
        {
            var e = CompensatioSnapshotGameEntity.Allocate(leftEntity.EntityKey);
            snapshot.AddEntity(e);
            e.ReleaseReference();
        }

        public override void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent,
            IGameEntity rightEntity, IGameComponent rightComponent)
        {
            var localEntity = snapshot.GetEntity(leftEntity.EntityKey);
            var localComponent = localEntity.AddComponent(rightComponent.GetComponentId());

            var comp = localComponent as IInterpolatableComponent;
            // ReSharper disable once PossibleNullReferenceException
            comp.Interpolate(leftComponent, rightComponent,
                interpolationInfo);

        }



        public override bool IsExcludeComponent(IGameComponent component)
        {
            return !(component is ICompensationComponent);
        }
    }
}