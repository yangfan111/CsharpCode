using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using App.Shared;
using Core.Playback;
using Core.Utils;
using JetBrains.Annotations;
using Utils.Singleton;

namespace Core.EntityComponent
{
    public interface IGameEntityCompareAgent
    {
        int Diff(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle);
    }

    public abstract class AbstractGameEntityCompareAgent : IGameEntityCompareAgent
    {
        protected EntityDiffData diffCacheData = new EntityDiffData();

        protected IEntityMapDiffHandler handler;

        public virtual void Init(IEntityMapDiffHandler handler)
        {
            this.handler = handler;
        }

        public abstract int Diff(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle);
       
    }

  

    
   

  
}