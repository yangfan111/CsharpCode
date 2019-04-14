using App.Client.Console.MessageHandler;
using App.Client.GameModules.GamePlay.Free.App;
using App.Client.GameModules.GamePlay.Free.Entity;
using App.Client.GameModules.GamePlay.Free.Game;
using App.Client.GameModules.GamePlay.Free.Player;
using App.Client.GameModules.GamePlay.Free.Scene;
using App.Client.GameModules.GamePlay.Free.UI;
using Assets.App.Client.GameModules.GamePlay.Free.App;
using Assets.Sources.Free.Effect;
using Assets.Sources.Free.Entity;
using Assets.Sources.Free.Scene;
using Assets.Sources.Free.UI;
using Core.IFactory;
using Core.Utils;
using Free.framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;

namespace Assets.Sources.Free
{
    public class SimpleMessageManager : AbstractClientMessageHandler<SimpleProto>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SimpleMessageManager));

        private static SimpleMessageManager _instance;

        private IList<ISimpleMesssageHandler> _handlers;

        public static SimpleMessageManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SimpleMessageManager();
                    //                    _instance.Init();
                }
                return _instance;
            }
        }

        public SimpleMessageManager()
        {
            _handlers = new List<ISimpleMesssageHandler>();
        }

        public void Init(ISoundEntityFactory soundEntityFactory)
        {
            if (_handlers == null)
                _handlers = new List<ISimpleMesssageHandler>();

            _handlers.Clear();

            _handlers.Add(new UiCreateMessageHandler());
            _handlers.Add(new UIShowMessageHandler());

            _handlers.Add(new UIDeleteMessageHandler());
            _handlers.Add(new EffectCreateHandler());
            _handlers.Add(new EffectShowHandler());
            _handlers.Add(new EffectUpdateHandler());
            _handlers.Add(new EffectDeleteHandler());
            _handlers.Add(new TipsMessageHandler());
            //            handlers.push(new ShakeMessageHandler());
            //            handlers.push(new PlaySoundHandler());
            //            handlers.push(new PlayerStateHandler());
            _handlers.Add(new SeeAllHandler());
            _handlers.Add(new AllowMouseKeysHandler());
            _handlers.Add(new FogHandler());
            _handlers.Add(new SkyBoxHandler());
            _handlers.Add(new SceneLightHandler());
            _handlers.Add(new ClosePvsHandler());
            _handlers.Add(new BloodSprayHandler());
            //            handlers.push(new FreeWeaponIniHandler());
            _handlers.Add(new UIRemoveAllHandler());
            _handlers.Add(new ChangeAvatarHandler());
            _handlers.Add(new PoisonCircleHandler());
            _handlers.Add(new UIDuplicateMessageHandler());
            _handlers.Add(new UIAddChildMessageHandler());
            _handlers.Add(new UIAddMarkHandler());
            _handlers.Add(new UIScoreInfoHandler());
            _handlers.Add(new TestFrameHandler());
            _handlers.Add(new UICountDownHandler());
            _handlers.Add(new UILockMouseHandler());
            _handlers.Add(new ShowCodeUIHandler());
            _handlers.Add(new PlayerAudioHandler(soundEntityFactory));
            _handlers.Add(new UIAirLineDataHandler());
            _handlers.Add(new GameOverHandler());
            _handlers.Add(new ShowSplitUIHandler());
            _handlers.Add(new ItemInfoHandler());
            _handlers.Add(new ClientSkillHandler());
            _handlers.Add(new GroupUIHandler());
            _handlers.Add(new PlayerMoveHandler());
			_handlers.Add(new PlayerCmdHandler());
            _handlers.Add(new PlayerAnimationHandler());
			_handlers.Add(new GroupClassicUIHandler());
            _handlers.Add(new BlastUIHandler());
            SingletonManager.Get<SimpleUIUpdater>().Add(new MinMapUpdater());
            SingletonManager.Get<SimpleUIUpdater>().Add(new DebugDataUpdater());
            _handlers.Add(new PlayerMoveSpeedHandler());
            _handlers.Add(new PlayerVisibilityHandler());
        }

        public override void DoHandle(int message, SimpleProto data)
        {
            _logger.DebugFormat("Recieve SimpleProto Key {0} \n{1}", data.Key, data);

            if(RenderSettings.ambientMode != UnityEngine.Rendering.AmbientMode.Flat)
            {
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            }
            
            for (var index = 0; index < _handlers.Count; index++)
            {
                var handler = _handlers[index];
                if (handler.CanHandle(data.Key))
                {
                    try
                    {
                        handler.Handle(data);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("handle " + message + " failed.\n" + data + "\n" + e.StackTrace);
                    }
                }
            }
        }
    }
}
