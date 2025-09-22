using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // Necesario para dibujar texto en la escena
#endif

/// <summary>
/// Clase estática con métodos de ayuda para dibujar visualizaciones de depuración en la escena.
/// </summary>
public static class DebugHelper
{
    /// <summary>
    /// Dibuja un círculo en el plano XZ.
    /// </summary>
    public static void DrawCircle(Vector3 position, float radius, Color color, int segments = 32)
    {
        if (radius <= 0) return;

        float angle = 0f;
        float angleStep = 360f / segments;
        Vector3 lastPoint = position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;

        for (int i = 1; i <= segments; i++)
            {
            angle += angleStep;
            Vector3 nextPoint = position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
            Debug.DrawLine(lastPoint, nextPoint, color);
            lastPoint = nextPoint;
        }
    }

    /// <summary>
    /// Dibuja una etiqueta de texto en la escena (solo visible en el Editor).
    /// </summary>
    public static void DrawLabel(Vector3 position, string text, Color color)
    {
#if UNITY_EDITOR
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        style.fontSize = 14;
        style.fontStyle = FontStyle.Bold;
        Handles.Label(position, text, style);
#endif
    }
}