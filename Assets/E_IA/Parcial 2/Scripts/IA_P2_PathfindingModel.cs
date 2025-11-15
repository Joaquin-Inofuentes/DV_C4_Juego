using CustomInspector;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingModel : MonoBehaviour
{
    [Button(nameof(ReCalcularVecinos))]
    [Header("Configuración de Vecinos")]
    public float maxNeighborDistance = 10f;
    public int angleSteps = 8;

    [Header("Capa de obstáculos")]
    public LayerMask obstacleLayer;

    [Header("Puntos del Grafo")]
    public List<PathNode> allNodes = new List<PathNode>();

    public static PathfindingModel Instance;

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
        allNodes = new List<PathNode>(FindObjectsOfType<PathNode>());
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

    public void GenerateNeighborsForNode(PathNode node)
    {
        if (node == null) return;

        node.neighbors.Clear();

        float segment = 360f / angleSteps; // 90 grados

        PathNode[] bestNode = new PathNode[angleSteps];
        float[] bestDist = new float[angleSteps];

        for (int i = 0; i < angleSteps; i++)
            bestDist[i] = Mathf.Infinity;

        foreach (var other in allNodes)
        {
            if (other == null || other == node) continue;

            float dist = Vector3.Distance(node.transform.position, other.transform.position);
            if (dist > maxNeighborDistance) continue;

            // angulo 0..360
            Vector3 dir = (other.transform.position - node.transform.position).normalized;
            float signed = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up); // -180..180
            float angle360 = (signed + 360f) % 360f;

            // ------ sector 0..3 ------
            int sectorIndex = Mathf.FloorToInt(angle360 / segment);
            sectorIndex = Mathf.Clamp(sectorIndex, 0, angleSteps - 1);

            // visibilidad (Line of Sight)
            if (!LineOfSight3D.Check(node.transform.position, other.transform.position, obstacleLayer))
                continue;

            // tomar solo el mas cercano del sector
            if (dist < bestDist[sectorIndex])
            {
                bestDist[sectorIndex] = dist;
                bestNode[sectorIndex] = other;
            }
        }

        // convertir mejores sectores en lista final
        for (int s = 0; s < angleSteps; s++)
        {
            if (bestNode[s] != null)
                node.neighbors.Add(bestNode[s]);
        }
    }



}
