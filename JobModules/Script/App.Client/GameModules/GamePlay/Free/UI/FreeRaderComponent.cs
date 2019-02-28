using Assets.Sources.Free.Utility;
using Assets.Sources.Free.Data;
using UnityEngine;

namespace Assets.Sources.Free.UI
{

    public class FreeRaderComponent : FreeBaseComponent, IFreeComponent
    {


        //        private FreeRader rader;

        public FreeRaderComponent()
        {
            _uiObject = UnityUiUtility.CreateEmptyDisplayObject("Rader");
        }

        public override void Frame(IUIDataManager uiDataManager, int frameTime)
        {
            //    rader.onFrame(uiDataManager, frameTime);
        }

        public IFreeComponent Clone()
        {
            var rader = new FreeRaderComponent();
            //    rader.rader = new FreeRader(GameGUIManager.stageWidth, GameGUIManager.stageHeight,
            //        GameGUIManager.getInstance().getTextureChangeManager());
            //
            //    rader.sprite.addChild(rader.rader);

            return rader;
        }

        public void Initial(params object[] ini)
        {

        }


        protected override void SetPureValue(string v)
        {
        }

        public int Type
        {
            get { return TYPE_RADER; }
        }


        public int ValueType
        {
            get { return SimpleFreeUI.DATA_STRING; }
        }

    }
}