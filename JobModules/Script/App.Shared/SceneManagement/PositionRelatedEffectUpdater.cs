using Shared.Scripts.RuntimeScripts;
using UnityEngine;

namespace App.Shared.SceneManagement
{
    class PositionRelatedEffectUpdater : IUpdatePositionRelatedEffect
    {
        private readonly PositionRelatedEffect _effectRoot;

        public PositionRelatedEffectUpdater(PositionRelatedEffect effectRoot)
        {
            _effectRoot = effectRoot;
        }

        public void UpdatePlayerPosition(Vector3 pos)
        {
            _effectRoot.transform.position = pos;
        }
    }
}