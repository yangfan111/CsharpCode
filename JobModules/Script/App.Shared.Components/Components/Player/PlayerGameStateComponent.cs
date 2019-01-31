using Core.CharacterBone;
using Core.Components;
using Core.EntityComponent;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace App.Shared.Components.Player
{
    [Player]
    public class PlayerGameStateComponent : IComponent
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerGameStateComponent));
        private PlayerSystemEnum _state;
        public PlayerSystemEnum CurrentPlayerSystemState {
            set
            {
                _state = value; 
                if(PlayerSystemEnum.NullState != _state)
                    _logger.InfoFormat("ChangeStartInSystem:  {0}", (PlayerSystemEnum)_state);
            }
            get { return _state; }
        }
    }

    public enum PlayerSystemEnum
    {
        NullState,
        PlayerStateUpdate,
        PlayerServerStateUpdate,
        PlayerAppearanceUpdate,
        PlayerCharacterBoneUpdate,
        PlayerClimbUpdate,
        PlayerDeadAnim,
        PlayerAnimationPlayBack,
        PlayerAppearancePlayBack,
        PlayerCharacterBonePlayBack,
        EndState
    }
}