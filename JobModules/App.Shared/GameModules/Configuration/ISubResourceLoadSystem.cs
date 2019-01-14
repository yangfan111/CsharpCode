using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using Utils.AssetManager;

namespace App.Shared.GameModules.Configuration
{
    public interface ISubResourceLoadSystem 
    {
        bool IsDone { get; }
        void OnLoadSucc(List<Tuple<AssetInfo, UnityEngine.Object>> subResources);
    }
}
