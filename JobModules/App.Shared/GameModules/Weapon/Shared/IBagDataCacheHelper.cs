using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace App.Shared.GameModules.Weapon
{
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