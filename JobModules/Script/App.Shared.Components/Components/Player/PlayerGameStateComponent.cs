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

        private PlayerLifeStateEnum _lifeState = PlayerLifeStateEnum.NullState;
        public PlayerLifeStateEnum CurrentPlayerLifeState
        {
            set
            {
                _lifeState = value;
                if(_lifeState != PlayerLifeStateEnum.NullState)
                    _logger.InfoFormat("SetCurrentLifeState:  {0}", _lifeState);
            }
            get { return _lifeState; }
        }
    }

    public enum PlayerLifeStateEnum
    {
        NullState,
        Reborn,
        Revive,
        Dying,
        Dead,
        EndState
    }
}