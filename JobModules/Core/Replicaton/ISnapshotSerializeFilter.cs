using Core.EntityComponent;
using Core.Network;
using UnityEngine;

namespace Core.Replicaton
{
    public interface ISnapshotSerializeFilter : IEntityMapFilter
    {
        EntityKey Self { get; }
        Vector3 Position { get; }
        
    }

   

}