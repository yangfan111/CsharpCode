using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using Core.Compensation;
using Core.Components;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.Prediction.UserPrediction.Cmd;
using Core.Replicaton;

namespace App.Client.StartUp
{
    public class UserPredictionInfoProvider : AbstractPredictionInfoProvider<IUserPredictionComponent>, IUserPredictionInfoProvider
    {
        private PlayerContext _playerContext;
        public UserPredictionInfoProvider(
            ISnapshotSelectorContainer snapshotSelector,
            PlayerContext playerContext,
            IGameContexts gameContexts)
            : base(snapshotSelector, gameContexts)
        {
            _playerContext = playerContext;
        }

        public override int RemoteHistoryId
        {
            get
            {
                var comp = LatestSnapshot.GetEntity(LatestSnapshot.Self).GetComponent<UserCmdSeqComponent>();
                return ((UserCmdSeqComponent) comp).LastCmdSeq;
            }
        }

        public IUserCmdOwner UserCmdOwner
        {
            get
            {   
                var owner = _owner;
                if (!owner.hasUserCmdOwner)
                    owner.AddUserCmdOwner(new UserCmdOwnerAdapter(owner));
                return owner.userCmdOwner.OwnerAdater;
            }

        }

        private EntityKey _self = EntityKey.Default;
        private PlayerEntity _cacheOwner;
        private PlayerEntity _owner
        {
            get{
                if (_self.Equals(LatestSnapshot.Self))
                {
                    return _cacheOwner;
                }
                else
                {
                    _cacheOwner = _playerContext.GetEntityWithEntityKey(LatestSnapshot.Self);
                    _self = LatestSnapshot.Self;
                    return _cacheOwner;
                }
            }
        }
        public override void AfterPredictionInit(bool isRewinded)
        {
            
        }

        public override bool IsReady()
        {
            return LatestSnapshot != null;
        }

       
    }
}