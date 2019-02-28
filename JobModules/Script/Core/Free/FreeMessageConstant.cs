using System.Collections.Generic;

namespace Core.Free
{
    public class FreeMessageConstant
    {
        public const string SpliterRecord = "원";
        public const string SpilterField = "빈";
        public const string SpliterStyle = "_$$$_";

        public const int ClientLoginSucess = 10;
        public const int PickUpItem = 11;
        public const int RecordFrame = 12;

        public const int ClickImage = 1;
        public const int MoveImage = 3;
        public const int DragImage = 5;
        public const int DebugData = 6;
        public const int SplitItem = 7;
        public const int ObservePlayer = 8;

        public const int MSG_SEE_ALL = 43;
        public const int CONFIG_ALLOW_MOUSE_KEYS = 44;
        public const int FROG_EFFECT = 45;
        public const int CHANGE_SKYBOX = 46;
        public const int LIGHTMAP = 47;
        public const int PVS = 48;
        public const int BLOOD_SPAY = 49;

        public const int MSG_UI_CREATE = 50;
        public const int MSG_UI_SHOW = 51;
        public const int MSG_UI_VALUE = 52;
        public const int MSG_UI_TIPS = 53;
        public const int MSG_UI_DELETE = 55;
        public const int MSG_REMOVE_ALL_UI = 56;

        public const int MSG_EFFECT_CREATE = 60;
        public const int MSG_EFFECT_SHOW = 61;
        public const int MSG_EFFECT_UPDATE = 62;
        public const int MSG_EFFECT_DELETE = 65;

        public const int ChangeAvatar = 81;
        public const int PoisonCircle = 82;
        public const int CountDown = 83;
        public const int DuplicateUI = 84;
        public const int RegisterCommand = 85;
        public const int AddChild = 86;
        public const int MarkPos = 87;
        public const int InventoyUI = 88;
        public const int ScoreInfo = 89;
        public const int TestFrame = 90;
        public const int LockMouse = 91;
        public const int ShowCodeUI = 92;
        public const int PlaySound = 93;
        public const int AirLineData = 94;
        public const int GameOver = 95;
        public const int ShowSplitUI = 96;
        public const int ItemInfo = 97;
        public const int ClientSkill = 98;
        public const int ClientShowUI = 99;
        public const int GroupScoreUI = 101;
        public const int GroupTechStatUI = 102;
        public const int GroupGameOverUI = 103;
        public const int EntityMoveTo = 104;
		public const int PlayerCmd = 105;
        public const int PlayerAnimation = 106;
		public const int BlastScoreUI = 107;
        public const int ChangeWeapon = 108;
        public const int CommonRoundOverUI = 109;
        public const int CountDownTipUI = 110;
        public const int ResetBattleData = 111;
        public const int BombAreaMarkUI = 112;
        public const int BombDropTipUI = 113;
        public const int RevengeTagUI = 114;
        public const int FetchUIValue = 113;

        private static Dictionary<int, string> MessageDesc = new Dictionary<int, string>();

        static FreeMessageConstant()
        {
            MessageDesc.Add(MSG_UI_CREATE, "创建UI");
            MessageDesc.Add(MSG_UI_SHOW, "展示UI");
            MessageDesc.Add(MSG_UI_VALUE, "更新UI值");

            MessageDesc.Add(MSG_EFFECT_CREATE, "创建特效");
            MessageDesc.Add(MSG_EFFECT_SHOW, "展示特效");
            MessageDesc.Add(MSG_EFFECT_UPDATE, "更新特效");
            MessageDesc.Add(MSG_EFFECT_DELETE, "删除特效");
            MessageDesc.Add(RegisterCommand, "注册控制台命令");
        }

        public static string GetMessageDesc(int msg)
        {
            if (MessageDesc.ContainsKey(msg))
            {
                return MessageDesc[msg];
            }

            return msg.ToString();
        }
    }
}
