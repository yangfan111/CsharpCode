using System;
using App.Client.GameModules.Free;
using App.Client.GameModules.GamePlay.Free.UI;
using Assets.Sources.Free.Data;
using Assets.Sources.Free.Utility;
using UnityEngine;
using UnityEngine.UI;
using Assets.Sources.Free.Render;
using Assets.Scripts.Utils.Coroutine;

namespace Assets.Sources.Free.UI
{
    public class FreeNumberComponent : FreeBaseComponent, IFreeComponent
    {
        private Text _text;

        private int _index = -1;

        private int index
        {
            get { return _index; }
            set
            {
                if (value == _index)
                    return;
                _index = value;
                FreeGlobalVars.Loader.LoadAsync("number", _index.ToString(), (font) => _text.font = (Font)font);
            }
        }
        private int width;
        private int height;
        private int len;

        private string pos;

        public FreeNumberComponent()
        {
            var gameObject = new GameObject("Number");
            _text = gameObject.AddComponent<Text>();
            //            _text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            _text.horizontalOverflow = HorizontalWrapMode.Overflow;
            _text.verticalOverflow = VerticalWrapMode.Overflow;
            _text.alignment = TextAnchor.UpperLeft;
            _uiObject = new UnityUiObject(gameObject);
        }

        public override void addEvent()
        {
        }

        public int Type
        {
            get { return TYPE_NUMBER; }
        }

        public int ValueType
        {
            get
            {
                return SimpleFreeUI.DATA_STRING;
            }
        }


        public IFreeComponent Clone()
        {
            return new FreeNumberComponent();
        }


        public override void Frame(IUIDataManager uiDataManager, int frameTime)
        {
            base.Frame(uiDataManager, frameTime);

            var fontAuto = GetAuto("font");
            if (fontAuto != null)
            {
                var font = fontAuto.Frame(frameTime) as string;
                var ss = font.Split('_');
                if (ss.Length == 3)
                {
                    index = int.Parse(ss[0]);
                    width = int.Parse(ss[1]);
                    height = int.Parse(ss[2]);
                }
            }

            if (ValueAuto != null && ValueAuto.Started)
            {
                var v = ValueAuto.Frame(frameTime);
                SetNumber(Convert.ToInt32(v));
            }
        }

        protected override void SetPureValue(string v)
        {
            var number = 0;
            if (!string.IsNullOrEmpty(v) && v != "null")
            {
                number = int.Parse(v);
                SetNumber(number);
            }
        }

        private void SetNumber(int number)
        {
            _text.text = showZero ? number.ToString().PadLeft(len, '0') : number.ToString();
        }

        public void Initial(params object[] ini)
        {

            InitialAuto(ini[1] as string);

            var ss = (ini[0] as string).Split('_');

            index = int.Parse(ss[1]);
            width = int.Parse(ss[2]);
            height = int.Parse(ss[3]);
            len = int.Parse(ss[4]);
            pos = ss[5];
            showZero = ss[6].ToLower() == "true";

            _uiObject.width = width * len;
            _uiObject.height = height;

            if (ss.Length >= 8)
            {
                _uiObject.scaleX = float.Parse(ss[7]);
                _uiObject.scaleY = float.Parse(ss[7]);
            }

            for (var i = 0; i < len; i++)
            {
                //    var image:Image = new Image(this.width, this.height,
                //        "/ui/GameGUIRes/common/arabicNumber.atf", UITextureType.STATIC_IMAGE_1);
                //    this.sprite.addChild(image);
            }

            if (pos == "center")
            {
                _text.alignment = TextAnchor.UpperCenter;
            }
            else if (pos == "left")
            {
                _text.alignment = TextAnchor.UpperLeft;
            }
            else if (pos == "right")
            {
                _text.alignment = TextAnchor.UpperRight;
            }


            SetValue(0, ss[0]);
        }

        public override void SetPos(IComponentGroup freeUI, float x, float y, int width, int height, int relative, int parent)
        {
            NewSetPos(freeUI, x, y, width, height, relative, parent);
        }

    }
}
