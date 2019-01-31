using Utils.AssetManager;
using Core.Components;
using Core.EntityComponent;
using Entitas.CodeGeneration.Attributes;
using Entitas.VisualDebugging.Unity;
using UnityEngine;

namespace App.Shared.Components.Common
{
   
    [Player]
    [Bullet]
    [ClientEffect]
    [Vehicle]
    [UseBaseComponentType]
    [SceneObject]
    [Sound]
    [FreeMove]
    [Throwing]
    [MapObject]
    [Weapon]
    public class FakeEntityAdapterComponent : EntityAdapterComponent,FakeComponent
    {
        
    }

    [ClientEffect]
    [UseBaseComponentType]
    public class FakeNormalComponent : NormalComponent,FakeComponent
    {

    }

    [Player]
    [UseBaseComponentType]
    [Unique]
    public class FakeFlagSelfComponent : FlagSelfComponent,FakeComponent
    {

    }

    [Bullet]
    [ClientEffect]
    [Vehicle]
    [UseBaseComponentType]
    [Sound]
    [SceneObject]
    [FreeMove]
    [Throwing]
    [MapObject]
    [Weapon]
    public class FakeOwnerIdComponent : OwnerIdComponent,FakeComponent
    {
        
    }


    [Player]
    [Bullet]
	[Vehicle]
    [ClientEffect]
    [UseBaseComponentType]
    [SceneObject]
    [FreeMove]
    [Throwing]
    [MapObject]
    public class FakeFlagSyncNonSelfComponent : FlagSyncNonSelfComponent,FakeComponent
    {

    }

    

    [Player]
    [Vehicle]
    [UseBaseComponentType]
    [SceneObject]
    [MapObject]
    [Weapon]
    public class FakeFlagSyncSelfComponent : FlagSyncSelfComponent,FakeComponent
    {

    }
    [Player]
    [Bullet]
    [ClientEffect]
	[Vehicle]
    [UseBaseComponentType]
    [SceneObject]
    [Sound]
    [FreeMove]
    [Throwing]
    [MapObject]
    [Weapon]
    public class FakeEntityIdComponent : EntityKeyComponent,FakeComponent
    {

    }


    [Player]
    [Bullet]
    [ClientEffect]
	[Vehicle]
    [UseBaseComponentType]
    [SceneObject]
    [Sound]
    [FreeMove]
    [Throwing]
    [MapObject]
    [Weapon]
    public class FakeFlagDestroyComponent : FlagDestroyComponent,FakeComponent
    {

    }

    [Player]
    [Vehicle]
    [UseBaseComponentType]
    public class FakeFlagCompensationComponent : FlagCompensationComponent,FakeComponent
    {

    }


    [ClientEffect]
    [Bullet]
    [UseBaseComponentType]
    [FreeMove]
    [Throwing]
    [Sound]
    [SceneObject]
    [MapObject]
    public class FakeLifeTimeComponent : LifeTimeComponent,FakeComponent{}

    [Player]
    [ClientEffect]
    [Bullet]
    [Vehicle]
    [UseBaseComponentType]
    [SceneObject]
    [Sound]
    [FreeMove]
    [Throwing]
    [MapObject]
    public class FakePositionComponent : PositionComponent,FakeComponent { }
    
    [Player]
    [ClientEffect]
    [Bullet]
    [Vehicle]
    [UseBaseComponentType]
    [SceneObject]
    [Sound]
    [FreeMove]
    [Throwing]
    [MapObject]
    public class FakeGlobalFlagComponent : GlobalFlagComponent,FakeComponent { } 
    
    [Player]
    [ClientEffect]
    [Bullet]
    [Vehicle]
    [UseBaseComponentType]
    [SceneObject]
    [Sound]
    [Throwing]
    [FreeMove]
    [MapObject]
    public class FakePositionFilterComponent : PositionFilterComponent,FakeComponent { }
    [Player]
    [ClientEffect]
    [Bullet]
    [Vehicle]
    [UseBaseComponentType]
    [SceneObject]
    [Sound]
    [Throwing]
    [FreeMove]
    [MapObject]
    public class FakeFlagPlayBackFilterComponent : FlagPlayBackFilterComponent, FakeComponent
    {
    }
    
   
    [Player]
    [ClientEffect]
    [Bullet]
    [Vehicle]
    [UseBaseComponentType]
    [SceneObject]
    [Sound]
    [Throwing]
    [FreeMove]
    [MapObject]
    public class FakeFlagImmutabilityComponent : FlagImmutabilityComponent, FakeComponent
    {
        
    }

    [Player]
    [Vehicle]
    [DontDrawComponent]
    [FreeMove]
    public class HitBoxComponent : IAssetComponent, IGameComponent
    {
        public BoundingSphere HitPreliminaryGeo;
        public GameObject HitBoxGameObject;
        public void Recycle(IUnityAssetManager assetManager)
        {
            UnityEngine.Object.Destroy(HitBoxGameObject);
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerHitbox;
        }

    }
}
