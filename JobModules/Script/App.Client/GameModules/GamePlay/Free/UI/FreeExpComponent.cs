using System.Collections.Generic;
using Assets.Sources.Free.Data;
using Assets.Sources.Free.Render;
using Assets.Sources.Free.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Sources.Free.UI
{
    public class FreeExpComponent : FreeBaseComponent, IFreeComponent
    {

        private Image image;
        private Sprite sprite;

        protected IUiObject observerPlayerBackImage;
        protected IList<IUiObject> observerPlayerName = new List<IUiObject>();

        //收益
        protected IUiObject profitBackImage;
        protected IUiObject totalExpSprite;
        protected IUiObject totalExpPerSprite;
        protected IUiObject totalGpSprite;
        protected IUiObject totalGpPerSprite;
        protected IUiObject totalCrystalSprite ;
        protected IList<IUiObject> expImageList;
        protected IList<IUiObject> expSpriteList;
        protected IList<IUiObject> gpImageList;
        protected IList<IUiObject> gpSpriteList;

        private IList<string> expUrlList;
        private IList<string> gpUrlList;

        protected const int ObserverPlayerH = 3;
        protected const int ObserverPlayerV = 5;

        private ProfitModelWrapper profitData;


        public int Type
        {
            get { return TYPE_EXP; }
        }

        public int ValueType
        {
            get { return SimpleFreeUI.DATA_STRING; }
        }

        public FreeExpComponent()
        {
            var go = new GameObject("Exp");
            go.AddComponent<RectTransform>();
            go.AddComponent<CanvasRenderer>();

            _uiObject = new UnityUiObject(go);

        }

        private void IniOb()
        {
            observerPlayerBackImage = UnityUiUtility.CreateImageDisplayObject(GetCommonUrl("observerPlayer/allKillInfoObserverPlayerBack.atf"), 700, 84);
            _uiObject.AddChild(observerPlayerBackImage);
            observerPlayerBackImage.x = 73;
            observerPlayerBackImage.y = 0;

            var index = 0;
            for (var i = 0; i < ObserverPlayerH; ++i)
            {
                for (var j = 0; j < ObserverPlayerV; ++j)
                {
                    var nameObj = UnityUiUtility.CreateEmptyDisplayObject();//UnityUIUtility.CreateImageDisplayObject("1", 128, 32);
                    _uiObject.AddChild(nameObj);
                    observerPlayerName.Add(nameObj);

                    nameObj.x = (observerPlayerBackImage.x + 15 + j * 130);
                    nameObj.y = (observerPlayerBackImage.y + 20 + i * 20);
                    nameObj.visible = false;
                    index++;
                }
            }
        }

        private void IniExp()
        {
            profitBackImage = UnityUiUtility.CreateImageDisplayObject(GetCommonUrl("killList/profitback.atf"), 845, 142);
            _uiObject.AddChild(profitBackImage);
            profitBackImage.x = 0;
            profitBackImage.y = 0;
            profitBackImage.visible = false;


            totalExpSprite = UnityUiUtility.CreateEmptyDisplayObject();
            _uiObject.AddChild(totalExpSprite);
            totalExpSprite.x = profitBackImage.x + 110;
            totalExpSprite.y = profitBackImage.y + 47;
            totalExpSprite.visible = false;


            totalExpPerSprite = UnityUiUtility.CreateEmptyDisplayObject();
            _uiObject.AddChild(totalExpPerSprite);
            totalExpPerSprite.x = totalExpSprite.x + 80;
            totalExpPerSprite.y = totalExpSprite.y + 3;
            totalExpPerSprite.visible = false;


            totalGpSprite = UnityUiUtility.CreateEmptyDisplayObject();
            _uiObject.AddChild(totalGpSprite);
            totalGpSprite.x = totalExpSprite.x + 325;
            totalGpSprite.y = profitBackImage.y + 45;
            totalGpSprite.visible = false;

            totalGpPerSprite = UnityUiUtility.CreateEmptyDisplayObject();
            _uiObject.AddChild(totalGpPerSprite);
            totalGpPerSprite.x = totalGpSprite.x + 80;
            totalGpPerSprite.y = totalGpSprite.y + 3;
            totalGpPerSprite.visible = false;

            totalCrystalSprite = UnityUiUtility.CreateEmptyDisplayObject();
            _uiObject.AddChild(totalCrystalSprite);
            totalCrystalSprite.x = profitBackImage.x + 770;
            totalCrystalSprite.y = profitBackImage.y + 76;
            totalCrystalSprite.visible = false;

//            for (var i = 0; i < 4; ++i)
//
//            {
                totalExpSprite.AddChild(UnityUiUtility.CreateNumberTextDisplayObject(120));
                totalExpPerSprite.AddChild(UnityUiUtility.CreateNumberTextDisplayObject(130));
                totalGpSprite.AddChild(UnityUiUtility.CreateNumberTextDisplayObject(120));
                totalGpPerSprite.AddChild(UnityUiUtility.CreateNumberTextDisplayObject(130));
                totalCrystalSprite.AddChild(UnityUiUtility.CreateNumberTextDisplayObject(120));
//            }

            expImageList = new List<IUiObject>();
            expSpriteList = new List<IUiObject>();
            for (var i = 0; i < 6; ++i)
            {
                var expImageObj = UnityUiUtility.CreateImageDisplayObject(GetCommonUrl("killList/honorary.atf"), 40, 49);
                expImageList.Add(expImageObj);
                _uiObject.AddChild(expImageObj);
                expImageObj.x = profitBackImage.x + 50 + (expImageObj.width + 10) * i;
                expImageObj.y = profitBackImage.y + 85;

                expImageObj.visible = false;

                var expSpriteObj = UnityUiUtility.CreateEmptyDisplayObject();
                expSpriteList.Add(expSpriteObj);
                _uiObject.AddChild(expSpriteObj);
                expSpriteObj.x = expImageObj.x + 2;
                expSpriteObj.y = expImageObj.y + 35;
                expSpriteObj.visible = false;

//                for (var j = 0; j < 4; ++j)
//                {
                    expSpriteObj.AddChild(UnityUiUtility.CreateNumberTextDisplayObject(140));
//                }
            }

            gpImageList = new List<IUiObject>();
            gpSpriteList = new List<IUiObject>();
            for (var i = 0; i < 4; ++i)
            {
                var gpImageObj = UnityUiUtility.CreateImageDisplayObject(GetCommonUrl("killList/honorary.atf"),100,100);
                gpImageList.Add(gpImageObj);
                _uiObject.AddChild(gpImageObj);
                gpImageObj.x = profitBackImage.x + 420 + (gpImageObj.width + 10) * i;
                gpImageObj.y = profitBackImage.y + 85;

                gpImageObj.visible = false;


                var gpSpriteObj = UnityUiUtility.CreateEmptyDisplayObject();
                gpSpriteList.Add(gpSpriteObj);
                _uiObject.AddChild(gpSpriteObj);
                gpSpriteObj.x = gpImageObj.x + 2;
                gpSpriteObj.y = gpImageObj.y + 35;
                gpSpriteObj.visible = false;


//                for (var j = 0; j < 4; ++j)
//
//                {
                    gpSpriteObj.AddChild(UnityUiUtility.CreateNumberTextDisplayObject(140));
//                }
            }

            expUrlList = new List<string>();
            expUrlList.Add("killList/honorary.atf");
            expUrlList.Add("killList/heroweapon.atf");
            expUrlList.Add("killList/bigexp.atf");
            expUrlList.Add("killList/smallexp.atf");
            expUrlList.Add("killList/friend.atf");
            expUrlList.Add("killList/accessory.atf");

            gpUrlList = new List<string>();
            gpUrlList.Add("killList/honorary.atf");
            gpUrlList.Add("killList/heroweapon.atf");
            gpUrlList.Add("killList/biggp.atf");
            gpUrlList.Add("killList/smallgp.atf");
        }

        private string GetCommonUrl(string url)
        {
            return "commonfree/" + url;
        }

        public override void Frame(IUIDataManager uiDataManager, int frameTime)
        {
            base.Frame(uiDataManager, frameTime);


            SetProfitData();

            SetObserverPlayerNameData(uiDataManager);
        }

        public IFreeComponent Clone()
        {
            return new FreeExpComponent();
        }

        public void Initial(params object[] ini)
        {
            IniOb();
            IniExp();
        }

        public override void SetPos(IComponentGroup freeUI, float x, float y, int width, int heigth, int relative, int parent)
        {
            base.SetPos(freeUI, x, y, width, heigth, relative, parent);


            _uiObject.width = 845;
            _uiObject.height = 142;

        }

        private bool _moved = false;
        private void MoveObserverImageToProfitImageBottom()
        {
            if (false == _moved)
            {
                _moved = true;
                observerPlayerBackImage.y = profitBackImage.y + profitBackImage.height;

                var index = 0;
                for (var i = 0; i < ObserverPlayerH; ++i)
                {
                    for (var j = 0; j < ObserverPlayerV; ++j)
                    {
                        observerPlayerName[index].y = (int)(observerPlayerBackImage.y + 20 + i * 20);
                        index++;
                    }
                }
            }
        }

        private AllObserverNameData allObserverNameData = null;
        protected void SetObserverPlayerNameData(IUIDataManager uiDataManager)
        {
            allObserverNameData = uiDataManager.AllObserverNameData;
            if (allObserverNameData.Changed)
            {
                allObserverNameData.Changed = false;

                var length = allObserverNameData.NameList.Count;
                if (length > 0)
                {
                    observerPlayerBackImage.visible = true;
                }
                else
                {
                    observerPlayerBackImage.visible = false;
                }

                for (var i = 0; i < length; ++i)
                {
                    if (i > ObserverPlayerH * ObserverPlayerV)
                    {
                        break;
                    }

                    UnityUiUtility.SetTexture(observerPlayerName[i], allObserverNameData.NameList[i]);
                    observerPlayerName[i].visible = true;
                }

                length = observerPlayerName.Count;
                for (var i = 0; i < length; ++i)
                {
                    observerPlayerName[i].visible = false;
                }
            }
        }

        private void SetProfitData()
        {
            var battleModel = GameModelLocator.GetInstance().GameModel;
            if (battleModel.isObserver)
            {
                return;
            }

            if (_uiObject.gameObject.transform.parent == null || !_uiObject.visible)
            {
                return;
            }

            profitData = battleModel.ProfitModel;
            if (profitData.changed)

            {
                profitData.changed = false;

                ShowProfitImage();
                HideProfitImage();

                MoveObserverImageToProfitImageBottom();

                UpdateTotalExpImage();
                UpdateTotalGpImage();
                UpdateTotalCrystalImage();
                UpdateExpImage();
                UpdateGpImage();
            }
        }

        private void UpdateGpImage()
        {
            var length = profitData.gpPlusList.Count;
            var index = 0;
            for (var i = 0; i < length; ++i)
            {
                if (profitData.gpPlusList[i] > 0)
                {
                    gpImageList[index].visible = true;
                    UnityUiUtility.SetTexture(gpImageList[index],GetCommonUrl(gpUrlList[i]));

                    gpSpriteList[index].visible = true;

                    SetNumber(gpSpriteList[index], profitData.gpPlusList[i]);
                    index++;
                }
            }
        }

        private void UpdateExpImage()
        {
            var length = profitData.expPlusList.Count;
            var index = 0;
            for (var i = 0; i < length; ++i)
            {
                if (profitData.expPlusList[i] > 0)
                {
                    expImageList[index].visible = true;
                    UnityUiUtility.SetTexture(expImageList[index], GetCommonUrl(expUrlList[i]));

                    expSpriteList[index].visible = true;
                    SetNumber(expSpriteList[index], profitData.expPlusList[i]);
                    index++;
                }
            }
        }

        private void UpdateTotalGpImage()
        {
            SetNumber(totalGpSprite,profitData.totalGp);
            SetNumber(totalGpPerSprite,profitData.totalGpPer);
        }

        private void UpdateTotalExpImage()
        {
            SetNumber(totalExpSprite, profitData.totalExp);
            SetNumber(totalExpPerSprite, profitData.totalExpPer);
        }

        private void UpdateTotalCrystalImage()
        {
            SetNumber(totalCrystalSprite, profitData.totalCrystal);
        }

        private void ShowProfitImage()
        {
            profitBackImage.visible = true;
            totalExpSprite.visible = true;
            totalExpPerSprite.visible = true;
            totalGpSprite.visible = true;
            totalGpPerSprite.visible = true;
            totalCrystalSprite.visible = true;
        }

        private void HideProfitImage()
        {
            var i = 0;

            for (i = 0; i < expImageList.Count; ++i)
            {
                expImageList[i].visible = false;
                expSpriteList[i].visible = false;
            }

            for (i = 0; i < gpImageList.Count; ++i)
            {
                gpImageList[i].visible = false;
                gpSpriteList[i].visible = false;
            }
        }

        protected override void SetPureValue(string vale)
        {
        }

    }
}
