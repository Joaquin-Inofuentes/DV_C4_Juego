using CustomInspector;
using System.Collections.Generic;
using UnityEngine;

public class IA_P2_PathfindingModel : MonoBehaviour
{
    [Button(nameof(OnEnable))]
    [Header("Configuración de Vecinos")]
    public float sideOffset = 0.5f; // ancho del agente

    [Header("Capa de obstáculos")]
    public LayerMask obstacleLayer;

    [Header("Puntos del Grafo")]
    public List<IA_P2_PathNode> allNodes = new List<IA_P2_PathNode>();

    public static IA_P2_PathfindingModel Instance;

    void OnEnable()
    {
        Instance = this;
        ReCalcularVecinos();
    }



    // ---------------------------------------------------------
    //   BUSCAR AUTOMÁTICAMENTE TODOS LOS NODOS DEL ESCENA
    // ---------------------------------------------------------
    public void ReCalcularVecinos()
    {
        allNodes = new List<IA_P2_PathNode>(FindObjectsOfType<IA_P2_PathNode>());
        GenerateNeighbors();
    }

    // ---------------------------------------------------------
    //   GENERA TODOS LOS VECINOS
    // ---------------------------------------------------------
    public void GenerateNeighbors()
    {
        foreach (var node in allNodes)
        {
            GenerateNeighborsForNode(node);
        }
    }

    public void GenerateNeighborsForNode(IA_P2_PathNode node)
    {
        if (node == null) return;

        node.neighbors.Clear();

        foreach (var other in allNodes)
        {
            if (other == null || other == node)
                continue;

            // ----------------------------
            //     Line of Sight ancho
            // ----------------------------
            Vector3 start = node.transform.position;
            Vector3 end = other.transform.position;
            Vector3 dir = (end - start).normalized;

            // vector perpendicular en el plano XZ
            Vector3 right = Vector3.Cross(dir, Vector3.up).normalized;

            bool centerClear = LineOfSight3D.Check(start, end, obstacleLayer);
            if (!centerClear) continue;

            // laterales
            Vector3 startLeft = start - right * sideOffset;
            Vector3 startRight = start + right * sideOffset;

            Vector3 endLeft = end - right * sideOffset;
            Vector3 endRight = end + right * sideOffset;

            bool leftClear = LineOfSight3D.Check(startLeft, endLeft, obstacleLayer);
            bool rightClear = LineOfSight3D.Check(startRight, endRight, obstacleLayer);

            if (!leftClear || !rightClear)
                continue;

            // Si pasó los 3 raycasts → es vecino válido
            node.neighbors.Add(other);
        }
    }





}
