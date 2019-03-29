using System;
using System.Collections.Generic;
using App.Protobuf;
using App.Shared;
using Free.framework;
using UnityEngine;
using Core.Utils.System46;
using Assets.App.Client.GameModules.Ui;
using Utils.Singleton;

namespace Assets.Sources.Free.UI
{
    public class FreeUiManager : DisposableSingleton<FreeUiManager>
    {
        private readonly IDictionary<string, SimpleFreeUI> _uis;

        private Contexts _contexts;

        private MyDictionary<string, SimpleProto> uiCache;

        public FreeUiManager()
        {
            _uis = new Dictionary<string, SimpleFreeUI>();
            uiCache = new MyDictionary<string, SimpleProto>();
        }

        public void CacheUI(string key, SimpleProto effect)
        {
            uiCache[key] = effect;
        }

        public SimpleProto GetUIData(string key)
        {
            if (uiCache.ContainsKey(key))
            {
                return uiCache[key];
            }
            else
            {
                return null;
            }
        }

        public SimpleProto FreeEvent
        {
            get
            {
                foreach (var ui in _uis.Values)
                {
                    var eventKey = ui.GetFreeUiEvent();

                    if (eventKey != null && ui.Visible)
                    {
                        var sp = eventKey.GetData();
                        ui.ResetEvent();
                        return sp;
                    }
                }
                return null;
            }
        }

        public bool ShowMouse
        {
            get
            {
                foreach (var ui in _uis.Values)
                    if (ui.HasEvent && ui.Visible)
                        return true;

                return false;
            }

        }

        public SimpleFreeUI GetSmallMap()
        {
            return GetUi("小地图UI");
        }

        public SimpleFreeUI GetAllKillInfo()
        {
            return GetUi("对战战绩UI");
        }

        public SimpleFreeUI GetUi(string key)
        {
            SimpleFreeUI ret = null;
            _uis.TryGetValue(key, out ret);
            return ret;
        }

        public void RemoveUi(SimpleFreeUI ui)
        {
            if (ui != null)
                _uis.Remove(ui.Key);
        }

        public void RemoveAllUi()
        {
            foreach (SimpleFreeUI ui in _uis.Values)
            {
                ui.OnDestroy();
            }
            _uis.Clear();
        }


        public void AddUi(SimpleFreeUI ui)
        {
            SimpleFreeUI old;
            _uis.TryGetValue(ui.Key, out old);
            if (old != null)
                old.Visible = false;
            _uis.Add(ui.Key, ui);
        }

        public IDictionary<string, SimpleFreeUI> FreeUIs
        {
            get { return _uis; }
        }

        public Contexts Contexts1
        {
            get { return _contexts; }
            set { _contexts = value; }
        }

        public void Update()
        {
            try
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    var ui = GetAllKillInfo();
                    if (ui != null)
                        ui.Visible = true;
                }

                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    var ui = GetSmallMap();
                    if (ui != null)
                        ui.Visible = true;
                }

                if (Input.GetKeyUp(KeyCode.Tab))
                {
                    var ui = GetAllKillInfo();
                    if (ui != null)
                        ui.Visible = false;
                }

                if (Input.GetKeyUp(KeyCode.BackQuote))
                {
                    var ui = GetSmallMap();
                    if (ui != null)
                        ui.Visible = false;
                }

                sendFreeMessage();
            }
            catch (Exception e1)
            {
                Debug.Log(e1.StackTrace);
            }
        }

        protected override void OnDispose()
        {
            RemoveAllUi();
        }

        private void sendFreeMessage()
        {
            var eventKey = this.FreeEvent;
            if(eventKey != null){
                Contexts1.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent,eventKey);
            }

//            var localKey = GetMouseCmdKey(userLocalCmd);
//            if(localKey != null){
//                gameNetworkStack.sendFreeEvent(localKey);
//            }
        }
    }
}
