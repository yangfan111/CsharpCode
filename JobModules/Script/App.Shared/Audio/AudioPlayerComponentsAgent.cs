
using UnityEngine;


namespace App.Shared.GameModules
{

    /// <summary>
    /// Defines the <see cref="PlayerComponentsReference" />
    /// </summary>
    public  class AudioPlayerComponentsAgent:PlayerAudioComponentsReference
    {
        public AudioPlayerComponentsAgent(PlayerEntity in_entity) : base(in_entity)
        {
            
        }
        
        public void RefreshStepPlayTimestamp()
        {
            RelatedAudio.LastFootPrintPlayStamp = RelatedTime;
        }

    }
    
}
