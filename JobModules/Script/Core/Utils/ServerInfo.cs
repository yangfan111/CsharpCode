using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Singleton;

namespace Core.Utils
{
    public class ServerInfo : Singleton<ServerInfo>
    {
        public int ServerId;
    }
}
