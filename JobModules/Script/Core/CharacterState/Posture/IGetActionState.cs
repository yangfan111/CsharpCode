using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlConfig;

namespace Core.CharacterState.Posture
{
    public interface IGetActionState
    {
        /// <summary>
        /// 获取action状态 
        /// </summary>
        /// <returns></returns>
        ActionInConfig GetActionState();
        ActionInConfig GetNextActionState();
        ActionKeepInConfig GetActionKeepState();
        ActionKeepInConfig GetNextActionKeepState();
    }
}
