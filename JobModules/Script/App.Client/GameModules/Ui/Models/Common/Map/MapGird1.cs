using System.Collections.Generic;
using App.Client.GameModules.Ui.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class MapGird1
    {
        public int gridThousandInterval = 1000;
        public int gridHundredInterval = 100;
        private int pixelOffSet = 1;
        private Color hundredLineColor = new Color32(189, 189, 189, 180);

        private Transform tran;
        private RectTransform rectTransform;
//        private Transform imgTf;
        private Transform rowLineModel;
        private Transform colLineModel;

        private Transform rowGirdModel;
        private Transform colGirdModel;

        private RawImage gridHundredImage;
        private RawImage gridThousandImage;

        public MapGird1(Transform tran)
        {
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            this.rectTransform = tran.GetComponent<RectTransform>();
//            imgTf = tran.Find("Imag");
            gridHundredImage = tran.Find("gridHundred").GetComponent<RawImage>();
            gridThousandImage = tran.Find("gridThousand").GetComponent<RawImage>();
            gridHundredImage.material = new Material(gridHundredImage.material);
            gridThousandImage.material = new Material(gridThousandImage.material);
            InitGird();
        }

        public void InitGird()
        {
            float lineWidth = Mathf.Lerp(0.004f, 0.007f, (1080f - Screen.height) / (1080f - 600f)) * 256 / rectTransform.rect.height;
            gridHundredImage.material.SetFloat("_gridWidth", lineWidth);
            gridThousandImage.material.SetFloat("_gridWidth", lineWidth + 0.001f);

            gridThousandImage.material.SetColor("_gridColor", Color.black);
        }

        private void UpdateGird(float miniMapRepresentWHByRice, float OriginalMiniMapRepresentWHByRice)
        {
            gridHundredImage.material.SetFloat("_tickWidth", 1 / (miniMapRepresentWHByRice / gridHundredInterval));
            gridThousandImage.material.SetFloat("_tickWidth", 1 / (miniMapRepresentWHByRice / gridThousandInterval));

            if (!OriginalMiniMapRepresentWHByRice.Equals(0))
            {
                if (miniMapRepresentWHByRice.Equals(OriginalMiniMapRepresentWHByRice))
                {
                    gridHundredImage.enabled = false;
                }
                else
                {
                    gridHundredImage.enabled = true;
                    hundredLineColor.a = 1.10f - 2 * miniMapRepresentWHByRice / OriginalMiniMapRepresentWHByRice;
                    gridHundredImage.material.SetColor("_gridColor", hundredLineColor);
                }
            }
        }



       

        private float _rate;
        private float _mapRealWHByRice;
        private Vector2 _selfPlayPos = Vector2.zero;
        private float _miniMapRepresentWHByRice;
        public void Update(float rate, float mapRealWHByRice, Vector2 selfPlayPos, float miniMapRepresentWHByRice, float OriginalMiniMapRepresentWHByRice)
        {
            if (!_miniMapRepresentWHByRice.Equals(miniMapRepresentWHByRice))
            {
                _rate = rate;
                UpdateGird(miniMapRepresentWHByRice, OriginalMiniMapRepresentWHByRice);
            }

            if (_selfPlayPos.Equals(selfPlayPos) && _miniMapRepresentWHByRice.Equals(miniMapRepresentWHByRice)) return;
            _selfPlayPos = selfPlayPos;
            _miniMapRepresentWHByRice = miniMapRepresentWHByRice;

            var offsetX = (selfPlayPos.x - miniMapRepresentWHByRice / 2) % gridHundredInterval;
            var offsetY = (selfPlayPos.y - miniMapRepresentWHByRice / 2) % gridHundredInterval;
            var girdNum = (_miniMapRepresentWHByRice / gridHundredInterval);
            var pX = (offsetX) / girdNum / gridHundredInterval;
            var pY = (offsetY) / girdNum / gridHundredInterval;

            gridHundredImage.material.SetFloat("_offSetX", pX);
            gridHundredImage.material.SetFloat("_offSetY", pY);

            offsetX = (selfPlayPos.x - miniMapRepresentWHByRice / 2) % gridThousandInterval;
            offsetY = (selfPlayPos.y - miniMapRepresentWHByRice / 2) % gridThousandInterval;
            girdNum = (_miniMapRepresentWHByRice / gridThousandInterval);
            pX = (offsetX) / girdNum / gridThousandInterval;
            pY = (offsetY) / girdNum / gridThousandInterval;

            gridThousandImage.material.SetFloat("_offSetX", pX);
            gridThousandImage.material.SetFloat("_offSetY", pY);
        }
    }
}