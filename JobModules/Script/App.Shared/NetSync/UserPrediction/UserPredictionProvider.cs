using App.Shared.Components.Player;
using App.Shared.GameModules;
using Core.EntityComponent;
using Core.Prediction;
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

        private PlayerEntity OwnerEntity
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

        public override IPlayerUserCmdGetter  UserCmdOwner
        {
            get { return OwnerEntity.UserCmdController(); }
        }

        public override void AfterPredictionInit(bool isRewinded)
        {
        }
    }
}