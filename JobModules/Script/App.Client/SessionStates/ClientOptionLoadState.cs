using App.Shared.Components;
using App.Shared.GameModules.Configuration;
using Core.SessionState;
using Entitas;
using I2.Loc;

//将一部分可选配置加载分离如果，根据地图信息，如果发现不需要加载，则可以直接跳过，节省资源
//目前主要针对大地图的配置文件，其他配置文件如果有相关情况也可以放到这个State中

namespace App.Client.SessionStates
{
    public class ClientOptionLoadState : AbstractSessionState
    {
        public ClientOptionLoadState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(contexts, (int)state, (int)next)
        {
        }

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts)contexts;
            var systems = new Feature("ClientOptionLoadState");
            systems.Add(new OptionConfigurationInitModule(this, _contexts.session.commonSession.AssetManager));
            return systems;
        }

        public override string LoadingTip
        {
            get { return ScriptLocalization.client_loadtip.preload; }
        }
    }
}