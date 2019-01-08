using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using App.Client;
using App.Client.Console;
using App.Client.Scripts;
using App.Shared;
using App.Shared.Client;
using App.Shared.Components.Player;
using App.Shared.Configuration;
using Assets.Scripts.Utility.Pool;
using Assets.Sources;
using Core;
using Core.GameModule.System;
using Core.Network;
using Core.Utils;
using App.Shared.Scripts;
using Utils.AssetManager;
using Core.Prediction.UserPrediction.Cmd;
using App.Client.Utility;
using App.Shared.Components.Serializer;
using App.Shared.Util;
using Common.LogAppender;
using Core.MyProfiler;
using Core.ObjectPool;
using UnityEngine.SceneManagement;
using Version = Core.Utils.Version;
using App.Shared.SceneManagement;
using Assets.Sources.Free.UI;
using UnityEngine.Profiling;
using Utils.Singleton;

public class ClientGameController : MonoBehaviour, ICoRoutineManager
{
    public bool Offline;
    public bool IsRobot;
    public bool DisableMutilThread;
    public bool DisableDoor;
    public string[] BootArg;


    private ClientRoom _clientRoom;

    private LoginClient _loginClient;


    private bool _isInit = false;

    private bool _isDestroy = false;

    private IAssetPool _assetLoader;

//必须需要，保证ObjectAllocators的初始化在最前面
    private int ObjectAllocatorsDummy = ObjectAllocators.DUMMY;

    private LoggerAdapter _logger;
    //From hall
    //[RoomId,Ip,Port,Token]
    public void StartGame(ArrayList args)
    {
        if (args.Count < 4)
        {
            _logger.ErrorFormat("args length error {0}", args.Count);
            return;
        }

        long roomId = (long) args[0];
        string ip = (string) args[1];
        int port = (int) args[2];
        string token = (string) args[3];

        _logger.InfoFormat("StartGame...roomId:{0},ip:{1},port:{2},token:{3}", roomId, ip, port, token);

        SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.BattleServer.IP = ip;
        SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.BattleServer.TcpPort = port;
        TestUtility.TestToken = token;
       
        //start
        DoStart();
       
    }

    private GameObject GetHallController()
    {
        var hallController = GameObject.Find("HallGameController");
        return hallController;
    }


    IEnumerator Start()
    {
        Log4ConfigManager.InitLog4net();
        Application.logMessageReceived += UnityLogCapturer.ApplicationOnLogMessageReceived;
#if !UNITY_EDITOR
        BootArg = new string[0];
#endif
        if (null == BootArg)
        {
            BootArg = new string[0];
        }

        var bootCmd =
            CmdParser.ParseCommandLine(BootArg.Length != 0 ? BootArg : System.Environment.GetCommandLineArgs());
       
        QualitySettings.shadowDistance = 120;
        PlayerPrefs.DeleteKey("unity_server_mode");
        
        _isDestroy = false;
        var hallController = GetHallController();
        if (hallController == null)
        {
#if UNITY_EDITOR
            SharedConfig.IsOffline = Offline;
            SharedConfig.IsRobot = IsRobot;
            SharedConfig.MutilThread = !DisableMutilThread;
            SharedConfig.DisableDoor = DisableDoor;
#endif
            SharedConfig.InitConfigBootArgs(bootCmd);
        }
        else
        {
            SharedConfig.IsOffline = false;
        }

        SharedConfig.IsServer = false;
        // 删除是否服务器数据
       

        _logger = new LoggerAdapter(typeof(ClientGameController));

        RegisterClientCallback();

        InitPhysicsSimulation();

        InitCamera();
        ComponentAllocateManager.Instance.Init();

        if (hallController == null)
        {
            _assetLoader = new AssetPool(this);
            yield return StartCoroutine(_assetLoader.Init(SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.Resource,
                SharedConfig.IsServer));
            DoStart();
        }
        else
        {
            _assetLoader = AssetPool.Instance;
            HallUtility.GetGameClientInfo();
            _logger.InfoFormat("GetGameClientInfo...");
        }

        MyHttpServer.Start(SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.HttpPort, _clientRoom);
#if UNITY_SOURCE_MODIFIED && !UNITY_EDITOR
        if (!SharedConfig.InSamplingMode && !SharedConfig.InLegacySampleingMode)
        {
            UnityProfiler.EnableProfiler(true);
        }
#endif
    }

    private void DoStart()
    {
        DefaultGo.SetStage(GameRunningStage.BattleClient, false);

        _logger.InfoFormat("DoStart");
        gameObject.AddComponent<VersionDisplay>();
        IUserCmdGenerator userCmd;
        if (!IsRobot)
        {
            userCmd = new UnityUserCmdGenerator();
        }
        else
        {
            userCmd = new RobotUserCmdGenerator(new DummyRobotUserCmdProvider());
        }

        gameObject.AddComponent<Luminosity.IO.InputManager>();
        var consoleCommands = gameObject.AddComponent<DeveloperConsoleCommands>();
        var pool = gameObject.AddComponent<UnityGameObjectPool>();
        ClientContextInitilizer intializer =
            new ClientContextInitilizer(userCmd, pool, this, _assetLoader, TestUtility.TestToken);


        _clientRoom = new ClientRoom(intializer);
        _clientRoom.AddCommand += (consoleCommands.RegisterFreeCommand);

        consoleCommands._handler = _clientRoom;
        consoleCommands._EcsDebugHelper = _clientRoom;
        consoleCommands._contexts = _clientRoom.Contexts;
        consoleCommands.RegisterOpenCallback(BlockAllInput);
        if (!SharedConfig.IsOffline)
        {
            _loginClient = new LoginClient(SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.BattleServer.IP,
                new NetworkPortInfo(SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.BattleServer.TcpPort,
                    SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.BattleServer.UdpPort),
                _clientRoom);
           
        }
        _isInit = true;
        GlobalProperties.Instance.Properties["serverIp"] = SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.BattleServer.IP;
        GlobalProperties.Instance.Properties["tcpPort"] = SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.BattleServer.TcpPort;
        GlobalProperties.Instance.Properties["token"] = TestUtility.TestToken;
        GlobalProperties.Instance.Properties["CS"] = "Client";
        GlobalProperties.Instance.Properties["mutilThread"] = SharedConfig.MutilThread;
        GlobalProperties.Instance.Properties["version"] = Version.Instance.LocalVersion;
        GlobalProperties.Instance.Properties["asset"] = Version.Instance.LocalAsset;
        GlobalProperties.Instance.Properties["isOffline"] = SharedConfig.IsOffline;

    }

    UserInputManager.Lib.IKeyReceiver _keyreceiver =
        new UserInputManager.Lib.KeyReceiver(UserInputManager.Lib.Layer.System, UserInputManager.Lib.BlockType.All);

    void BlockAllInput(bool block)
    {
        if (block)
        {
            _clientRoom.Contexts.userInput.userInputManager.Instance.RegisterKeyReceiver(_keyreceiver);
        }
        else
        {
            _clientRoom.Contexts.userInput.userInputManager.Instance.UnregisterKeyReceiver(_keyreceiver);
        }
    }

    void InitPhysicsSimulation()
    {
        Physics.defaultContactOffset = 0.001f;
        PhysicsUtility.SetAutoSimulation(false);
        _logger.InfoFormat("InitPhysicsSimulation");
    }

    void OnGUI()
    {
        if (_clientRoom != null)
        {
            _clientRoom.OnGUI();
        }
    }


    void InitCamera()
    {
        var cameras = Camera.allCameras;
        //keep only the main camera enabled
        foreach (var cmr in cameras)
        {
            if (cmr.transform.parent != null)
            {
                cmr.enabled = false;
            }
        }
    }

    private int _seq;

    void Update()
    {
        _seq++;
        if (Debug.developerConsoleVisible)
            Debug.developerConsoleVisible = false;

        if(ClientDebugScript.Enabled)
            ClientDebugScript.DoUpdate(this);

        try
        {
            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.GameController);
            if (_isDestroy)
                return;

            if (_clientRoom != null)
            {
                _clientRoom.Update();
            }

            if (_loginClient != null)
            {
                _loginClient.Update();
                _loginClient.FlowTick(Time.time);
            }
        }
        catch (Exception e)
        {
            _logger.ErrorFormat("unknown error {0}", e);
        }
        finally
        {
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.GameController);
           // SingletonManager.Get<MyProfilerManager>().RecordToLog(_seq);
        }
    }

    void LateUpdate()
    {
        if (_clientRoom != null)
        {
            _clientRoom.LateUpdate();
        }
    }

    void OnDrawGizmos()
    {
        if (_clientRoom != null)
        {
            _clientRoom.OnDrawGizmos();
        }
    }


    public void OnDestroy()
    {
        Debug.Log("OnDestroy");
        _logger.InfoFormat("Start Destroy...");
        _isDestroy = true;
        MyHttpServer.Stop();
        if (_loginClient != null)
            _loginClient.Dispose();

        _logger.InfoFormat("Login Client Disposed.");
        HallUtility.ClearAciton();

        _clientRoom.Dispose();
        DefaultGo.Clear(GameRunningStage.BattleClient);
        SingletonManager.Dispose();

#if UNITY_SOURCE_MODIFIED && !UNITY_EDITOR
            UnityProfiler.DisableProfiler();
#endif
        _logger.InfoFormat("Client Destroy Completed.");
    }

    private void DisposeClient()
    {
        string sceneName = "ClientScene";
        ArrayList args = new ArrayList();

        if (gameObject != null && this.gameObject.scene != null)
            sceneName = gameObject.scene.name;
        args.Add(sceneName);

        args.AddRange(SingletonManager.Get<MapConfigManager>().GetLoadedMapNames());

        _logger.InfoFormat("Hall OnGameOver ... unload count:{0}", args.Count);
        foreach (var arg in args)
        {
            _logger.InfoFormat("unload ... name:{0}", arg);
            try
            {
                SceneManager.UnloadSceneAsync((string) arg);
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Hall OnGameOver Error ... {0}", e.Message);
            }
        }
    }

    public void OnGameOver()
    {
        HallUtility.GameOver();
        _logger.InfoFormat("OnGameOver...");
    }

    public void SendDebugScriptInfo(string info)
    {
        if (_clientRoom != null)
        {
            _clientRoom.SendDebugScriptInfo(info);
        }
    }

    public void SendChatMessageToGame(object broadcastMessageData)
    {
        var action = SingletonManager.Get<FreeUiManager>().Contexts1.ui.chat.AddChatMessageDataAction;
        if (action != null)
        {
            action.Invoke(broadcastMessageData);
        }
    }

    private void SendCheckPersonalOnlineStatusToGame(object list)
    {
        var action = SingletonManager.Get<FreeUiManager>().Contexts1.ui.chat.GetPersonalOnlineStatusCallback;
        if (action != null)
        {
            action.Invoke(list);
        }
    }

    void RegisterClientCallback()
    {
        HallUtility.RegisterCallback(HallUtility.Method_StartGame, (data) => StartGame(data as ArrayList));
        HallUtility.RegisterCallback(HallUtility.Method_DisposeClient, (data) => DisposeClient());
        HallUtility.RegisterCallback(HallUtility.Method_SendChatMessageToGamne,
            SendChatMessageToGame);
        HallUtility.RegisterCallback(HallUtility.Method_SendCheckPersonalOnlineStatusToGame,
            SendCheckPersonalOnlineStatusToGame);
    }

    public void CallByHall(ArrayList args)
    {
        HallUtility.MessageReceived(args);
    }

    public UnityEngine.Coroutine StartCoRoutine(IEnumerator enumerator)
    {
        return StartCoroutine(enumerator);
    }

    public void StopCoRoutine(IEnumerator enumerator)
    {
        StopCoroutine(enumerator);
    }

    void OnApplicationFocus(bool focusStatus)
    {
        SharedConfig.isFouces = focusStatus;
    }
}
