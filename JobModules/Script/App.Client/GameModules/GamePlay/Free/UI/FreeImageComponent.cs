using System;
using App.Client.GameModules.Free;
using App.Client.GameModules.GamePlay.Free.UI;
using Assets.Sources.Free.Auto;
using Assets.Sources.Free.Data;
using Assets.Sources.Free.Render;
using Assets.Sources.Free.Utility;
using Assets.Scripts.Utils.Coroutine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using Assets.App.Client.GameModules.GamePlay.Free;

namespace Assets.Sources.Free.UI
{
    public class FreeImageComponent : FreeBaseComponent, IFreeComponent
    {
        private RawImage _image;
        private bool center;
        private bool isFixed;
        private EventTrigger _eventTrigger;

        private float beginDragX;
        private float beginDragY;

        private float beginSfX;
        private float beginSfY;

        private int imgX;
        private int imgY;
        private int coverWidth;
        private int coverHeight;
        private int imageWidth;
        private int imageHeight;

        private bool mouseIn;

        private double begineTime;

        private string _imageUrl;

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                if (_imageUrl == value)
                    return;
                _imageUrl = value;

                if (_image == null)
                    return;
                if (string.IsNullOrEmpty(_imageUrl) || _imageUrl == "null")
                {
                    _image.texture = ResourceUtility.GetTransparentTexture();
                }
                else
                {
                    try
                    {
                        int last = _imageUrl.LastIndexOf("/");
                        FreeUILoader.Load(_image, _imageUrl.Substring(0, last),
                            _imageUrl.Substring(last + 1));
                    }
                    catch (Exception e)
                    {
                        Debug.Log("image " + _imageUrl + " does not existed.");
                    }
                }
            }
        }

        public FreeImageComponent()
        {
            var gameObject = new GameObject("Image");
            _image = gameObject.AddComponent<RawImage>();
            //            _image.texture = ResourceUtility.GetTransparentTexture();
            _uiObject = new UnityUiObject(gameObject);
        }

        public override void Destroy()
        {
            base.Destroy();
            _image = null;
        }

        private void AddEntry(EventTriggerType entryType, UnityAction<PointerEventData> call)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = entryType;
            entry.callback.AddListener((data) => { call((PointerEventData)data); });
            _eventTrigger.triggers.Add(entry);
        }

        private SimpleFreeUI uiGroup
        {
            get
            {
                if (group is SimpleFreeUI)
                {
                    return (SimpleFreeUI)(group);
                }
                else
                {
                    return null;
                }
            }
        }

        public override void addEvent()
        {
            _eventTrigger = _uiObject.gameObject.AddComponent<EventTrigger>();

            AddEntry(EventTriggerType.PointerClick, OnPointerClickDelegate);
            //AddEntry(EventTriggerType.PointerEnter, OnPointerEnterDelegate);
            //AddEntry(EventTriggerType.PointerExit, OnPointerExitDelegate);
            AddEntry(EventTriggerType.BeginDrag, OnBeginDragDelegate);
            AddEntry(EventTriggerType.Drag, OnDragDelegate);
            AddEntry(EventTriggerType.EndDrag, OnEndDragDelegate);

        }

        private void OnBeginDragDelegate(PointerEventData eventData)
        {
            var pos = eventData.position;
            beginDragX = pos.x;
            beginDragY = pos.y;
            var sf = uiGroup;
            if (sf != null)
            {
                beginSfX = sf.X;
                beginSfY = sf.Y;
            }

            GameObject obj = eventData.pointerCurrentRaycast.gameObject;
            Debug.Log("begin:" + obj.name);
        }

        private void OnDragDelegate(PointerEventData eventData)
        {
            var sf = uiGroup;
            if (sf != null && !isFixed)
            {
                sf.gameObject.transform.SetSiblingIndex(10000);
                var pos = eventData.position;
                sf.X = beginSfX + (pos.x - beginDragX);
                sf.Y = beginSfY + -(pos.y - beginDragY);
            }
        }

        private void OnEndDragDelegate(PointerEventData eventData)
        {
            GameObject obj = eventData.pointerCurrentRaycast.gameObject;
            Debug.Log("end:" + obj.name);

            var sf = uiGroup;
            if (sf != null && !isFixed)
            {
                freeEvent.Event = FreeUIEvent.EVENT_MOVE;
                freeEvent.Key.Key = freeEvent.Event;
                freeEvent.Key.Ss.Add(_eventKey);

                // 偏移量
                freeEvent.Key.Ins.Add((int)sf.X);
                freeEvent.Key.Ins.Add((int)sf.Y);

                // 全局坐标
                var pos = eventData.position;
                freeEvent.Key.Ins.Add((int)pos.x);
                freeEvent.Key.Ins.Add((int)(pos.y));

                // 屏幕尺寸
                freeEvent.Key.Ins.Add(Screen.width);
                freeEvent.Key.Ins.Add(Screen.height);

                // 初始位置
                freeEvent.Key.Ins.Add((int)beginDragX);
                freeEvent.Key.Ins.Add((int)beginDragY);
            }
        }

        private void OnPointerExitDelegate(PointerEventData eventData)
        {
            freeEvent.Event = FreeUIEvent.EVENT_MOUSE_OUT;
            freeEvent.Key.Key = freeEvent.Event;
            freeEvent.Key.Ss.Add(_eventKey);
        }

        private void OnPointerEnterDelegate(PointerEventData eventData)
        {
            freeEvent.Event = FreeUIEvent.EVENT_MOUSE_ENTER;
            freeEvent.Key.Key = freeEvent.Event;
            freeEvent.Key.Ss.Add(_eventKey);
        }

        private void OnPointerClickDelegate(PointerEventData eventData)
        {
            freeEvent.Event = FreeUIEvent.EVENT_CLICK;
            freeEvent.Key.Key = freeEvent.Event;
            freeEvent.Key.Ss.Add(_eventKey);
        }

        public int Type
        {
            get { return TYPE_IMAGE; }
        }

        public int ValueType
        {
            get { return SimpleFreeUI.DATA_STRING; }
        }

        public override void Frame(IUIDataManager uiDataManager, int frameTime)
        {
            base.Frame(uiDataManager, frameTime);

            for (var index = 0; index < Autos.Count; index++)
            {
                var af = Autos[index];
                if (af.field == VALUE || af.field == null || af.field == "")
                {
                    var n = float.Parse(af.auto.Frame(frameTime).ToString());
                    SetPercent(n, 0, _image);
                }
                if (af.field == "v1")
                {
                    var n = float.Parse(af.auto.Frame(frameTime).ToString());
                    SetPercent(n, 1, _image);
                }
                if (af.field == "v2")
                {
                    var n = float.Parse(af.auto.Frame(frameTime).ToString());
                    SetPercent(n, 2, _image);
                }
                if (af.field == "cover")
                {
                    //        var c:ImageCoverUV = af.auto.frame(frameTime) as ImageCoverUV;
                    //        c.setUV(this);
                }
            }
        }

        protected override void SetPureValue(string value)
        {
            // 如果有图片url自动化的情况可以直接设置index的值来设置图片
            if (value.Length < 3 && ValueAuto != null && ValueAuto is AutoTimeStringValue)
            {
                var i = int.Parse(value);
                var vAuto = ValueAuto as AutoTimeStringValue;
                if (i > 0 && i <= vAuto.Size())
                {
                    //					image.textureUrl = GetUIUrl(vAuto.GetUrl(i - 1));
                    this.ImageUrl = vAuto.GetUrl(i - 1);
                }
            }
            else
            {
                //				image.textureUrl = GetUIUrl(value);
                this.ImageUrl = value;
            }
        }

        protected void SetImageOpacityUv(int origWidth, int origHeight, Image image)
        {
            //			var legalWidth:int  = TextureUtils.getNextPowerOfTwo(origWidth);
            //			var legalHeight:int = TextureUtils.getNextPowerOfTwo(origHeight);
            //			var opacityUV:Vector.<Number> = new Vector.<Number>();
            //			opacityUV.push( 0 );
            //			opacityUV.push( 0 );
            //			opacityUV.push( origWidth/legalWidth );
            //			opacityUV.push( 0 );
            //			opacityUV.push( 0 );
            //			opacityUV.push( origHeight/legalHeight );
            //			opacityUV.push( origWidth/legalWidth );
            //			opacityUV.push( origHeight/legalHeight );
            //			image.opacityUV = opacityUV;
        }

        public void Initial(params object[] ini)
        {

            InitialAuto(ini[1] as string);

            var a = (ini[0] as string).Split("_$$$_");

            var has = false;
            var hasCover = false;

            if (a.Length > 1)
            {
                var c = a[1].Split(',');
                if (c.Length == 2)
                {
                    //                    image.pivotX = Number(c[0]);
                    //                    image.pivotY = Number(c[1]);
                    _image.rectTransform.pivot = new Vector2(float.Parse(c[0]), float.Parse(c[1]));
                }

                if (a.Length > 2)
                {
                    c = a[2].Split(',');
                    if (c.Length == 2)
                    {
                        imageWidth = int.Parse(c[0]);
                        imageHeight = int.Parse(c[1]);
                        //                        Image.rectTransform.sizeDelta = new Vector2(int.Parse(c[0]), int.Parse(c[1]));
                        has = true;
                    }
                }

                if (a.Length > 3)
                {
                    if (a[3].IndexOf(".atf") > 0)
                    {

                        //                        this.image.opacityTextureUrl = GetUIUrl(a[3]);

                        hasCover = true;

                    }
                }

                if (a.Length > 4)
                {
                    if (a[4] == "true")
                    {

                        this.isFixed = true;

                    }
                }

                if (a.Length > 5)
                {
                    if (a[5] == "true")
                    {

                        this.nomouse = true;

                    }
                }

                if (a.Length > 6)
                {
                    if (a[6] == "true")
                    {

                        //                        this.image.useMaskImage = true;

                    }
                }

                if (a.Length > 7)
                {
                    if (a[7] == "true")
                    {

                        //                        this.image.isMaskImage = true;

                    }
                }
            }

            if (!has)
            {
                imageWidth = _uiObject.width;
                imageHeight = _uiObject.height;
            }


            //            setImageTexCoordsXY(image.imageWidth, image.imageHeight, this.image);

            if (hasCover)
            {

                //                SetImageOpacityUv(image.width, image.height, image);
            }


            this.SetValue(0, a[0]);
        }

        public override void SetPos(IComponentGroup freeUI, float x, float y, int width, int heigth, int relative, int parent)
        {
            if (width == -1 && heigth == -1)
            {
                FreeLayoutConverter.FullScreen(_uiObject.gameObject.GetComponent<RectTransform>());
                FreeLayoutConverter.FullScreen(_image.rectTransform);
            }
            else
            {
                NewSetPos(freeUI, x, y, width, height, relative, parent);
                _image.rectTransform.sizeDelta = new Vector2(width, heigth);
            }
        }

        public IFreeComponent Clone()
        {
            return new FreeImageComponent();
        }

        protected void SetPercent(float p, int v, RawImage image)
        {
            if (v == 1)
            {
                image.rectTransform.localScale = new Vector3(1, p / 1000f, 1);
            }
            else if (v == 2)
            {
                image.rectTransform.localScale = new Vector3(1, -p / 1000f, 1);
            }
            else
            {
                image.rectTransform.localScale = new Vector3(p / 1000f, 1, 1);
            }

        }


    }
}
