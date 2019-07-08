// 
// Created 9/1/2015 16:01:46
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using StatsMonitor.Core;
using StatsMonitor.Util;
using UnityEngine;
using UnityEngine.UI;


namespace StatsMonitor.View
{
	/// <summary>
	///		View class that displays the textual stats information.
	/// </summary>
	internal class StatsView : View2D
	{
		// ----------------------------------------------------------------------------
		// Properties
		// ----------------------------------------------------------------------------

		private readonly StatsMonitor _statsMonitor;

		private Text _text1;


		private string[] _fpsTemplates;
		private string _fpsMinTemplate;
		private string _fpsMaxTemplate;
		private string _fpsAvgTemplate;
		private string _fxuTemplate;
		private string _msTemplate;
		private string _objTemplate;
		private string _memTotalTemplate;
		private string _memAllocTemplate;
		private string _memMonoTemplate;


		// ----------------------------------------------------------------------------
		// Constructor
		// ----------------------------------------------------------------------------

		internal StatsView(StatsMonitor statsMonitor)
		{
			_statsMonitor = statsMonitor;
			Invalidate();
		}


		// ----------------------------------------------------------------------------
		// Public Methods
		// ----------------------------------------------------------------------------

		public override void Reset()
		{
			/* Clear all text fields. */
			_text1.text =  "";
		}


		public override void Update()
		{
			_text1.text = _fpsTemplates[_statsMonitor.fpsLevel] + _statsMonitor.fps + "</color>"+

				_fpsMinTemplate + (_statsMonitor.fpsMin > -1 ? _statsMonitor.fpsMin : 0) + "</color>  "
				+ _fpsMaxTemplate + (_statsMonitor.fpsMax > -1 ? _statsMonitor.fpsMax : 0) + "</color>\n"+
				_fpsAvgTemplate + _statsMonitor.fpsAvg + "</color> " + _msTemplate + "" + _statsMonitor.ms.ToString("F1") + "MS</color> "
				+ _fxuTemplate + _statsMonitor.fixedUpdateRate + " </color>\n"+
			//	+ _objTemplate + "OBJ:" + _statsMonitor.renderedObjectCount + "/" + _statsMonitor.renderObjectCount
			//	+ "/" + _statsMonitor.objectCount + "</color>";
				_memTotalTemplate + _statsMonitor.memTotal.ToString("F1") + "MB</color> "
				+ _memAllocTemplate + _statsMonitor.memAlloc.ToString("F1") + "MB</color> "
				+ _memMonoTemplate + _statsMonitor.memMono.ToString("F1") + "MB</color>";
		}


		public override void Dispose()
		{
			Destroy(_text1);
			_text1 = null;
			base.Dispose();
		}


		// ----------------------------------------------------------------------------
		// Protected & Private Methods
		// ----------------------------------------------------------------------------

		protected override GameObject CreateChildren()
		{
			_fpsTemplates = new string[3];

			GameObject container = new GameObject();
			container.name = "StatsView";
			container.transform.SetParent(_statsMonitor.transform,false);

			var g = new GraphicsFactory(container, _statsMonitor.colorFPS, _statsMonitor.fontFace, _statsMonitor.fontSizeSmall);
			_text1 = g.Text("Text1", "");
            var layout = container.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = true;
            layout.childControlWidth = true;

            return container;
		}


		protected override void UpdateStyle()
		{
			_text1.font = _statsMonitor.fontFace;
			_text1.fontSize = _statsMonitor.FontSizeLarge;

			if (_statsMonitor.colorOutline.a > 0.0f)
			{
				GraphicsFactory.AddOutlineAndShadow(_text1.gameObject, _statsMonitor.colorOutline);

			}
			else
			{
				GraphicsFactory.RemoveEffects(_text1.gameObject);

			}

			_fpsTemplates[0] = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorFPS) + ">FPS:";
			_fpsTemplates[1] = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorFPSWarning) + ">FPS:";
			_fpsTemplates[2] = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorFPSCritical) + ">FPS:";
			_fpsMinTemplate = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorFPSMin) + ">MIN:";
			_fpsMaxTemplate = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorFPSMax) + ">MAX:";
			_fpsAvgTemplate = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorFPSAvg) + ">AVG:";
			_fxuTemplate = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorFXD) + ">FXD:";
			_msTemplate = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorMS) + ">";
			_objTemplate = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorObjCount) + ">";
			_memTotalTemplate = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorMemTotal) + ">TOTAL:";
			_memAllocTemplate = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorMemAlloc) + ">ALLOC:";
			_memMonoTemplate = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorMemMono) + ">MONO:";
		}


		protected override void UpdateLayout()
		{
            if(string.IsNullOrEmpty(_text1.text))
            {
                _text1.text=_fpsTemplates[_statsMonitor.fpsLevel] + "000" + "</color>" +

                _fpsMinTemplate + "000" + "</color>  "
                + _fpsMaxTemplate + "000" + "</color>\n" +
                _fpsAvgTemplate + "00" + "</color> " + _msTemplate + "00000"   + "MS</color> "
                + _fxuTemplate + "0000" + " </color>\n" +
                //	+ _objTemplate + "OBJ:" + _statsMonitor.renderedObjectCount + "/" + _statsMonitor.renderObjectCount
                //	+ "/" + _statsMonitor.objectCount + "</color>";
                _memTotalTemplate + "000000" + "MB</color> "
                + _memAllocTemplate + "0000" + "MB</color> "
                + _memMonoTemplate + "00000" + "MB</color>";
            }
            else
            {
                _text1.text = _text1.text;
            }
           
            Canvas.ForceUpdateCanvases();
		}

	}
}
