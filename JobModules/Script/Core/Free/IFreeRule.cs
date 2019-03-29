using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Free
{
    public interface IFreeRule
    {
        long ServerTime { get; }
        string FreeType { get; }
        bool GameOver { get; set; }
        bool GameExit { get; set; }
    }
}
