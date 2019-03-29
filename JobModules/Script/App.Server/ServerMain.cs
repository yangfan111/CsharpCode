using System;
using System.Collections.Generic;
using System.Linq;
using App.Server.Scripts.Config;
using App.Shared;
using App.Shared.GameModules.Configuration;
using Core;
using Core.Network;
using Core.Room;
using Core.SessionState;
using Core.Utils;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Server.Scripts.Scripts
{
    
    public class SimpleSessionState : ISessionCondition
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SimpleSessionState));
        private HashSet<string> _conditions = new HashSet<string>();
        public void CreateExitCondition(string conditionId)
        {
            _logger.InfoFormat("{0} create exit condition {1}", GetType(), conditionId);
            _conditions.Add(conditionId);
        }

        public void FullfillExitCondition(string conditionId)
        {
            if (_conditions.Contains(conditionId))
            {
                _logger.InfoFormat("{0} fullfill exit condition {1}", GetType(), conditionId);
                _conditions.Remove(conditionId);
            }
        }

        public void CreateExitCondition(Type conditionId)
        {
            CreateExitCondition(conditionId.FullName);
        }

        public void FullfillExitCondition(Type conditionId)
        {
            FullfillExitCondition(conditionId.FullName);
        }

        private DateTime _lastPrintTime = DateTime.Now;
        private int _logInterval = 5000;
        public bool IsFullfilled
        {
            get
            {
                bool rc = _conditions.Count == 0;
                if (!rc && _logger.IsWarnEnabled)
                {
                    if ((DateTime.Now - _lastPrintTime).TotalMilliseconds > _logInterval)
                    {
                        _lastPrintTime = DateTime.Now;
                        _logger.ErrorFormat("remain contains {0}", string.Join(",", _conditions.ToArray()));
                        if (_logInterval < 60000)
                            _logInterval *= 2;

                    }
                }
                return rc;
            }
        }

        public void Dispose()
        {
        }
    }
    public class ServerMain
    {
        private readonly ICoRoutineManager _coRoutineManager;
        private readonly IUnityAssetManager _assetManager;
        private readonly IRoomListener _roomListener;
        private readonly  SimpleSessionState _initState = new SimpleSessionState();
        private readonly BaseConfigurationInitModule _baseConfigInitModule;
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerMain));
        private IRoomManager _roomManager;
        private LoginServer _loginServer;
        private HallServerNetwork _hallServerNetwork;
        private RoomEventDispatcher _dispatcher;
        private ServerRoomFactory _roomFactory;
        

        public ServerMain(ICoRoutineManager coRoutineManager, IUnityAssetManager assetManager,
            IRoomListener roomListener)
        {
            _coRoutineManager = coRoutineManager;
            _assetManager = assetManager;
            _roomListener = roomListener;
            _baseConfigInitModule = new BaseConfigurationInitModule(_initState, _assetManager);
        }

        public bool IsInit { get; set; }

        private void init()
        {
            _dispatcher = new RoomEventDispatcher();
            _roomFactory = new ServerRoomFactory(_dispatcher, _coRoutineManager, _assetManager);
            _roomManager = new SingleRoomManager(_roomFactory, _dispatcher, false, _roomListener);


            _loginServer = new LoginServer(_dispatcher, _roomManager);
            _loginServer.Start(new NetworkPortInfo(
                SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.BattleServer.TcpPort,
                SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.BattleServer.UdpPort));

            _hallServerNetwork = new HallServerNetwork(_dispatcher);
            _hallServerNetwork.StartServer(new NetworkPortInfo(
                SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.HallRoomServer.ListenPort, 0));

            MyHttpServer.Start(SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.HttpPort);
            if (SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.AllocationServer.Open == 1)
            {
                _hallServerNetwork.ClientConnect(new NetworkPortInfo(
                    SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.AllocationServer.ConnectPort, 0));
            }
            else
            {
                _logger.InfoFormat("Server Configuration: Do Not connect to Allocation Server!");
            }
           
            
        }

        public void Update()
        {
            if (IsInit)
            {
                RoomUpdate();
                HallServerUpdate();
                LoginServerUpdate();
            }
            else
            {
                if(!_initState.IsFullfilled)
                    _baseConfigInitModule.Execute();
                else
                {
                    init();
                    IsInit = true;
                }
                
            }
        }

        private void RoomUpdate()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.Room);
                _dispatcher.Update();
                _roomManager.Update();
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.Room);
            }
        }

        private void HallServerUpdate()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart("HallServerNetWork");
                _hallServerNetwork.Update();
                ServerStatusCollectUtil.HallRoomServerStatus = _hallServerNetwork.IsServerValid();
                ServerStatusCollectUtil.AllocationClientStatus = _hallServerNetwork.IsClientValid();
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd("HallServerNetWork");
            }
        }

        private void LoginServerUpdate()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart("LoginServer");
                _loginServer.Update();
                ServerStatusCollectUtil.LoginServerStatus = _loginServer.IsValid();
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd("LoginServer");
            }
        }

        public void LateUpdate()
        {
            if (_roomManager != null)
                _roomManager.LateUpdate();
        }

        public void OnDestroy()
        {
            if (_loginServer != null)
                _loginServer.Dispose();
            if (_hallServerNetwork != null)
                _hallServerNetwork.Dispose();
        }
    }
}