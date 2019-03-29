using System.Collections.Generic;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="IGrenadeCacheHelper" />
    /// </summary>
    public interface IGrenadeCacheHelper 
    {
        bool AddCache(int data);

        int ShowCount(int data);

        int RemoveCache(int data);

        void ClearCache(bool includeCurrent = true);

        void ClearEntityCache();
        Dictionary<int, int> HeldGrenades { get; }
        void Rewind();

        List<int> GetOwnedIds();

        int LastGrenadeId { get; }

        int GetHoldGrenadeIndex();
    }
   
}
