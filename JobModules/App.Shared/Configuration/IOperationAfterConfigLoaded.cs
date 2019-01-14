using System;
using System.Collections.Generic;
using Utils.AssetManager;

namespace App.Shared.Configuration
{
    public interface IOperationAfterConfigLoaded
    {
        void ServerOperation(object config, Action callBack);
        void ClientOperation(object config, Action callBack);
        IList<SceneRequest> LoadInitialScene();
    }
}
