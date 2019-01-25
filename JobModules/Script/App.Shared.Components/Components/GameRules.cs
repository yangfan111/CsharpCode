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


        //天梯模式
        public const int LadderSoloSurvival = 400201; //单人吃鸡
        public const int LadderDoubleSurvival = 400202;//双人吃鸡
        public const int LadderFourGroupSurvival = 400203;//四人吃鸡
        public const int LadderFiveGroupSurvival = 400204;//五人吃鸡

        //生存模式
        public const int SoloSurvival = 100201; //单人吃鸡
        public const int DoubleSurvival = 100202;//双人吃鸡
        public const int FourGroupSurvival = 100203;//四人吃鸡
        public const int FiveGroupSurvival = 100204;//五人吃鸡

        //竞技模式
        public const int Bomb = 2002;//爆破战
        public const int Team = 2003;//团队战
        public const int Annihilation = 2004;//歼灭战

        public static bool IsChicken(int rule)
        {
            switch (rule)
            {
                case LadderDoubleSurvival:
                case LadderSoloSurvival:
                case LadderFourGroupSurvival:
                case LadderFiveGroupSurvival:
                case SoloSurvival:
                case DoubleSurvival:
                case FourGroupSurvival:
                case FiveGroupSurvival:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNormal(int rule)
        {
            switch (rule)
            {
                case Bomb:
                case Team:
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum GameMode
    {
        Normal = 1,
        Survival = 2,
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

}