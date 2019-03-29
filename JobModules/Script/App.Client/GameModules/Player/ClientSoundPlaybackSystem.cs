using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using System.Collections.Generic;
using XmlConfig;
using App.Shared.Sound;
using Core.IFactory;
using Core.Configuration.Sound;
using UnityEngine;

namespace App.Client.GameModules.Player
{
    public class ClientSoundPlaybackSystem : IPlaybackSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientSoundPlaybackSystem));
        private IGroup<PlayerEntity> _playGroup;
        private PlayerContext _playerContext; 
        private List<short> _playSoundList = new List<short>();
        private ISoundEntityFactory _soundEntityFactory;
        private ISoundConfigManager _soundConfigManager;

        public ClientSoundPlaybackSystem(PlayerContext playerContext, 
            ISoundEntityFactory soundEntityFactory,
            ISoundConfigManager soundConfigManager)
        {
            _playGroup = playerContext.GetGroup(PlayerMatcher.Sound);
            _soundEntityFactory = soundEntityFactory;
            _soundConfigManager = soundConfigManager;
            _playerContext = playerContext;
        }

        public void OnPlayback()
        {
            var selfPlayer = _playerContext.flagSelfEntity;
            if(null == selfPlayer)
            {
                return;
            }
            foreach(var player in _playGroup.GetEntities())
            {
                SoundSyncUtil.NewNonLoopSoundFromComponent(player.sound, _playSoundList);
                for(var i = 0; i < _playSoundList.Count; i++)
                {
                    var cfg = _soundConfigManager.GetSoundById(_playSoundList[i]);
                    if(null != cfg)
                    {
                        if(Vector3.Distance(player.position.Value, selfPlayer.position.Value) <= cfg.Distance )
                        {
                            _soundEntityFactory.CreateSelfOnlyMoveSound(player.position.Value, player.entityKey.Value, _playSoundList[i], false);
                        }
                    }
                }
                SoundSyncUtil.NewLoopSoundFromComponent(player.sound, _playSoundList);
                for(var i = 0; i < _playSoundList.Count; i++)
                {
                    _soundEntityFactory.CreateSelfOnlyMoveSound(player.position.Value, player.entityKey.Value, _playSoundList[i], true);
                }
            }
        }
    }
}