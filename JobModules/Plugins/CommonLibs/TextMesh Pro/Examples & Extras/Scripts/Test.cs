using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{


    private Canvas m_Canvas;
    private Camera m_Camera;

    private TextMeshProUGUI m_TextMeshPro;

    // Use this for initialization
    void Start()
    {
        m_TextMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        m_Canvas = gameObject.GetComponentInParent<Canvas>();
        if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            m_Camera = null;
        else
            m_Camera = m_Canvas.worldCamera;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, m_Camera);
        if (linkIndex != -1)
        {

            TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];

            // Debug.Log("Link ID: \"" + linkInfo.GetLinkID() + "\"   Link Text: \"" + linkInfo.GetLinkText() + "\""); // Example of how to retrieve the Link ID and Link Text.

            Vector3 worldPointInRectangle = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_TextMeshPro.rectTransform, Input.mousePosition, m_Camera, out worldPointInRectangle);

            switch (linkInfo.GetLinkID())
            {
                case "id_01": // 100041637: // id_01
                    Debug.Log(linkInfo.GetLinkID());
                    break;
                case "id_02": // 100041638: // id_02
                    Debug.Log(linkInfo.GetLinkID());
                    break;
            }
        }
    }
}
