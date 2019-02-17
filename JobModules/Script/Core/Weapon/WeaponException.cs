using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Core
{
    public class GameRunTimeException:Exception
    {
        public GameRunTimeException(string message,string prefix = "")
           : base("Game run time exception**\n" + prefix+ message)
        {
        }
    }
    public class ExWeaponNotFoundException : GameRunTimeException
    {
        public ExWeaponNotFoundException(string message,params object[]args):base(string.Format(message,args))
        {
            
        }
    }
}
 
