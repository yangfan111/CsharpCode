using System.Collections.Generic;
using App.Client.GameModules.GamePlay.Free;
using App.Server.GameModules.GamePlay;
using App.Server.Scripts.Config;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ServerSession;
using App.Shared.ContextInfos;
using App.Shared.EntityFactory;
using App.Shared.FreeFramework.Free.Weapon;
using App.Shared.GameModules.Weapon.Behavior;
using com.wd.free.@event;
using com.wd.free.trigger;
using Core;
using Core.Configuration.Sound;
using Core.EntityComponent;
using Core.Free;
using Core.GameTime;
using Core.MyProfiler;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.Replicaton;
using Core.Room;
using Core.SpatialPartition;
using UnityEngine;
using Utils.AssetManager;
using Utils.Singleton;
using VehicleCommon;

public class ContextsServerWrapper
{
    public Contexts contexts { get; private set; }

    public void Dispose()
    {
        Bin2DManager.Dispose();
        contexts.session.commonSession.Dispose();
        contexts.session.serverSessionObjects.Dispose();
        contexts.Reset();
        contexts = null;
    }
    public IBin2DManager Bin2DManager
    {
        get { return contexts.session.serverSessionObjects.Bin2dManager; }
    }

    public IUnityAssetManager AssetManager
    {
        get { return contexts.session.commonSession.AssetManager; }
    }

    public ICoRoutineManager CoRoutineManager
    {
        get { return contexts.session.commonSession.CoRoutineManager; }
    }
    public ICurrentTime CurrentTime
    {
        get { return contexts.session.currentTimeObject; }
    }

    public ISimulationTimer SimulationTimer
    {
        get { return contexts.session.serverSessionObjects.SimulationTimer; }
    }

    public int GetNextSeq()
    {
        return contexts.session.serverSessionObjects.GetNextSnapshotSeq();
    }
    public ISnapshotSelector SnapshotPool
    {
        get { return contexts.session.serverSessionObjects.SnapshotSelector; }
    }
    public IGameContexts GameContexts
    {
        get { return contexts.session.commonSession.GameContexts; }
    }

    public FreeRuleEventArgs FreeArgs
    {
        get { return contexts.session.commonSession.FreeArgs as FreeRuleEventArgs; }
        set { contexts.session.commonSession.FreeArgs = value; }
    }
    public RoomInfo RoomInfo
    {
        get { return contexts.session.commonSession.RoomInfo; }
    }
    public ISessionMode SessionMode
    {
        get { return contexts.session.commonSession.SessionMode; }
        set { contexts.session.commonSession.SessionMode = value; }
    }

   
    private IBin2DManager CreateBin2DManager()
    {
        return GameContextsUtility.GetReplicationBin2DManager(-9000, -9000, 9000, 9000, 16000, new Dictionary<int, int>
        {
                        {(int) EEntityType.SceneObject, 32},
                        {(int) EEntityType.MapObject, 256},
                        {(int) EEntityType.Bullet, 1000},
                        {(int) EEntityType.Sound, 32},
                        {(int) EEntityType.Player, 1000},
                        {(int) EEntityType.FreeMove, 10000},

                        {(int) EEntityType.Throwing, 1000},
                        {(int) EEntityType.Vehicle, 1000},
                        {(int) EEntityType.ClientEffect, 1000}
        });
    }

    public ContextsServerWrapper(Contexts contexts, IUnityAssetManager assetManager, ICoRoutineManager coRoutineManager)
    {
        var ruleId = RuleMap.GetRuleId(SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Rule);
        this.contexts = contexts;
#if (!ENTITASDISABLEVISUALDEBUGGING && UNITYEDITOR)
            this.contexts.InitializeContexObservers();
#endif

        SingletonManager.Get<MyProfilerManager>().Contexts = contexts;
        IBin2DManager bin2DManager = CreateBin2DManager();
        IniCurrentTimeSession();

        var entityIdGenerator          = new EntityIdGenerator(EntityIdGenerator.GlobalBaseId);
        var equipmentEntityIdGenerator = new EntityIdGenerator(EntityIdGenerator.EquipmentBaseId);
        InitEntityFactorySession(entityIdGenerator, equipmentEntityIdGenerator);


        InitCommonSession(bin2DManager, entityIdGenerator, equipmentEntityIdGenerator, ruleId, assetManager,
            coRoutineManager);

        InitServerSession(bin2DManager, ruleId);

        contexts.vehicle.SetSimulationTime(0);
    

   
        InitialWeaponSkill();
    }
     
    private void InitialWeaponSkill()
    {
        FreeRuleConfig config = FreeRuleConfig.GetRule("weaponSkill",
            SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Mysql);
        foreach (GameTrigger trigger in config.Triggers.GetTriggers())
        {
            WeaponSkillFactory.RegisterSkill(FreeArgs as IEventArgs, trigger);
        }
    }
    private void InitServerSession(IBin2DManager bin2DManager, int ruleId)
    {
        contexts.session.SetServerSessionObjects();
        var serverSessionObjects = contexts.session.serverSessionObjects;
        serverSessionObjects.SnapshotSelector = new SnapshotPool();
        serverSessionObjects.Bin2DConfig      = new Bin2DConfig(-9000, -9000, 9000, 9000, 100, 16000);

        serverSessionObjects.Bin2dManager = bin2DManager;

        serverSessionObjects.SimulationTimer             = new ServerSimulationTimer();
        serverSessionObjects.SimulationTimer.CurrentTime = 0;
        serverSessionObjects.VehicleTimer                = new VehicleTimer();

        serverSessionObjects.GameRule   = ruleId;
        serverSessionObjects.FpsSatatus = new FpsSatatus();
    }


    private void InitCommonSession(IBin2DManager bin2DManager, EntityIdGenerator entityIdGenerator,
                                   EntityIdGenerator equipmentEntityIdGenerator, int ruleId,
                                   IUnityAssetManager assetManager, ICoRoutineManager coRoutineManager)
    {
        contexts.session.SetCommonSession();
        var commonSession = contexts.session.commonSession;
        commonSession.InitPosition = Vector3.zero;

        commonSession.AssetManager = assetManager;

        commonSession.CoRoutineManager = coRoutineManager;
        commonSession.GameContexts =
                        GameContextsUtility.GetReplicationGameContexts(contexts, bin2DManager);
        commonSession.EntityIdGenerator          = entityIdGenerator;
        commonSession.EquipmentEntityIdGenerator = equipmentEntityIdGenerator;
        commonSession.RoomInfo = new RoomInfo
        {
                        MapId  = SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.MapId,
                        ModeId = ruleId
        };

        MakeWeaponLogicManager();
    }

    private void MakeWeaponLogicManager()
    {
        var commonSession = contexts.session.commonSession;
        //commonSession.PlayerWeaponResourceConfigManager = new PlayerWeaponResourceConfigManager(SingletonManager.Get<WeaponPartsConfigManager>(),
        //    SingletonManager.Get<WeaponDataConfigManager>());

        //            var weaponComponentsFacoty = new WeaponFireScriptsInitializer(contexts,
        //               contexts.session.commonSession.EntityIdGenerator);
        var fireScriptsProvider = new WeaponFireScriptsProvider(contexts);
        commonSession.WeaponFireUpdateManager =
                        new WeaponFireUpdateManagaer(fireScriptsProvider, contexts.session.commonSession.FreeArgs);
    }

    private void IniCurrentTimeSession()
    {
        if (!contexts.session.hasCurrentTimeObject)
        {
            contexts.session.SetCurrentTimeObject();
        }
        else
        {
            Debug.LogError("CurrentTimeObject already exist");
        }

        var currentTimeObject = contexts.session.currentTimeObject;
        currentTimeObject.CurrentTime = 0;
    }

    private void InitEntityFactorySession(EntityIdGenerator entityIdGenerator,
                                          EntityIdGenerator equipmentEntityIdGenerator)
    {
        contexts.session.SetEntityFactoryObject();
        var entityFactoryObject = contexts.session.entityFactoryObject;
        // entityFactoryObject.SoundEntityFactory = new ServerSoundEntityFactory(contexts.sound, contexts.player,
        //     entityIdGenerator, contexts.session.currentTimeObject, SingletonManager.Get<SoundConfigManager>());
        entityFactoryObject.SceneObjectEntityFactory = new ServerSceneObjectEntityFactory(contexts.sceneObject,
            contexts.player, entityIdGenerator, equipmentEntityIdGenerator, contexts.session.currentTimeObject);
        entityFactoryObject.MapObjectEntityFactory =
                        new ServerMapObjectEntityFactory(contexts.mapObject, entityIdGenerator);
    }
}