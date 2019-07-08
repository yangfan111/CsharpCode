using Entitas;
using System;
using System.Collections.Generic;
using Core.EntitasAdpater;
using Core.EntityComponent;

namespace App.Shared.Components.Player
{
    [Player]
    public class KeepWatchForAOIComponent: IComponent
    {
        /// <summary>
        ///  视线内残留对象的key 以及 其加入字典的信息（目前信息是时间(单位ms)，以后也可能是别的信息）
        /// </summary>
        public IWatchDict watchMap;
    }

}