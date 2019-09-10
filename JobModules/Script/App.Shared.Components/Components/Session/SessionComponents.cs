using System;
using App.Shared.Components.ClientSession;
using App.Shared.Components.ServerSession;
using Core;
using Core.Attack;

using Core.Configuration;
using Core.EntityComponent;
using Core.Free;
using Core.GameModule.System;
using Core.GameTime;
using Core.IFactory;
using Core.Network;
using Core.OC;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction;
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

using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using Utils.AssetManager;
using Utils.Replay;
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
        [DontInitilize]public IEntityIdGenerator EntityIdGenerator{ get; set; }
        [DontInitilize]public IEntityIdGenerator EquipmentEntityIdGenerator { get; set; }
        [DontInitilize]public IFreeArgs FreeArgs { get; set; }
   //     [DontInitilize]public IBulletInfoCollector BulletInfoCollector { get; set; }
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
        LoadOptionConfig,
        LoadSubResourceConfig,
        RequestSceneInfo,
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
        [DontInitilize] public IPlayerInfo PlayerInfo;
        [DontInitilize] public string LoginToken;
        
        
        [DontInitilize] public ClientUpdateLatestManager ClientUpdateLatestMgr;
        [DontInitilize] public ISnapshotSelector SnapshotSelctor;
        [DontInitilize] public ITimeManager TimeManager;
        
        [DontInitilize] public IUserCmdGenerator UserCmdGenerator;
        [DontInitilize] public PlaybackInfoProvider PlaybackInfoProvider;
        [DontInitilize] public PlaybackManager PlaybackManager;
        
        [DontInitilize] public PredictionManager UserPredictionManager;
    //    [DontInitilize] public PredictionManager VehiclePredictionManager;
        [DontInitilize] public IVehicleCmdExecuteSystemHandler VehicleCmdExecuteSystemHandler;
        [DontInitilize] public AbstractPredictionProvider UserPredictionProvider;
      //  [DontInitilize] public AbstractPredictionProvider VehiclePredictionProvider;
       
       
     //   [DontInitilize] public SyncLatestProvider SyncLatestProvider;
        [DontInitilize] public SyncLastestManager NetSyncManager;
        [DontInitilize] public IClientSimulationTimer SimulationTimer;
        [DontInitilize] public VehicleTimer VehicleTimer;

       
        [DontInitilize] public ISoundPlayer SoundPlayer;
        [DontInitilize] public int GameRule;
        [DontInitilize] public IGlobalEffectManager GlobalEffectManager;
      

        //服务器状态
        [DontInitilize] public FpsSatatus FpsSatatus;
        [DontInitilize] public ServerStatus ServerFpsSatatus;

        [DontInitilize] public IOcclusionCullingController OCController;
        [DontInitilize] public ITerrainRenderer TerrainRenderer;
        [DontInitilize] public IRecordManager Record;
        [DontInitilize] public IReplayManager Replay;
       
        public void Dispose()
        {
            if(NetworkChannel!=null)
                NetworkChannel.Dispose();
            if(SnapshotSelctor!=null)
                SnapshotSelctor.Dispose();
            if(OCController != null)
                OCController.Dispose();
            if(Record!=null)
                Record.Dispose();  
            if(Replay!=null)
                Replay.Dispose(); 
        }
    }
     [Session]
    [Unique]
    [Serializable]
    public class ServerSessionObjectsComponent : IComponent,IDisposable
    {
        [DontInitilize] public ISnapshotSelector SnapshotSelector;

        [DontInitilize] public IRoomEventDispatchter RoomEventDispatchter;

        [DontInitilize] public Bin2DConfig Bin2DConfig;
        [DontInitilize] public IBin2DManager Bin2dManager;
     

        [DontInitilize] public ISimulationTimer SimulationTimer;
        [DontInitilize] public VehicleTimer VehicleTimer;

        [DontInitilize] public int GameRule;


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
            if(SnapshotSelector!=null)
                SnapshotSelector.Dispose();
            if(Bin2dManager!=null)
                Bin2dManager.Dispose();
        }
    }
}