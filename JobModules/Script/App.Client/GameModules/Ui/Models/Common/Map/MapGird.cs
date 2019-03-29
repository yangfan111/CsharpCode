using System.Collections.Generic;
using App.Client.GameModules.Ui.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class MapGird
    {
        public int gridThousandInterval = 1000;
        private int pixelOffSet = 1;
        private Transform tran;
        private RectTransform rectTransform;
//        private Transform imgTf;
        private Transform rowLineModel;
        private Transform colLineModel;

        private Transform rowGirdModel;
        private Transform colGirdModel;

        private List<Transform> rowlines = new List<Transform>();
        private List<Transform> collines = new List<Transform>();

        private List<Transform> rowGirds = new List<Transform>();
        private List<Transform> colGirds = new List<Transform>();

        public MapGird(Transform tran)
        {
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            this.rectTransform = tran.GetComponent<RectTransform>();
//            imgTf = tran.Find("Imag");
            rowLineModel = tran.Find("rowLineModel");
            colLineModel = tran.Find("colLineModel");

            rowGirdModel = tran.Find("rowGirdModel");
            colGirdModel = tran.Find("colGirdModel");
        }

        private void UpdateGird()
        {
            var rect = rectTransform.rect;
            var rawNum = Mathf.CeilToInt(_mapRealWHByRice / gridThousandInterval);
            var cloNum = Mathf.CeilToInt(_mapRealWHByRice / gridThousandInterval);
            int i = 0;
            for (; i < rawNum; i++)
            {
                Transform line = null;
                if (rowGirds.Count <= i)
                {
                    line = GameObject.Instantiate(rowGirdModel, tran, true);
                    rowGirds.Add(line);
                }
                else
                {
                    line = rowGirds[i];
                }
                var lineRectTf = line.GetComponent<RectTransform>();
                lineRectTf.anchoredPosition = new Vector2(gridThousandInterval * i * _rate + pixelOffSet, 0);
                lineRectTf.sizeDelta = new Vector2(gridThousandInterval - 100, _mapRealWHByRice);
                lineRectTf.localScale = new Vector3(rect.width / _mapRealWHByRice, 1,1);
                UIUtils.SetActive(line, true);
            }

            while (i < rowGirds.Count)
            {
                UIUtils.SetActive(rowGirds[i++], false);
            }

            for (i = 0; i < cloNum; i++)
            {
                Transform line = null;
                if (colGirds.Count <= i)
                {
                    line = GameObject.Instantiate(colGirdModel, tran, true);
                    colGirds.Add(line);
                }
                else
                {
                    line = colGirds[i];
                }
                var lineRectTf = line.GetComponent<RectTransform>();
                lineRectTf.anchoredPosition = new Vector2(0, gridThousandInterval * i * _rate + pixelOffSet);
                lineRectTf.sizeDelta = new Vector2(_mapRealWHByRice, gridThousandInterval - 100);
                lineRectTf.localScale = new Vector3(1, rect.height / _mapRealWHByRice, 1);
                UIUtils.SetActive(line, true);
            }
            while (i < colGirds.Count)
            {
                UIUtils.SetActive(colGirds[i++], false);
            }
        }
        private void UpdateLine()
        {
            var rect = rectTransform.rect;
            var rawNum = Mathf.FloorToInt(rect.width / _rate / gridThousandInterval);
            var cloNum = Mathf.FloorToInt(rect.height / _rate / gridThousandInterval);
            int i = 1;
            for (; i < rawNum; i++)
            {
                Transform line = null;
                if (rowlines.Count < i)
                {
                    line = GameObject.Instantiate(rowLineModel, tran, true);
                    rowlines.Add(line);
                }
                else
                {
                    line = rowlines[i];
                }

                line.GetComponent<RectTransform>().anchoredPosition = new Vector2(gridThousandInterval * i * _rate, 0);
                UIUtils.SetActive(line, true);
            }

            while (i < rowlines.Count)
            {
                UIUtils.SetActive(rowlines[i++], false);
            }

            for (i = 1; i < cloNum; i++)
            {
                Transform line = null;
                if (collines.Count < i)
                {
                    line = GameObject.Instantiate(colLineModel, tran, true);
                    collines.Add(line);
                }
                else
                {
                    line = collines[i];
                }

                line.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, gridThousandInterval * i * _rate);
                UIUtils.SetActive(line, true);
            }
            while (i < collines.Count)
            {
                UIUtils.SetActive(collines[i++], false);
            }
        }

        private void UpdateSate()
        {
            var wr = Mathf.FloorToInt((-_miniMapRepresentWHByRice * _rate / 2 + _selfPlayPos.x) / gridThousandInterval);
            var wl = Mathf.FloorToInt((_miniMapRepresentWHByRice * _rate / 2 + _selfPlayPos.x) / gridThousandInterval);
            if (wr < 0) wr = 0;
            int i = 0;
            while (i < rowGirds.Count)
            {
                UIUtils.SetActive(rowGirds[i], i == wr || i == wl);
                i++;
            }
            i = 0;
            while (i < rowlines.Count)
            {
                UIUtils.SetActive(rowlines[i], i >= wr && i <= wl);
                i++;
            }

            var hr = Mathf.FloorToInt((-_miniMapRepresentWHByRice * _rate / 2 + _selfPlayPos.y) / gridThousandInterval);
            var hl = Mathf.FloorToInt((_miniMapRepresentWHByRice * _rate / 2 + _selfPlayPos.y) / gridThousandInterval);
            if (hr < 0) hr = 0;
            i = 0;
            while (i < colGirds.Count)
            {
                UIUtils.SetActive(colGirds[i], i == hr || i == hl);
                i++;
            }
            i = 0;
            while (i < collines.Count)
            {
                UIUtils.SetActive(collines[i], i >= hr && i <= hl);
                i++;
            }
        }

        private float _rate;
        private float _mapRealWHByRice;
        private Vector2 _selfPlayPos = Vector2.zero;
        private float _miniMapRepresentWHByRice;
        public void Update(float rate, float mapRealWHByRice, Vector2 selfPlayPos, float miniMapRepresentWHByRice)
        {
            if (!_rate.Equals(rate) && !_mapRealWHByRice.Equals(mapRealWHByRice))
            {
                _rate = rate;
                _mapRealWHByRice = mapRealWHByRice;
                UpdateGird();
                UpdateLine();
            }
            

            if (_selfPlayPos.Equals(selfPlayPos) && _miniMapRepresentWHByRice.Equals(miniMapRepresentWHByRice)) return;
            _selfPlayPos = selfPlayPos;
            _miniMapRepresentWHByRice = miniMapRepresentWHByRice;
            UpdateSate();
        }
    }
}