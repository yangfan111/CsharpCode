using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.GameModule.Module;
using Core.SessionState;
using Utils.Configuration;

namespace App.Shared.GameModules.Configuration
{
    public class SubResourceConfigurationInitModule  : GameModule
    {

        public SubResourceConfigurationInitModule(ISessionState sessionState)
        {
            AddSystem(new SubResourceLoadSystem(sessionState, new CharacterSpeedSubResourceHandler()));
            AddSystem(new SubResourceLoadSystem(sessionState, new WeaponAvatarAnimSubResourceHandler()));
        }
    }
}
