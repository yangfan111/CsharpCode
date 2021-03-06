﻿using Core;
using Core.Prediction.UserPrediction.Cmd;
using System.Collections.Generic;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public static class UserCmdInputConverter
    {
        private static List<InputBlockGroup> inputBlockGrps;

        static UserCmdInputConverter()
        {
            inputBlockGrps = new List<InputBlockGroup>();
            inputBlockGrps.Add(InputBlockGroup.Create(new[]
            {
                EPlayerInput.IsSprint,
                EPlayerInput.IsSlightWalk
            }));
            inputBlockGrps.Add(InputBlockGroup.Create(new[]
            {
                EPlayerInput.IsCrouch,
                EPlayerInput.IsProne,
                EPlayerInput.IsSwitchWeapon,
                EPlayerInput.IsReload,
                EPlayerInput.IsSwitchFireMode,
                EPlayerInput.IsLeftAttack,
                EPlayerInput.IsRightAttack,
            }));
            inputBlockGrps.Add(InputBlockGroup.Create(new[]
             {
                 EPlayerInput.IsProne,
                 EPlayerInput.IsDrawWeapon,
             }));
            /*inputBlockGrps.Add(InputBlockGroup.Create(new[]
            {
                EPlayerInput.IsJump,
                EPlayerInput.IsSprint,
            }));*/
            inputBlockGrps.Add(InputBlockGroup.Create(new[]
            {
                EPlayerInput.IsCrouch,
                EPlayerInput.IsProne,
                EPlayerInput.ChangeCamera,
                //EPlayerInput.IsSprint,
            }));
            inputBlockGrps.Add(InputBlockGroup.Create(new []
            {
                EPlayerInput.IsF,
                EPlayerInput.IsCameraFocus,
                EPlayerInput.IsRightAttack
            }));
        }

        /// <summary>
        /// 将UserCmd映射到FilteredInput
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="input"></param>
        public static void ApplyCmdToInput(IUserCmd cmd, IFilteredInput input)
        {
            //auto
            input.SetInput(EPlayerInput.ChangeCamera, cmd.ChangeCamera);
            input.SetInput(EPlayerInput.IsCameraFocus, cmd.IsCameraFocus);
            input.SetInput(EPlayerInput.IsCameraFree, cmd.IsCameraFree);
            input.SetInput(EPlayerInput.IsCrouch, cmd.IsCrouch);
            input.SetInput(EPlayerInput.IsDrawWeapon, cmd.IsDrawWeapon);
            input.SetInput(EPlayerInput.IsDropWeapon, cmd.IsDropWeapon);
            input.SetInput(EPlayerInput.IsJump, cmd.IsJump);
            input.SetInput(EPlayerInput.IsLeftAttack, cmd.IsLeftAttack);
            input.SetInput(EPlayerInput.IsPeekLeft, cmd.IsPeekLeft);
            input.SetInput(EPlayerInput.IsPeekRight, cmd.IsPeekRight);
            input.SetInput(EPlayerInput.IsProne, cmd.IsProne);
            input.SetInput(EPlayerInput.IsReload, cmd.IsReload);
            input.SetInput(EPlayerInput.IsRightAttack, cmd.IsRightAttack);
            input.SetInput(EPlayerInput.IsSprint, cmd.IsRun);
            input.SetInput(EPlayerInput.IsSlightWalk, cmd.IsSlightWalk);
            input.SetInput(EPlayerInput.IsSwitchFireMode, cmd.IsSwitchFireMode);
            input.SetInput(EPlayerInput.IsThrowing, cmd.IsThrowing);
            input.SetInput(EPlayerInput.IsSwitchWeapon, cmd.IsSwitchWeapon | cmd.CurWeapon > 0);
            input.SetInput(EPlayerInput.IsUseAction, cmd.IsUseAction);
            input.SetInput(EPlayerInput.IsF, cmd.IsF);

            ApplySpecialInput(cmd, input);
        
        }

        //特殊的输入设置
        private static void ApplySpecialInput(IUserCmd cmd, IFilteredInput input)
        {
            //manual
            input.SetInput(EPlayerInput.IsRun, cmd.MoveHorizontal != 0 || cmd.MoveVertical != 0);
            input.SetInput(EPlayerInput.MeleeAttack, input.IsInput(EPlayerInput.IsLeftAttack) | input.IsInput(EPlayerInput.IsRightAttack));
        }

        public static void ApplyInputInterrupt(IFilteredInput input)
        {
            var isNotThrowing = !input.IsInput(EPlayerInput.IsThrowing) || input.IsInput(EPlayerInput.IsThrowingInterrupt);
            input.SetInput(EPlayerInput.IsThrowing, !isNotThrowing);
        }
        /// <summary>
        /// 根据组别和优先级筛选生效的输入，用于处理输入同时存在的情况
        /// </summary>
        /// <param name="filteredInput"></param>
        public static void ApplyInputStaticBlock(IFilteredInput filteredInput)
        {
            foreach (var group in inputBlockGrps)
            {
                group.DoBlock(filteredInput);
            }
        }
    }
}