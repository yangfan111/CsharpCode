using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtPlugins
{
    public class MagickTextureTool
    {
        private Color[] pixels;
        private int width, height;

        public MagickTextureTool(Texture2D tex)
        {
            if (tex == null)
            {
                Debug.LogError("MagickTextureTool error, tex is null");
                return;
            }

            Texture2D newTex = new Texture2D(tex.width, tex.height, tex.format, tex.mipmapCount > 1);
            Graphics.CopyTexture(tex, newTex);
            newTex.Apply(true);
            pixels = newTex.GetPixels();
            width = tex.width;
            height = tex.height;
            Object.DestroyImmediate(newTex);
        }

        public Color GetPixelBilinear(float u, float v)
        {
            if (pixels == null || pixels.Length == 0)
            {
                Debug.LogError("MagickTextureTool.GetPixelBilinear error, pixels is empty");
                return Color.black;
            }
            int w = Mathf.RoundToInt(u * width), h = Mathf.RoundToInt(v * height);
            if (w >= width) w = width - 1;
            if (h >= height) h = height - 1;
            Color color = pixels[h * width + w];
            return color;
        }
    }
}
