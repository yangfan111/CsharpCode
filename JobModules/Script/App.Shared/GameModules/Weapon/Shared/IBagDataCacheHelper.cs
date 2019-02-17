using System.Collections.Generic;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="IBagDataCacheHelper" />
    /// </summary>
    public interface IBagDataCacheHelper
    {
        bool AddCache(int data);

        int ShowCount(int data);

        bool RemoveCache(int data);

        void ClearCache();

        void Rewind();

        List<int> GetOwnedIds();
    }
}
