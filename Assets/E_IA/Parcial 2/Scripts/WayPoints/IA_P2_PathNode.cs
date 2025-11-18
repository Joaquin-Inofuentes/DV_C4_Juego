using CustomInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IA_P2_PathNode : MonoBehaviour
{
    [Button(nameof(ReCalcularCercanos))]
    public float movementCost = 1f;
    public List<IA_P2_PathNode> Vecinos = new List<IA_P2_PathNode>();

    public void ReCalcularCercanos()
    {
        if (IA_P2_PathfindingModel.Instance != null)
        {
            IA_P2_PathfindingModel.Instance.OnEnable();
        }
    }
#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        if (Selection.activeTransform == null) return;

        bool selectedSelf = Selection.activeTransform == transform;
        bool selectedParent = transform.parent != null && Selection.activeTransform == transform.parent;

        if (!selectedSelf && !selectedParent) return;

        float verticalOffset = 0.5f;
        float cutPercent = 0.2f;

        foreach (var n in Vecinos)
        {
            if (n == null) continue;

            Vector3 start = transform.position;
            Vector3 end = n.transform.position;

            // 20% azul
            Vector3 p20 = Vector3.Lerp(start, end, cutPercent);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(start, p20);

            // 80% amarillo
            Gizmos.color = Color.yellow;

            if (selectedParent)
            {
                Vector3 p80 = Vector3.Lerp(start, end, 1f - cutPercent - 0.2f);
                Gizmos.DrawLine(p20, p80);
            }
            else
            {
                Gizmos.DrawLine(p20, end);
            }

            // Marca vertical en vecino
            Vector3 top = end + Vector3.up * verticalOffset;
            Gizmos.DrawLine(end, top);
        }
    }



    void OnDrawGizmosSelected()
    {
        float verticalOffset = 0.5f;

        bool selectedSelf = Selection.activeTransform == transform;
        bool selectedParent = transform.parent != null && Selection.activeTransform == transform.parent;

        if (!selectedSelf && !selectedParent) return;

        // --- COSTO DEL NODO ---
        Vector3 topSelf = transform.position + Vector3.up * (verticalOffset + 0.3f);
        DrawLabel(topSelf, Mathf.RoundToInt(movementCost) + "C");

        // --- VECINOS ---
        foreach (var n in Vecinos)
        {
            if (n == null) continue;

            Vector3 start = transform.position;
            Vector3 end = n.transform.position;

            string value;

            if (selectedSelf)
            {
                float dist = Vector3.Distance(start, end);
                value =
                    dist.ToString("F2") + "m\n" +
                    Mathf.RoundToInt(n.movementCost) + "C";
            }
            else // selectedParent
            {
                value = Mathf.RoundToInt(n.movementCost) + "C";
            }

            Vector3 top = end + Vector3.up * (verticalOffset + 0.05f);
            DrawLabel(top, value);
        }
    }

    void DrawLabel(Vector3 worldPos, string text)
    {
#if UNITY_EDITOR
        Handles.BeginGUI();

        Vector3 screenPos = HandleUtility.WorldToGUIPoint(worldPos);

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 12;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        Vector2 textSize = style.CalcSize(new GUIContent(text));

        
        screenPos.y += textSize.y * 0.8f;

        Rect rect = new Rect(
            screenPos.x - textSize.x / 2f,
            screenPos.y - textSize.y / 2f,
            textSize.x,
            textSize.y
        );

        // Fondo NEGRO
        EditorGUI.DrawRect(rect, Color.black);

        // Texto
        GUI.Label(rect, text, style);

        Handles.EndGUI();
#endif
    }


#endif
}
