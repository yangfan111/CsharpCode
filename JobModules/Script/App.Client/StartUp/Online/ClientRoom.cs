using App.Client.GameModules.GamePlay.Free;
using App.Client.GameModules.Ui;
using App.Client.Scripts;
using App.Client.SessionStates;
using App.Protobuf;
using App.Shared;
using App.Shared.Client;
using App.Shared.DebugHandle;
using Assets.App.Client.GameModules.GamePlay.Free;
using Assets.Sources.Free.UI;
using Core.Components;
using Core.EntityComponent;
using Core.GameModule.Step;
using Core.GameModule.System;
using Core.MyProfiler;
using Core.Network;
using Core.SessionState;
using Core.Utils;
using Entitas;
using System;
using UnityEngine;
using Core.UpdateLatest;
using Utils.Singleton;

namespace App.Client.Console
{
    public class ClientRoom : IClientRoom, IDebugCommandHandler, IEcsDebugHelper
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientRoom));


        private Contexts _contexts;
        private SessionStateMachine _clientSessionStateMachine;


       
        private readonly IDebugCommandHandler _commandHandler;

     

       

        public Contexts Contexts
        {
            get { return _contexts; }
        }


        private bool _isDisposed = false;

        public ClientRoom(Contexts contexts)
        {
            _logger.InfoFormat("Platform Endianness is little = {0}", BitConverter.IsLittleEndian);

            _contexts = contexts;

            if (SharedConfig.InSamplingMode || SharedConfig.InLegacySampleingMode)
                _clientSessionStateMachine = new ClientProfileSessionStateMachine(_contexts);
            else
                _clientSessionStateMachine = new ClientSessionStateMachine(_contexts);
            SingletonManager.Get<MyProfilerManager>().Contexts = _contexts;
            
            _commandHandler = new DebugCommandHandler(_clientSessionStateMachine, _contexts);
        }


        public string LoginToken
        {
            set { _contexts.session.clientSessionObjects.LoginToken = value; }
        }


      

        public void Update()
        {
            try
            {
                if (_isDisposed)
                    return;
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.Room);
                var sessionObjects = _contexts.session.clientSessionObjects;

                    sessionObjects.MessageDispatcher.DriveDispatch();
                StepExecuteManager.Instance.Update();
                _clientSessionStateMachine.Update();
            }

            catch (Exception e)
            {
                _logger.ErrorFormat("{0}",e);
#if UNITY_EDITOR
              
                Debug.LogError("e.Message : " + e.Message + " unknown error : " + e.StackTrace);
#endif
            }

            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.Room);
            }
        }

        public void LateUpdate()
        {
            _clientSessionStateMachine.LateUpdate();
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            try
            {
                _clientSessionStateMachine.Dispose();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("_clientSessionStateMachine.Disposeerror:{0}", ex);
            }

            try
            {
                FreePrefabLoader.Destroy();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("FreePrefabLoader.Destroy(){0}", ex);
            }

            try
            {
                _contexts.session.commonSession.Dispose();
                _contexts.session.clientSessionObjects.Dispose();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(" _contexts.session {0}", ex);
            }

            var _sessionObjects = _contexts.session.commonSession;
            try
            {
                if (_contexts.session.clientSessionObjects.NetworkChannel != null)
                {
                    _contexts.session.clientSessionObjects.NetworkChannel.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(" _contexts.NetworkChannel {0}", ex);
            }


            RecycleEntitys(_contexts);

            try
            {
                _clientSessionStateMachine.ShutDown();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(" _clientSessionStateMachine.ShutDown {0}", ex);
            }

            try
            {
                _contexts.Reset();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("contexts.Reset error:{0}", ex.Message);
            }

            try
            {
                UiModule.DestroyAll();
                FreeUILoader.Destroy();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("contexts.Reset error:{0}", ex.Message);
            }
        }

        private void RecycleEntitys(Contexts context)
        {
            var _sessionObjects = context.session.commonSession;
            foreach (var info in _sessionObjects.GameContexts.AllContexts)
            {
                foreach (IGameEntity entity in info.GetEntities())
                {
                    foreach (var comp in entity.ComponentList)
                    {
                        if (comp is IAssetComponent)
                        {
                            try
                            {
                                ((IAssetComponent) comp).Recycle(_sessionObjects.AssetManager);
                            }
                            catch (Exception e)
                            {
                                _logger.ErrorFormat("RecycleEntitys {0}", e);
                            }
                        }
                    }

                    if (_sessionObjects.AssetManager != null)
                        _sessionObjects.AssetManager.LoadCancel(entity.RealEntity);
                    entity.Destroy();
                }
            }

            foreach (Entity entity in context.ui.GetEntities())
            {
                DestroyEntity(_sessionObjects, entity);
            }


            foreach (Entity entity in context.sceneObject.GetEntities())
            {
                DestroyEntity(_sessionObjects, entity);
            }
        }

        private void DestroyEntity(ICommonSessionObjects sessionObjects, Entity entity)
        {
            foreach (var comp in entity.GetComponents())
            {
                if (comp is IAssetComponent)
                {
                    try
                    {
                        ((IAssetComponent) comp).Recycle(sessionObjects.AssetManager);
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFormat("RecycleEntitys {0}", e);
                    }
                }
            }

            if (sessionObjects.AssetManager != null)
                sessionObjects.AssetManager.LoadCancel(entity);
            entity.Destroy();
        }


      

        public void SendGameOverMessage()
        {
            try
            {
                var selfPlayer = _contexts.player.flagSelfEntity;
                if (selfPlayer.hasNetwork)
                {
                    var channel = selfPlayer.network.NetworkChannel;
                    if (channel != null)
                    {
                        var msg = GameOverMesssage.Allocate();
                        channel.SendReliable((int) EClient2ServerMessage.GameOver, msg);
                        msg.ReleaseReference();

                        _logger.InfoFormat("Send GameOver Message to Server");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Fail to Send GameOver Message to Server", e);
            }
        }

        public void SendDebugScriptInfo(string info)
        {
            try
            {
                var selfPlayer = _contexts.player.flagSelfEntity;
                if (selfPlayer.hasNetwork)
                {
                    var channel = selfPlayer.network.NetworkChannel;
                    if (channel != null)
                    {
                        var msg = DebugScriptInfo.Allocate();
                        msg.Info = info;
                        channel.SendReliable((int) EClient2ServerMessage.DebugScriptInfo, msg);
                        msg.ReleaseReference();

                        _logger.DebugFormat("Send Debug Script Message {0}", info);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Fail to Send DebugScriptInfo", e);
            }
        }

        public void OnDrawGizmos()
        {
            _clientSessionStateMachine.OnDrawGizmos();
        }

        public void OnGUI()
        {
            _clientSessionStateMachine.OnGUI();
        }


        public SessionStateMachine GetSessionStateMachine()
        {
            return _clientSessionStateMachine;
        }

        public string OnDebugMessage(DebugCommand message)
        {
            return _commandHandler.OnDebugMessage(message);
        }
    }
}