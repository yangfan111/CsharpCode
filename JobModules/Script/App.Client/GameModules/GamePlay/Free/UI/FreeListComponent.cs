using System;
using System.Collections.Generic;
using App.Protobuf;
using Assets.Sources.Free.Data;
using Assets.Sources.Free.Render;
using Assets.Sources.Free.Utility;
using Free.framework;
using UnityEngine;

namespace Assets.Sources.Free.UI
{
    public class FreeListComponent : FreeBaseComponent, IFreeComponent, IComponentGroup
    {
        private IList<IFreeComponent> list;
        private int _row;
        private int _colum;
        private readonly GameObject sprite;

        private int _height;
        private int _width;

        private float _deltaY;

        private IList<int> index;

        private IList<List<IFreeComponent>> llist;

        public FreeListComponent()
        {
            sprite = new GameObject("List");
            sprite.AddComponent<RectTransform>();
            sprite.AddComponent<CanvasRenderer>();

            _uiObject = new UnityUiObject(sprite);
            list = new List<IFreeComponent>();
            llist = new List<List<IFreeComponent>>();

            index = new List<int>();
            for (var i = 0; i < 8; i++)
                index.Add(0);
        }

        public int Type
        {
            get
            {
                return TYPE_LIST;
            }
        }

        public int ValueType
        {
            get
            {
                return SimpleFreeUI.DATA_SP;
            }
        }

        public override void Frame(IUIDataManager uiDataManager, int frameTime)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var fc = list[i];
                fc.Frame(uiDataManager, frameTime);
            }
        }

        public override void SetValue(params object[] vs)
        {
            var value = vs[0] as string;

            for (var i = 0; i < list.Count; i++)
            {
                var free = list[i];
                free.SetValue(0, "");
            }

            if (string.IsNullOrEmpty(value) || value == "null")
                return;
            var ss = value.Split("_");

            var sp = (SimpleProto)vs[1];

            for (var i = 0; i < Math.Min(ss.Length, _row); i++)
            {
                var index = Convert.ToInt32(ss[i]);
                var subSp = sp.Ps[index];

                SimpleFreeUI.SetValue(Key, subSp, llist[i]);
            }

            foreach(Transform tr in sprite.GetComponentsInChildren<Transform>(true))
            {
                tr.gameObject.SetActive(true);
            }
        }

        protected override void SetPureValue(string v)
        {
        }

        public void Initial(params object[] ini)
        {
            if (ini[0] is string)
            {
                var ss = (ini[0] as string).Split("_");
                if (ss.Length == 2)
                {

                    _colum = int.Parse(ss[0]);
                    _row = int.Parse(ss[1]);

                    _deltaY = _height / _row;

                }
            }
            else if (ini[0] is SimpleProto)
            {
                var simpleProto = (SimpleProto)ini[0];
#pragma warning disable CS0219 // The variable 'lCount' is assigned but its value is never used
                var lCount = 0;
#pragma warning restore CS0219 // The variable 'lCount' is assigned but its value is never used
                for (var j = 0; j < _row; j++)
                {
                    var subList = new List<IFreeComponent>();
                    for (var i = 0; i < simpleProto.Ks.Count - 1; i++)
                    {
                        var newPo = FreeUIUtil.GetInstance().GetComponent(simpleProto.Ks[i + 1]);

                        var control = newPo.ToUI();

                        list.Add(newPo);
                        control.gameObject.transform.parent = sprite.transform;
                        subList.Add(newPo);

                        newPo.SetPos(this, simpleProto.Fs[i * 2], simpleProto.Fs[i * 2 + 1] + _deltaY * j,
                                                    simpleProto.Ins[i * 4], simpleProto.Ins[i * 4 + 1],
                                                    simpleProto.Ins[i * 4 + 2], simpleProto.Ins[i * 4 + 3]);
                        newPo.Initial(simpleProto.Ss[i * 3 + 2], simpleProto.Ss[i * 3 + 3]);
                        newPo.EventKey = simpleProto.Ss[i * 3 + 4];
                    }

                    llist.Add(subList);
                }
            }
        }

        public override void SetPos(IComponentGroup freeUI, float x, float y, int width, int height, int relative, int parent)
        {
            NewSetPos(freeUI, x, y, width, height, relative, parent);

            _uiObject.width = width;
            _uiObject.height = height;

            _width = width;
            _height = height;
        }

        //public void SetPos(IComponentGroup freeUI, float x, float y, int width, int height, int relative, int parent)
        //{
        //    base.SetPos(freeUI, x, y, width, height, relative, parent);


        //    _width = width;
        //    _height = height;
        //}


        public IFreeComponent Clone()
        {
            return new FreeListComponent();
        }

        public int ComponentCount
        {
            get { return _colum * _row; }
        }

        public IFreeComponent GetComponent(int index)
        {
            if (index < list.Count)
                return list[index];
            return null;
        }
    }

}
