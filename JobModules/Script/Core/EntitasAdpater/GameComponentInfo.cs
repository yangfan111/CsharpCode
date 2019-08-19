using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Components;
using Core.ObjectPool;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Core.EntityComponent
{
    public class GameComponentInfo : IGameComponentInfo
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(GameComponentInfo));

        public static GameComponentInfo Instance = new GameComponentInfo();
        private IObjectAllocator[] _allocators = new IObjectAllocator[0];
      
        private int _maxId;
        private GameComponentInfo()
        {
          
          
        }

        public void Init(IObjectAllocator[] allocators, int maxId)
        {
            _allocators = allocators;
            _maxId = maxId;
        }
        public IGameComponent Allocate(int componentId)
        {
            if(_allocators[componentId]== null)
            {
                throw new ArgumentException(string.Format("componentId:{0}", componentId));
            }
            var rc = (IGameComponent)_allocators[componentId].Allocate();
            if (rc is IResetableComponent)
            {
                (rc as IResetableComponent).Reset();
            }

            return rc;
        }

        public void Free(IGameComponent component)
        {
            _allocators[component.GetComponentId()].Free(component);
        }

        public void Free(int componentId, IGameComponent component)
        {
            _allocators[componentId].Free(component);
        }

        public int MaxComponentId { get { return _maxId; } }
    }
}