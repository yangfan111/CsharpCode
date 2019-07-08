// 
// GraphicsFactory.cs
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace StatsMonitor.Util
{
	/// <summary>
	///     A graphics factory class that can be used to quickly create various
	///     UnityEngine.UI.Graphic objects.
	/// </summary>
	public class GraphicsFactory
	{
		// ----------------------------------------------------------------------------
		// Properties
		// ----------------------------------------------------------------------------

		public GameObject parent;
		public Color defaultColor;
		public Font defaultFontFace;
		public int defaultFontSize;

		static public Vector2 defaultEffectDistance = new Vector2(1, -1);


		// ----------------------------------------------------------------------------
		// Constructor
		// ----------------------------------------------------------------------------

		/// <summary>
		///     Creates a GraphicsFactory instance.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="defaultColor"></param>
		/// <param name="defaultFontFace"></param>
		/// <param name="defaultFontSize"></param>
		public GraphicsFactory(GameObject parent, Color defaultColor,
			Font defaultFontFace = null, int defaultFontSize = 16)
		{
			this.parent = parent;
			this.defaultFontFace = defaultFontFace;
			this.defaultFontSize = defaultFontSize;
			this.defaultColor = defaultColor;
		}


		// ----------------------------------------------------------------------------
		// Public Methods
		// ----------------------------------------------------------------------------

		/// <summary>
		///     Creates an object of type UnityEngine.UI.Graphic, wraps it into a
		///     GameObject and applies a RectTransform to it, optionally setting
		///     defaultWidth and panelHeight if specified.
		/// </summary>
		/// <param name="name">Name for the graphic wrapper.</param>
		/// <param name="type">
		///     Type of the graphic class. Must be subclass of
		///     Graphic!
		/// </param>
		/// <param name="color"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <returns></returns>
		public Graphic Graphic(string name, Type type, float x = 0, float y = 0,
			float w = 0, float h = 0, Color? color = null)
		{
			GameObject wrapper = new GameObject();
			wrapper.name = name;
			wrapper.transform.SetParent(parent.transform,false);
			Graphic g = (Graphic) wrapper.AddComponent(type);
			g.color = color ?? defaultColor;
 
			RectTransform tr = wrapper.GetComponent<RectTransform>();
			if (tr == null) tr = wrapper.AddComponent<RectTransform>();
			tr.pivot = Vector2.up;
			tr.anchorMin = Vector2.up;
			tr.anchorMax = Vector2.up;
			tr.anchoredPosition = new Vector2(x, y);
			if (w > 0 && h > 0)
			{
				tr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
				tr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
			}

			return g;
		}


		/// <summary>
		///     Creates an object of type UnityEngine.UI.Image, wraps it into a
		///     GameObject and applies a RectTransform to it, optionally setting
		///     defaultWidth and panelHeight if specified.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="color"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <returns></returns>
		public Image Image(string name, float x = 0, float y = 0, float w = 0,
			float h = 0, Color? color = null)
		{
			return (Image) Graphic(name, typeof (Image), x, y, w, h, color);
		}


		/// <summary>
		///     Creates an object of type UnityEngine.UI.RawImage, wraps it into a
		///     GameObject and applies a RectTransform to it, optionally setting
		///     defaultWidth and panelHeight if specified.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public RawImage RawImage(string name, float x = 0, float y = 0, float w = 0,
			float h = 0, Color? color = null)
		{
			return (RawImage)Graphic(name, typeof(RawImage), x, y, w, h, color);
		}


		/// <summary>
		///     Creates an object of type UnityEngine.UI.Text, wraps it into a
		///     GameObject and applies a RectTransform to it, optionally setting
		///     defaultWidth and panelHeight if specified.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="color"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <param name="fontFace"></param>
		/// <param name="fontSize"></param>
		/// <param name="text"></param>
		/// <param name="fitH"></param>
		/// <param name="fitV"></param>
		/// <returns></returns>
		public Text Text(string name, float x = 0, float y = 0, float w = 0,
			float h = 0, string text = "", Color? color = null, int fontSize = 0,
			Font fontFace = null, bool fitH = false, bool fitV = false)
		{
			Text t = (Text)Graphic(name, typeof (Text), x, y, w, h, color);
			t.font = fontFace ?? defaultFontFace;
			t.fontSize = fontSize < 1 ? defaultFontSize : fontSize;
			if (fitH) t.horizontalOverflow = HorizontalWrapMode.Overflow;
			if (fitV) t.verticalOverflow = VerticalWrapMode.Overflow;
			t.text = text;
			if (fitH || fitV) FitText(t, fitH, fitV);
			return t;
		}


		///  <summary>
		/// 		Used to create a text field whose size adapts to the text content.
		///  </summary>
		///  <param name="name"></param>
		///  <param name="text"></param>
		///  <param name="color"></param>
		///  <param name="fontSize"></param>
		///  <param name="fontFace"></param>
		/// <param name="fitH"></param>
		/// <param name="fitV"></param>
		/// <returns></returns>
		public Text Text(string name, string text = "", Color? color = null, int fontSize = 0, Font fontFace = null, bool fitH = true, bool fitV = true)
		{
			Text t = (Text)Graphic(name, typeof(Text), 0, 0, 0, 0, color);
			t.font = fontFace ?? defaultFontFace;
			t.fontSize = fontSize < 1 ? defaultFontSize : fontSize;
			if (fitH) t.horizontalOverflow = HorizontalWrapMode.Overflow;
			if (fitV) t.verticalOverflow = VerticalWrapMode.Overflow;
			t.text = text;
			if (fitH || fitV) FitText(t, fitH, fitV);
			return t;
		}


		/// <summary>
		///		Adds a ContentSizeFitter to a given Text.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="h"></param>
		/// <param name="v"></param>
		public static void FitText(Text text, bool h, bool v)
		{
			ContentSizeFitter csf = text.gameObject.GetComponent<ContentSizeFitter>();
			if (csf == null) csf = text.gameObject.AddComponent<ContentSizeFitter>();
			if (h) csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			if (v) csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		}


		public static Outline AddOutline(GameObject obj, Color color, Vector2? distance = null)
		{
			Outline outline = obj.GetComponent<Outline>();
			if (outline == null) outline = obj.AddComponent<Outline>();
			outline.effectColor = color;
			outline.effectDistance = distance ?? defaultEffectDistance;
			return outline;
		}


		public static Shadow AddShadow(GameObject obj, Color color, Vector2? distance = null)
		{
			Shadow shadow = obj.GetComponent<Shadow>();
			if (shadow == null) shadow = obj.AddComponent<Shadow>();
			shadow.effectColor = color;
			shadow.effectDistance = distance ?? defaultEffectDistance;
			return shadow;
		}


		public static void AddOutlineAndShadow(GameObject obj, Color color, Vector2? distance = null)
		{
			Shadow shadow = obj.GetComponent<Shadow>();
			if (shadow == null) shadow = obj.AddComponent<Shadow>();
			shadow.effectColor = color;
			shadow.effectDistance = distance ?? defaultEffectDistance;
			Outline outline = obj.GetComponent<Outline>();
			if (outline == null) outline = obj.AddComponent<Outline>();
			outline.effectColor = color;
			outline.effectDistance = distance ?? defaultEffectDistance;
		}


		public static void RemoveEffects(GameObject obj)
		{
			Shadow shadow = obj.GetComponent<Shadow>();
			if (shadow != null) Object.Destroy(shadow);
			Outline outline = obj.GetComponent<Outline>();
			if (outline != null) Object.Destroy(outline);
		}
	}
}
