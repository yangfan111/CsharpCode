using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.UserInput;
using App.Shared.Util;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameModules.Player
{
    public class InputSchemeUpdateSystem: AbstractUserCmdExecuteSystem,IBeforeUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(InputSchemeUpdateSystem));
        private static readonly string HorizontalAction = "Horizontal";
        private static readonly string VerticalAction = "Vertical";
        private static readonly List<string> BlockActions = new List<string>()
        {
            HorizontalAction, VerticalAction
        };

        protected override bool filter(PlayerEntity player)
        {
            return player.hasAppearanceInterface && player.appearanceInterface.Appearance!=null;
        }

        protected override void ExecuteUserCmd(PlayerEntity player, IUserCmd cmd)
        {
            UpdateAction(player);
        }

        private static void BlockAction(PlayerEntity player)
        {
            var stateManager = player.stateInterface.State;
            var postureState = stateManager.GetCurrentPostureState();
            var item = SingletonManager.Get<InputBlockConfigManager>().GetConfig(postureState);
            if (item != null)
            {
                switch (item.BlockType)
                {
                    case InputBlockType.BlockMovement:
                    {
                        BlockMovement();
                        break;
                    }
                    default:
                        Logger.InfoFormat("unknown type!!!");
                        break;
                }
            }
            else
            {
                ReleaseMovement();
            }
        }

        private static void ReleaseMovement()
        {
            foreach (string blockAction in BlockActions)
            {
                PlayerInputManager.ReleaseAction(blockAction);
            }
        }

        private static void BlockMovement()
        {
            foreach (string blockAction in BlockActions)
            {
                PlayerInputManager.BlockAction(blockAction);
            }
        }


        private static void UpdateAction(PlayerEntity player)
        {
            var updateList = player.appearanceInterface.Appearance.GetInputSchemeActionFieldToUpdate();
            foreach (KeyValuePair<int, string> keyValuePair in updateList)
            {
                PlayerInputManager.UpdateAction(keyValuePair.Key, keyValuePair.Value);
                Logger.InfoFormat("updateListCount key:{0}, value:{1}", keyValuePair.Key, keyValuePair.Value);
            }

            updateList.Clear();
        }

        public void BeforeExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
			return;
//            var player = owner.OwnerEntity as PlayerEntity;
//            AssertUtility.Assert(player != null);
//            BlockAction(player);

        }
    }
}
