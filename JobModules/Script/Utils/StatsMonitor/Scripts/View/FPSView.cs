// 
// Created 8/27/2015 12:26:08
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
	///		View class that displays only an FPS counter.
	/// </summary>
	internal class FPSView : View2D
	{
		// ----------------------------------------------------------------------------
		// Properties
		// ----------------------------------------------------------------------------

		private readonly StatsMonitor _statsMonitor;
		private Text _text;
		private string[] _fpsTemplates;


		// ----------------------------------------------------------------------------
		// Constructor
		// ----------------------------------------------------------------------------

		internal FPSView(StatsMonitor statsMonitor)
		{
			_statsMonitor = statsMonitor;
			Invalidate();
		}


		// ----------------------------------------------------------------------------
		// Public Methods
		// ----------------------------------------------------------------------------

		public override void Reset()
		{
			_text.text = "";
		}


		public override void Update()
		{
			_text.text = _fpsTemplates[_statsMonitor.fpsLevel] + _statsMonitor.fps + "FPS</color>";
		}


		public override void Dispose()
		{
			Destroy(_text);
			_text = null;
			base.Dispose();
		}


		// ----------------------------------------------------------------------------
		// Protected & Private Methods
		// ----------------------------------------------------------------------------

		protected override GameObject CreateChildren()
		{
			_fpsTemplates = new string[3];

			GameObject container = new GameObject();
			container.name = "FPSView";
			container.transform.SetParent(_statsMonitor.transform,false);

			var g = new GraphicsFactory(container, _statsMonitor.colorFPS, _statsMonitor.fontFace, _statsMonitor.fontSizeSmall);
			_text = g.Text("Text", "000FPS");
			_text.alignment = TextAnchor.MiddleCenter;

			return container;
		}


		protected override void UpdateStyle()
		{
			_text.font = _statsMonitor.fontFace;
			_text.fontSize = _statsMonitor.FontSizeLarge;
			_text.color = _statsMonitor.colorFPS;

			if (_statsMonitor.colorOutline.a > 0.0f)
				GraphicsFactory.AddOutlineAndShadow(_text.gameObject, _statsMonitor.colorOutline);
			else
				GraphicsFactory.RemoveEffects(_text.gameObject);

			_fpsTemplates[0] = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorFPS) + ">";
			_fpsTemplates[1] = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorFPSWarning) + ">";
			_fpsTemplates[2] = "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorFPSCritical) + ">";
		}


		protected override void UpdateLayout()
		{
			int padding = _statsMonitor.padding;
			_text.rectTransform.anchoredPosition = new Vector2(padding, -padding);

			/* Center the text object */
			_text.rectTransform.anchoredPosition = Vector2.zero;
			_text.rectTransform.anchorMin = _text.rectTransform.anchorMax = _text.rectTransform.pivot = new Vector2(0.5f, 0.5f);

			/* Update panel size with calculated dimensions. */
			int w = padding + (int)_text.preferredWidth + padding;
			int h = padding + (int)_text.preferredHeight + padding;
			/* Normalize width to even number to prevent texture glitches. */
			w = w % 2 == 0 ? w : w + 1;

			SetRTransformValues(0, 0, w, h, Vector2.one);
		}
	}
}
