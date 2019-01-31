using Core.GameInputFilter;
using Core.Prediction.UserPrediction.Cmd;
using System.Collections.Generic;
using System.Reflection;
using XmlConfig;

namespace App.Shared.GameInputFilter
{
    public class UserCommandMapper
    {
        private Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();
        private List<List<EPlayerInput>> _groups = new List<List<EPlayerInput>>();
        public UserCommandMapper()
        {
            var properties = typeof(UserCmd).GetProperties();
            for(int i = 0; i < properties.Length; i++)
            {
                _properties[properties[i].Name] = properties[i];
            }
            _groups.Add(new List<EPlayerInput>
            {
                EPlayerInput.IsSprint,
                EPlayerInput.IsSlightWalk,
                //跑步
                //TOOD 屏息
            });
            _groups.Add(new List<EPlayerInput>
            {
                EPlayerInput.IsCrouch,
                EPlayerInput.IsProne,
                EPlayerInput.IsSwitchWeapon,
                EPlayerInput.IsReload,
                EPlayerInput.IsSwitchFireMode,
                EPlayerInput.IsLeftAttack,
                EPlayerInput.IsRightAttack,
            });
            _groups.Add(new List<EPlayerInput>
            {
                EPlayerInput.IsJump,
                EPlayerInput.IsSprint,
            });
            _groups.Add(new List<EPlayerInput>
            {
                EPlayerInput.ChangeCamera,
                EPlayerInput.IsCrouch,
                EPlayerInput.IsProne,
                EPlayerInput.IsSwitchWeapon,
                EPlayerInput.IsSprint,
            });

        }

        public void Map(IUserCmd cmd, IFilteredInput input)
        {
            ManualMap(cmd, input); 
        }
        
        /// <summary>
        /// 将UserCmd映射到FilteredInput
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="input"></param>
        private void ManualMap(IUserCmd cmd, IFilteredInput input)
        {
            //并非一一对应
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
            input.SetInput(EPlayerInput.IsRun, cmd.MoveHorizontal != 0 || cmd.MoveVertical != 0);
            input.SetInput(EPlayerInput.IsSlightWalk, cmd.IsSlightWalk);
            input.SetInput(EPlayerInput.IsSwitchFireMode, cmd.IsSwitchFireMode);
            input.SetInput(EPlayerInput.IsThrowing, cmd.IsThrowing);
            input.SetInput(EPlayerInput.IsSwitchWeapon, cmd.IsSwitchWeapon | cmd.CurWeapon > 0);
            input.SetInput(EPlayerInput.MeleeAttack, input.IsInput(EPlayerInput.IsLeftAttack) | input.IsInput(EPlayerInput.IsRightAttack));
            input.SetInput(EPlayerInput.IsUseAction, cmd.IsUseAction);
            SpecialStateMap(cmd, input);
            ApplyInputBlock(input);
        }

        //特殊的输入设置
        private void SpecialStateMap(IUserCmd cmd, IFilteredInput input)
        {
            //拉栓为自动触发，这里的true只表示可以执行
            input.SetInput(EPlayerInput.IsPullbolting, true);
        }

        /// <summary>
        /// 根据组别和优先级筛选生效的输入，用于处理输入同时存在的情况
        /// </summary>
        /// <param name="filteredInput"></param>
        public void ApplyInputBlock(IFilteredInput filteredInput)
        {
            foreach(var group in _groups)
            {
                var block = false;
                foreach(var input in group)
                {
                    if(block)
                    {
                        filteredInput.SetInput(input, false);
                    }
                    else
                    {
                        if(filteredInput.IsInput(input))
                        {
                            block = true; 
                        }
                    }
               }
            }
        }
    }
}
