using Core.EntityComponent;
using Entitas;
using UnityEngine;
using XmlConfig;

namespace Core.IFactory
{
    public interface ISoundEntityFactory
    {
        Entity CreateSyncSound(Entity playerEntity, SoundConfigItem soundConfig, bool loop);
        Entity CreateNonSyncSound(Entity playerEntity, SoundConfigItem soundConfig, bool loop);

        Entity CreateSelfOnlySound(int id, Vector3 position, bool loop);
        Entity CreateSelfOnlySound(int id, bool loop);
        Entity CreateSelfOnlyMoveSound(Vector3 startPosition, EntityKey target, int id, bool loop);
    }
}