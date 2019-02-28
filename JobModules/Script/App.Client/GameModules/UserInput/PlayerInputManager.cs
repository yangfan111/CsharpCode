using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using Core.Utils;
using Luminosity.IO;

namespace App.Client.GameModules.UserInput
{
    public static class PlayerInputManager
    {

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerInputManager));
        private static Dictionary<int, string> IdToName = new Dictionary<int, string>()
        {
            {0, "default" },
            {1, "normal" },
            {2, "Dive" }
        };
        public static void UpdateAction(string schemeName, string action, Luminosity.IO.PlayerID id)
        {
            var exists = Luminosity.IO.InputManager.Exists;
            if (exists)
            {
                Luminosity.IO.InputManager.UpdateControlSchemeAction(schemeName, action, id);
            }
            else
            {
                if (!SharedConfig.IsServer)
                {
                    Logger.InfoFormat("Luminosity.IO.InputManager is not init!");
                }
            }
        }

        public static void UpdateAction(int schemeId, string action)
        {
            var exists = Luminosity.IO.InputManager.Exists;
            if (exists)
            {
                string name;
                IdToName.TryGetValue(schemeId, out name);
                Luminosity.IO.InputManager.UpdateControlSchemeAction(name, action, Luminosity.IO.PlayerID.One);
            }
            else
            {
                if (!SharedConfig.IsServer)
                {
                    Logger.InfoFormat("Luminosity.IO.InputManager is not init!");
                }
            }
        }
    }
}
