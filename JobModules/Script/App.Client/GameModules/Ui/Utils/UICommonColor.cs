using UnityEngine;
using Utils.Configuration;

namespace App.Client.GameModules.Ui.Utils
{
    public class UiCommonColor
    {
        public static Color KillerPlayerColor
        {
            get { return new Color32(153, 199, 255, 255); }
        }

        public static Color DeadPlayerColor
        {
            get { return new Color32(243, 83, 83, 255); }
        }

        public static Color GetChatChannelColor(ChatChannel channel)
        {
            if (channel == ChatChannel.GameTeam)
            {
                return KillerPlayerColor;
            }
            else if (channel == ChatChannel.PrivateChat)
            {
                return Color.red;
            }

            return Color.white;
        }

        public static float ColorGammaCorrect(float val)
        {
            return Mathf.Pow(val, 1f / 2.2f);
        }

        public static Color HpLowColor
        {
            get { return new Color32(237, 129, 129, 255); }
        }

        public static Color HpHighColor
        {
            get { return new Color32(247, 238, 201, 255); }
        }
    }
}
