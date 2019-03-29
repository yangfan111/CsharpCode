using UnityEngine;

namespace App.Shared.Terrains
{
    public interface ITerrainEffectItem
    {
        int EffectId { get; }
        GameObject EffectGo { get; }
    }

    public class TerrainEffectItem : ITerrainEffectItem
    {
        public int EffectId { get; set; }
        public GameObject EffectGo { get; set; }
    }
}
