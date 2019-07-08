using System;
using System.Net.Sockets;
using System.Threading;
using App.Client.Utility;
using App.Protobuf;
using App.Shared;
using App.Shared.Util;
using Assets.App.Client.GameModules.Ui;
using Core.GameModule.Interface;
using Core.Network;
using Core.Utils;
using Entitas;
using Sharpen;
using UIComponent.UI.Manager.Alert;
using UnityEngine;
/*using Wenzil.Console;*/

namespace App.Client.ClientSystems
{
    public class PingSystem : IRenderSystem, IOnGuiSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PingSystem));
        private const float HeartBeatAckInterval = 2.0f;
        private const float NetWorkStatusCheckInterval = 5.0f;
        private const float ReconnectInterval = 3.0f;
        private Contexts _contexts;
       
        private float _next;
        GUIStyle _bb = new GUIStyle();
        private float LastReconnectCheckTime = 0.0f;
        private float LastReconnectTime = 0.0f;
        private bool ReconnectShow = false;


        public PingSystem(Contexts contexts)
        {
            _contexts = contexts;
          

            _bb.normal.background = null; //这是设置背景填充的
            _bb.normal.textColor = new Color(1.0f, 1f, 1.0f); //设置字体颜色的
            _bb.fontSize = 10; //当然，这是字体大小
        }

        private int _seq = 0;
        private float _last;
        //private int _channelId;
        double _lastTime;

        public void OnRender()
        {
            var time = Time.time;
            var delta = time - _last;
            _contexts.session.clientSessionObjects.FpsSatatus.Tick(time, delta);
            _last = time;
            if (time > _next)
            {
                _next = time + HeartBeatAckInterval;
                var channel = _contexts.session.clientSessionObjects.NetworkChannel;
                _seq++;
                if (!SharedConfig.IsOffline)
                {
                    TcpMsg(channel);
                    UdpMsg(channel);
                }

                var playerEntity = _contexts.player.flagSelfEntity;
                if (playerEntity != null && playerEntity.hasPingStatistics)
                {
                    playerEntity.pingStatistics.Ping =
                        (short) _contexts.session.clientSessionObjects.ServerFpsSatatus.UdpPing;
                    playerEntity.pingStatistics.Fps5 = (short) _contexts.session.clientSessionObjects.FpsSatatus.Fps5;
                }
            }
        }

        private void TcpMsg(INetworkChannel channel)
        {
            var ping = PingRequestMessage.Allocate();

            ping.Id = _seq;
            ping.Type = true;
            ping.Time = DateTime.UtcNow.ToMillisecondsSinceEpoch();
            ;
            channel.SendReliable((int) EClient2ServerMessage.Ping, ping);
        }

        private void UdpMsg(INetworkChannel channel)
        {
            var ping = PingRequestMessage.Allocate();
            ping.Id = _seq;
            ping.Type = false;
            ping.Time = DateTime.UtcNow.ToMillisecondsSinceEpoch();
            channel.SendRealTime((int) EClient2ServerMessage.Ping, ping);
            ping.ReleaseReference();
           
        }

        private bool _display = false;
        private bool _lastButtonStat = false;
        string[] _labels = new string[5];

        public void OnGUI()
        {
            //if (Input.GetKey(KeyCode.H))
            //{
            //    if (!_lastButtonStat)
            //        _display = !_display;
            //    _lastButtonStat = true;
            //}
            //else
            //{
            //    _lastButtonStat = false;
            //}
            //if (ConsoleSwitch.ReconnectGUI)
            //{
            //    DebugGUI();
            //}
            ShowNetWorkStatus();
            ReconnectCheck();

            //ShowFps();
        }

        private void DebugGUI()
        {
            if (GUILayout.Button("断开Tcp连接"))
            {
                GameController().SendMessage("CloseConnect", ProtocolType.Tcp);
            }
            if (GUILayout.Button("断开Udp连接"))
            {
                GameController().SendMessage("CloseConnect", ProtocolType.Udp);
            }
            if (GUILayout.Button("断开Tcp-Udp连接"))
            {
                GameController().SendMessage("CloseConnect", ProtocolType.Unspecified);
            }
        }

        private void ReconnectCheck()
        {
            float time = Time.time;

            if (ReconnectShow)
            {
                LastReconnectCheckTime = time;
                var channel = _contexts.session.clientSessionObjects.NetworkChannel;
                if (channel.IsConnected) {
                    UiCommon.AlertManager.ClearQueueAndClose();
                    ReconnectShow = false;
                    /*GameController().SendMessage("ChangeState");*/
                } else {
                    // 定时重连
                    /*ReConnect(time);*/
                }
                return;
            }

            if (time - LastReconnectCheckTime > NetWorkStatusCheckInterval) {

                var utTcp = (int)(DateTime.UtcNow.ToMillisecondsSinceEpoch() -
                _contexts.session.clientSessionObjects.ServerFpsSatatus.LastTcpPing) / 1000;
                var utUdp = (int)(DateTime.UtcNow.ToMillisecondsSinceEpoch() -
                _contexts.session.clientSessionObjects.ServerFpsSatatus.LastUdpPing) / 1000;


                if ((utTcp > NetWorkStatusCheckInterval ||
                    utUdp > NetWorkStatusCheckInterval ) &&
                    !ReconnectShow) {
                    UiCommon.AlertManager.AddDataToQueueAndShow(AlertWindowStyle.YES, "网络连接中断", /*Application.Quit*/YesCB, null, "确定", null,
                        0, null);
                    ReconnectShow = true;
                }

                LastReconnectCheckTime = time;
            }
        }

        private GameObject GameController()
        {
            var gameController = GameObject.Find("GameController");
            return gameController;
        }

        protected void ReConnect(float time)
        {
            if (time - LastReconnectTime > ReconnectInterval) {
                GameController().SendMessage("ReConnect");
                LastReconnectTime = time;
            }
        }

        private void YesCB()
        {
            ReconnectShow = false;
            HallUtility.GameOver();
        }

        // 退出至大厅
        private void NoCB()
        {
            ReconnectShow = false;
        }

        private void ShowFps()
        {
            if (_display)
            {
                _lastTime += Time.deltaTime;
                if (_lastTime > 1)
                {
                    _lastTime = 0;
                    var sessionObjects = _contexts.session.clientSessionObjects;

                    var tt = (int) (DateTime.UtcNow.ToMillisecondsSinceEpoch() -
                                    sessionObjects.ServerFpsSatatus.LastTcpPing) / 1000;

                    var ut = (int) (DateTime.UtcNow.ToMillisecondsSinceEpoch() -
                                    sessionObjects.ServerFpsSatatus.LastUdpPing) / 1000;

                    _labels[0] = string.Format("client :{0}", sessionObjects.FpsSatatus);
                    _labels[1] = string.Format("server :{0}", _contexts.session.clientSessionObjects.ServerFpsSatatus);
                    _labels[2] = string.Format("ping t:{0}, u:{1}, tt:{2}, ut:{3}",
                        sessionObjects.ServerFpsSatatus.TcpPing, sessionObjects.ServerFpsSatatus.UdpPing,
                        tt, ut);
                    if (!SharedConfig.IsOffline)
                    {
                        _labels[3] = string.Format("tcp:{0}",
                            _contexts.session.clientSessionObjects.NetworkChannel.TcpFlowStatus);

                        _labels[4] = string.Format("udp:{0}",
                            _contexts.session.clientSessionObjects.NetworkChannel.UdpFlowStatus);
                    }
                }
                var h = Screen.height - 100;
                var w = Screen.width - 250;
                GUI.Box(new Rect(w - 4, h - 4, 240, 60), "");
                //居中显示FPS
                for (int i = 0; i < _labels.Length; i++)
                {
                    if (_labels[i] != null)
                        GUI.Label(new Rect(w, h + i * 10, w + 200, h + 10 + i * 10), _labels[i], _bb);
                }
            }
        }

        private GUIStyle _connectedStyle;

        private GUIStyle _disconnectStyle;


        void ShowNetWorkStatus()
        {
            var ut = (int) (DateTime.UtcNow.ToMillisecondsSinceEpoch() -
                            _contexts.session.clientSessionObjects.ServerFpsSatatus.LastUdpPing) / 1000;

            if (ut < NetWorkStatusCheckInterval)
            {
                if (null == _connectedStyle)
                {
                    _connectedStyle = new GUIStyle()
                    {
                        normal = new GUIStyleState() {textColor = Color.green},
                        fontSize = 20,
                    };
                }

                GUI.Label(new Rect(150, 0, Screen.width * 0.5f, Screen.height * 0.1f), SharedConfig.IsOffline?"脱机":"在线", _connectedStyle);
            }
            else
            {
                if (null == _disconnectStyle)
                {
                    _disconnectStyle = new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            textColor = Color.red
                        },
                        fontSize = 20,
                    };
                }

                GUI.Label(new Rect(150, 0, Screen.width * 0.5f, Screen.height * 0.1f), "离线", _disconnectStyle);
            }
        }
    }
}