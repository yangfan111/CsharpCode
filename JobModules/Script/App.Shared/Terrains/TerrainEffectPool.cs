using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.Terrains
{
    public class TerrainEffectPool
    {
        //<EffectId, GameObject>
        private Dictionary<int, GameObject> _dictEffects = new Dictionary<int, GameObject>();
        //<EffectId, [ITerrainEffectItem]>
        private Dictionary<int, List<ITerrainEffectItem>> _dictEffectPool = new Dictionary<int, List<ITerrainEffectItem>>();

        public void AddEffectPrefab(int effectId, GameObject go)
        {
            if (!_dictEffects.ContainsKey(effectId))
            {
                _dictEffects.Add(effectId, go);
            }
        }

        public ITerrainEffectItem GetNewEffect(int effectId)
        {
            GameObject prefab;
            _dictEffects.TryGetValue(effectId, out prefab);
            if (null != prefab)
            {
                List<ITerrainEffectItem> list;
                _dictEffectPool.TryGetValue(effectId, out list);
                if (null == list)
                {
                    list = new List<ITerrainEffectItem>();
                    _dictEffectPool.Add(effectId, list);
                }
                if (list.Count == 0)
                {
                    TerrainEffectItem item = new TerrainEffectItem();
                    item.EffectId = effectId;
                    item.EffectGo = Object.Instantiate(prefab);
                    return item;
                }
                else
                {
                    ITerrainEffectItem item = list[0];
                    list.RemoveAt(0);
                    return item;
                }
            }
            return null;
        }

        public void ReleaseEffect(ITerrainEffectItem item)
        {
            List<ITerrainEffectItem> list;
            _dictEffectPool.TryGetValue(item.EffectId, out list);
            if (null == list)
            {
                list = new List<ITerrainEffectItem>();
                _dictEffectPool.Add(item.EffectId, list);
            }
            list.Add(item);
        }
    }
}
