using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public float movementCost = 1f;
    public List<PathNode> neighbors = new List<PathNode>();


#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        if (Selection.activeTransform == null) return;

        bool selectedSelf = Selection.activeTransform == transform;
        bool selectedParent = transform.parent != null && Selection.activeTransform == transform.parent;

        if (!selectedSelf && !selectedParent) return;

        float verticalOffset = 0.5f;
        float cutPercent = 0.2f;   // 20%

        foreach (var n in neighbors)
        {
            if (n == null) continue;

            Vector3 start = transform.position;
            Vector3 end = n.transform.position;

            // ------------------------------------
            // 1) Tramo 20% → AZUL (sin desplazamiento vertical)
            // ------------------------------------
            Vector3 p20 = Vector3.Lerp(start, end, cutPercent);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(start, p20);

            // ------------------------------------
            // 2) Tramo restante → AMARILLO o recortado
            // ------------------------------------
            Gizmos.color = Color.yellow;

            if (selectedParent)
            {
                // Si está seleccionado el padre → recortar 20% inicial y final
                Vector3 p80 = Vector3.Lerp(start, end, 1f - cutPercent);
                Gizmos.DrawLine(p20, p80);
            }
            else
            {
                // Nodo seleccionado → línea completa desde el 20% hasta el final
                Gizmos.DrawLine(p20, end);
            }

            // ------------------------------------
            // MARCA vertical en el vecino
            // ------------------------------------
            Vector3 top = end + Vector3.up * verticalOffset;
            Gizmos.DrawLine(end, top);

            // ------------------------------------
            // LABEL
            // ------------------------------------
            string value =
                selectedSelf
                ? Vector3.Distance(start, end).ToString("F2") + "m"
                : movementCost.ToString("F2");

            DrawLabelWithBackground(top + Vector3.up * 0.05f, value, 1.3f);
        }
    }

    void DrawLabelWithBackground(Vector3 worldPos, string text, float bgScale)
    {
        GUIStyle textStyle = new GUIStyle(EditorStyles.label);
        textStyle.normal.textColor = Color.white;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontSize = 12;

        Handles.BeginGUI();
        Vector3 guiPoint = HandleUtility.WorldToGUIPoint(worldPos);

        Camera sceneCam = SceneView.lastActiveSceneView?.camera;
        if (sceneCam != null)
        {
            Vector3 camCheck = sceneCam.WorldToViewportPoint(worldPos);
            if (camCheck.z < 0f)
            {
                Handles.EndGUI();
                return;
            }
        }

        Vector2 textSize = textStyle.CalcSize(new GUIContent(text));
        Vector2 bgSize = textSize * bgScale;

        Rect bgRect = new Rect(
            guiPoint.x - bgSize.x / 2f,
            guiPoint.y - bgSize.y / 2f,
            bgSize.x,
            bgSize.y
        );

        Rect textRect = new Rect(
            guiPoint.x - textSize.x / 2f,
            guiPoint.y - textSize.y / 2f,
            textSize.x,
            textSize.y
        );

        EditorGUI.DrawRect(bgRect, Color.black);
        GUI.Label(textRect, text, textStyle);

        Handles.EndGUI();
    }
#endif
}
