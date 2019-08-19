using System.Collections.Generic;
using System;
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

        private static Color32[] QualityColor =
        {
            new Color32(0xff, 0xff, 0xff, 0xff),
            new Color32(0x9a, 0x99, 0x99, 0xff),
            new Color32(0x56, 0x96, 0x6c, 0xff),
            new Color32(0x66, 0x90, 0xbc, 0xff),
            new Color32(0xbe, 0x76, 0xff, 0xff),
            new Color32(0xff, 0xb3, 0x2a, 0xff),
        };

        public static Color GetQualityColor(int quality)
        {
            if (QualityColor.Length <= quality) return Color.white;
            return QualityColor[quality];
        }

        public static Color HpHighColor
        {
            get { return new Color32(247, 238, 201, 255); }
        }

        private static Dictionary<ChatChannel, Color> chatColors = new Dictionary<ChatChannel, Color>
        {
            {ChatChannel.GameTeam,new Color32(0x83,0xab,0xed,0xff) },
            {ChatChannel.Near,new Color32(0xee,0xee,0xee,0xff) },
            {ChatChannel.PrivateChat,new Color32(0xbe,0x79,0xe5,0xff)},
            {ChatChannel.Camp,new Color32(0x83,0xab,0xed,0xff) }
        };

        private static readonly Color _chatSenderColor = new Color32(0xdf, 0xf7, 0xff, 0xff);

        public static Color ChatSenderColor
        {
            get { return _chatSenderColor; }
    }

        private static readonly Color _systemMessageColor = new Color32(0xf8, 0x35, 0x35, 0xff);

        public static Color SystemMessageColor
        {
            get { return _systemMessageColor; }
}

        public static Color GetChatColorByChatChannel(ChatChannel channel)
        {
            //Color color = new Color32(0xee, 0xee, 0xee, 0xff);
            Color color;
            if (chatColors.TryGetValue(channel, out color))
            {
                return color;
            }

            return new Color32(0xee, 0xee, 0xee, 0xff);
            //chatColors.TryGetValue(channel, out color);
            //return color;
        }
    }
}
