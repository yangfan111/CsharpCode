// 
// Created 8/24/2015 20:27:31
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;


namespace StatsMonitor.Util
{
	/// <summary>
	///		Collection of various required util methods.
	/// </summary>
	public class Utils
	{
		// ----------------------------------------------------------------------------
		// Public Methods
		// ----------------------------------------------------------------------------

		/// <summary>
		///		Remove HTML from string with compiled Regex.
		/// </summary>
		public static string StripHTMLTags(string s)
		{
			return Regex.Replace(s, "<.*?>", string.Empty);
		}


		/// <summary>
		/// Creates a Color object from RGB values.
		/// </summary>
		public static Color RGBAToColor(float r, float g, float b, float a)
		{
			return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
		}


		/// <summary>
		///		Converts a Color32 object to a hex color string.
		/// </summary>
		public static string Color32ToHex(Color32 color)
		{
			return color.r.ToString("x2") + color.g.ToString("x2")
				+ color.b.ToString("x2") + color.a.ToString("x2");
		}


		/// <summary>
		///		Converts a RGBA hex color string into a Color32 object.
		/// </summary>
		public static Color HexToColor32(string hex)
		{
			if (hex.Length < 1) return Color.black;
			return new Color32(byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
				byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
				byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
				byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber));
		}


		/// <summary>
		///		Resets the transform of given object.
		/// </summary>
		public static void ResetTransform(GameObject obj)
		{
			obj.transform.position = Vector3.zero;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.rotation = Quaternion.identity;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
		}


		/// <summary>
		///		RectTransform's a given gameObject.
		/// </summary>
		public static RectTransform RTransform(GameObject obj, Vector2 anchor,
			float x = 0, float y = 0, float w = 0, float h = 0)
		{
			RectTransform t = obj.GetComponent<RectTransform>();
			if (t == null) t = obj.AddComponent<RectTransform>();
			t.pivot = t.anchorMin = t.anchorMax = anchor;
			t.anchoredPosition = new Vector2(x, y);
			if (w > 0.0f)
				t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
			if (h > 0.0f)
				t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
			return t;
		}


		/// <summary>
		///		Fills a Texture2D with a given color.
		/// </summary>
		public static void Fill(Texture2D texture, Color color)
		{
			Color[] a = texture.GetPixels();
			for (int i = 0; i < a.Length; i++)
			{
				a[i] = color;
			}
			texture.SetPixels(a);
			texture.Apply();
		}


		/// <summary>
		///		Adds the given game object to the UI layer if the layer exists.
		/// </summary>
		public static void AddToUILayer(GameObject obj)
		{
			int uiLayerID = LayerMask.NameToLayer("UI");
			if (uiLayerID > -1) obj.layer = uiLayerID;
		}


		/// <summary>
		///		Returns a scale factor based on 96dpi for the current running screen DPI.
		///		Return -1 if the screen DPI could not be detected. Will not return
		///		a factor that is lower than 1.0.
		/// </summary>
		public static float DPIScaleFactor(bool round = false)
		{
			float dpi = Screen.dpi;
			if (dpi <= 0) return -1.0f;
			float factor = dpi / 96.0f;
			if (factor < 1.0f) return 1.0f;
			return round ? Mathf.Round(factor) : factor;
		}
	}
}
