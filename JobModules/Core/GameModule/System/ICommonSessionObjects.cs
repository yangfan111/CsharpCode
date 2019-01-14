using App.Shared.Components;
using Utils.AssetManager;
using Core.EntitasAdpater;
using Core.GameInputFilter;
using Core.IFactory;
using Core.WeaponLogic;
using Core.Common;
using Core.GameModeLogic;
using Core.Room;
using Core.Configuration;
using Core.Free;
using Core.BulletSimulation;

namespace Core.GameModule.System
{
    public interface IEntityFactoryObject
    {
        ISceneObjectEntityFactory SceneObjectEntityFactory { get; set; }
        ISoundEntityFactory SoundEntityFactory{ get; set; }
        IBulletEntityFactory BulletEntityFactory{ get; set; }
    }

    public interface ICommonSessionObjects
    {
      
        IGameObjectPool GameObjectPool { get; set; }
        ILoadRequestManager LoadRequestManager { get; set; }
        ICoRoutineManager CoRoutineManager { get; set; }
        IAssetPool AssetPool { get; set; }
        IGameContexts GameContexts { get; set; }
       
        IWeaponModeLogic WeaponModeLogic { get; set; }
        IGameStateProcessorFactory GameStateProcessorFactory{ get; set; }
        RoomInfo RoomInfo { get; set; }
        RuntimeGameConfig RuntimeGameConfig { get; set; }
        IEntityIdGenerator EntityIdGenerator{ get; set; }
        IEntityIdGenerator EquipmentEntityIdGenerator { get; set; }
        IFreeArgs FreeArgs{ get; set; }
        IBulletInfoCollector BulletInfoCollector { get; set; }
    }
}