using System.Collections.Generic;
using UnityEngine;

public static class IA_F_PathFinding_Theta
{
    public static List<Vector3> OptimizarConTheta(List<Vector3> recorridoAStar, LayerMask obstacleLayer)
    {
        // ================================
        // 1) Validaciones básicas
        // ================================

        if (recorridoAStar == null)
            return new List<Vector3>();

        if (recorridoAStar.Count == 0)
            return new List<Vector3>();

        if (recorridoAStar.Count == 1)
            return new List<Vector3>(recorridoAStar);

        // ================================
        // 2) Crear lista resultado
        // ================================

        List<Vector3> resultado = new List<Vector3>();

        // Siempre agregamos el primer punto
        resultado.Add(recorridoAStar[0]);

        // ================================
        // 3) Si veo directo del primero al último → listo
        // ================================

        Vector3 inicio = recorridoAStar[0];
        Vector3 destino = recorridoAStar[recorridoAStar.Count - 1];

        if (IA_P2_LineOfSight3D.Check(inicio, destino, obstacleLayer))
        {
            resultado.Add(destino);
            return resultado;
        }

        // ================================
        // 4) Algoritmo Theta (suavizado)
        // ================================

        // anchorIndex = último punto confirmado
        int anchorIndex = 0;

        // Mientras no lleguemos al final
        while (anchorIndex < recorridoAStar.Count - 1)
        {
            // Intentamos ir lo más lejos posible
            int indiceMasLejanoVisible = anchorIndex + 1;

            // Probamos cada punto más adelante
            for (int i = anchorIndex + 2; i < recorridoAStar.Count; i++)
            {
                Vector3 desde = recorridoAStar[anchorIndex];
                Vector3 hacia = recorridoAStar[i];

                bool hayVisionDirecta = IA_P2_LineOfSight3D.Check(desde, hacia, obstacleLayer);

                if (hayVisionDirecta)
                {
                    // Podemos saltar más lejos
                    indiceMasLejanoVisible = i;
                }
                else
                {
                    // Si ya no veo, corto acá
                    break;
                }
            }

            // Agregamos el punto más lejano alcanzable
            resultado.Add(recorridoAStar[indiceMasLejanoVisible]);

            // Movemos el anchor
            anchorIndex = indiceMasLejanoVisible;
        }

        return resultado;
    }
}