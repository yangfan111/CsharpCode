using System.IO;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Patch
{
    public abstract class AbstractComponentPatch: BaseRefCounter
    {
        

        private IGameComponent _component;

        public bool DoCompress = true;

        protected AbstractComponentPatch()
        {
            
        }

        public IGameComponent Component
        {
            get { return _component; }
        }
        
        public abstract void ApplyPatchTo(IGameEntity entity,INetworkObjectSerializerManager serializerManager);
	    public abstract void ApplyPatchTo(INetworkObject target, INetworkObjectSerializerManager serializerManager);
        public abstract void Serialize(MyBinaryWriter writer ,INetworkObjectSerializerManager serializerManager);
        public abstract void DeSerialize(BinaryReader reader, INetworkObjectSerializerManager serializerManager);

        protected int GetComponentTypeId()
        {

            return 0;
        }

	    protected void CreateGameComponent(int componentId)
	    {
		    if (_component != null)
		    {
				GameComponentInfo.Instance.Free(_component);
			}
			_component = GameComponentInfo.Instance.Allocate(componentId);
	    }

	    protected override void OnCleanUp()
	    {
		    GameComponentInfo.Instance.Free(_component);
		    _component = null;
	    }
    }
}
