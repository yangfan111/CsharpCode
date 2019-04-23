using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

namespace App.Shared.Util
{
    public class CmdParser
    {
        private static  LoggerAdapter Logger =new LoggerAdapter(typeof(CmdParser));
        public  static Dictionary<string, string> ParseCommandLine(string[] args, string delimiter = "--")
        {
           
                Logger.InfoFormat("GameBootConfig: {0}", string.Join(" ",args));
            var arguments = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (args[i].StartsWith(delimiter))
                {
                    var key = arg.Remove(0, delimiter.Length);
                    var value = "";
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        value = args[i + 1];
                    }
                    Logger.InfoFormat("Commands: {0} {1}", key , value);
                    arguments[key] = value;
                }
            }
          
            return arguments;
        }

    }
}