using System.Collections.Generic;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="IGrenadeCacheHelper" />
    /// </summary>
    public interface IGrenadeCacheHelper : ISlotHelper
    {
        bool AddCache(int data);

        int ShowCount(int data);

        bool RemoveCache(int data);

        void ClearCache();

        void Rewind();

        List<int> GetOwnedIds();
    }
    public interface ISlotHelper
    {

    }
}
