using System.Collections.Generic;
using System.IO;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Replicaton;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Patch
{
    public abstract class AbstractEntityPatch : BaseRefCounter
    {
       

        protected override void OnCleanUp()
        {
            foreach (var comp in _componentPatchList)
            {
                comp.ReleaseReference();
            }
            _componentPatchList.Clear();
        }
        private List<AbstractComponentPatch> _componentPatchList;
        public EntityKey Key
        {
            get; set;
        }
        public List<AbstractComponentPatch> ComponentPatchList {
            get { return _componentPatchList; }
            set { _componentPatchList = value; }

        }

        protected AbstractEntityPatch(EntityKey entityKey)
        {
            Key = entityKey;
            ComponentPatchList = new List<AbstractComponentPatch>();
        }

        protected AbstractEntityPatch(EntityKey entityKey, List<AbstractComponentPatch> patchList)
        {
            Key = entityKey;
            ComponentPatchList = patchList;
        }

        public AbstractEntityPatch()
        {
            ComponentPatchList = new List<AbstractComponentPatch>();
        }
        public abstract void ApplyPatchTo(ISnapshot snap,INetworkObjectSerializerManager serializerManager);

        public void AddComponentPatch(AbstractComponentPatch cp)
        {
            cp.AcquireReference();
            ComponentPatchList.Add(cp);
        }

        public int GetComponentPatchCount()
        {
            return ComponentPatchList.Count;
        }

        public abstract void Serialize(MyBinaryWriter writer, INetworkObjectSerializerManager manager);
        public abstract void DeSerialize(BinaryReader reader,  INetworkObjectSerializerManager manager);

    }
}
