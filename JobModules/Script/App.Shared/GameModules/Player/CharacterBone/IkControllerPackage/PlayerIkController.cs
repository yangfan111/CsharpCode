using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using Core.CharacterBone;
using Utils.Appearance;

namespace App.Shared.GameModules.Player.CharacterBone.IkControllerPackage
{
    public class PlayerIkController
    {
        class Command
        {
            private Action _playerIk;

            public void SetPlayerIk(Action action)
            {
                _playerIk = action;
            }

            public void Execute()
            {
                if (_playerIk != null)
                {
                    _playerIk.Invoke();
                    _playerIk = null;
                }
            }
        }

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerIkController));
        private List<Command> _outerCommand = new List<Command>();
        private int _currentCommandIndex;

        public bool EnableIK;

        private Command GetAvailableCommand()
        {
            if (_currentCommandIndex >= _outerCommand.Count)
            {
                _outerCommand.Add(new Command());
            }

            return _outerCommand[_currentCommandIndex++];
        }

        public void Execute()
        {
            for (int i = 0; i < _currentCommandIndex; i++)
            {
                _outerCommand[i].Execute();
            }
            _currentCommandIndex = 0;
        }

        public void SyncTo(CharacterBoneComponent value)
        {
            value.EnableIK = EnableIK;
        }

        public void SetEnableIk()
        {
            var cmd = GetAvailableCommand();
            cmd.SetPlayerIk(SetEnableIkImpl);
            Logger.Debug("SetEnableIk");
        }

        public void SetDisableIk()
        {
            var cmd = GetAvailableCommand();
            cmd.SetPlayerIk(SetDisablekImpl);
            Logger.Debug("SetDisableIk");
        }

        private void SetEnableIkImpl()
        {
            EnableIK = true;
        }

        private void SetDisablekImpl()
        {
            EnableIK = false;
        }
    }
}
