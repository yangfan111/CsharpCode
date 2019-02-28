using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Free;
using Assets.Sources.Free.Data;
using Assets.Sources.Free.Render;
using Assets.Sources.Free.UI;
using Assets.Sources.Free.Utility;
using Core.Free;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Free.framework;
using App.Shared;
using Assets.App.Client.GameModules.GamePlay.Free.Entitas;
using Assets.App.Client.GameModules.GamePlay.Free;
using Assets.Sources.Free.Auto;
using Core.AssetManager;
using App.Client.GameModules.GamePlay.Free.Auto.Prefab;
using App.Client.GameModules.Ui.Models.Common;
using UIComponent.UI.Manager;
using Utils.AssetManager;
using App.Client.GameModules.GamePlay.Free.App;
using Assets.App.Client.GameModules.Ui;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class FreePrefabComponent : FreeBaseComponent, IFreeComponent
    {

        public const int Click = 1;
        public const int ClickAndMove = 2;
        public const int Drop = 3;
        public const int ClickAndClone = 4;

        private string name;
        public HashSet<string> valueList;
        private string events;

        private string allEventKey;

        public GameObject currentObject;

        public GameObject parentObject;

        public object TipData;

        public long iniTime;

        public AssetInfo assetInfo;

        private const string PrefabAuto = "prefab";

        private string lastEvent;

        public FreePrefabComponent()
        {
            _uiObject = UnityUiUtility.CreateEmptyDisplayObject("Prefab");
            RectTransform rt = _uiObject.gameObject.GetComponent<RectTransform>();
            FreeLayoutConverter.FullScreen(rt);

            this.valueList = new HashSet<string>();
        }

        public override void Frame(IUIDataManager uiDataManager, int frameTime)
        {
            base.Frame(uiDataManager, frameTime);

            IAutoValue auto = GetAuto(PrefabAuto);
            if (auto != null)
            {
                auto.Frame(frameTime);
            }
        }

        public IFreeComponent Clone()
        {
            return new FreePrefabComponent();
        }

        public void Initial(params object[] ini)
        {
            InitialAuto(ini[1] as string);

            this.iniTime = DateTime.Now.Ticks;

            string[] settings = (ini[0] as string).Split(FreeMessageConstant.SpliterStyle);

            this.name = settings[0];
            this.valueList.Add(settings[1]);
            this.events = settings[2];

            int last = name.LastIndexOf("/");

            this.assetInfo = new AssetInfo(name.Substring(0, last),
                name.Substring(last + 1));

            FreePrefabLoader.Load(assetInfo.BundleName, assetInfo.AssetName,
                (prefab) =>
                {
                    if (prefab == null)
                    {
                        return;
                    }
                    this.currentObject = (GameObject)prefab;

                    ((GameObject)prefab).transform.SetParent(_uiObject.gameObject.transform, false);
                    RectTransform rt = ((GameObject)prefab).GetComponent<RectTransform>();
                    ((GameObject)prefab).SetActive(true);

                    try
                    {
                        foreach (string values in valueList)
                        {
                            SetPureValue(values);
                        }
                        valueList.Clear();
                        AddEvent(events);
                        updateEventKey();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("prefab component failed." + e.StackTrace);
                    }

                    if (parentObject != null)
                    {
                        if (UIAddChildMessageHandler.clearTime.ContainsKey(parentObject.GetInstanceID()))
                        {
                            long clearTime = UIAddChildMessageHandler.clearTime[parentObject.GetInstanceID()];
                            if (iniTime > clearTime)
                            {
                                ((GameObject)prefab).transform.SetParent(parentObject.transform, false);
                            }
                            else
                            {
                                GameObject.Destroy(currentObject);
                                Debug.LogWarning(this.name + "=" + this.valueList + " is invalid");
                            }
                        }
                        else
                        {
                            ((GameObject)prefab).transform.SetParent(parentObject.transform, false);
                        }
                    }

                    UIAddChildMessageHandler.HandleNoeDone();

                    rt.localScale = new Vector3(1, 1, 1);
                }, _uiObject.gameObject);

        }

        private void updateEventKey()
        {
            if (!string.IsNullOrEmpty(allEventKey))
            {
                RectTransform[] res = currentObject.GetComponentsInChildren<RectTransform>(true);
                foreach (RectTransform rec in res)
                {
                    GameObject obj = rec.gameObject;
                    ItemComponent item = obj.GetComponent<ItemComponent>();
                    if (item != null)
                    {
                        item.EventKey = allEventKey;
                    }
                }
            }
        }

        private void AddEvent(string v)
        {
            if (!string.IsNullOrEmpty(v))
            {
                string[] fieldEvents = v.Split(FreeMessageConstant.SpliterRecord);
                foreach (string fieldEvent in fieldEvents)
                {
                    string[] vs = fieldEvent.Split(FreeMessageConstant.SpilterField);
                    if (vs.Length == 3)
                    {
                        GameObject obj = UnityUiUtility.FindUIObject(currentObject, vs[0].Trim());
                        if (obj != null)
                        {
                            Graphic gra = obj.GetComponent<Graphic>();
                            if (gra != null)
                            {
                                if (gra is RawImage || gra is Image || gra is Text)
                                {
                                    ItemComponent item = obj.AddComponent<ItemComponent>();
                                    item.EventKey = vs[1].Trim();
                                    AddEvent(obj, int.Parse(vs[2].Trim()));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddEvent(GameObject obj, int type)
        {
            EventTrigger eventTrigger = obj.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = obj.AddComponent<EventTrigger>();
            }
            eventTrigger.triggers.Clear();

            AddEntry(eventTrigger, EventTriggerType.PointerEnter, OnPointerEnterDelegate);
            AddEntry(eventTrigger, EventTriggerType.PointerExit, OnPointerExitDelegate);

            switch (type)
            {
                case Click:
                    AddEntry(eventTrigger, EventTriggerType.PointerClick, OnPointerClickDelegate);
                    break;
                case ClickAndMove:
                    AddEntry(eventTrigger, EventTriggerType.PointerClick, OnPointerClickDelegate);
                    AddEntry(eventTrigger, EventTriggerType.BeginDrag, OnBeginDragDelegate);
                    AddEntry(eventTrigger, EventTriggerType.Drag, OnDragDelegate);
                    AddEntry(eventTrigger, EventTriggerType.EndDrag, OnEndDragDelegate);
                    break;
                case ClickAndClone:
                    AddEntry(eventTrigger, EventTriggerType.PointerClick, OnPointerClickDelegate);
                    AddEntry(eventTrigger, EventTriggerType.BeginDrag, OnBeginCloneDelegate);
                    AddEntry(eventTrigger, EventTriggerType.Drag, OnCloneDelegate);
                    AddEntry(eventTrigger, EventTriggerType.EndDrag, OnEndCloneDelegate);
                    break;
                case Drop:
                    //AddEntry(eventTrigger, EventTriggerType.EndDrag, OnEndDragDelegate);
                    break;
                default:
                    break;
            }


        }

        private void AddEntry(EventTrigger eventTrigger, EventTriggerType entryType, UnityAction<PointerEventData> call)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = entryType;
            entry.callback.AddListener((data) => { call((PointerEventData)data); });
            eventTrigger.triggers.Add(entry);
        }

        private GameObject dragObj;

        private void OnBeginCloneDelegate(PointerEventData eventData)
        {
            if (dragObj == null)
            {
                dragObj = new GameObject();
                RectTransform t = dragObj.AddComponent<RectTransform>();
                dragObj.AddComponent<ItemComponent>();
                dragObj.AddComponent<Image>();
            }

            ItemComponent item = dragObj.GetComponent<ItemComponent>();
            item.EventKey = eventData.pointerDrag.GetComponent<ItemComponent>().EventKey;
            Image pointerImg = eventData.pointerDrag.GetComponent<Image>();

            Texture2D tex = null;

            if (pointerImg != null)
            {
                tex = (Texture2D)pointerImg.mainTexture;
            }

            GameObject clone = UnityUiUtility.FindUIObject(this.currentObject, "IMG_WeaponIcon");
            if (clone != null && this.name == "bag/ItemBar")
            {
                tex = (Texture2D)clone.GetComponent<Image>().mainTexture;
            }
            Image img = dragObj.GetComponent<Image>();
            img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            img.raycastTarget = false;

            dragObj.transform.position = eventData.position;
            dragObj.transform.parent = UnityUiUtility.RootCanvas.transform;
        }

        private void OnCloneDelegate(PointerEventData eventData)
        {
            dragObj.transform.position = eventData.position;
        }

        private void OnPointerEnterDelegate(PointerEventData eventData)
        {
            string eventKey = string.Empty;

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                ItemComponent toItem = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemComponent>();
                if (toItem != null)
                {
                    eventKey = toItem.EventKey;
                }
            }

            if (TipUtil.HasTip(eventKey))
            {
                UiCommon.TipManager.RegisterTip<CommonTipModel>(currentObject.transform, TipUtil.GetTip(eventKey));
            }
        }

        private void OnPointerExitDelegate(PointerEventData eventData)
        {
            string eventKey = string.Empty;

            if (eventData.pointerEnter != null)
            {
                ItemComponent toItem = eventData.pointerEnter.GetComponent<ItemComponent>();
                if (toItem != null)
                {
                    eventKey = toItem.EventKey;
                }
            }

            if (TipUtil.HasTip(eventKey))
            {
                UiCommon.TipManager.UnRegisterTip(currentObject.transform);
            }
        }

        private void OnEndCloneDelegate(PointerEventData eventData)
        {
            SimpleProto data = FreePool.Allocate();
            data.Key = FreeMessageConstant.DragImage;
            ItemComponent fromItem = dragObj.GetComponent<ItemComponent>();
            data.Ss.Add(fromItem.EventKey);

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                ItemComponent toItem = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemComponent>();
                if (toItem != null)
                {
                    data.Ss.Add(toItem.EventKey);
                }
                else
                {
                    data.Ss.Add("");
                }
            }
            else
            {
                data.Ss.Add("");
            }

            data.Bs.Add(Input.GetKey(KeyCode.LeftControl));

            SingletonManager.Get<FreeUiManager>().Contexts1.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, data);

            dragObj.GetComponent<Image>().raycastTarget = true;
            dragObj.transform.parent = null;
        }

        private void OnBeginDragDelegate(PointerEventData eventData)
        {
            eventData.pointerDrag.gameObject.GetComponent<Image>().raycastTarget = false;
            ItemComponent item = eventData.pointerDrag.gameObject.GetComponent<ItemComponent>();
            if (item != null)
            {
                item.ParentTransform = eventData.pointerDrag.gameObject.transform.parent;
            }
            eventData.pointerDrag.gameObject.transform.parent = UnityUiUtility.RootCanvas.transform;
        }

        private void OnDragDelegate(PointerEventData eventData)
        {
            eventData.pointerDrag.GetComponent<Transform>().position = eventData.position;
        }

        private void OnEndDragDelegate(PointerEventData eventData)
        {
            ItemComponent item = eventData.pointerDrag.gameObject.GetComponent<ItemComponent>();
            if (item != null)
            {
                eventData.pointerDrag.gameObject.transform.parent = item.ParentTransform;
            }

            SimpleProto data = FreePool.Allocate();
            data.Key = FreeMessageConstant.DragImage;
            ItemComponent fromItem = eventData.pointerDrag.GetComponent<ItemComponent>();
            ItemComponent toItem = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemComponent>();
            data.Ss.Add(fromItem.EventKey);
            if (toItem != null)
            {
                data.Ss.Add(toItem.EventKey);
            }
            else
            {
                data.Ss.Add("");
            }

            SingletonManager.Get<FreeUiManager>().Contexts1.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, data);
            eventData.pointerDrag.gameObject.GetComponent<Image>().raycastTarget = true;
        }

        private void OnPointerClickDelegate(PointerEventData eventData)
        {
            SimpleProto data = FreePool.Allocate();
            data.Key = FreeMessageConstant.ClickImage;
            ItemComponent item = eventData.pointerPress.GetComponent<ItemComponent>();
            data.Ss.Add(item.EventKey);
            data.Bs.Add(eventData.button == PointerEventData.InputButton.Right);
            data.Bs.Add(Input.GetKey(KeyCode.LeftControl));

            SingletonManager.Get<FreeUiManager>().Contexts1.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, data);
        }

        public void SetValues(string v)
        {
            if (!string.IsNullOrEmpty(v))
            {
                this.valueList.Add(v);

                if (currentObject != null)
                {
                    SetPureValue(v);
                }
            }
        }

        public void SetAllEventKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                this.allEventKey = key;
                if (currentObject != null)
                {
                    updateEventKey();
                }
            }
        }

        public void SetEvents(string v)
        {
            if (!string.IsNullOrEmpty(v))
            {
                this.events = v;

                if (currentObject != null)
                {
                    AddEvent(v);
                }
            }
        }

        public void SetFieldValue(string field, string value)
        {
            SetOneValue(new string[] { field, value });
        }

        protected override void SetPureValue(string v)
        {
            if (currentObject == null)
            {
                valueList.Add(v);
            }

            if (currentObject != null)
            {
                //Debug.Log(this.name + "=" + v);
                if (string.IsNullOrEmpty(v))
                {
                    return;
                }
                string[] fieldValues = v.Split(FreeMessageConstant.SpliterRecord);

                foreach (string fieldValue in fieldValues)
                {
                    string[] vs = fieldValue.Split(FreeMessageConstant.SpilterField);
                    if (vs.Length == 2)
                    {
                        SetOneValue(vs);
                    }
                }
            }
        }

        private void SetOneValue(string[] vs)
        {
            GameObject obj = UnityUiUtility.FindUIObject(currentObject, vs[0].Trim());
            if (obj != null)
            {
                Graphic gra = obj.GetComponent<Graphic>();
                if (gra != null)
                {
                    if (gra is Text)
                    {
                        ((Text)gra).text = vs[1].Trim();
                    }

                    if (gra is RawImage || gra is Image)
                    {
                        if (string.IsNullOrEmpty(vs[1]))
                        {
                            if (gra is RawImage)
                            {
                                ((RawImage)gra).texture = ResourceUtility.GetTransparentTexture();
                            }
                            if (gra is Image)
                            {
                                Texture2D tex = ResourceUtility.GetTransparentTexture();
                                ((Image)gra).sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                            }
                        }
                        else
                        {
                            int last = vs[1].Trim().LastIndexOf("/");
                            string buddleName = vs[1].Trim().Substring(0, last);
                            string assetName = vs[1].Trim().Substring(last + 1);
                            FreeUILoader.Load(gra, buddleName, assetName);
                        }
                    }
                }

            }
        }

        public override void SetPos(IComponentGroup freeUI, float x, float y, int width, int heigth, int relative, int parent)
        {
            RectTransform rt = _uiObject.gameObject.GetComponent<RectTransform>();
            FreeLayoutConverter.FullScreen(rt);

            this.relative = relative;
            this.parent = parent;
            this.group = freeUI;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = heigth;
        }

        public int Type
        {
            get { return TypePrefab; }
        }

        public int ValueType
        {
            get { return SimpleFreeUI.DATA_STRING; }
        }
    }
}
