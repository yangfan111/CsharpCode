
using App.Shared.Components.Player;
using UnityEngine;


namespace App.Shared.GameModules
{

    /// <summary>
    /// Defines the <see cref="PlayerComponentsReference" />
    /// </summary>
    public  class PlayerAudioComponentsReference:PlayerComponentsReference
    {
        public PlayerAudioComponentsReference(PlayerEntity in_entity) : base(in_entity)
        {
            
        }

        public Vector3 RelatedPlayerPos
        {
            get { return entity.position.Value; }
        }
        public PlayerAudioComponent RelatedAudio
        {
            get { return entity.playerAudio; }
        }
    }
    
}
