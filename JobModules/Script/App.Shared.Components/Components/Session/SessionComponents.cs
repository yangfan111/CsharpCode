using System;
using App.Shared.Components.ClientSession;
using App.Shared.Components.ServerSession;
using Core;
using Core.BulletSimulation;
using Core.Common;
using Core.Configuration;
using Core.EntitasAdpater;
using Core.Free;
using Core;
using Core;
using Core.GameModule.System;
using Core.GameTime;
using Core.IFactory;
using Core.Network;
using Core.OC;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction.Cmd;
using Core.Prediction.VehiclePrediction;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.Replicaton;
using Core.Room;
using Core.SceneManagement;
using Core.Sound;
using Core.SpatialPartition;
using Core.SyncLatest;
using Core.UpdateLatest;
using Core.Utils;
using Core.WeaponLogic;

using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using Utils.AssetManager;
using VehicleCommon;

namespace App.Shared.Components
{
    [Session]
    [Unique]
    [Serializable]
    public class EntityFactoryObject : IComponent, IEntityFactoryObject
    {
        [DontInitilize] public ISceneObjectEntityFactory SceneObjectEntityFactory { get; set; }
        [DontInitilize] public IMapObjectEntityFactory MapObjectEntityFactory { get; set; }
        [DontInitilize] public ISoundEntityFactory SoundEntityFactory { get; set; }
    }

    /// <summary>
    /// 程序运行的当前时间，单位ms，int类型
    /// </summary>
    [Session]
    [Unique]
    [Serializable]
    public class CurrentTimeObjectComponent : IComponent, Core.GameTime.ICurrentTime
    {
        [DontInitilize] public int CurrentTime { get; set; }
    }
    [Session]
    [Unique]
    [Serializable]
    public class CommonSessionComponent : ICommonSessionObjects, IComponent,IDisposable
    {
        
        [DontInitilize]public IUnityAssetManager AssetManager { get; set; }
        [DontInitilize]public ICoRoutineManager CoRoutineManager { get; set; }
        [DontInitilize]public IGameContexts GameContexts { get; set; }
      
      //[DontInitilize]public IWeaponMode WeaponModeLogic { get; set; }

        [DontInitilize] public ISessionMode SessionMode{ get; set; }
   //     [System.Obsolete]
       // [DontInitilize]public PlayerStateCollectorPool PlayerStateCollectorPool { get; set; }
        [DontInitilize]public RoomInfo RoomInfo { get; set; }
        [DontInitilize]public RuntimeGameConfig RuntimeGameConfig { get; set; }
        [DontInitilize]public IEntityIdGenerator EntityIdGenerator{ get; set; }
        [DontInitilize]public IEntityIdGenerator EquipmentEntityIdGenerator { get; set; }
        [DontInitilize]public IFreeArgs FreeArgs { get; set; }
        [DontInitilize]public IBulletInfoCollector BulletInfoCollector { get; set; }
        [DontInitilize]public ILevelManager LevelManager { get; set; }
        [DontInitilize]public Vector3 InitPosition { get; set; }
        [DontInitilize] public IWeaponFireUpdateManagaer WeaponFireUpdateManager { get; set; }
     //   [DontInitilize] public IPlayerWeaponResourceConfigManager PlayerWeaponResourceConfigManager { get; set; }
     public void Dispose()
     {
         if(LevelManager!=null)
             LevelManager.Dispose();
     }
    }
    
    public enum EClientSessionStates
    {
        LoadConfig,
        LoadSubResourceConfig,
        RequestRoomInfo,
        LoadSceneMapConfig,
        LoadOCConfig,
        PreloadResource,
        PreparingPlayer,
        InitUiModule,
        RequestSnapshot,
        Running,
        ProfilePreparation,
        Profile,
        End,
    }

    [Session]
    [Unique]
    [Serializable]
    public class ClientSessionObjectsComponent : IComponent,IDisposable
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientSessionObjectsComponent));
        [DontInitilize] public NetworkMessageDispatcher MessageDispatcher;
        [DontInitilize] public INetworkChannel NetworkChannel;
        [DontInitilize] public IPlayerInfo PlayerInfo { get; set; } 
        [DontInitilize] public string LoginToken;
        
        
        [DontInitilize] public IUpdateLatestHandler UpdateLatestHandler;
        [DontInitilize] public ISnapshotPool SnapshotPool;
        [DontInitilize] public ISnapshotSelectorContainer SnapshotSelectorContainer;
        [DontInitilize] public ITimeManager TimeManager;
        
        [DontInitilize] public IUserCmdGenerator UserCmdGenerator;
        [DontInitilize] public IPlaybackInfoProvider PlaybackInfoProvider;
        [DontInitilize] public IPlaybackManager PlaybackManager;
        
        [DontInitilize] public IPredictionInitManager UserPredictionInitManager;
        [DontInitilize] public IPredictionInitManager VehiclePredictionInitManager;
        [DontInitilize] public IVehicleCmdExecuteSystemHandler VehicleCmdExecuteSystemHandler;
        [DontInitilize] public IUserPredictionInfoProvider UserPredictionInfoProvider;
        [DontInitilize] public IPredictionRewindInfoProvider VehiclePredictionInfoProvider;
       
       
        [DontInitilize] public ISyncLatestHandler SyncLatestHandler;
        [DontInitilize] public ISyncLatestManager SyncLatestManager;
        [DontInitilize] public IClientSimulationTimer SimulationTimer;
        [DontInitilize] public VehicleTimer VehicleTimer;

       
        [DontInitilize] public ISoundPlayer SoundPlayer;
        [DontInitilize] public int GameRule { get; set; }
        [DontInitilize] public IGlobalEffectManager GlobalEffectManager;
      
        //服务器状态
        [DontInitilize] public FpsSatatus FpsSatatus { get; set; }
        [DontInitilize] public ServerStatus ServerFpsSatatus { get; set; }

        [DontInitilize] public IOcclusionCullingController OCController { get; set; }
        [DontInitilize] public ITerrainRenderer TerrainRenderer { get; set; }

        public void Dispose()
        {
            if(NetworkChannel!=null)
                NetworkChannel.Dispose();
            if(SnapshotPool!=null)
                SnapshotPool.Dispose();
            if(OCController != null)
                OCController.Dispose();
        }
    }
     [Session]
    [Unique]
    [Serializable]
    public class ServerSessionObjectsComponent : IComponent,IDisposable
    {
        [DontInitilize] public ISnapshotPool CompensationSnapshotPool;
        [DontInitilize] public ISnapshotSelectorContainer CompensationSnapshotSelector;

        [DontInitilize] public IRoomEventDispatchter RoomEventDispatchter;

        [DontInitilize] public Bin2DConfig Bin2DConfig;
        [DontInitilize] public IBin2DManager Bin2dManager;
     

        [DontInitilize] public ISimulationTimer SimulationTimer;
        [DontInitilize] public VehicleTimer VehicleTimer;

        [DontInitilize] public int GameRule;
        [DontInitilize] public IUpdateMessagePool UpdateMessagePool { get; set; }


        //服务器状态
        [DontInitilize] public FpsSatatus FpsSatatus;

        [DontInitilize] public int DeathOrder;

        private int _nextSnapshotSeq;

        public int GetNextSnapshotSeq()
        {
            return _nextSnapshotSeq++;
        }

        public void Dispose()
        {
            if(CompensationSnapshotPool!=null)
            CompensationSnapshotPool.Dispose();
            if(Bin2dManager!=null)
                Bin2dManager.Dispose();
            if(UpdateMessagePool!=null)
                UpdateMessagePool.Dispose();
        }
    }
}