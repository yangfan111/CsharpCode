using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.Utils;
using XmlConfig.BootConfig;

namespace App.Client.ArtModule
{
    public static class ArtModule
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ArtModule));

        public static bool IsArtMode;

        public static void Initialize(ClientBootConfig bootConfig)
        {
            try
            {
                IsArtMode = bootConfig.ArtMode;
                if (IsArtMode)
                {
                    var artModule = Type.GetType("App.Art.Module");
                    if (artModule != null)
                    {
                        var initMethodInfo = artModule.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);
                        initMethodInfo.Invoke(null, null);
                    }
                    else
                    {
                        _logger.InfoFormat("Can not load art module.");
                    }
                    
                }
                
                _logger.InfoFormat("Client In Art Mode ? {0}", IsArtMode);
            }
            catch (Exception e)
            {
                _logger.Error("can not load art module", e);
            }
            
        }

    }
}
