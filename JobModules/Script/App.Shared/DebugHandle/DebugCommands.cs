namespace App.Shared.DebugHandle
{
    public class DebugCommand
    {
        public string Command { get; set; }
        public string[] Args { get; set; }

        public DebugCommand(string command)
        {
            Command = command;
            Args = new string[0];
        }
        public DebugCommand(string command, string[] args)
        {
            Command = command;
            Args = args;
        }
    }

    public class DebugCommands
    {
        public const string SetFrameRate = "SFR";
        public const string GetUnityQuality = "GetUnityQuality";
        public const string GetQualityList = "GetQualityList";
        public const string SetQuality = "SetQuality";

        /// <summary>
        ///  画质GM命令
        /// </summary>
        public const string GetQuality = "GetQuality";
        public const string Quality = "Quality";
        public const string VideoSetting = "videosetting";


        public const string VSync = "Vsync";

        public const string ShowDrawHitBoxOnBullet = "ShowDrawHitBoxOnBullet";
        public const string HideDrawHitBoxOnBullet = "HideDrawHitBoxOnBullet";
        public const string ShowDrawHitBoxOnFrame = "ShowDrawHitBoxOnFrame";
        public const string HideDrawHitBoxOnFrame = "HideDrawHitBoxOnFrame";
        public const string EnableDrawBullet = "ShowBullet";
        public const string DisableDrawBullet = "HideBullet";
        public const string KillMe = "Kill";
        public const string DyingMe = "Dying";
        public const string ChangeHp = "hp";
        public const string TestFrame = "testFrame";
        public const string ChangeBag = "sbg";
        public const string ShowVehicleDebugInfo = "SVD";
        public const string SetDynamicPrediction = "SDP";
        public const string SetVehicleHp = "SVH";
        public const string SetVehicleFuel = "SVF";
        public const string SetVehicleInput = "SVI";
        public const string DragCar = "DCar";
        public const string ShowExplosionRange = "ShowExplosionRange";
        public const string HideExplosionRange = "HideExplosionRange";
        public const string EnableVehicleCollisionDamage = "EVC";
        public const string EnableVehicleCollisionDebug = "EVD";
        public const string SetVehicleDynamicPrediction = "SVP";
        public const string ShowClientVehicle = "ClientVehicle";
        public const string ShowServerVehicle = "ServerVehicle";
        public const string ResetVehicle = "ResetVehicle";
        public const string EnableVehicleCull = "VCull";
        public const string SetVehicleActiveUpdateRate = "ActiveUpdateRate";
        public const string CreateVehicle = "CVH";

        public const string SetCurBullet = "sb";
        public const string SetReservedBullet = "srb";
        public const string Shoot = "shoot";

        public const string SetWeapon = "sw";
        public const string SetAudio = "audio";
        public const string AudioEmitter = "ae";
        public const string AudioBgm = "aeb";
        public const string SetGrenade = "sg";
        public const string DropWeapon = "dw";
        public const string TestWeaponAssemble = "twa";
        public const string TestCmd = "tc";
        public const string SetAttachment = "sa";
        public const string SetWeaponAttachment = "swam";
        public const string ClearAttachment = "ca";
        public const string SwitchAttachment = "swa";
        public const string ShowAvaliablePartType = "showpt";
        public const string ReloadConfig = "rc";

        public const string SetEquip = "se";

        public const string ShowConfig = "showconfig";
        public const string CreateSceneObject = "sobj";
        public const string ClearSceneObject = "clrsobj";
        public const string ShowArtTools = "art";
        public const string ListDoorEntity = "listDoorEntity";

        public const string ShowTerrainTrace = "stt";
        public const string PrintEntity = "printEntity";
        public const string ListEntity = "listEntity";
        public const string CountEntity = "countEntity";

        public const string ShowAniInfo = "ShowAniInfo";
        public const string DebugAnimation = "DebugAnimation";
        public const string ShowBox = "ShowBox";
        public const string ShowGround = "ShowGround";
        public const string Speed = "speed";
        public const string SlideOff = "EnableSlide";


        public const string ClientMove = "m";
        public const string DebugTime = "dt";

        public const string QualitySetting = "qs";
        public const string Dump = "dump";
        public const string Culling = "culling";
        public const string Terrain = "terrain";
        public const string SetFps = "setfps";
        public const string TreeDistance = "tree";
        public const string LodBias = "lod";
        public const string TestMap = "tt";

        public const string UIList = "uils";
        public const string HideUI = "hideui";
        public const string ShowUI = "showui";

        public const string EnableProfiler = "enprof";

        public const string HeatMapPause = "heatmappause";
        public const string HeatMapRestart = "heatmaprestart";
        public const string HeatMapStopAndExit = "heatmapstop&exit";
        public const string HeatMapStop = "heatmapstop";
        public const string HeatMapPoints = "heatmappoints";
        public const string HeatMapScenes = "heatmapscenes";
        public const string EnableRecordProfiler = "enrp";

        public const string EnableFlagImmutability = "efi";

        public const string SetMaxQuality = "SetMaxHighQuality";
        public const string GetMaxQuality = "GetMaxHighQuality";

        public const string EnableMinRendererSet = "enableminrenderset";
        public const string DisableMinRendererSet = "disableminrenderset";

        public const string WaterReflectUseCam = "waterreflectusecam";
        public static string Event = "event";
        public static string FilterPlayer = "filterPlayer";

        public const string ForbidSystem = "stopsys";
        public const string PermitSystem = "startsys";
        public const string ShowSystem = "sys";

        public const string ListTriggerObj = "mapObj";

        public const string GetVisibleRenders = "getvisiblerenders";
        public static string CustomProfile = "customProfile";

        public const string WoodToggle = "woodtoggle";
        public const string WoodResetTrees = "woodresettrees";
        public const string WoodResetDetails = "woodresetdetails";
        public const string WoodDecreaseTrees = "wooddecreasetrees";
        public const string WoodDecreaseDetails = "wooddecreasedetails";
        public const string WoodGetTrees = "woodgettrees";
        public const string WoodPostFxProfile = "woodpostfxprofile";

        public const string GetQualitySettings = "getqualitysettings";

        public const string ShowVideoSetting = "video";
        public const string ShiftWorld = "shift";
        public const string DetailToggle = "detailtoggle";
        public const string TreeToggle = "treetoggle";
        public const string TerrainToggle = "terraintoggle";
        public const string MeshRendererToggle = "meshrenderertoggle";
        public const string WaterToggle = "watertoggle";
        public const string PostFxToggle = "postfxtoggle";
        public const string GetProfilerTime = "getprofilertime";
        public const string GetTrees = "gettrees";

        public const string SetShadow = "setshadow";

        public const string EnableAnimator = "enableanimator";
        public const string DisableAnimator = "disableanimator";
        public const string Monster = "monster";
        public const string ShowEvent = "showevent";

        public const string OnlyDirLight = "onlydirlight";
        public const string PostFxOpt = "postfxopt";

        public const string Gpui = "gpui";

        public const string Shadowmap = "Shadowmap";
        public const string Light = "Light";
        public const string Road = "Road";

        public const string Prop = "Prop";

        public const string PostProcess = "PostProcess";

        public const string POC = "poc";

        public const string TerrainDist = "terraindist";

        public const string GrassDraw = "grassdraw";

        public const string ZPrepass = "zprepass";
        public const string ZPrepassCull = "zprepasscull";
        public const string HZBCull = "hzbcull";

        public const string ProfileGPUFast = "profilegpufast";

        public const string Revive = "revive";
        public const string PosShow = "posshow";
        public const string PosEdit = "posEdit";

        public const string ShowCamera = "showcamera";

        public const string IndoorLightCull = "indoorlightcull";
        public const string ShowImportantObject = "showimportantobject";

        public const string LoadResource = "loadres";
        public const string God = "god";

        public const string GcThreshold = "gcthreshold";

        public const string GetMaxQueuedFrames = "getmaxqueuedframes";
        public const string SetMaxQueuedFrames = "setmaxqueuedframes";
    }

    public interface IDebugCommandHandler
    {
        string OnDebugMessage(DebugCommand message);
    }
}
