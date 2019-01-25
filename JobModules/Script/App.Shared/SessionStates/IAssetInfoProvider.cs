using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.AssetManager;

namespace App.Shared.SessionStates
{
    public interface IAssetInfoProvider
    {
        List<AssetInfo> AssetInfos { get; }
    }
}
