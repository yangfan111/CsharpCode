using StatsMonitor.Core;
using StatsMonitor.Util;
using UnityEngine;
using UnityEngine.UI;

namespace StatsMonitor.View
{
    class CustomInfoView :View2D
    {
        private readonly StatsMonitor _statsMonitor;
        private Text _text1;
        private int _width;
        private int _height;
        // ----------------------------------------------------------------------------
        // Constructor
        // ----------------------------------------------------------------------------

        internal CustomInfoView(StatsMonitor statsMonitor)
        {

            _statsMonitor = statsMonitor;
            Invalidate();

        }


        // ----------------------------------------------------------------------------
        // Public Methods
        // ----------------------------------------------------------------------------

        public override void Reset()
        {
            _text1.text = "";
        }


        public override void Update()
        {
            string content = "";
            int color = 1;
            foreach (var item in _statsMonitor.profilerList)
            {
                Color c = Color.white;
                if (color < GraphView.customColor.Length)
                {
                    c = GraphView.customColor[color];
                }
                content += "<color=#" + Util.Utils.Color32ToHex(c) + ">" + item.Name + ":" + item.SampleValue() + "</color>\n";
            }
            _text1.text = content;
        }


    

        // ----------------------------------------------------------------------------
        // Protected & Private Methods
        // ----------------------------------------------------------------------------

        protected override GameObject CreateChildren()
        {
            GameObject container = new GameObject();
            container.name = "StatsView";
            container.transform.SetParent(_statsMonitor.transform, false);
            var rect = container.AddComponent<RectTransform>();
            var g = new GraphicsFactory(container, _statsMonitor.colorFPS, _statsMonitor.fontFace, _statsMonitor.fontSizeSmall);
            _text1 = g.Text("Text1", "");
            var _image = container.AddComponent<RawImage>();
           
            _image.color = Util.Utils.HexToColor32("00314ABE");

            return container;
        }


        protected override void UpdateStyle()
        {
            _text1.font = _statsMonitor.fontFace;
            _text1.fontSize = _statsMonitor.FontSizeLarge;
        }

        internal void SetWidth(float width)
        {
            _width = (int)width;
        }

        protected override void UpdateLayout()
        {
            _height = (int)_text1.preferredHeight ;
            SetRTransformValues(0, 0, _width, _height, Vector2.one);
        }

        public override void Dispose()
        {
            Destroy(_text1);
            
            _text1 = null;
          
            base.Dispose();
        }
    }
}
