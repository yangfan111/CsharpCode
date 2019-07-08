// 
// Created 8/26/2015 21:10:09
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using StatsMonitor.Core;
using StatsMonitor.Util;
using UnityEngine;
using UnityEngine.UI;


namespace StatsMonitor.View
{
	internal class SysInfoView : View2D
	{
		// ----------------------------------------------------------------------------
		// Properties
		// ----------------------------------------------------------------------------

		private readonly StatsMonitor _statsMonitor;
		private int _width;
		private int _height;
		private Text _text;
		private bool _isDirty;


		// ----------------------------------------------------------------------------
		// Constructor
		// ----------------------------------------------------------------------------

		internal SysInfoView(StatsMonitor statsMonitor)
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
			if (!_isDirty) return;

			string s = ""
				+ "<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorSysInfoOdd)
				+ ">OS:" + SystemInfo.operatingSystem
				+ "</color>\n<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorSysInfoEven)
				+ ">CPU:" + SystemInfo.processorType
				+ " [" + SystemInfo.processorCount + " cores]"
				+ "</color>\n<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorSysInfoOdd) 
				+ ">GRAPHICS:" + SystemInfo.graphicsDeviceName
				+ "\nAPI:" + SystemInfo.graphicsDeviceVersion
				+ "\nShader Level:" + SystemInfo.graphicsShaderLevel
				+ ", Video RAM:" + SystemInfo.graphicsMemorySize + " MB"
				+ "</color>\n<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorSysInfoEven)
				+ ">SYSTEM RAM:" + SystemInfo.systemMemorySize + " MB"
				+ "</color>\n<color=#" + Util.Utils.Color32ToHex(_statsMonitor.colorSysInfoOdd) 
				+ ">SCREEN:" + Screen.currentResolution.width + " x "
			    + Screen.currentResolution.height
			    + " @" + Screen.currentResolution.refreshRate + "Hz"
			    + ",\nwindow size:" + Screen.width + " x " + Screen.height
			    + " " + Screen.dpi + "dpi</color>";

			_text.text = s;
			_height = _statsMonitor.padding + (int)_text.preferredHeight + _statsMonitor.padding;

			Invalidate(ViewInvalidationType.Layout);

			/* Invalidate stats monitor once more to update correct height but don't
			 * reinvalidate children or we'd be stuck in a loop! */
			_statsMonitor.Invalidate(ViewInvalidationType.Layout, InvalidationFlag.Text, false);

			_isDirty = false;
		}


		public override void Dispose()
		{
			Destroy(_text);
			_text = null;
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
			GameObject container = new GameObject();
			container.name = "SysInfoView";
			container.transform.SetParent(_statsMonitor.transform,false);

			var g = new GraphicsFactory(container, _statsMonitor.colorFPS, _statsMonitor.fontFace, _statsMonitor.fontSizeSmall);
			_text = g.Text("Text", "", null, 0, null, false);

			return container;
		}


		protected override void UpdateStyle()
		{
			_text.font = _statsMonitor.fontFace;
			_text.fontSize = _statsMonitor.FontSizeSmall;
			if (_statsMonitor.colorOutline.a > 0.0f)
				GraphicsFactory.AddOutlineAndShadow(_text.gameObject, _statsMonitor.colorOutline);
			else
				GraphicsFactory.RemoveEffects(_text.gameObject);
			_isDirty = true;
		}


		protected override void UpdateLayout()
		{
			int padding = _statsMonitor.padding;

			_text.rectTransform.anchoredPosition = new Vector2(padding, -padding);
			_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _width - (padding * 2));
			_height = padding + (int)_text.preferredHeight + padding;
			SetRTransformValues(0, 0, _width, _height, Vector2.one);
			_isDirty = true;
		}
	}
}
