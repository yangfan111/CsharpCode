// 
// Created 8/25/2015 17:24:34
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using StatsMonitor.Core;
using StatsMonitor.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


namespace StatsMonitor.View
{
    /// <summary>
    ///		View class that displays the stats graph.
    /// </summary>
    internal class GraphView : View2D
    {
        // ----------------------------------------------------------------------------
        // Properties
        // ----------------------------------------------------------------------------


        private readonly StatsMonitor _statsMonitor;
        private RawImage _image;
        private Bitmap2D _graph;
        private int _oldWidth;
        private int _width;
        private int _height;
        private int _graphStartX;
        private int _graphMaxY;
        private int _memCeiling;
        private int _lastGCCollectionCount = -1;
        private Color?[] _fpsColors;

        public static Color[] customColor = new Color[]{
                Color.white,
                Util.Utils.HexToColor32("FFA000FF"),
                Util.Utils.HexToColor32("FF0000FF"),
                Util.Utils.HexToColor32("00C8DCFF"),
                Util.Utils.HexToColor32("C68D00FF"),
                Util.Utils.HexToColor32("00B270FF"),
                Util.Utils.HexToColor32("870000FF"),
                Util.Utils.HexToColor32("4080FFFF"),
                Util.Utils.HexToColor32("B480FFFF"),
                Util.Utils.HexToColor32("FF66D1FF")
        };

        // ----------------------------------------------------------------------------
        // Constructor
        // ----------------------------------------------------------------------------

        public GraphView(StatsMonitor statsMonitor)
        {
            _statsMonitor = statsMonitor;
            Invalidate();
        }


        // ----------------------------------------------------------------------------
        // Public Methods
        // ----------------------------------------------------------------------------

        public override void Reset()
        {
            if (_graph != null) _graph.Clear();
        }

        Vector3[] posCache;
        Vector3[] valueToPosCache;

        public override void Update()
        {

            if (_graph == null) return;
            
            if (fpsLine==null)
            {
                posCache = new Vector3[lineLength];
                fpsLine = CreateLineRenders();

                valueToPosCache = new Vector3[(int)((1.2)*lineLength)];
            }

            UpdateLineRender();
            var line = fpsLine;
            int color = 0;
            DrawLine(line,60, _statsMonitor.fps, _statsMonitor.fpsMax,posCache, color);
            color++;
            foreach (var item in _statsMonitor.profilerList)
            {
                var lineRender = profileLinerDic[item];
                float max = CalMaxValue(vector3Cache[lineRender],line);
                DrawLine(lineRender, 0, item.SampleValue(), max, vector3Cache[lineRender], color);
                color++;
            }
        }

        private void DrawLine(LineRenderer line,float baseValue,float value,float maxValue,Vector3[] pos,int color)
        {
            var count = line.GetPositions(valueToPosCache);
            var xOffset = _statsMonitor.space;
            for (int i = 0; i < count; i++)
            {
                pos[i] = new Vector3(pos[i].x - xOffset, pos[i].y);
            }
            if (count == pos.Length)
            {
                count = count - 1;
                for (int i = 0; i < pos.Length - 1; i++)
                {
                    pos[i] = pos[i + 1];
                }
            }
            else
            {
                line.positionCount += 1;
            }

            var x = Width-1;

            
            pos[count] = new Vector3(x, value, 0);
            CalYPos(maxValue,baseValue,pos,valueToPosCache);
            line.SetPositions(valueToPosCache);

            Color c = Color.white;
            if (color < customColor.Length)
            {
                c = customColor[color];
            }
            line.startColor = c;
            line.endColor = c;
        }

        void CalYPos(float maxValue,float baseValue,Vector3[] pos,Vector3[] cache)
        {
            for (int i = 0; i < pos.Length; i++)
            {
                var y = Mathf.Min(_graphMaxY, ((pos[i].y * 1.0f / (maxValue > baseValue ? maxValue : baseValue)) * _graphMaxY));
                cache[i] = new Vector3(pos[i].x,y, 0);
            }
        }

        float CalMaxValue(Vector3[] pos, LineRenderer line)
        {
            var count = line.positionCount;
            float max = 0;
            for (int i = 0; i < count; i++)
            {
                float value = pos[i].y;
                if (value>max)
                {
                    max = value;
                }
            }
            return max;
        }

        public override void Dispose()
        {
            if (_graph != null) _graph.Dispose();
            _graph = null;
            Destroy(_image);
            _image = null;
            base.Dispose();
        }


        // ----------------------------------------------------------------------------
        // Internal Methods
        // ----------------------------------------------------------------------------

        internal void SetWidth(float width)
        {
            _width = (int)width;
        }


        // ----------------------------------------------------------------------------
        // Protected & Private Methods
        // ----------------------------------------------------------------------------

        protected override GameObject CreateChildren()
        {
            _fpsColors = new Color?[3];

            GameObject container = new GameObject();
            container.name = "GraphView";
            container.transform.SetParent(_statsMonitor.transform,false);

            _graph = new Bitmap2D(10, 10, _statsMonitor.colorGraphBG);

            _image = container.AddComponent<RawImage>();
            _image.rectTransform.sizeDelta = new Vector2(10, 10);
            _image.color = Color.white;
            _image.texture = _graph.texture;

            /* Calculate estimated memory ceiling for application. */
            int sysMem = SystemInfo.systemMemorySize;
            if (sysMem <= 1024) _memCeiling = 512;
            else if (sysMem > 1024 && sysMem <= 2048) _memCeiling = 1024;
            else _memCeiling = 2048;

            gameObject = container;

            var element = container.AddComponent<LayoutElement>();
            element.preferredHeight = _statsMonitor.GraphHeight;
            return container;
        }


        protected override void UpdateStyle()
        {
            if (_graph != null) _graph.color = _statsMonitor.colorGraphBG;
            if (_statsMonitor.colorOutline.a > 0.0f)
                GraphicsFactory.AddOutlineAndShadow(_image.gameObject, _statsMonitor.colorOutline);
            else
                GraphicsFactory.RemoveEffects(_image.gameObject);
            _fpsColors[0] = null;
            _fpsColors[1] = new Color(_statsMonitor.colorFPSWarning.r, _statsMonitor.colorFPSWarning.g, _statsMonitor.colorFPSWarning.b, _statsMonitor.colorFPSWarning.a / 4);
            _fpsColors[2] = new Color(_statsMonitor.ColorFPSCritical.r, _statsMonitor.ColorFPSCritical.g, _statsMonitor.ColorFPSCritical.b, _statsMonitor.ColorFPSCritical.a / 4);
        }


        protected override void UpdateLayout()
        {
            /* Make sure that dimensions for text size are valid! */
            if ((_width > 0 && _statsMonitor.graphHeight > 0) && (_statsMonitor.graphHeight != _height || _oldWidth != _width))
            {
                _oldWidth = _width;

                _height = _statsMonitor.graphHeight;
                _height = _height % 2 == 0 ? _height : _height + 1;

                /* The X position in the graph for pixels to be drawn. */
                _graphStartX = _width - 1;
                _graphMaxY = _height - 1;

                _image.rectTransform.sizeDelta = new Vector2(_width, _height);
                _graph.Resize(_width, _height);
                _graph.Clear();

                SetRTransformValues(0, 0, _width, _height, Vector2.one);
            }
        }


        List<LineRenderer> lineRenderList = new List<LineRenderer>();
        LineRenderer fpsLine;
        Dictionary<IProfiler, LineRenderer> profileLinerDic = new Dictionary<IProfiler, LineRenderer>();
        Dictionary<LineRenderer, Vector3[]> vector3Cache = new Dictionary<LineRenderer, Vector3[]>();
        LineRenderer CreateLineRenders()
        {
            GameObject go = new GameObject("LineRender");
            Util.Utils.AddToUILayer(go); 
            go.AddComponent<SortingGroup>().sortingOrder = short.MaxValue;
            var l = go.AddComponent<LineRenderer>();
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            rect.pivot = Vector2.zero;
            l.useWorldSpace = false;
            l.alignment = LineAlignment.Local;
            l.sortingOrder = short.MaxValue + 1;
            l.startWidth = 2f;
            l.endWidth = 2f;
            go.transform.SetParent(gameObject.transform, false);
            l.material = Resources.Load("LineMaterial") as Material;
            l.positionCount = 0;

            return l;

        }

        public void UpdateLineRender()
        {

            foreach (var item in _statsMonitor.profilerList)
            {
                if(!profileLinerDic.ContainsKey(item))
                {
                    var line = CreateLineRenders();
                    profileLinerDic[item] = line;
                    vector3Cache[line] = new Vector3[lineLength];
                }
            }

            List<IProfiler> temp = new List<IProfiler>();

            foreach (var item in profileLinerDic)
            {
                if(!_statsMonitor.profilerList.Contains(item.Key))
                {
                    temp.Add(item.Key);
                    
                }
            }
            foreach (var item in temp)
            {
                Destroy(profileLinerDic[item].gameObject);
                profileLinerDic.Remove(item);
                
            }
            
        }

      
        int lineLength
        {
            get
            {
                if (Width > 0)
                    return (int)Width/_statsMonitor.space;
                return 400;
            }

        }


    }
}
