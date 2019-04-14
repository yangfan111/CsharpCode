using System.Collections.Generic;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="IGrenadeCacheHandler" />
    /// </summary>
    public interface IGrenadeCacheHandler 
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
