using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Components;
using Core.EntityComponent;
using Core.Event;
using Core.GameModule.Interface;
using Core.ObjectPool;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Core.UpdateLatest;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
  
    [Player]
    [Serializable]
    
    public class LocalEventsComponent : IComponent, IReusableObject
    {
        [NetworkProperty] public PlayerEvents Events;

        public void ReInit()
        {
            if (Events == null) Events = new PlayerEvents();
            Events.ReInit();
        }
    }

    [Player]
    [Serializable]
    public class UploadEventsComponent : IReusableObject, IUpdateComponent
    {
        [NetworkProperty] public PlayerEvents Events;
      
        
        public void ReInit()
        {
            if (Events == null) Events = new PlayerEvents();
            Events.ReInit();
        }

        public void CopyFrom(object rightComponent)
        {
            if (Events == null) Events = new PlayerEvents();
            var right = (UploadEventsComponent) rightComponent;
            Events.CopyFrom(right.Events);
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.UploadEvents;
        }
    }

    [Player]
    [Serializable]
    
    public class RemoteEventsComponent : IReusableObject, IPlaybackComponent
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RemoteEventsComponent));
        [NetworkProperty] public PlayerEvents Events;

        public void CopyFrom(object rightComponent)
        {
            if (Events == null) Events = new PlayerEvents();
            var right = (RemoteEventsComponent) rightComponent;
            Events.CopyFrom(right.Events);
         
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.RemoteEvents;
        }

        public void ReInit()
        {
           
            if (Events == null) Events = new PlayerEvents();
            Events.ReInit();
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            if (Events == null) Events = new PlayerEvents();
           
            if (Events.ServerTime != interpolationInfo.LeftServerTime)
            {
                
                var l = left as RemoteEventsComponent;
                Events.CopyFrom(l.Events);
                Events.HasHandler = false;
                Events.ServerTime = interpolationInfo.LeftServerTime;
                
            }
        }
    }
}