﻿using Core.EntityComponent;
using Entitas;
using System.Collections.Generic;

namespace Core.EntityComponent
{
    public interface IWatchDict
    {
        void Update(List<IGameEntity> preEntitys, IContexts _newContexts);
        System.Action<IGameEntity, bool> OnInsertFun { get; }
    }
}