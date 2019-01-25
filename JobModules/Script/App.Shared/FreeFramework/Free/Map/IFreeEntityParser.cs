using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.GameModules.GamePlay.Free.Map
{
    public interface IFreeEntityParser
    {
        object FromString(string value);
        string ToString(object obj);
    }
}
