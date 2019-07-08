using App.Shared;
using App.Shared.Audio;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core;
using Core.Free;
using Core.IFactory;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Scene
{

    public partial class PlayerAudioHandler : ISimpleMesssageHandler
    {
        private ISoundEntityFactory _soundEntityFactory;

        public PlayerAudioHandler(ISoundEntityFactory soundEntityFactory)
        {
            _soundEntityFactory = soundEntityFactory;
        }

        public bool CanHandle(int key)
        {
            return FreeMessageConstant.PlaySound == key;
        }

        public void Handle(SimpleProto data)
        {
            if (data.Ks[0] == 0)
            {
                //使用物品声音
                PlayerEntity player = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;
                if (player != null)
                {
                    var uniqueId = AudioUtil.ToUseItemAudioUniqueId(data.Ins[0]);
                    player.AudioController().PlaySimpleAudio(uniqueId);
                }
            }

            if (data.Ks[0] == 1)
            {
                //游戏开始关闭环境音
                MapAmbInfo ambInfo;
                Wwise_IDs.GetMapAmb(data.Ins[0], out ambInfo);
                ambInfo.StopAmb();
            }

            if (data.Ks[0] == 2)
            {
                PlayerEntity player = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;
                if (player != null)
                {
                    player.AudioController().PlaySimpleAudio((EAudioUniqueId) data.Ins[0]);
                }
            }
        }
    }
}