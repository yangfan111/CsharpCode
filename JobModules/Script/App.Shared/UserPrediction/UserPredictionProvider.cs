using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using Core.EntityComponent;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.Prediction.UserPrediction.Cmd;
using Core.Replicaton;

namespace App.Client.StartUp
{
    public class UserPredictionProvider : AbstractPredictionProvider
    {
        private PlayerEntity cacheOwner;
        private PlayerContext playerContext;

        private EntityKey self = EntityKey.Default;

        public UserPredictionProvider(ISnapshotSelector snapshotSelector, PlayerContext playerContext,
                                      IGameContexts gameContexts) : base(snapshotSelector, gameContexts)
        {
            this.playerContext = playerContext;
        }

        private PlayerEntity remoteOwnerEntity
        {
            get
            {
                var s = LatestSnapshot.Self;
                if (self != s)
                {
                    cacheOwner = playerContext.GetEntityWithEntityKey(s);
                    self       = s;
                }
                return cacheOwner;
            }
        }

        public override int LastSelfUserCmdSeqId
        {
            get
            {
                var comp = LatestSnapshot.GetEntity(LatestSnapshot.Self).GetComponent<UserCmdSeqComponent>();
                return comp.LastCmdSeq;
            }
        }

        public override IUserCmdOwner UserCmdOwner
        {
            get
            {
                var tmpowner = remoteOwnerEntity;
                if (!tmpowner.hasUserCmdOwner)
                    tmpowner.AddUserCmdOwner(new UserCmdOwnerAdapter(tmpowner));
                return tmpowner.userCmdOwner.OwnerAdater;
            }
        }

        public override void AfterPredictionInit(bool isRewinded)
        {
        }
    }
}