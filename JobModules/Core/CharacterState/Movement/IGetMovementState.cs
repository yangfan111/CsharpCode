using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlConfig;

namespace Core.CharacterState.Movement
{
    public interface IGetMovementState
    {
        MovementInConfig GetCurrentMovementState();
        MovementInConfig GetNextMovementState();
    } 
}
