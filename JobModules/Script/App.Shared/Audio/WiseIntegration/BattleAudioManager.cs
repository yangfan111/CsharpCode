using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

namespace App.Shared.Audio
{
    public class BattleAudioManager : AudioProjectManager<BattleAudioManager>, IAudioProjectManager
    {
        public static LoggerAdapter Logger = new LoggerAdapter(typeof(BattleAudioManager));
        protected override void ProjectInitialize()
        {
            InitBattleListner();
            GameAudioMedia.Prepare();
        }

        public AudioSourceType SourceType
        {
            get { return AudioSourceType.Battle; }
        }
        
    }
}