using Assets.Sources.Free.Utility;
using Assets.Sources.Free.Data;

namespace Assets.Sources.Free.UI
{
    public class FreeSmallMapComponent : FreeBaseComponent, IFreeComponent
    {


        //    private FreeSmallMap smallMap;

        public FreeSmallMapComponent()
        {
            _uiObject = UnityUiUtility.CreateEmptyDisplayObject("SmallMap");
        }

        public override void Frame(IUIDataManager uiDataManager, int frameTime)
        {
            //        smallMap.onFrame(uiDataManager, frameTime);
        }

        public IFreeComponent Clone()
        {
            var sm = new FreeSmallMapComponent();

            //        sm.smallMap = new FreeSmallMap(GameGUIManager.stageWidth, GameGUIManager.stageHeight,
            //            GameGUIManager.getInstance().getTextureChangeManager());
            //        sm.smallMap.visible = true;
            //			
            //        sm.sprite.addChild(sm.smallMap);

            return sm;
        }

        public void Initial(params object[] ini)
        {

        }

        protected override void SetPureValue(string v)
        {
        }

        public int Type
        {
            get { return TYPE_SMALLMAP; }
        }

        public int ValueType
        {
            get
            {
                return SimpleFreeUI.DATA_STRING;
            }
        }

    }
}
