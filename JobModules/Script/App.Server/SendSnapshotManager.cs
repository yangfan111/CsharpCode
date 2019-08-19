
#define ENABLE_NEW_SENDSNAPSHOT_THREAD
using System;
using System.Collections.Generic;
using App.Shared;
using App.Shared.Components.Player;
using Core.EntityComponent;
using Core.EntityComponent;
using Core.Network;
using Core.ObjectPool;
using Core.Replicaton;
using Core.SpatialPartition;
using Core.ThreadUtils;
using Core.Utils;
using Entitas;
using Sharpen;
using Utils.Concurrent;
using Utils.Singleton;
using Utils.Utils;
using System.Threading;

namespace App.Server
{
    public class CreateSnapshotParams : BaseRefCounter
    {
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(CreateSnapshotParams))
            {
            }

            public override object MakeObject()
            {
                return new CreateSnapshotParams();
            }
        }

        public CreateSnapshotParams Build(SnapshotFactory snapshotFactory, PlayerEntity player, Bin2DConfig bin2DConfig,
            IBin2DManager bin, int serverTime, int snapshotSeq, int vehicleSimulationTime, INetworkChannel channel,
            Contexts _newContexts)
        {
            SnapshotFactory = snapshotFactory;
            Player = player;
            Bin2DConfig = bin2DConfig;
            Bin = bin;
            ServerTime = serverTime;
            SnapshotSeq = snapshotSeq;
            VehicleSimulationTime = vehicleSimulationTime;
            Channel = channel;
            PreEnitys.Clear();
            _contexts = _newContexts;
            Status=0;
            return this;
        }

        public SnapshotFactory SnapshotFactory;
        public PlayerEntity Player;
        public Bin2DConfig Bin2DConfig;
        public IBin2DManager Bin;
        public int ServerTime;
        public int SnapshotSeq;
        public int VehicleSimulationTime;
        public INetworkChannel Channel;
        public ISnapshot Snapshot;
        public int Status;
        public List<IGameEntity> PreEnitys = new List<IGameEntity>();

        public Contexts _contexts;

        protected override void OnCleanUp()
        {
            Snapshot = null;
            SnapshotFactory = null;
            Player = null;
            Bin2DConfig = null;
            Bin = null;
            ServerTime = 0;
            SnapshotSeq = 0;
            VehicleSimulationTime = 0;
            Channel = null;

            PreEnitys.Clear();
            _contexts = null;
            ObjectAllocatorHolder<CreateSnapshotParams>.Free(this);
        }
    }


    public class SendSnapshotManager : IDisposable
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SendSnapshotManager));


        private ConsumerThread<CreateSnapshotParams, int>[] _createSnapshopThreads;
        private bool[] _createSnapshopThreadsStat;
        private readonly Contexts _contexts;
        private readonly IGroup<FreeMoveEntity> _globalFreeMoveEntities;
        private readonly IGroup<WeaponEntity> _globalWeaponEntities;
        private readonly IGroup<PlayerEntity> _playerEntities;
        private readonly CustomProfileInfo SendSnapshotWait;
        public SendSnapshotManager(Contexts contexts)
        {
            SendSnapshotWait = SingletonManager.Get<DurationHelp>()
                .GetProfileInfo(CustomProfilerStep.SendSnapshotWait);
            _contexts = contexts;
            _globalFreeMoveEntities =
                _contexts.freeMove.GetGroup(FreeMoveMatcher.AllOf(FreeMoveMatcher.GlobalFlag,
                    FreeMoveMatcher.EntityAdapter));
            _globalWeaponEntities = _contexts.weapon.GetGroup(WeaponMatcher.AllOf(WeaponMatcher.EntityKey, WeaponMatcher.EntityAdapter));
            _playerEntities = _contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.Network));
#if ENABLE_NEW_SENDSNAPSHOT_THREAD
            InitThreads();
#endif
        }
        BlockingQueue<CreateSnapshotParams> _queue = new BlockingQueue<CreateSnapshotParams>(100);
        private void InitThreads()
        {
            if (SharedConfig.MutilThread)
            {
                _createSnapshopThreadsStat = new bool[SharedConfig.CreateSnapshotThreadCount];
                _createSnapshopThreads =
                    new ConsumerThread<CreateSnapshotParams, int>[SharedConfig.CreateSnapshotThreadCount];
                for (int i = 0; i < _createSnapshopThreads.Length; i++)
                {
                    _createSnapshopThreads[i] =
                        new ConsumerThread<CreateSnapshotParams, int>(
                            string.Format("CreateSnapshotThread_{0}", i),
                            CreateSendSnapshot, _queue);
                    _createSnapshopThreads[i].Start();
                }
            }
        }

        List<CreateSnapshotParams> _sendSnapshotTasks = new List<CreateSnapshotParams>();

        public void SendSnapshot(int interval, SnapshotFactory snapshotFactory)
        {
            var sessionObjects = _contexts.session.serverSessionObjects;
            Bin2DConfig bin2DConfig = sessionObjects.Bin2DConfig;
            IBin2DManager bin = sessionObjects.Bin2dManager;
            int snapshotSeq = sessionObjects.GetNextSnapshotSeq();
            int vehicleSimulationTime = sessionObjects.SimulationTimer.CurrentTime;
            int serverTime = _contexts.session.currentTimeObject.CurrentTime;
            _sendSnapshotTasks.Clear();
            var freeMoveEntitys = _globalFreeMoveEntities.GetEntities();
            var weaponEntities = _globalWeaponEntities.GetEntities();
   
            foreach (var player in _playerEntities.GetEntities())
            {
                if (player.hasStage &&
                    player.stage.CanSendSnapshot() &&
                    player.network.NetworkChannel.IsConnected &&
                    !player.network.NetworkChannel.Serializer.MessageTypeInfo.SkipSendSnapShot(serverTime))
                {
                    var p = ObjectAllocatorHolder<CreateSnapshotParams>.Allocate().Build(snapshotFactory, player,
                        bin2DConfig, bin, serverTime,
                        snapshotSeq,
                        vehicleSimulationTime,  player.network.NetworkChannel,
                        _contexts);

                    var entitys = p.PreEnitys;
                    AddTeamPlayers(player, entitys, _contexts);
                    AddGlobalFreeMove(player, entitys, freeMoveEntitys);
                    AddWeapon(player, entitys, weaponEntities);
                   
                    _sendSnapshotTasks.Add(p);
#if ENABLE_NEW_SENDSNAPSHOT_THREAD

                    if (SharedConfig.MutilThread)
                    {
                        _queue.AddRef();
                        _queue.Enqueue(p);
                    }
#endif
   
                    //_logger.InfoFormat("SendSnapshot:{0} {1}",player.entityKey.Value, player.position.Value);
                }
                else
                {
                    player.network.NetworkChannel.Serializer.MessageTypeInfo.IncSendSnapShot();
                    _logger.DebugFormat("channel:{2} skip SendSnapshot :{0} {1}!",  player.network.NetworkChannel.IsConnected,
                        !player.network.NetworkChannel.Serializer.MessageTypeInfo.SkipSendSnapShot(serverTime),
                        player.network.NetworkChannel.IdInfo());
                }
            }

            if (SharedConfig.MutilThread)
            {
#if ENABLE_NEW_SENDSNAPSHOT_THREAD

                ConsumerExecute();
#else
                MutilExecute();
#endif
            }
            else
            {
                MainExecute();
            }

            _logger.DebugFormat("SendSnapshot Threads Done;");
        }


        private void MainExecute()
        {
            foreach (var p in _sendSnapshotTasks)
            {
                CreateSendSnapshot(p);
            }
        }

        private void MutilExecute()
        {
            var spinWait = SpinWaitUtils.GetSpinWait();
            MutilExecute<int, CreateSnapshotParams> mutilExecute =
                new MutilExecute<int, CreateSnapshotParams>(SharedConfig.CreateSnapshotThreadCount,
                    _sendSnapshotTasks, CreateSendSnapshot);
            mutilExecute.Start();
           
            try
            {
              
                SendSnapshotWait.BeginProfile();
                while (!mutilExecute.IsDone())
                {
                    _logger.DebugFormat("SendSnapshot ThreadsRunning;{0}", mutilExecute.ThreadsRunning);
                    spinWait.SpinOnce();
                }
            }
            finally
            {
                SendSnapshotWait.EndProfile();
            }



        }

        private void ConsumerExecute()
        {
            try
            {
                ChannelWorker.IsSuspend = true;
                


                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.SendSnapshotWait);

                WaitConsumerExecute();
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.SendSnapshotWait);
                ChannelWorker.IsSuspend = false;
            }
        }

        private void WaitConsumerExecute()
        {
            bool isRunning = true;
            int createSnapshoThreadsCount = _createSnapshopThreads.Length;

            for (int i = 0; i < createSnapshoThreadsCount; i++)
            {
                _createSnapshopThreadsStat[i] = false;
            }
            var spinWait = SpinWaitUtils.GetSpinWait();
            long start = DateTime.UtcNow.ToMillisecondsSinceEpoch();
            while (isRunning)
            {
                spinWait.SpinOnce();
                int doneCount = 0;
                isRunning = false;
                var now = DateTime.UtcNow.ToMillisecondsSinceEpoch();
                if (now - start > 10000) {
                    _logger.DebugFormat("while error, enter offline !");
                    break;
                }
                for (int i = 0; i < createSnapshoThreadsCount; i++)
                {

                    var consumerThread = _createSnapshopThreads[i];
                    if (!consumerThread.IsDone() && !_createSnapshopThreadsStat[i])
                    {
                        isRunning = true;
                    }
                    else
                    {
                        if (!_createSnapshopThreadsStat[i])
                        {
                            _createSnapshopThreadsStat[i] = true;
                        }

                        doneCount++;
                    }
                }
            }
        }

        private void AddTeamPlayers(PlayerEntity player, List<IGameEntity> entitys, Contexts contexts)
        {
            var preEntity = contexts.player.GetEntitiesWithPlayerInfo(player.playerInfo.TeamId);
            foreach (var playerEntity in preEntity)
            {
                if(playerEntity.hasStage && playerEntity.stage.Value == EPlayerLoginStage.Running)
                    entitys.Add(playerEntity.entityAdapter.SelfAdapter);
            }
        }

        private void AddGlobalFreeMove(PlayerEntity player, List<IGameEntity> entitys, FreeMoveEntity[] freeMoveEntitys)
        {
            foreach (var freeMoveEntity in freeMoveEntitys)
            {
                entitys.Add(freeMoveEntity.entityAdapter.SelfAdapter);
            }
        }

        private void AddWeapon(PlayerEntity player, List<IGameEntity> entities, WeaponEntity[] weaponEntities)
        {
            foreach (var weaponEntity in weaponEntities)
            {
                if (weaponEntity.isFlagSyncSelf && weaponEntity.hasOwnerId && weaponEntity.ownerId.Value.Equals(player.entityKey.Value))
                {
                    entities.Add(weaponEntity.entityAdapter.SelfAdapter);
                }
                else if (weaponEntity.isFlagSyncNonSelf)
                {
                    entities.Add(weaponEntity.entityAdapter.SelfAdapter);
                }
            }
        }


        

        private static int CreateSendSnapshot(CreateSnapshotParams createSnapshotParams)
        {
            ISnapshot snapshot = null;

            try
            {
                Interlocked.Increment(ref createSnapshotParams.Status);
                if (createSnapshotParams.Status != 1)
                {
                    _logger.ErrorFormat(createSnapshotParams.GetHashCode().ToString() + " .Status == " + createSnapshotParams.Status.ToString());
                }

                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.SendSnapshotCreate);

                createSnapshotParams.Player.keepWatchForAOI.watchMap.Update(createSnapshotParams.PreEnitys, createSnapshotParams._contexts);

                snapshot = createSnapshotParams.SnapshotFactory.GeneratePerPlayerSnapshot(
                    createSnapshotParams.SnapshotSeq,
                    createSnapshotParams.Player.entityKey.Value,
                    createSnapshotParams.Player.gamePlay.IsObserving()
                        ? createSnapshotParams.Player.observeCamera.ObservedPlayerPosition
                        : createSnapshotParams.Player.position.Value,
                    createSnapshotParams.Bin2DConfig,
                    createSnapshotParams.Bin,
                    createSnapshotParams.PreEnitys,
                    createSnapshotParams.Player.stage.IsAccountStage(),
                    createSnapshotParams.Player.stage.IsWaitStage()
                    , createSnapshotParams.Player.keepWatchForAOI.watchMap.OnInsertFun
                    );

                snapshot.ServerTime = createSnapshotParams.ServerTime;
                snapshot.SnapshotSeq = createSnapshotParams.SnapshotSeq;
                snapshot.VehicleSimulationTime = createSnapshotParams.VehicleSimulationTime;
                snapshot.LastUserCmdSeq =
                    createSnapshotParams.Player.updateMessagePool.UpdateMessagePool.LatestMessageSeq;
                snapshot.Self = createSnapshotParams.Player.entityKey.Value;

                createSnapshotParams.Snapshot = snapshot;

                createSnapshotParams.Channel.SendRealTime((int)EServer2ClientMessage.Snapshot, snapshot);
                _logger.DebugFormat("{4}  send snapshot seq {0}, entity count {1}, self {2} {3}",
                    snapshot.SnapshotSeq,
                    snapshot.EntityList.Count,
                    snapshot.Self, 0, createSnapshotParams.Player.entityKey.Value);

            }
            catch (Exception e)
            {
                _logger.ErrorFormat("CreateSendSnapshot to {0}  is Exception{1}", createSnapshotParams.Player.entityKey,
                    e);
            }
            finally
            {
                Interlocked.Decrement(ref createSnapshotParams.Status);
                if (snapshot != null)
                {
                    RefCounterRecycler.Instance.ReleaseReference(snapshot);

                }
                RefCounterRecycler.Instance.ReleaseReference(createSnapshotParams);
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.SendSnapshotCreate);

            }

            return 0;

        }

        public void Dispose()
        {
            if (_createSnapshopThreads != null)
            {
                foreach (var thread in _createSnapshopThreads)
                {
                    thread.Dispose();
                }
            }
        }
    }
}