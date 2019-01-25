using Core.EntityComponent;
using UnityEngine;

namespace App.Shared.Sound
{
    public interface ISoundPlayerEntityAdapter
    {
        EntityKey EntityKey { get; }
        Vector3 Position { get; }
    }

    public class SoundPlayerEntityAdapter : ISoundPlayerEntityAdapter
    {
        PlayerEntity _playerEntity;
        public SoundPlayerEntityAdapter(PlayerEntity playerEntity)
        {
            _playerEntity = playerEntity;
        }

        public EntityKey EntityKey
        {
            get
            {
                return _playerEntity.entityKey.Value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return _playerEntity.position.Value;
            }
        }
    }

}
