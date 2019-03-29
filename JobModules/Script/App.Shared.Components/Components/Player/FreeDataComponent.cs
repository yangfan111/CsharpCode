using Entitas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Free;

namespace App.Shared.Components.Player
{
    [Player]
    public class FreeDataComponent : IComponent
    {
        public IFreeData FreeData;
    }
}
