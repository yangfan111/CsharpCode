using App.Shared.Components;
using Utils.AssetManager;
using Core.EntitasAdpater;
using Core.GameInputFilter;
using Core.IFactory;
using Core.WeaponLogic;
using Core.Common;
using Core;
using Core.Room;
using Core.Configuration;
using Core.Free;
using Core.BulletSimulation;
using Core.SceneManagement;
using UnityEngine;
using App.Shared;

namespace Core.GameModule.System
{
    public interface IEntityFactoryObject
    {
        ISceneObjectEntityFactory SceneObjectEntityFactory { get; set; }
        ISoundEntityFactory SoundEntityFactory{ get; set; }
    }

    public interface ICommonSessionObjects
    {
        IUnityAssetManager AssetManager { get; set; }
        ICoRoutineManager CoRoutineManager { get; set; }
        IGameContexts GameContexts { get; set; }
       
       // IWeaponMode WeaponModeLogic { get; set; }
        IGameStateProcessorFactory GameStateProcessorFactory{ get; set; }
        RoomInfo RoomInfo { get; set; }
        RuntimeGameConfig RuntimeGameConfig { get; set; }
        IEntityIdGenerator EntityIdGenerator{ get; set; }
        IEntityIdGenerator EquipmentEntityIdGenerator { get; set; }
        IFreeArgs FreeArgs{ get; set; }
        IBulletInfoCollector BulletInfoCollector { get; set; }
        ILevelManager LevelManager { get; set; }
        Vector3 InitPosition { get; set; }
    }
}