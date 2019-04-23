using System.Collections.Generic;
using WindowsInput;
using WindowsInput.Native;
using App.Server.GameModules.GamePlay.free.player;
using Core.Prediction.UserPrediction.Cmd;
using UserInputManager.Lib;

namespace App.Shared.FreeFramework.framework.ai.player
{
    public class PlayerKeyCmdHandler : IPlayerCmdHandler
    {
        public bool CanHandle(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            return !player.playerIntercept.PressKeys.Empty || !player.playerIntercept.InterceptKeys.Empty || !player.playerIntercept.RealPressKeys.Empty;
        }

        public void Handle(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            if(!player.playerIntercept.PressKeys.Empty){
                int[] keys = player.playerIntercept.PressKeys.Keys;
                foreach (int key in keys)
                {
                    SimplePlayerInput.PressKey(cmd, key);
                }
                player.playerIntercept.PressKeys.Frame();
            }
            
            if(!player.playerIntercept.InterceptKeys.Empty){
                int[] keys = player.playerIntercept.InterceptKeys.Keys;
                foreach (int key in keys)
                {
                    SimplePlayerInput.ReleaseKey(cmd, key);
                }
                player.playerIntercept.InterceptKeys.Frame();
            }

            if (!player.playerIntercept.RealPressKeys.Empty)
            {
                var simulator = new InputSimulator();
                List<int> remove = player.playerIntercept.RealPressKeys.Frame();
                if (remove.Count > 0)
                {
                    foreach (int key in remove)
                    {
                        if (key == 1)
                        {
                            simulator.Mouse.LeftButtonUp();
                        }
                        else if(key == 2)
                        {
                            simulator.Mouse.RightButtonUp();
                        }
                        else
                        {
                            simulator.Keyboard.KeyUp((VirtualKeyCode) key);
                        }
                    }
                }
            }
        }
    }
}
