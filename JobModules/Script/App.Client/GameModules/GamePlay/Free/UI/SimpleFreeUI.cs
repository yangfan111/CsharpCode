using System.Collections.Generic;
using App.Protobuf;
using Assets.Sources.Free.Data;
using Assets.Sources.Free.Render;
using Free.framework;
using UnityEngine;
using Utils.Singleton;

namespace Assets.Sources.Free.UI
{
    public class SimpleFreeUI : MonoBehaviour, IComponentGroup, IUIUpdater
    {

        public const int DATA_INT = 1;
        public const int DATA_STRING = 2;
        public const int DATA_BOOL = 3;
        public const int DATA_FLOAT = 4;
        public const int DATA_DOUBLE = 5;
        public const int DATA_LONG = 6;
        public const int DATA_SP = 7;

        private const int UI_SHOW = 51;
        private const int UI_VALUE = 52;

        private string _key;

        private IList<IFreeComponent> _components;

        private int _totalTime;
        public bool AtBottom;

        private IShowStyle _showStyle;

        private int _currentTime;

        public float OrignalX;
        public float OrignalY;
        public int OrignalWidth;
        public int OrignalHeight;
        private IUiObject _uiObject;

        private int _screenWidth;
        private int _screenHeight;

        void Awake()
        {
            _components = new List<IFreeComponent>();
            SingletonManager.Get<SimpleUIUpdater>().Add(this);

            _uiObject = new UnityUiObject(gameObject);

            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
        }

        public void OnDestroy()
        {
            for (var index = 0; index < _components.Count; index++)
            {
                var comp = _components[index];
                comp.Destroy();
            }

            SingletonManager.Get<SimpleUIUpdater>().Remove(this);
        }

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public bool Visible
        {
            get { return gameObject.activeSelf; }
            set
            {
                gameObject.SetActive(value);
                //foreach (Transform tr in gameObject.GetComponentsInChildren<Transform>(true))
                //{
                //    tr.gameObject.SetActive(value);
                //}
            }
        }

        public float X
        {
            get { return _uiObject.x; }
            set { _uiObject.x = value; }
        }

        public float Y
        {
            get { return _uiObject.y; }
            set { _uiObject.y = value; }
        }

        public int Width
        {
            get { return _uiObject.width; }
            set { _uiObject.width = value; }
        }

        public int Height
        {
            get { return _uiObject.height; }
            set { _uiObject.height = value; }
        }

        public float Alpha
        {
            get { return _uiObject.alpha; }
            set { _uiObject.alpha = value; }
        }

        public float ScaleX
        {
            get { return _uiObject.scaleX; }
            set { _uiObject.scaleX = value; }
        }

        public float ScaleY
        {
            get { return _uiObject.scaleY; }
            set { _uiObject.scaleY = value; }
        }

        public void ResetEvent()
        {
            for (var index = 0; index < _components.Count; index++)
            {
                var fc = _components[index];
                fc.FreeUIEvent.ResetEvent();
            }
        }

        public FreeUIEvent GetFreeUiEvent()
        {
            for (var index = 0; index < _components.Count; index++)
            {
                var fc = _components[index];
                if (fc.FreeUIEvent.HasEvent())
                {
                    return fc.FreeUIEvent;
                }
            }
            return null;
        }

        public bool HasEvent
        {
            get
            {
                for (var index = 0; index < _components.Count; index++)
                {
                    var fc = _components[index];
                    if (!fc.IsNoMouse && !string.IsNullOrEmpty(fc.EventKey) && fc.EventKey != "null")
                    {
                        return true;
                    }
                }
                return false;
            }

        }

        public string EventKey
        {
            get
            {
                return "";
            }
        }

        public IList<IFreeComponent> FreeComponents
        {
            get
            {
                return _components;
            }
        }

        public void AddComponent(IFreeComponent component)
        {
            this._components.Add(component);
            component.ToUI().gameObject.transform.SetParent(transform, false);
        }

        /*
        key 代表哪个UI
        ks    ks[0]为数据长度，ks[1,2,...] 代表了每个数据的类型其中，1为整数，2为字符串，3为布尔，4为float，5为double，6为long
        ins   整数数据,前1-n个数据为每个component的自动化值，后面为正常的int数据
        fs     float 数据
        ds   double 数据
        ls    long 数据
        bs   布尔数据
        ss    字符串数据
        ps  为以上协议的数据形式
        数组形式
        */
        public static void SetValue(string key, SimpleProto simpleProto, IList<IFreeComponent> components)
        {
            var index = new List<int>();

            for (var i = 0; i < 8; i++)
            {
                index.Add(0);
            }
            for (var i = 1; i < simpleProto.Ks.Count; i++)
            {
                if (simpleProto.Ks[i] <= components.Count)
                {
                    var po = components[simpleProto.Ks[i] - 1];
                    var t = po.ValueType;
                    switch (t)
                    {
                        case DATA_INT:
                            po.SetValue(simpleProto.Ins[i - 1], simpleProto.Ins[components.Count + index[t]]);
                            break;
                        case DATA_STRING:
                            po.SetValue(simpleProto.Ins[i - 1], simpleProto.Ss[index[t] + 1]);
                            break;
                        case DATA_BOOL:
                            po.SetValue(simpleProto.Ins[i - 1], simpleProto.Bs[index[t]]);
                            break;
                        case DATA_FLOAT:
                            po.SetValue(simpleProto.Ins[i - 1], simpleProto.Fs[index[t]]);
                            break;
                        case DATA_DOUBLE:
                            po.SetValue(simpleProto.Ins[i - 1], simpleProto.Ds[index[t]]);
                            break;
                        case DATA_LONG:
                            po.SetValue(simpleProto.Ins[i - 1], simpleProto.Ls[index[t]]);
                            break;
                        case DATA_SP:
                            po.SetValue(simpleProto.Ss[index[t] + 1], simpleProto);
                            index[DATA_STRING] = index[DATA_STRING] + 1;
                            break;
                        default:
                            break;
                    }
                    index[t] = index[t] + 1;
                }
                else
                {
                    //                    ErrorEngineProxy.catchError(new Error("UI对象更新出错at '" + key + "', 总共"
                    //                            + components.length + "个组件，希望设置第" + simpleProto.ks[i] + "个的值"));
                }
            }
        }

        public void Show(SimpleProto sp)
        {
            _showStyle = FreeUIUtil.GetInstance().GetShowStyle(sp);
            _totalTime = sp.Ks[2];
            _currentTime = 0;
        }

        public void Show(int time)
        {
            _showStyle = new ShowSimpleStyle();
            _currentTime = 0;
            _totalTime = time;
        }

        public void SetValue(SimpleProto sp)
        {
            SetValue(Key, sp, _components);
        }

        public void OnFrame(IUIDataManager uiDataManager, int frameTime)
        {
            SingletonManager.Get<FreeUiManager>().Update();

            for (var i = 0; i < _components.Count; i++)
            {
                var po = _components[i];
                po.Frame(uiDataManager, frameTime);
            }
            _currentTime += frameTime;

            if (_showStyle != null)
            {
                _showStyle.Show(this, _currentTime, _totalTime);
            }

            //            if (_screenWidth != Screen.width || _screenHeight != Screen.height)
            //            {
            //                for (var i = 0; i < _components.Count; i++)
            //                {
            //                    var po = _components[i];
            //                    po.RefreshPosition();
            //                }
            //                _screenWidth = Screen.width;
            //                _screenHeight = Screen.height;
            //            }
        }

        public void OnFrameEnd()
        {

        }

        public int ComponentCount
        {
            get { return _components.Count; }
        }

        public bool IsDisabled { get; set; }

        public IFreeComponent GetComponent(int index)
        {
            if (index < _components.Count && index >= 0)
            {
                return _components[index];
            }
            return null;
        }

        public void UIUpdate(int frameTime)
        {
            OnFrame(SingletonManager.Get<UIDataManager>(), frameTime);
        }

        public int CloseDelay { get; set; }
    }

}
