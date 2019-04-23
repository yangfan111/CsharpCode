using System.Collections.Generic;
using UnityEngine;
using Utils.AssetManager;

namespace Utils.Appearance
{
    public interface ICharacterLoadResource
    {
        List<AbstractLoadRequest> GetLoadRequests();
        List<UnityObject> GetRecycleRequests();
        void ClearRequests();
    }
}
