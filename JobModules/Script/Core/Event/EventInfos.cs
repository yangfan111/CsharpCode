using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Components;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Core.Event
{
    public class EventInfos:IEventInfos
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(EventInfos));
        public static EventInfos Instance = new EventInfos();
        IObjectAllocator[] _allocators = new IObjectAllocator[50];
        IEventHandler[] _eventHandlers = new IEventHandler[50];
        Type[] _types = new Type[50];
        private int _maxId;

        public EventInfos()
        {
            InitEventAllocator();
            InitEventHandlers();
        }

        private void InitEventHandlers()
        {
            var types = FindAllGameComponentType(typeof(IEventHandler));
            foreach (var type in types)
            {
                try
                {
                    IEventHandler instance = (IEventHandler) Activator.CreateInstance(type);

                    var id = (int)instance.EventType;
                    if (ArrayUtility.SafeGet(_allocators, id) == null)
                    {
                        var msg = String.Format("_allocators id not exist id {0}, type {1}, type {2}", id, _types[id],
                            instance.GetType());
                        AssertUtility.Assert(false, msg);
                        _logger.Error(msg);
                    }else if (ArrayUtility.SafeGet(_eventHandlers, id) != null)
                    {
                        var msg = String.Format("IEventHandler id already exist id {0}, type {1}, type {2}", id, _types[id],
                            instance.GetType());
                        AssertUtility.Assert(false, msg);
                        _logger.Error(msg);
                    }
                    else
                    {
                        ArrayUtility.SafeSet(ref _eventHandlers, id, instance);
                       
                    }

                    _maxId = Mathf.Max(id, _maxId);
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("construct instance of type {0} failed {1}", type, e);
                }
            }
        }

        private void InitEventAllocator()
        {
            var types = FindAllGameComponentType(typeof(IEvent));
            foreach (var type in types)
            {
                try
                {
                    IEvent instance = (IEvent) Activator.CreateInstance(type);

                    int id = (int) instance.EventType;
                    if (ArrayUtility.SafeGet(_types, id) != null)
                    {
                        var msg = String.Format("component id already exist id {0}, type {1}, type {2}", id, _types[id],
                            instance.GetType());
                        AssertUtility.Assert(false, msg);
                        _logger.Error(msg);
                    }
                    else
                    {
                        ArrayUtility.SafeSet(ref _types, id, type);
                        ArrayUtility.SafeSet(ref _allocators, id, ObjectAllocators.GetAllocator(type));
                    }

                    _maxId = Mathf.Max(id, _maxId);
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("construct instance of type {0} failed {1}", type, e);
                }
            }
        }

        public List<Type> FindAllGameComponentType(  Type iType)
        {
            
            Assembly[] assemblyList = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> resList = new List<Type>();
            foreach (var assembly in assemblyList)
            {
                try
                {
                  
                    List<Type> typeListInAssembly = DiscoverIGameComponentInAssembly(assembly,iType);
                    resList.AddRange(typeListInAssembly);

                }
                catch (Exception e)
                {
                    var v = assembly;
                    _logger.ErrorFormat("error {0} in {1}", e, v);
                }
            }
            return resList;
        }

        private List<Type> DiscoverIGameComponentInAssembly(Assembly assembly, Type iType)
        {
            try
            {
                Type[] typeList = assembly.GetTypes();
                List<Type> resList = new List<Type>();
                foreach (var type in typeList)
                {
                    
                    if (iType.IsAssignableFrom(type) && (!type.IsAbstract && !type.IsInterface))
                    {
                        resList.Add(type);
                    }
                }
                return resList;
            }
            catch (NotSupportedException)
            {
                //dononthing
                return new List<Type>();
            }


             
        }

        public IEvent Allocate(EEventType eventType, bool isRemote)
        {
            var rc = (IEvent)_allocators[(int)eventType].Allocate();
            rc.IsRemote = isRemote;
            return rc;
        }

     

        public void Free(IEvent e)
        {
            _allocators[(int)e.EventType].Free(e);
        }

        public IEventHandler GetEventHandler(EEventType eEventType)
        {
            return _eventHandlers[(int) eEventType];
        }

       
    }
}
