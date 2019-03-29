using App.Client.GameModules.Ui.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class MapLabel
    {
        private Transform tran;
        private Transform c4Locate;
        private RectTransform c4LocateRtf;

        public MapLabel(Transform tran)
        {
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            c4Locate = tran.Find("C4Locate");
            c4LocateRtf = c4Locate.GetComponent<RectTransform>();
            UIUtils.SetActive(c4Locate, false);
        }

        private bool _isC4Drop = false;
        private float _rate;
        public void UpdateC4(bool isC4Drop, Vector3 c4DropPos, float rate)
        {
            if (_isC4Drop.Equals(isC4Drop) && rate.Equals(_rate)) return;
            _rate = rate;
            _isC4Drop = isC4Drop;


            UIUtils.SetActive(c4Locate, isC4Drop);
            if (isC4Drop)
            {
                c4LocateRtf.anchoredPosition = new Vector2(c4DropPos.x, c4DropPos.z)  * rate;                  //更新标记位置
            }
        }
    }
}