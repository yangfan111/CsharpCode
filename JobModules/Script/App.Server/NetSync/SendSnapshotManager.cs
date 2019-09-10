#define ENABLE_NEW_SENDSNAPSHOT_THREAD
using System;
using System.Collections.Generic;
using System.Threading;
using App.Shared;
using App.Shared.Components.Player;
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

namespace App.Server
{
 

    /// <summary>
    /// SendSnapshot只send最后一帧的snapshot
    /// </summary>
    public class SendSnapshotManager : IDisposable
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SendSnapshotManager));
        private readonly Contexts _contexts;
        private readonly IGroup<FreeMoveEntity> _globalFreeMoveEntities;
        private readonly IGroup<WeaponEntity> _globalWeaponEntities;
        private readonly IGroup<PlayerEntity> _playerEntities;
        private readonly CustomProfileInfo SendSnapshotWait;


        private ConsumerThread<CreateSnapshotTask, int>[] _createSnapshopThreads;
        private bool[] _createSnapshopThreadsStat;
        private BlockingQueue<CreateSnapshotTask> _queue = new BlockingQueue<CreateSnapshotTask>(100);

        private List<CreateSnapshotTask> snapshotSendTasks = new List<CreateSnapshotTask>();

        public SendSnapshotManager(Contexts contexts)
        {
            SendSnapshotWait = SingletonManager.Get<DurationHelp>().GetProfileInfo(CustomProfilerStep.SendSnapshotWait);
            _contexts        = contexts;
            _globalFreeMoveEntities =
                            _contexts.freeMove.GetGroup(FreeMoveMatcher.AllOf(FreeMoveMatcher.GlobalFlag,
                                FreeMoveMatcher.EntityAdapter));
            _globalWeaponEntities =
                            _contexts.weapon.GetGroup(WeaponMatcher.AllOf(WeaponMatcher.EntityKey,
                                WeaponMatcher.EntityAdapter));
            _playerEntities = _contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.Network));
#if ENABLE_NEW_SENDSNAPSHOT_THREAD
            InitThreads();
#endif
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

        private void InitThreads()
        {
            if (SharedConfig.MutilThread)
            {
                _createSnapshopThreadsStat = new bool[SharedConfig.CreateSnapshotThreadCount];
                _createSnapshopThreads =
                                new ConsumerThread<CreateSnapshotTask, int>[SharedConfig.CreateSnapshotThreadCount];
                for (int i = 0; i < _createSnapshopThreads.Length; i++)
                {
                    _createSnapshopThreads[i] =
                                    new ConsumerThread<CreateSnapshotTask, int>(
                                        string.Format("CreateSnapshotThread_{0}", i), CreateSendSnapshot, _queue);
                    _createSnapshopThreads[i].Start();
                }
            }
        }

        private SnapshotFactory snapshotFactory;

        public void SendSnapshot(int interval, SnapshotFactory snapshotFactory)
        {
            _logger.InfoFormat("SendSnapShot");
            this.snapshotFactory = snapshotFactory;
            var           sessionObjects        = _contexts.session.serverSessionObjects;
            Bin2DConfig   bin2DConfig           = sessionObjects.Bin2DConfig;
            IBin2DManager bin                   = sessionObjects.Bin2dManager;
            int           snapshotSeq           = sessionObjects.GetNextSnapshotSeq();
            int           vehicleSimulationTime = sessionObjects.SimulationTimer.CurrentTime;
            int           serverTime            = _contexts.session.currentTimeObject.CurrentTime;
            snapshotSendTasks.Clear();
            var freeMoveEntitys = _globalFreeMoveEntities.GetEntities();
            var weaponEntities  = _globalWeaponEntities.GetEntities();

            foreach (var player in _playerEntities.GetEntities())
            {
                if (player.hasStage && player.stage.CanSendSnapshot() && player.network.NetworkChannel.IsConnected &&
                    !player.network.NetworkChannel.Serializer.MessageTypeInfo.SkipSendSnapShot(serverTime))
                {
                    var p = ObjectAllocatorHolder<CreateSnapshotTask>.Allocate().Build(player,
                        bin2DConfig, bin, serverTime, snapshotSeq, vehicleSimulationTime, player.network.NetworkChannel,
                        _contexts);

                    AddTeamPlayers(player, p.PreEntitas, _contexts);
                    AddGlobalFreeMove(player, p.PreEntitas, freeMoveEntitys);
                    AddWeapon(player, p.PreEntitas, weaponEntities);

                    snapshotSendTasks.Add(p);
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
                    _logger.DebugFormat("channel:{2} skip SendSnapshot :{0} {1}!",
                        player.network.NetworkChannel.IsConnected,
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
            foreach (var p in snapshotSendTasks)
            {
                CreateSendSnapshot(p);
            }
        }

        private void MutilExecute()
        {
            var spinWait = SpinWaitUtils.GetSpinWait();
            MutilExecute<int, CreateSnapshotTask> mutilExecute =
                            new MutilExecute<int, CreateSnapshotTask>(SharedConfig.CreateSnapshotThreadCount,
                                snapshotSendTasks, CreateSendSnapshot);
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
            bool isRunning                 = true;
            int  createSnapshoThreadsCount = _createSnapshopThreads.Length;

            for (int i = 0; i < createSnapshoThreadsCount; i++)
            {
                _createSnapshopThreadsStat[i] = false;
            }

            var  spinWait = SpinWaitUtils.GetSpinWait();
            long start    = DateTime.UtcNow.ToMillisecondsSinceEpoch();
            while (isRunning)
            {
                spinWait.SpinOnce();
                int doneCount = 0;
                isRunning = false;
                var now = DateTime.UtcNow.ToMillisecondsSinceEpoch();
                if (now - start > 10000)
                {
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
                if (playerEntity.hasStage && playerEntity.stage.Value == EPlayerLoginStage.Running)
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
                if (weaponEntity.isFlagSyncSelf && weaponEntity.hasOwnerId &&
                    weaponEntity.ownerId.Value.Equals(player.entityKey.Value))
                {
                    entities.Add(weaponEntity.entityAdapter.SelfAdapter);
                }
                else if (weaponEntity.isFlagSyncNonSelf)
                {
                    entities.Add(weaponEntity.entityAdapter.SelfAdapter);
                }
            }
        }
        

        private  int CreateSendSnapshot(CreateSnapshotTask createSnapshotTask)
        {
            ISnapshot snapshot = null;

            try
            {
                Interlocked.Increment(ref createSnapshotTask.Status);
                if (createSnapshotTask.Status != 1)
                {
                    _logger.ErrorFormat(createSnapshotTask.GetHashCode() + " .Status == " +
                        createSnapshotTask.Status);
                }

                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.SendSnapshotCreate);

                createSnapshotTask.Player.keepWatchForAOI.watchMap.Update(createSnapshotTask.PreEntitas,
                    createSnapshotTask._contexts);

                snapshot = snapshotFactory.GeneratePerPlayerSnapshot(
                    createSnapshotTask.SnapshotSeq, createSnapshotTask.Player.entityKey.Value,
                    createSnapshotTask.Player.gamePlay.IsObserving()
                                    ? createSnapshotTask.Player.observeCamera.ObservedPlayerPosition
                                    : createSnapshotTask.Player.position.Value, createSnapshotTask.Bin2DConfig,
                    createSnapshotTask.Bin, createSnapshotTask.PreEntitas,
                    createSnapshotTask.Player.stage.IsAccountStage(), createSnapshotTask.Player.stage.IsWaitStage(),
                    createSnapshotTask.Player.keepWatchForAOI.watchMap.InsertFun);

                snapshot.ServerTime            = createSnapshotTask.ServerTime;
                snapshot.SnapshotSeq           = createSnapshotTask.SnapshotSeq;
                snapshot.VehicleSimulationTime = createSnapshotTask.VehicleSimulationTime;
                snapshot.LastUserCmdSeq =
                                createSnapshotTask.Player.updateMessagePool.Value.LatestMessageSeq;
                snapshot.Self = createSnapshotTask.Player.entityKey.Value;

                createSnapshotTask.Snapshot = snapshot;

               createSnapshotTask .Channel.SendRealTime((int) EServer2ClientMessage.Snapshot, snapshot);
                _logger.DebugFormat("{4}  send snapshot seq {0}, entity count {1}, self {2} {3}", snapshot.SnapshotSeq,
                    snapshot.EntityList.Count, snapshot.Self, 0, createSnapshotTask.Player.entityKey.Value);
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("CreateSendSnapshot to {0}  is Exception{1}", createSnapshotTask.Player.entityKey,
                    e);
            }
            finally
            {
                Interlocked.Decrement(ref createSnapshotTask.Status);
                if (snapshot != null)
                {
                    RefCounterRecycler.Instance.ReleaseReference(snapshot);
                }

                RefCounterRecycler.Instance.ReleaseReference(createSnapshotTask);
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.SendSnapshotCreate);
            }

            return 0;
        }
    }
}