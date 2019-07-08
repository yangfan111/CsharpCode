
using App.Client.GameModules.GamePlay.Free.Auto.Prefab;
using App.Client.GameModules.GamePlay.Free.UI;
using App.Client.GameModules.Ui.UiAdapter;
using App.Protobuf;
using App.Shared.Components.Ui;
using Assets.Sources.Free.Utility;
using Core.Free;
using Core.Utils;
using Free.framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;

namespace Assets.Sources.Free.UI
{
    public class UIAddChildMessageHandler : ISimpleMesssageHandler
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(UIAddChildMessageHandler));

        private static List<SimpleProto> notDone;

        public static Dictionary<int, long> clearTime;

        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.AddChild || key == FreeMessageConstant.InventoyUI;
        }

        public static void HandleNoeDone()
        {
            if (notDone == null)
            {
                notDone = new List<SimpleProto>();
            }

            for (int i = notDone.Count - 1; i >= 0; i--)
            {
                SimpleProto data = notDone[i];
                if (data.Ks.Count > 0 && data.Ks[0] == 5)
                {
                    if (HandleVisible(data, false))
                    {
                        notDone.Remove(data);
                    }
                }
                else
                {
                    if (Handle(data, false))
                    {
                        notDone.Remove(data);
                    }
                }
            }
        }

        public void Handle(SimpleProto sp)
        {
            //if (clearTime == null)
            //{
            //    clearTime = new Dictionary<int, long>();

            //    notDone = new List<SimpleProto>();
            //}

            //if (simpleUI.Ks.Count > 0 && simpleUI.Ks[0] == 5)
            //{
            //    HandleVisible(simpleUI, true);
            //}
            //else
            //{
            //    Handle(simpleUI, true);
            //}

            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            var ui = contexts.ui.uI;

            if (sp.Bs[0])
            {
                ui.ChickenBagItemDataList = new List<IBaseChickenBagItemData>();
                for (int i = 0; i < sp.Ks[1]; i++)
                {
                    ChickenBagItemUiData data = new ChickenBagItemUiData();
                    data.cat = sp.Ins[i * 3];
                    data.id = sp.Ins[i * 3 + 1];
                    data.count = sp.Ins[i * 3 + 2];
                    ui.ChickenBagItemDataList.Add(data);
                    data.key = sp.Ss[i];
                }
            }

            ui.TotalBagWeight = sp.Ks[2];
            ui.CurBagWeight = sp.Ks[3];
        }

        private static GameObject GetGameObject(int key, string part)
        {
            return UnityUiUtility.FindUIObject("WeaponSlot" + key + "/VISIBLE_WeaponPartSlot" + part);
        }

        private static bool HandleVisible(SimpleProto data, bool addNotDone)
        {
            int weaponKey = data.Ins[0];
            bool show = data.Bs[0];
            string pargs = data.Ss[0];

            foreach (string part in pargs.Split(","))
            {
                GameObject obj = GetGameObject(weaponKey, part.Trim());
                if (obj != null)
                {
                    obj.SetActive(show);
                }
                else
                {
                    if (addNotDone)
                    {
                        notDone.Add(data);
                    }

                    return false;
                }
            }

            return true;
        }

        private static bool Handle(SimpleProto simpleUI, bool addNotDone)
        {
            if (simpleUI.Ss[0] == AutoSimpleBag.BagParentPath)
            {
                AutoSimpleBag.CurrentBag = simpleUI;
            }

            return true;
        }
    }
}
