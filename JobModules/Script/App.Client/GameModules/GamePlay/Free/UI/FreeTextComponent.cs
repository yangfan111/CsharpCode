using App.Client.GameModules.GamePlay.Free.UI;
using Assets.Sources.Free.Data;
using Assets.Sources.Free.Render;
using Assets.Sources.Free.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Sources.Free.UI
{
    public class FreeTextComponent : FreeBaseComponent, IFreeComponent
    {
        private Text _text;

        private string style;

        public FreeTextComponent()
        {
            var gameObject = new GameObject("Text");
            _text = gameObject.AddComponent<Text>();
            _text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            _text.horizontalOverflow = HorizontalWrapMode.Overflow;
            _text.verticalOverflow = VerticalWrapMode.Overflow;
            _uiObject = new UnityUiObject(gameObject);
        }

        public int Type
        {
            get { return TYPE_TEXT; }
        }

        public int ValueType
        {
            get { return SimpleFreeUI.DATA_STRING; }
        }


        public override void Frame(IUIDataManager uiDataManager, int frameTime)
        {
            base.Frame(uiDataManager, frameTime);

            if (ValueAuto != null)
            {
                var v = ValueAuto.Frame(frameTime);
                if (v != null)
                    SetText(v as string);
            }
        }

        protected override void SetPureValue(string v)
        {
            SetText(v);
        }

        private void SetText(string value)
        {
            _text.text = value;
            //            var key = style + value;
            //            if (!cache.containsKey(key))
            //            {
            //
            //                createTextFieldImage(typeCache.get(style) as int, value as String);
            //                cache.put(key, 1);
            //            }
            //            image.textureType = typeCache.get(style) as int;
            //            image.textureUrl = value as String;
        }

        public void Initial(params object[] ini)
        {

            InitialAuto(ini[1] as string);


            style = ini[0] as string;


            //            style = style + "_$$$_" + text.width + "_$$$_" + text.height;

            //            if (!typeCache.containsKey(style))
            //            {
            //                typeCache.put(style, typeCache.length() + 1000);
            //                var textureChangeManager:UITextureChangeManager = GameGUIManager.getInstance().getTextureChangeManager();
            //                textureChangeManager.addTextData(style, typeCache.get(style) as int);
            //            }

            var ss = (ini[0] as string).Split("_$$$_");


            _text.fontSize = (int)float.Parse(ss[1]);
            var colorString = "0x" + ss[2];

            Color color;
            ColorUtility.TryParseHtmlString(colorString, out color);
            _text.color = color;

            if (ss[3] == "true")
                _text.fontStyle = FontStyle.Bold;

            var font = ss[4];
            var hA = ss[5];
            if (string.IsNullOrEmpty(hA))
                hA = "center";
            var vA = ss[6];
            if (string.IsNullOrEmpty(vA))
                vA = "center";

            if (hA == "left" && vA == "bottom")
            {
                _text.alignment = TextAnchor.LowerLeft;
            }
            else if (hA == "left" && vA == "center")
            {
                _text.alignment = TextAnchor.MiddleLeft;
            }
            else if (hA == "left" && vA == "top")
            {
                _text.alignment = TextAnchor.UpperLeft;
            }
            else if (hA == "center" && vA == "bottom")
            {
                _text.alignment = TextAnchor.LowerCenter;
            }
            else if (hA == "center" && vA == "center")
            {
                _text.alignment = TextAnchor.MiddleCenter;
            }
            else if (hA == "center" && vA == "top")
            {
                _text.alignment = TextAnchor.UpperCenter;
            }
            else if (hA == "right" && vA == "bottom")
            {
                _text.alignment = TextAnchor.LowerRight;
            }
            else if (hA == "right" && vA == "center")
            {
                _text.alignment = TextAnchor.MiddleRight;
            }
            else if (hA == "right" && vA == "top")
            {
                _text.alignment = TextAnchor.UpperRight;
            }


            SetValue(0, ss[0]);
        }

        protected void CreateTextFieldImage(int textureType, string textureUrl)
        {
            //			var textureChangeManager:UITextureChangeManager = GameGUIManager.getInstance().getTextureChangeManager();
            //        text.text = textureUrl;
            //			var bitmapData:BitmapData = text.createBitmapData();
            //			var textRectangle:Rectangle = text.textBounds;
            //			textureChangeManager.addBitmapData( bitmapData, textureUrl, textureType, textRectangle );
            //			textureChangeManager.addUITextureInfo( textureUrl, textureType, textRectangle );
        }


        public override void SetPos(IComponentGroup freeUI, float x, float y, int width, int height, int relative, int parent)
        {
            NewSetPos(freeUI, x, y, width, height, relative, parent);

            _uiObject.width = width;
            _uiObject.height = height;
        }


        public IFreeComponent Clone()
        {
            return new FreeTextComponent();
        }

    }
}
