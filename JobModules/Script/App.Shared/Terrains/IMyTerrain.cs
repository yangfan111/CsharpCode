using Core.Enums;
using Shared.Scripts.Terrains;
using UnityEngine;
using VehicleCommon;

namespace App.Shared.Terrains
{
    public interface IMyTerrain : IVehicleGroundMaterial
    {
        int _mapId { get; set; }
        //Terrain init position
        Vector3 InitPosition { get; }
        //Terrain width/height
        float TerrainSize { get; }

        //Friction
        STFriction GetFriction(Vector3 worldPos);
        float GetGripFriction(Vector3 worldPos);
        float GetDragFriction(Vector3 worldPos);

        int GetTerrainPositionMatType(Vector3 worldPos);
        //Vehicle Friction
        STFriction GetVehicleFriction(Vector3 worldPos, int vehicleId);

        //TextureId
        int GetId(Vector3 worldPos);
        
        //SoundId
        int GetSoundId(Vector3 worldPos, ETerrainSoundType soundType);
        //Sound
        AudioClip GetSound(Vector3 worldPos, ETerrainSoundType soundType);
        
        //EffectId
        int GetEffectId(Vector3 worldPos, ETerrainEffectType effectType);
        //EffectItem
        ITerrainEffectItem GetEffect(Vector3 worldPos, ETerrainEffectType effectType);
        //Release Effect
        void ReleaseEffect(ITerrainEffectItem item);

        //MaterialId
        int GetMaterialId(Vector3 worldPos, ETerrainMaterialType materialType);
        //Material
        Material GetMaterial(Vector3 worldPos, ETerrainMaterialType materialType);
    }
}
