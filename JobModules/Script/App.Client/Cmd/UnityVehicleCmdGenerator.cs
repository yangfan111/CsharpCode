using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using Core.Prediction.VehiclePrediction.Cmd;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.Cmd
{
    public class UnityVehicleCmdGenerator : IVehicleCmdGenerator
    {
        private int _seq;
        private int _lastCmdExecuteTime = 0;
        private UserInputManager.Lib.UserInputManager _inputManager;
        private VehicleCmd _lastCmd;

        private GameObject autoGo;
        private bool firstSet = true;

        public UnityVehicleCmdGenerator(UserInputManager.Lib.UserInputManager manager)
        {
            
            _lastCmd = new VehicleCmd();
            _inputManager = manager;
            // 界面不会影响车辆控制
            var keyhandler = new KeyHandler(Layer.Ui, BlockType.None);
            _inputManager.RegisterKeyhandler(keyhandler);
            keyhandler.BindKeyAction(UserInputKey.MoveVertical, data => _lastCmd.MoveVertical = data.Axis);
            keyhandler.BindKeyAction(UserInputKey.MoveHorizontal, data => _lastCmd.MoveHorizontal = data.Axis);
            keyhandler.BindKeyAction(UserInputKey.VehicleSpeedUp, data => _lastCmd.IsSpeedup = true);
            keyhandler.BindKeyAction(UserInputKey.VehicleBrake, data => _lastCmd.IsHandbrake = true);
            keyhandler.BindKeyAction(UserInputKey.VehicleHorn, data => _lastCmd.IsHornOn = true);

            keyhandler.BindKeyAction(UserInputKey.VehicleLeftShift, data => _lastCmd.IsLeftShift = true);
            keyhandler.BindKeyAction(UserInputKey.VehicleRightShift, data => _lastCmd.IsRightShift = true);
            keyhandler.BindKeyAction(UserInputKey.VehicleStunt, data => _lastCmd.IsStunt = true);
        }

        public IVehicleCmd GeneratorVehicleCmd(int currentSimulationTime)
        {
            if (currentSimulationTime <= 0 || _lastCmdExecuteTime >= currentSimulationTime)
            {
                return null;
            }

            var vehicleCmd = VehicleCmd.Allocate();
            {
                vehicleCmd.Seq = _seq++;
                vehicleCmd.CmdSeq = vehicleCmd.Seq;
            };

            _lastCmd.CopyInputTo(vehicleCmd);
            _lastCmd.ResetInput();
            vehicleCmd.ExecuteTime = currentSimulationTime;
            vehicleCmd.Seq = currentSimulationTime;
            _lastCmdExecuteTime = currentSimulationTime;

            if (firstSet)
            {
                autoGo = GameObject.Find("VehicleCmdAutoGenerator");
                firstSet = false;
                if (autoGo != null)
                {
                    autoGo.SetActive(false);
                }
            }

            if (autoGo != null && autoGo.activeSelf)
            {
                vehicleCmd.MoveVertical = 1;
                vehicleCmd.MoveHorizontal = 0.6f;
            }
         
            return vehicleCmd;
        }
    }
}
