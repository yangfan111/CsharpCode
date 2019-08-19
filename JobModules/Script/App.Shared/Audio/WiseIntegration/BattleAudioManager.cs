using Core.Utils;

namespace App.Shared.Audio
{
    public class BattleAudioManager : AudioProjectManager<BattleAudioManager>, IAudioProjectManager
    {
        public static LoggerAdapter Logger = new LoggerAdapter(typeof(BattleAudioManager));
        protected override void ProjectInitialize()
        {
            InitBattleListner();
            GameAudioMedia.Prepare();
            Logger.Info("[Wwise] BattleAudioManager media prepare sucess");
            
        }

        AudioSourceType sourceType = AudioSourceType.Battle;
        public AudioSourceType SourceType
        {
            get { return sourceType; }
            set { sourceType = value; }
        }

    }
}