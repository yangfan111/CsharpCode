using Utils.AssetManager;
using Core.Components;
using Core.Playback;
using Core.Prediction;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Shared.Scripts.Vehicles;
using VehicleCommon;

namespace App.Shared.Components.Vehicle
{
    
    [Vehicle]
    
    public class GameObjectComponent : SingleAssetComponent,INetworkObject
    {
        public override int GetComponentId() { { return (int)EComponentIds.VehicleGameObject; } }
        public VehicleCommonController Controller;
        public void CopyFrom(object rightComponent)
        {
            
        }

        public override void Recycle(IGameObjectPool objectPool)
        {
            if (Controller != null)
            {
                Controller.ResetGameObject(typeof(VehicleMaterialLoader), false);
            }
            
            base.Recycle(objectPool);
        }
    }

}
