/*
uGui-Effect-Tool
Copyright (c) 2016 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UiEffect
{
    public class GradientSidesAlpha : BaseMeshEffect
    {
        private const int ONE_TEXT_VERTEX = 6;

        [SerializeField, Range(0f, 1f)]
        private float m_alphaTop = 1f;
        [SerializeField, Range(0f, 1f)]
        private float m_alphaBottom = 1f;
        [SerializeField, Range(0f, 1f)]
        private float m_alphaLeft = 1f;
        [SerializeField, Range(0f, 1f)]
        private float m_alphaRight = 1f;
        [SerializeField, Range(-1f, 1f)]
        private float m_gradientOffsetVertical = 0f;
        [SerializeField, Range(-1f, 1f)]
        private float m_gradientOffsetHorizontal = 0f;
        [SerializeField]
        private bool m_splitTextGradient = false;

        [SerializeField, Range(0f, 1f)]
        private float m_offSetLeft = 0f;
        [SerializeField, Range(0f, 1f)]
        private float m_offSetRight = 0f;

        public float alphaTop { get { return m_alphaTop; } set { if (m_alphaTop != value) { m_alphaTop = Mathf.Clamp01(value); Refresh(); } } }
        public float alphaBottom { get { return m_alphaBottom; } set { if (m_alphaBottom != value) { m_alphaBottom = Mathf.Clamp01(value); Refresh(); } } }
        public float alphaLeft { get { return m_alphaLeft; } set { if (m_alphaLeft != value) { m_alphaLeft = Mathf.Clamp01(value); Refresh(); } } }
        public float alphaRight { get { return m_alphaRight; } set { if (m_alphaRight != value) { m_alphaRight = Mathf.Clamp01(value); Refresh(); } } }
        public float gradientOffsetVertical { get { return m_gradientOffsetVertical; } set { if (m_gradientOffsetVertical != value) { m_gradientOffsetVertical = Mathf.Clamp(value, -1f, 1f); Refresh(); } } }
        public float gradientOffsetHorizontal { get { return m_gradientOffsetHorizontal; } set { if (m_gradientOffsetHorizontal != value) { m_gradientOffsetHorizontal = Mathf.Clamp(value, -1f, 1f); Refresh(); } } }
        public bool splitTextGradient { get { return m_splitTextGradient; } set { if (m_splitTextGradient != value) { m_splitTextGradient = value; Refresh(); } } }


//        private float minX = 0f, minY = 0f, maxX = 0f, maxY = 0f, width = 0f, height = 0;

        private static int vertexNum = 12;
        private List<UIVertex> vList = new List<UIVertex>();
        List<int> indices = new List<int>();

        public override void ModifyMesh(VertexHelper vh)
        {
            if (IsActive() == false)
            {
                return;
            }

            InitVertexList();

            List<UIVertex> oList = UiEffectListPool<UIVertex>.Get();
            vh.GetUIVertexStream(oList);

//            CalculateInitQuare(oList);
            SplitTwoQuare(oList);
            ModifyVertices(vList);

            vh.Clear();
            vh.AddUIVertexStream(vList, indices);
            UiEffectListPool<UIVertex>.Release(oList);
            vList.Clear();
        }

        private void InitVertexList()
        {
            if (indices.Count == 0)
            {
                for (var i = 0; i < vertexNum; i++)
                {
                    indices.Add(i);
                }
            }
        }

//        private void CalculateInitQuare(List<UIVertex> vList)
//        {
//            var i = 0;
//            minX = vList[i].position.x;
//            minY = vList[i].position.y;
//            maxX = vList[i].position.x;
//            maxY = vList[i].position.y;
//
//            int vertNum = vList.Count;
//
//            for (int k = i; k < vertNum; k++)
//            {
//                if (k >= vList.Count)
//                {
//                    break;
//                }
//                UIVertex vertex = vList[k];
//                minX = Mathf.Min(minX, vertex.position.x);
//                minY = Mathf.Min(minY, vertex.position.y);
//                maxX = Mathf.Max(maxX, vertex.position.x);
//                maxY = Mathf.Max(maxY, vertex.position.y);
//            }
//
//            width = maxX - minX;
//            height = maxY - minY;
//        }

        private void SplitTwoQuare(List<UIVertex> oList)
        {
            var vertex0 = oList[0];
            var vertex1 = oList[1];
            var vertex2 = oList[2];
            var vertex4 = oList[4];
            var bb = (vertex1.color.r + vertex2.color.r);
            var aa = (byte)((vertex1.color.r + vertex2.color.r) / 2);
            UIVertex upVertex = new UIVertex()
            {
                position  = (vertex1.position + vertex2.position) / 2f,
                color = new Color((vertex1.color.r + vertex2.color.r) / 2f / 255f, (vertex1.color.g + vertex2.color.g) / 2f / 255f, (vertex1.color.b + vertex2.color.b) / 2f / 255f, (vertex1.color.a + vertex2.color.a) / 2f / 255f),
                uv0 = (vertex1.uv0 + vertex2.uv0) / 2,
                uv1 = (vertex1.uv1 + vertex2.uv1) / 2,
                uv2 = (vertex1.uv2 + vertex2.uv2) / 2,
                uv3 = (vertex1.uv3 + vertex2.uv3) / 2,
            };
            UIVertex downVertex = new UIVertex()
            {
                position  = (vertex0.position + vertex4.position) / 2,
                color = new Color((vertex0.color.r + vertex4.color.r) / 2f / 255f, (vertex0.color.g + vertex4.color.g) / 2f / 255f, (vertex0.color.b + vertex4.color.b) / 2f / 255f, (vertex0.color.a + vertex4.color.a) / 2f / 255f),
                uv0 = (vertex0.uv0 + vertex4.uv0) / 2,
                uv1 = (vertex0.uv1 + vertex4.uv1) / 2,
                uv2 = (vertex0.uv2 + vertex4.uv2) / 2,
                uv3 = (vertex0.uv3 + vertex4.uv3) / 2,
            };

            vList.Add(oList[0]);
            vList.Add(vertex1);
            vList.Add(upVertex);

            vList.Add(oList[0]);
            vList.Add(downVertex);
            vList.Add(upVertex);

            vList.Add(downVertex);
            vList.Add(upVertex);
            vList.Add(oList[3]);

            vList.Add(downVertex);
            vList.Add(oList[3]);
            vList.Add(oList[4]);
        }

        private void ModifyVertices(List<UIVertex> vList)
        {
            if (IsActive() == false || vList == null || vList.Count == 0)
            {
                return;
            }
            UIVertex newVertex0 = vList[0];
            UIVertex newVertex1 = vList[1];
            UIVertex newVertex3 = vList[3];
            newVertex3.color.a = newVertex1.color.a = newVertex0.color.a = (byte)(m_alphaLeft*255);
            vList[0] = newVertex0;
            vList[1] = newVertex1;
            vList[3] = newVertex3;

            UIVertex newVertex10 = vList[10];
            UIVertex newVertex11 = vList[11];
            UIVertex newVertex8 = vList[8];
            newVertex8.color.a = newVertex11.color.a = newVertex10.color.a = (byte)(m_alphaRight * 255);
            vList[10] = newVertex10;
            vList[11] = newVertex11;
            vList[8] = newVertex8;
        }

        private void Refresh()
        {
            if (graphic != null)
            {
                graphic.SetVerticesDirty();
            }
        }
    }
}
