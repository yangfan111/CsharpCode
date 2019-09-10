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

            UIUtils.SetActive(c4Locate, false);
            UIUtils.SetActive(ALocateRtf, false);
            UIUtils.SetActive(BLocateRtf, false);
        }

        private bool _isC4Drop = false;
        private float _rate;
        public void UpdateC4(IMiniMapUiAdapter adapter, float rate)
        {
            if (!adapter.isBombMode)
                return;
            _rate = rate;
            _isC4Drop = adapter.IsC4Drop;
            UIUtils.SetActive(c4Locate, adapter.IsC4Drop);
            if (adapter.IsC4Drop)
            {
                c4LocateRtf.anchoredPosition = new Vector2(adapter.C4DropPosition.x, adapter.C4DropPosition.z)  * rate;                  //更新标记位置
            }
            UIUtils.SetActive(ALocateRtf, true);
            UIUtils.SetActive(BLocateRtf, true);
            ALocateRtf.anchoredPosition = new Vector2(adapter.APosition.x, adapter.APosition.z) * rate;
            BLocateRtf.anchoredPosition = new Vector2(adapter.BPosition.x, adapter.BPosition.z) * rate;

            int C4SetStatus = adapter.C4SetStatus;
            if (C4SetStatus == 0)
            {
                UIUtils.SetActive(Anormal, true);
                UIUtils.SetActive(Bnormal, true);
                UIUtils.SetActive(Ared, false);
                UIUtils.SetActive(Bred, false);

            }
            else if (C4SetStatus == 1)
            {

                if (adapter.IsCampPass())
                {
                    UIUtils.SetActive(Anormal, false);
                    UIUtils.SetActive(Bnormal, true);
                    UIUtils.SetActive(Ared, true);
                    UIUtils.SetActive(Bred, false);
                }
                else
                {
                    UIUtils.SetActive(Anormal, false);
                    UIUtils.SetActive(Bnormal, false);
                    UIUtils.SetActive(Ared, true);
                    UIUtils.SetActive(Bred, true);
                }
            }
            else if (C4SetStatus == 2)
            {

                if (adapter.IsCampPass())
                {
                    UIUtils.SetActive(Anormal, true);
                    UIUtils.SetActive(Bnormal, false);
                    UIUtils.SetActive(Ared, false);
                    UIUtils.SetActive(Bred, true);
                }
                else
                {
                    UIUtils.SetActive(Anormal, false);
                    UIUtils.SetActive(Bnormal, false);
                    UIUtils.SetActive(Ared, true);
                    UIUtils.SetActive(Bred, true);
                }
            }

        }
    }
}