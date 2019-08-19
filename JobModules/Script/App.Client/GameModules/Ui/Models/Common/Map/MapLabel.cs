using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class MapLabel
    {
        private Transform tran;
        private Transform c4Locate;
        private RectTransform c4LocateRtf;
        private RectTransform ALocateRtf;
        private RectTransform BLocateRtf;
        private GameObject Anormal;
        private GameObject Ared;
        private GameObject Bnormal;
        private GameObject Bred;

        private ActiveSetter ALocateRtfActiveSetter;
        private ActiveSetter BLocateRtfActiveSetter;
        private ActiveSetter c4LocateActiveSetter;
        private ActiveSetter anormalActiveSetter;
        private ActiveSetter bnormalActiveSetter;
        private ActiveSetter aRedActiveSetter;
        private ActiveSetter bRedActiveSetter;

        public MapLabel(Transform tran)
        {
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            c4Locate = tran.Find("C4Locate");
            c4LocateRtf = c4Locate.GetComponent<RectTransform>();

            ALocateRtf = tran.Find("ALocate").GetComponent<RectTransform>();
            BLocateRtf = tran.Find("BLocate").GetComponent<RectTransform>();
            Anormal = tran.Find("ALocate").Find("Normal").gameObject;
            Ared = tran.Find("ALocate").Find("Red").gameObject;
            Bnormal = tran.Find("BLocate").Find("Normal").gameObject;
            Bred = tran.Find("BLocate").Find("Red").gameObject;

            ALocateRtfActiveSetter = new ActiveSetter(ALocateRtf.gameObject);
            BLocateRtfActiveSetter = new ActiveSetter(BLocateRtf.gameObject);
            c4LocateActiveSetter = new ActiveSetter(c4Locate.gameObject);
            anormalActiveSetter = new ActiveSetter(Anormal.gameObject);
            bnormalActiveSetter = new ActiveSetter(Bnormal.gameObject);
            aRedActiveSetter = new ActiveSetter(Ared.gameObject);
            bRedActiveSetter = new ActiveSetter(Bred.gameObject);

            c4LocateActiveSetter.Active = false;
            ALocateRtfActiveSetter.Active = false;
            BLocateRtfActiveSetter.Active = false;
        }

        private bool _isC4Drop = false;
        private float _rate;
        public void UpdateC4(IMiniMapUiAdapter adapter, float rate)
        {
            if (!adapter.isBombMode)
                return;
            _rate = rate;
            _isC4Drop = adapter.IsC4Drop;
            c4LocateActiveSetter.Active = adapter.IsC4Drop;
            if (adapter.IsC4Drop)
            {
                c4LocateRtf.anchoredPosition = new Vector2(adapter.C4DropPosition.x, adapter.C4DropPosition.z)  * rate;                  //更新标记位置
            }
            ALocateRtfActiveSetter.Active = true;
            BLocateRtfActiveSetter.Active = true;
            ALocateRtf.anchoredPosition = new Vector2(adapter.APosition.x, adapter.APosition.z) * rate;
            BLocateRtf.anchoredPosition = new Vector2(adapter.BPosition.x, adapter.BPosition.z) * rate;

            int C4SetStatus = adapter.C4SetStatus;
            if (C4SetStatus == 0)
            {
                anormalActiveSetter.Active = true;
                bnormalActiveSetter.Active = true;
                aRedActiveSetter.Active = false;
                bRedActiveSetter.Active = false;

            }
            else if (C4SetStatus == 1)
            {

                if (adapter.IsCampPass())
                {
                    anormalActiveSetter.Active = false;
                    bnormalActiveSetter.Active = true;
                    aRedActiveSetter.Active = true;
                    bRedActiveSetter.Active = false;
                }
                else
                {
                    anormalActiveSetter.Active = false;
                    bnormalActiveSetter.Active = false;
                    aRedActiveSetter.Active = true;
                    bRedActiveSetter.Active = true;
                }
            }
            else if (C4SetStatus == 2)
            {

                if (adapter.IsCampPass())
                {
                    anormalActiveSetter.Active = true;
                    bnormalActiveSetter.Active = false;
                    aRedActiveSetter.Active = false;
                    bRedActiveSetter.Active = true;
                }
                else
                {
                    anormalActiveSetter.Active = false;
                    bnormalActiveSetter.Active = false;
                    aRedActiveSetter.Active = true;
                    bRedActiveSetter.Active = true;
                }
            }

        }
    }
}