using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.Components
{
    public static class GameRules
    {
        public const int Offline = 1;
        public const int SimpleTest = 2;
        // 模式名称
        public const string RuleGroupNormal = "groupNormal"; // 经典团战
        public const string RuleChicken = "chicken"; // 吃鸡模式
        public const string RuleEmpty = "empty"; // 空模式
        public const string RuleBomb = "bomb"; // 爆破
        public const string RuleGroupClassic = "groupClassic"; // 歼灭战
        //生存模式
        public const int Chicken = 1002; //吃鸡模式
        public const int SoloSurvival = 100201; //单人吃鸡
        public const int DoubleSurvival = 100202;//双人吃鸡
        public const int FourGroupSurvival = 100203;//四人吃鸡
        //竞技模式
        public const int Bomb = 2002;//爆破战
        public const int Team = 2003;//团队战
        public const int Elimation = 2004;//歼灭战
        public const int BioMain = 2005; //变异战
        //特殊
        public const int Knife = 3002; //刀战
        public const int Snipe = 3003; //狙击战
        public const int Pistol = 3004; //手枪战
        public const int Grenade = 3005; //雷战
        //深渊
        public const int AbyssSoloSurvival = 5001; //深渊单人
        public const int AbyssGroupNormal = 5002;//深渊团队
        public const int AbyssDoubleSurvival = 5003;//深渊双人
        public const int AbyssFourGroupSurvival = 5004;//深渊四人
        public const int AbyssBomb = 5005; //深渊爆破
        public const int AbyssBio = 5006;//深渊变异
        public const int AbyssBighead = 5007; //深渊大头
        public const int AbyssElimation = 5008; //深渊歼灭
        //天梯模式
        public const int LadderSoloSurvival = 6001; //天梯单人吃鸡
        public const int LadderGroupNormal = 6002; //天梯团队
        public const int LadderDoubleSurvival = 6009;//天梯双人吃鸡
        public const int LadderFourGroupSurvival = 6010;//天梯四人吃鸡

        public static bool IsChicken(int rule)
        {
            var gameModeRule = SingletonManager.Get<GameModeConfigManager>().GetConfigById(rule);
            return gameModeRule == null ? false : gameModeRule.ChickenType == 1;
        }

        public static bool IsNormal(int rule)
        {
            var gameModeRule = SingletonManager.Get<GameModeConfigManager>().GetConfigById(rule);
            return gameModeRule == null ? false : gameModeRule.ParentId == 1;
        }

        public static bool IsSpecial(int rule)
        {
            switch (rule)
            {
                case Knife:
                case Snipe:
                case Pistol:
                case Grenade:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsBomb(int rule)
        {
            switch (rule)
            {
                case Bomb:
                case AbyssBomb:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsTeam(int rule)
        {
            switch (rule)
            {
                case Team:
                case AbyssGroupNormal:
                case LadderGroupNormal:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsElimation(int rule)
        {
            switch (rule)
            {
                case Elimation:
                case AbyssElimation:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsBio(int rule)
        {
            switch (rule)
            {
                case BioMain: return true;
                default: return false;
            }
        }

        public static bool IsAbyss(int rule)
        {
            var gameModeRule = SingletonManager.Get<GameModeConfigManager>().GetConfigById(rule);
            return gameModeRule == null ? false : gameModeRule.ParentId == 2;
        }

        public static int ClothesType(int rule)
        {
            var gameModeRule = SingletonManager.Get<GameModeConfigManager>().GetConfigById(rule);
            return gameModeRule == null ? 0 : gameModeRule.Clothes;
        }
    }

    public static class GameState
    {
        public const int Normal = 0;
        public const int Invisible = 1;
        public const int Visible = 2;
        public const int AirPlane = 3;
        public const int Poison = 4;
        public const int JumpPlane = 5;
        public const int Gliding = 6;
    }

    public enum EGameModeClothes
    {
        Default,
        SingleCamp,
        DualCamp
    }
}