using System;
using App.Client.GameModules.GamePlay.Free.UI;
using UnityEngine;
using Assets.Sources.Free.Auto;
using Assets.Sources.Free.Data;
using Assets.Sources.Free.Render;

namespace Assets.Sources.Free.UI
{
    public delegate void AddComponent(GameObject obj);

    public abstract class FreeBaseComponent : BaseAutoFields
    {
        public const string VALUE = "v";
        public const string X = "x";
        public const string Y = "y";
        public const string WIDTH = "w";
        public const string HEIGHT = "h";
        public const string ALPHA = "a";
        public const string ROTATION = "r";
        public const string VISIBLE = "vi";
        public const string AVATAR_POINT = "ap";
        public const string AVATAR_ROTATION = "ar";

        public const int TYPE_IMAGE = 1;
        public const int TYPE_TEXT = 2;
        public const int TYPE_NUMBER = 3;
        public const int TYPE_LIST = 4;
        public const int TYPE_RADER = 5;
        public const int TYPE_EXP = 6;
        public const int TYPE_SMALLMAP = 7;
        public const int TYPE_R_IMAGE = 8;
        public const int TypePrefab = 9;

        protected IUiObject _uiObject;

        protected string _key;
        protected bool showZero;

        protected int relative;
        protected int parent;
        protected bool nomouse;
        protected IComponentGroup group;
        protected float x;
        protected float y;
        protected int width;
        protected int height;

        protected string _eventKey;

        protected FreeUIEvent freeEvent;

        public event AddComponent ComponentAdded;

        public FreeBaseComponent()
        {
            freeEvent = new FreeUIEvent();
        }

        public virtual void Destroy()
        {
        }

        public float originalX
        {
            get { return x; }
        }

        public float originalY
        {
            get { return y; }
        }

        public bool IsNoMouse
        {
            get
            {
                return nomouse;
            }

        }

        public string EventKey
        {
            get { return _eventKey; }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != "null")
                {
                    _eventKey = value;
                    addEvent();
                }
            }
        }


        public virtual void addEvent()
        {

        }

        public FreeUIEvent FreeUIEvent
        {
            get
            {
                if (freeEvent == null)
                {
                    freeEvent = new FreeUIEvent();
                }
                return freeEvent;
            }
        }

        public IUiObject ToUI()
        {
            return _uiObject;
        }

        public virtual void SetValue(params object[] vs)
        {
            var sa = (int)vs[0];
            var auto = sa / 100;
            var index = sa % 100;
            if (auto == AUTO_START_NEW || auto == AUTO_START_OLD)
            {

                StartAuto(index);
            }
            else
            {

                StopAuto(index);
            }

            if (auto == AUTO_START_NEW || auto == AUTO_STOP_NEW || auto == AUTO_SET)
            {
                var value = vs[1] as string;
                if (value == null || value == "null" || value == "")
                {

                    ToUI().visible = false;
                }
                else
                {

                    ToUI().visible = true;
                }
                if (auto == AUTO_START_NEW || auto == AUTO_STOP_NEW)
                {

                    SetAutoValue(value, index);
                }
                if (auto == AUTO_SET)
                {

                    SetPureValue(value);
                }
            }
        }

        protected abstract void SetPureValue(string v);

        public virtual void SetPos(IComponentGroup freeUI, float x, float y, int width, int height, int relative, int parent)
        {

            this.relative = relative;
            this.parent = parent;
            this.group = freeUI;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;


            var control = ToUI();

            control.width = width;

            control.height = height;


            float startX = 0;

            float startY = 0;

            var rW = 0;

            var rH = 0;

            if (parent <= 0)
            {
                rW = Screen.width;
                rH = Screen.height;
            }
            else
            {
                var fc = freeUI.GetComponent(parent - 1);
                rW = fc.ToUI().width;
                rH = fc.ToUI().height;

                startX = fc.ToUI().x;
                startY = fc.ToUI().y;

            }

            SetXy(control, relative, startX, startY, rW, rH, x, y);
            if (freeUI is FreeListComponent)
            {
                control.y = y;
            }
        }

        public void NewSetPos(IComponentGroup freeUI, float x, float y, int width, int height, int relative, int parent)
        {
            this.group = freeUI;
            this.relative = relative;
            this.parent = parent;
            this.group = freeUI;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            RectTransform parentTransform = null;
            if (parent > 0)
            {
                parentTransform = freeUI.GetComponent(parent - 1).ToUI().gameObject.GetComponent<RectTransform>();
            }
            FreeLayoutConverter.Convert(relative, x, y, width, height,
                _uiObject.gameObject.GetComponent<RectTransform>(), parentTransform);
        }

        public void RefreshPosition()
        {
            //SetPos(group, x, y, width, height, relative, parent);
        }

        // 相对位置 0位左上，1位右上，2位左下，3位右下，4位中间左上，5位中间右上，6位中间左下，7位中间右下
        private void SetXy(IUiObject control, int relative, float startX, float startY, int width, int height, float x, float y)
        {
            switch (relative)
            {
                case 0:
                    control.x = startX + x;
                    control.y = startY + y;
                    break;
                case 1:
                    control.x = startX + width - x;
                    control.y = startY + y;
                    break;
                case 2:
                    control.x = startX + x;
                    control.y = startY + height - y;
                    break;
                case 3:
                    control.x = startX + width - x;
                    control.y = startY + height - y;
                    break;
                case 4:
                    control.x = startX + width / 2 - x;
                    control.y = startY + height / 2 - y;
                    break;
                case 5:
                    control.x = startX + width / 2 + x;
                    control.y = startY + height / 2 - y;
                    break;
                case 6:
                    control.x = startX + width / 2 - x;
                    control.y = startY + height / 2 + y;
                    break;
                case 7:
                    control.x = startX + width / 2 + x;
                    control.y = startY + height / 2 + y;
                    break;
                case 8:
                    control.x = startX + width / 2 - x;
                    control.y = startY + y;
                    break;
                case 9:
                    control.x = startX + width / 2 - x;
                    control.y = startY + height - y;
                    break;
                case 10:
                    control.x = startX + x;
                    control.y = startY + height / 2 - y;
                    break;
                case 11:
                    control.x = startX + width - x;
                    control.y = startY + height / 2 - y;
                    break;
                default:
                    break;
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }
            set { _key = value; }

        }

        protected IAutoValue ValueAuto
        {
            get
            {
                return GetAuto(VALUE);
            }

        }

        protected override void InitialAutoValue(string field, IAutoValue auto)
        {
            if (auto != null)
            {
                if (X == field)
                {
                    auto.SetValue(this.x);
                }
                if (Y == field)
                {
                    auto.SetValue(this.y);
                }
                if (WIDTH == field)
                {
                    auto.SetValue(ToUI().width);
                }
                if (HEIGHT == field)
                {
                    auto.SetValue(ToUI().height);
                }
                if (ALPHA == field)
                {
                    auto.SetValue(ToUI().alpha * 1000);
                }
                if (ROTATION == field)
                {
                    auto.SetValue(ToUI().rotation * 180 / Math.PI);
                }
                if (VISIBLE == field)
                {
                    auto.SetValue(ToUI().visible);
                }
                if (AVATAR_POINT == field)
                {
                    auto.SetValue(ToUI().x + "," + ToUI().y + "," + ToUI().rotation);
                }
                if (AVATAR_ROTATION == field)
                {
                    auto.SetValue(ToUI().x + "," + ToUI().y + "," + ToUI().rotation);
                }
            }
        }

        protected override void SetAutoValueTo(AutoField autoField, int frammeTime)
        {
            if (X == autoField.field)
            {
                var nx = Convert.ToSingle(autoField.auto.Frame(frammeTime));

                NewSetPos(this.group, nx, this.y, ToUI().width, ToUI().height, this.relative, this.parent);
            }
            if (Y == autoField.field)
            {
                var ny = Convert.ToSingle(autoField.auto.Frame(frammeTime));

                NewSetPos(this.group, this.x, ny, ToUI().width, ToUI().height, this.relative, this.parent);
            }
            if (WIDTH == autoField.field)
            {

                ToUI().width = Convert.ToInt32(autoField.auto.Frame(frammeTime));
            }
            if (HEIGHT == autoField.field)
            {

                ToUI().height = Convert.ToInt32(autoField.auto.Frame(frammeTime));
            }
            if (ALPHA == autoField.field)
            {

                ToUI().alpha = Convert.ToSingle(autoField.auto.Frame(frammeTime)) / 1000;
            }
            if (ROTATION == autoField.field)
            {

                ToUI().rotation = Convert.ToSingle(autoField.auto.Frame(frammeTime)) * (float)Math.PI / 180;
            }
            if (VISIBLE == autoField.field)
            {

                ToUI().visible = Convert.ToBoolean(autoField.auto.Frame(frammeTime));
            }
            if (AVATAR_POINT == autoField.field)
            {
                var v = (Vector3)autoField.auto.Frame(frammeTime);

                ToUI().x = v.x;

                ToUI().y = v.y;
            }
            if (AVATAR_ROTATION == autoField.field)
            {
                var v1 = (Vector3)autoField.auto.Frame(frammeTime);

                ToUI().rotation = v1.z;
            }
        }

        public virtual void Frame(IUIDataManager uiDataManager, int frammeTime)
        {
            AutoValue(frammeTime);
        }

        protected void SetArabicNumberSpriteCenter(int number, int index, Sprite numberSprite)
        {
            var imageCount = SetArabicNumberData(number, index, numberSprite);

            SetArabicNumberCenter(imageCount, numberSprite);
        }

        protected void SetArabicNumberSpriteRight(int number, int index, Sprite numberSprite)
        {
            var imageCount = SetArabicNumberData(number, index, numberSprite);

            SetArabicNumberRight(imageCount, numberSprite);
        }

        protected void SetArabicNumberSpriteLeft(int number, int index, Sprite numberSprite)
        {
            var imageCount = SetArabicNumberData(number, index, numberSprite);

            SetArabicNumberLeft(imageCount, numberSprite);
        }

        private int SetArabicNumberData(int number, int index, Sprite numberSprite)
        {
            // temp implemention
            return 0;
            //temp end


            //			var imageIndex = 0;
            //			var imageCount = numberSprite.numChildren;
            //		    Image image = null;
            //		    var tempNumber = number;
            //			var arabicNumber = 0;
            //			
            //			while( true )
            //			{
            //				arabicNumber = tempNumber%10;
            //				image = numberSprite.getChildAt( imageIndex ) as Image;
            //
            //                setArabicNumberTexCood(arabicNumber, image, index );
            //image.visible = true;
            //				
            //				tempNumber *= 0.1;
            //				imageIndex++;
            //				
            //				if( imageIndex >= imageCount )
            //					break;
            //				
            //				if( tempNumber <= 0 ){
            //					if(!showZero){
            //						break;
            //					}
            //				}
            //			}
            //			
            //			for( var i:int = imageIndex; i<imageCount; ++i )
            //			{
            //				image = numberSprite.getChildAt( i ) as Image;
            //				image.visible = false;
            //			}
            //			
            //			return imageIndex;
        }

        private void SetArabicNumberRight(int imageCount, Sprite numberSprite)
        {
            //			var image:Image = numberSprite.getChildAt(0) as Image;
            //			var imageX:Number = (numberSprite.numChildren-imageCount)* image.width;
            //			for( var i:int = imageCount-1; i >= 0; --i )
            //			{
            //				image = numberSprite.getChildAt(i) as Image;
            //				image.x = imageX + (imageCount-1-i)* image.width;
            //			}
        }

        private void SetArabicNumberLeft(int imageCount, Sprite numberSprite)
        {
            //			var image:Image = numberSprite.getChildAt(0) as Image;
            //			var imageX:Number = 0;
            //			for( var i:int = imageCount-1; i >= 0; --i )
            //			{
            //				image = numberSprite.getChildAt(i) as Image;
            //				image.x = imageX + (imageCount-1-i)* image.width;
            //			}
        }

        private void SetArabicNumberCenter(int imageCount, Sprite numberSprite)
        {
            //			var image:Image = numberSprite.getChildAt(0) as Image;			
            //			var imageX:Number = (numberSprite.numChildren-imageCount)*0.5* image.width;
            //			for( var i:int = imageCount-1; i >= 0; --i )
            //			{
            //				image = numberSprite.getChildAt(i) as Image;
            //				image.x = imageX + (imageCount-1-i)* image.width;
            //			}
        }

        //		protected void setArabicNumberTexCood(int number,Image image,int index)
        //		{
        //			var textureChangeManager:UITextureChangeManager = GameGUIManager.getInstance().getTextureChangeManager();
        //var pointList:Vector.<Point> = textureChangeManager.getTexCoodPoint( index+number );
        //			for( var i:int = 0; i< 4; i++ )
        //			{
        //				image.setTexCoords( i, pointList[i] );
        //			}
        //		}

        //		protected function setImageTexCoordsXY(origWidth:int, origHeight:int, image:Image):void
        //		{
        //			var legalWidth:int  = TextureUtils.getNextPowerOfTwo(origWidth);
        //			var legalHeight:int = TextureUtils.getNextPowerOfTwo(origHeight);
        //			
        //			image.setTexCoordsXY( 1, origWidth/legalWidth, 0 );
        //			image.setTexCoordsXY( 2, 0, origHeight/legalHeight );
        //			image.setTexCoordsXY( 3, origWidth/legalWidth, origHeight/legalHeight );
        //		}

        protected void SetNumber(IUiObject sprite, int number)
        {
            var text = sprite.FindComponentInChildren<UnityEngine.UI.Text>();
            text.text = number.ToString();
        }

        public static string GetUIUrl(string url)
        {
            if (url == null || url == "null" || url == "")
            {
                return "";
            }
            if (url.IndexOf("/ui") == 0 || url.IndexOf("/weapon") == 0)
            {
                return url;
            }
            else if (url.IndexOf("commonfree/") == 0 ||
               url.IndexOf("common/") == 0 ||
               url.IndexOf("halfLoadedRes/") == 0 ||
               url.IndexOf("promiseModel/") == 0)
            {

                return "/ui/GameGUIRes/" + url;
            }

            return "/ui/GameGUIRes/" +
                   //temp add
                   "Model/" + url; ;
            //temp end

            //				GameModelLocator.getInstance().gameModel.roomData.freeType 
            //				+ "Model/" + url;
        }
    }
}
