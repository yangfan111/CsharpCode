using Core.GameModule.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.GameModules.GamePlay
{
    public class PubgRuleModule : GameModule
    {
        public PubgRuleModule(Contexts contexts)
        {
            AddSystem(new PlayerOnPlaneSystem(contexts));
        }
    }
}
