// 
// Created 8/26/2015 00:23:25
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using UnityEngine;
using Object = UnityEngine.Object;


namespace StatsMonitor.Core
{
	/// <summary>
	///		A wrapper class for Texture2D that makes it easier to work with a
	///		Texture2D used in UI.
	/// </summary>
	public class Bitmap2D
	{
		// ----------------------------------------------------------------------------
		// Properties
		// ----------------------------------------------------------------------------

		public Texture2D texture;
		public Color color;

		protected Rect _rect;


		// ----------------------------------------------------------------------------
		// Constructor
		// ----------------------------------------------------------------------------

		/// <summary>
		/// Bitmap2D
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="color"></param>
		public Bitmap2D(int width, int height, Color? color = null)
		{
			texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
			texture.filterMode = FilterMode.Bilinear;
			_rect = new Rect(0, 0, width, height);
			this.color = color ?? Color.black;
			Clear();
		}


		/// <summary>
		/// Bitmap2D
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="color"></param>
		public Bitmap2D(float width, float height, Color? color = null)
		{
			texture = new Texture2D((int)width, (int)height, TextureFormat.ARGB32, false);
			texture.filterMode = FilterMode.Point;
			this.color = color ?? Color.black;
			Clear();
		}


		// ----------------------------------------------------------------------------
		// Public Methods
		// ----------------------------------------------------------------------------

		public void Resize(int width, int height)
		{
			texture.Resize(width, height);
			texture.Apply();
		}


		/// <summary>
		///		Clears the Bitmap2D by filling it with a given color. If color is null
		///		the default color is being used.
		/// </summary>
		/// <param name="color"></param>
		public void Clear(Color? color = null)
		{
			Color c = color ?? this.color;
			Color[] a = texture.GetPixels();
			int i = 0;
			while (i < a.Length) a[i++] = c;
			texture.SetPixels(a);
			texture.Apply();
		}


		/// <summary>
		///		Fills an area in the Bitmap2D with a given color. If rect is null the
		///		whole bitmap is filled. If color is null the default color is used.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="color"></param>
		public void FillRect(Rect? rect = null, Color? color = null)
		{
			Rect r = rect ?? _rect;
			Color c = color ?? this.color;
			Color[] a = new Color[(int)(r.width * r.height)];
			int i = 0;
			while (i < a.Length) a[i++] = c;
			texture.SetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height, a);
		}


		/// <summary>
		///		Fills an area in the Bitmap2D with a given color. If rect is null the
		///		whole bitmap is filled. If color is null the default color is used.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <param name="color"></param>
		public void FillRect(int x, int y, int w, int h, Color? color = null)
		{
			Color c = color ?? this.color;
			Color[] a = new Color[w * h];
			int i = 0;
			while (i < a.Length) a[i++] = c;
			texture.SetPixels(x, y, w, h, a);
		}


		/// <summary>
		///		Fills a one pixel column in the bitmap.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="color"></param>
		public void FillColumn(int x, Color? color = null)
		{
			FillRect(new Rect(x, 0, 1, texture.height), color);
		}


		public void FillColumn(int x, int y, int height, Color? color = null)
		{
			FillRect(new Rect(x, y, 1, height), color);
		}


		/// <summary>
		///		Fills a one pixel row in the bitmap.
		/// </summary>
		/// <param name="y"></param>
		/// <param name="color"></param>
		public void FillRow(int y, Color? color = null)
		{
			FillRect(new Rect(0, y, texture.width, 1), color);
		}


		/// <summary>
		///		Sets a pixel at x, y.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		public void SetPixel(int x, int y, Color color)
		{
			texture.SetPixel(x, y, color);
		}


		/// <summary>
		///		Sets a pixel at x, y.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		public void SetPixel(float x, float y, Color color)
		{
			texture.SetPixel((int)x, (int)y, color);
		}


		///  <summary>
		/// 		Scrolls the bitmap by a certain amount of pixels.
		///  </summary>
		///  <param name="x"></param>
		///  <param name="y"></param>
		/// <param name="fillColor"></param>
		public void Scroll(int x, int y, Color? fillColor = null)
		{
			// TO DO Add vertical scrolling!
			int sx = 0;
			int tx = x;
			int fx = 0;

			if (x < 0)
			{
				x = sx = ~x + 1;
				tx = 0;
				fx = texture.width - x;
			}

			Color[] a = texture.GetPixels(sx, y, texture.width - x, texture.height - y);
			texture.SetPixels(tx, 0, texture.width - x, texture.height - y, a);
			FillRect(fx, 0, x, texture.height, fillColor);
		}


		/// <summary>
		///		Applies changes to the bitmap.
		/// </summary>
		public void Apply()
		{
			texture.Apply();
		}


		public void Dispose()
		{
			Object.Destroy(texture);
		}
	}
}
