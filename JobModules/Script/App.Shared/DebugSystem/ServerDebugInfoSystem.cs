using System;
using System.Collections.Generic;
using App.Shared.DebugSystem;
using Core.GameModule.Interface;

namespace App.Shared.DebugSystem
{
    public class ServerDebugInfoSystem : AbstractDebugInfoSystem<ServerDebugInfoSystem, ServerDebugInfo>
    {
        private IServerDebugInfoAccessor _accessor;
        public ServerDebugInfoSystem(IServerDebugInfoAccessor accessor)
        {
            _accessor = accessor;
        }

        public void Update()
        {
            OnGamePlay();
        }

        protected override ServerDebugInfo GetDebugInfo(object param)
        {
            var debugInfo = _accessor.GetDebugInfo();
            return debugInfo;
        }
    }
}
