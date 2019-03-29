using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Free;

namespace App.Client.GameModules.GamePlay.Free.Utility
{
    public class FreeResourceUtil
    {
        public static FreeUrl Convert(string url)
        {
            int last = url.LastIndexOf("/");
            return new FreeUrl(url.Substring(0, last),
                url.Substring(last + 1));
        }
    }

    public struct FreeUrl
    {
        public string BuddleName;
        public string AssetName;

        public FreeUrl(string b, string a)
        {
            BuddleName = b;
            AssetName = a;
        }
    }
}
