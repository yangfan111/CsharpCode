namespace App.Shared.Audio
{
    public class EVENTS
    {
        public static uint CAR_START = 2744306108U;
        public static uint CAR_STOP = 1465464336U;
        public static uint GUN_56_SHOT = 2397386488U;
        public static uint GUN_56_SHOT_CONTINUE_STOP = 3771645931U;
    } // public class EVENTS

    public class STATES
    {
        public class GLOBAL_CAMERA
        {
            public static uint GROUP = 3396107022U;

            public class STATE
            {
                public static uint GLOBAL_CAMERA_OVERWATER = 2338144800U;
                public static uint GLOBAL_CAMERA_UNDERWATER = 1315062734U;
            } // public class STATE
        } // public class GLOBAL_CAMERA

    } // public class STATES

    public class SWITCHES
    {
        public class AQUTIC
        {
            public static uint GROUP = 1284892144U;

            public class SWITCH
            {
                public static uint AQUTIC_OVERWATER = 3899244178U;
                public static uint AQUTIC_UNDERWATER = 4206223920U;
            } // public class SWITCH
        } // public class AQUTIC

        public class FOOTSTEP_ACT_TYPE
        {
            public static uint GROUP = 3312628285U;

            public class SWITCH
            {
                public static uint CRAWL = 3115216662U;
                public static uint JUMP = 3833651337U;
                public static uint SQUAT = 1990176681U;
                public static uint WALK = 2108779966U;
            } // public class SWITCH
        } // public class FOOTSTEP_ACT_TYPE

        public class FOOTSTEP_MATERIAL_TYPE
        {
            public static uint GROUP = 2413444692U;

            public class SWITCH
            {
                public static uint CONCRETE = 841620460U;
                public static uint GRASS = 4248645337U;
                public static uint METAL = 2473969246U;
                public static uint ROCK = 2144363834U;
                public static uint RUG = 712161697U;
                public static uint SAND = 803837735U;
                public static uint WETLAND = 65510648U;
                public static uint WOOD = 2058049674U;
            } // public class SWITCH
        } // public class FOOTSTEP_MATERIAL_TYPE

        public class GUN_BULLET_TYPE
        {
            public static uint GROUP = 1650757475U;

            public class SWITCH
            {
                public static uint GUN_BULLET_TYPE_PISTOL = 3085918499U;
                public static uint GUN_BULLET_TYPE_RIFLE = 1785440132U;
                public static uint GUN_BULLET_TYPE_SHOTGUN = 1760602584U;
                public static uint GUN_BULLET_TYPE_SNIPER = 508351779U;
                public static uint GUN_BULLET_TYPE_SUBMACHINE = 160710359U;
            } // public class SWITCH
        } // public class GUN_BULLET_TYPE

        public class GUN_SHOT_MODE_TYPE
        {
            public static uint GROUP = 2973703745U;

            public class SWITCH
            {
                public static uint GUN_SHOT_MODE_TYPE_CONTINUE = 3388677639U;
                public static uint GUN_SHOT_MODE_TYPE_SINGLE = 740487104U;
                public static uint GUN_SHOT_MODE_TYPE_TRIPLE = 337121988U;
            } // public class SWITCH
        } // public class GUN_SHOT_MODE_TYPE

    } // public class SWITCHES

    public class GAME_PARAMETERS
    {
        public static uint MODEL_CAR_ENGINE = 1909548668U;
    } // public class GAME_PARAMETERS

    public class BANKS
    {
        public static uint INIT = 1355168291U;
        public static uint TEST = 3157003241U;
    } // public class BANKS

    public class BUSSES
    {
        public static uint BUS_ACT = 4077520580U;
        public static uint BUS_AMBIENCE = 3501367136U;
        public static uint BUS_CAR = 17837210U;
        public static uint BUS_GUN = 3843619678U;
        public static uint MASTER_AUDIO_BUS = 3803692087U;
        public static uint MASTERBUS = 835198467U;
    } // public class BUSSES

    public class AUX_BUSSES
    {
        public static uint UNDERWATER_FX = 2445731875U;
    } // public class AUX_BUSSES

    public class AUDIO_DEVICES
    {
        public static uint NO_OUTPUT = 2317455096U;
        public static uint SYSTEM = 3859886410U;
    } // public class AUDIO_DEVICES

}// namespace AK

