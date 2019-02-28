using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class FreeLayoutConverter
    {
        private const int LeftTop = 0;
        private const int RightTop = 1;
        private const int LeftBottom = 2;
        private const int RightBottom = 3;
        private const int LeftMiddle = 10;
        private const int RightMiddle = 11;
        private const int TopMiddle = 8;
        private const int BottomMiddle = 9;

        private static Vector2 LeftTopV = new Vector2(0, 1);
        private static Vector2 RightTopV = new Vector2(1, 1);
        private static Vector2 LeftBottomV = new Vector2(0, 0);
        private static Vector2 RightBottomV = new Vector2(1, 0);
        private static Vector2 LeftMiddleV = new Vector2(0, 0.5f);
        private static Vector2 RightMiddleV = new Vector2(1, 0.5f);
        private static Vector2 TopMiddleV = new Vector2(0.5f, 1);
        private static Vector2 BottomMiddleV = new Vector2(0.5f, 0);
        private static Vector2 CenterV = new Vector2(0.5f, 0.5f);

        public static void FullScreen(RectTransform rt)
        {
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.pivot = CenterV;
        }

        public static void Convert(int layout, float x, float y, int width, int height, RectTransform rt, RectTransform parent)
        {
            if (parent != null)
            {
                rt.SetParent(parent, false);
            }

            Vector2 point = GetPoint(layout);
            rt.anchorMin = point;
            rt.anchorMax = point;
            rt.pivot = LeftTopV;
            rt.localScale = new Vector3(1, 1, 1);

            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.sizeDelta = new Vector2(width, height);
            switch (layout)
            {
                case LeftTop:
                    rt.anchoredPosition = new Vector2(x, -y);
                    break;
                case RightTop:
                    rt.anchoredPosition = new Vector2(-x, -y);
                    break;
                case LeftBottom:
                    rt.anchoredPosition = new Vector2(x, y);
                    break;
                case RightBottom:
                    rt.anchoredPosition = new Vector2(-x, y);
                    break;
                case 4:
                    rt.anchoredPosition = new Vector2(-x, y);
                    break;
                case 5:
                case 6:
                case 7:
                    FullScreen(rt);
                    break;
                case TopMiddle:
                    rt.anchoredPosition = new Vector2(-x, -y);
                    break;
                case BottomMiddle:
                    rt.anchoredPosition = new Vector2(-x, y);
                    break;
                case LeftMiddle:
                    rt.anchoredPosition = new Vector2(x, -y);
                    break;
                case RightMiddle:
                    rt.anchoredPosition = new Vector2(-x, y);
                    break;
                default:
                    rt.anchoredPosition = new Vector2(x, y);
                    break;
            }
        }

        private static Vector2 GetPoint(int layout)
        {
            switch (layout)
            {
                case 4:
                case 5:
                case 6:
                case 7:
                    return CenterV;
                case LeftTop:
                    return LeftTopV;
                case LeftBottom:
                    return LeftBottomV;
                case RightBottom:
                    return RightBottomV;
                case RightTop:
                    return RightTopV;
                case LeftMiddle:
                    return LeftMiddleV;
                case RightMiddle:
                    return RightMiddleV;
                case TopMiddle:
                    return TopMiddleV;
                case BottomMiddle:
                    return BottomMiddleV;
                default:
                    return Vector2.zero;
            }
        }

    }
}
