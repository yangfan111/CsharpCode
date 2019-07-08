using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.Utils;
using Core.Ui.Map;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common.Map
{

    public class BioMark
    {
        enum EBioMarkType
        {
            Mother = 1,
            Hero,
            Hum,
            Supply
        }
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BioMark));

        private Transform _root;
        private RectTransform bioMotherRtf, bioHeroRtf, bioHumRtf,bioSupplyBoxRtf;

        private Dictionary<EBioMarkType, List<GameObject>> _markPool;

        private Dictionary<EBioMarkType, GameObject> _origGoDict;

        public BioMark(Transform tran)
        {
            UIUtils.SetActive(tran, true);
            this._root = tran;
            InitBioGroup(tran);
            InitPool();
        }

        private void InitPool()
        {
            _markPool = new Dictionary<EBioMarkType, List<GameObject>>();

            foreach(EBioMarkType type in Enum.GetValues(typeof(EBioMarkType)))
            {
                _markPool.Add(type,new List<GameObject>());
            }

            _origGoDict = new Dictionary<EBioMarkType, GameObject>();

            _origGoDict.Add(EBioMarkType.Mother, bioMotherRtf.gameObject);
            _origGoDict.Add(EBioMarkType.Hero, bioHeroRtf.gameObject);
            _origGoDict.Add(EBioMarkType.Hum, bioHumRtf.gameObject);
            _origGoDict.Add(EBioMarkType.Supply, bioSupplyBoxRtf.gameObject);
        }

        private void InitBioGroup(Transform tran)
        {
            var root = tran;
            try
            {
                bioMotherRtf = root.Find("MotherLabel") as RectTransform;
                bioHeroRtf = root.Find("HeroLabel") as RectTransform;
                bioHumRtf = root.Find("HumLabel") as RectTransform;
                bioSupplyBoxRtf = root.Find("SupplyLabel") as RectTransform;
                UIUtils.SetActive(bioMotherRtf, false);
                UIUtils.SetActive(bioHeroRtf, false);
                UIUtils.SetActive(bioHumRtf, false);
                UIUtils.SetActive(bioSupplyBoxRtf, false);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private float _rate;

        public void UpdateBioLabel(List<Vector3> motherPos, List<Vector3> heroPos, List<Vector3> humanPos,Dictionary<string, MapFixedVector3> supplyBoxPos, float rate)
        {
            UpdateBioLabel(motherPos, rate, EBioMarkType.Mother);
            UpdateBioLabel(heroPos, rate, EBioMarkType.Hero);
            UpdateBioLabel(humanPos, rate, EBioMarkType.Hum);
            UpdateBioLabel(supplyBoxPos, rate, EBioMarkType.Supply);
        }

        private void UpdateBioLabel(List<Vector3> posList, float rate, EBioMarkType type)
        {
            int count = posList.Count;
            var goList = GetMark(type, count);

            for (int i = 0; i < count; i++)
            {
                ((goList[i].transform) as RectTransform).anchoredPosition = new Vector2(posList[i].x, posList[i].z) * rate;
            }
        }

        private void UpdateBioLabel(Dictionary<string, MapFixedVector3> posDict, float rate, EBioMarkType type) {
            int count = posDict.Count;
            var goList = GetMark(type, count);
            Dictionary<string, MapFixedVector3>.Enumerator it = posDict.GetEnumerator();
            int i = 0;
            while (it.MoveNext()) {
                var pos = it.Current.Value.ShiftedUIVector3();
                ((goList[i].transform) as RectTransform).anchoredPosition = new Vector2(pos.x, pos.z) * rate;
                i++;
            }
        }

        private void UpdateBioLabel(List<MapFixedVector3> posList, float rate, EBioMarkType type)
        {
            int count = posList.Count;
            var goList = GetMark(type, count);

            for (int i = 0; i < count; i++)
            {
                var pos = posList[i].ShiftedUIVector3();
                ((goList[i].transform) as RectTransform).anchoredPosition = new Vector2(pos.x, pos.z) * rate;
            }

        }

        #region CreateMark

        private List<GameObject> GetMark(EBioMarkType type,int len)
        {
            var list = _markPool[type];
            int needAddCount = 0;
            int needHideCount = 0;
            int listCount = list.Count;
            if (len > listCount)
            {
                needAddCount = -list.Count + len;
            }
            else
            {
                needHideCount = list.Count - len;
            }

            for (int i = 0; i < needAddCount; i++)
            {
                CreateMark(type);
            }

            for (int j = 0; j < needHideCount; j++)
            {
                UIUtils.SetActive(list[listCount - j - 1], false);
            }

            for (int k = 0; k < len; k++)
            {
                UIUtils.SetActive(list[k], true);
            }

            return list;
        }

        private GameObject CreateMark(EBioMarkType type)
        {
            var origTf = _origGoDict[type];
            var dict = _markPool[type];
            var newGo = UnityEngine.Object.Instantiate(origTf, _root, false);
            dict.Add(newGo);
            return newGo;
        }

        #endregion
    }
}
