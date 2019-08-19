using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Core.Utils
{
    class ColorUtils
    {
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
    }
}
