using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Necesario para OrderBy

public static class IA_P2_PathfindingManager
{
    // [NUEVO] Helper interno para ordenar nodos por distancia
    private class NodeDistance
    {
        public IA_P2_PathNode node;
        public float distance;
    }

    /// <summary>
    /// [MODIFICADO] Solicita un camino, probando múltiples nodos para encontrar el más corto.
    /// </summary>
    public static List<Vector3> RequestPath(Vector3 Origen, Vector3 targetPos)
    {
        var model = IA_P2_PathfindingModel.Instance;
        if (model == null) return null;

        var nodes = model.allNodes;
        if (nodes == null || nodes.Count == 0) return null;

        LayerMask obstacleLayer = model.obstacleLayer;

        // 🔹 Optimización 1: Si el target es visible desde el origen, esa es la respuesta.
        if (LineOfSight3D.Check(Origen, targetPos, obstacleLayer))
        {
            return new List<Vector3>() { targetPos };
        }

        // 🔹 Optimización 2: Estrategia KNN (K-Nearest Neighbors)
        // Buscamos los K nodos más cercanos y visibles desde el inicio y el final.
        const int K = 3;

        // 1) Encontrar K nodos más cercanos (y visibles) al Origen
        List<IA_P2_PathNode> startNodes = FindKClosestVisibleNodes(Origen, K, obstacleLayer);

        // 2) Encontrar K nodos más cercanos (y visibles) al Target
        List<IA_P2_PathNode> endNodes = FindKClosestVisibleNodes(targetPos, K, obstacleLayer);

        // Si no se encontró ningún nodo visible desde el origen o el destino, no hay camino.
        if (startNodes.Count == 0 || endNodes.Count == 0)
        {
            Debug.LogWarning("PathfindingManager: No se encontraron nodos visibles desde el Origen o el Destino.");
            return null;
        }

        List<Vector3> bestPath = null;
        float bestPathLength = float.MaxValue;

        // 3) Probar todas las combinaciones (K*K) de nodos de inicio y fin
        foreach (var startNode in startNodes)
        {
            foreach (var endNode in endNodes)
            {
                // 4) Ejecutar A* para esta combinación
                List<IA_P2_PathNode> nodePath = RunAStar(startNode, endNode, targetPos);
                if (nodePath == null) continue; // No se encontró camino entre estos dos nodos

                // 5) Construir la lista final de Vector3
                List<Vector3> currentPath = new List<Vector3>();
                foreach (var n in nodePath)
                    currentPath.Add(n.transform.position);

                currentPath.Add(targetPos); // Siempre añadimos la posición exacta al final

                // 6) Calcular la longitud total del camino (Origen -> ... -> Target)
                float currentPathLength = CalculateTotalPathLength(currentPath, Origen);

                // 7) Guardar si es el mejor camino encontrado hasta ahora
                if (currentPathLength < bestPathLength)
                {
                    bestPathLength = currentPathLength;
                    bestPath = currentPath;
                }
            }
        }

        return bestPath; // Devuelve el camino más corto de todas las combinaciones
    }

    /// <summary>
    /// [NUEVO] Encuentra los K nodos más cercanos a una posición que tienen línea de visión directa.
    /// </summary>
    static List<IA_P2_PathNode> FindKClosestVisibleNodes(Vector3 pos, int k, LayerMask obstacleLayer)
    {
        var model = IA_P2_PathfindingModel.Instance;

        // 1. Obtener todos los nodos con su distancia
        List<NodeDistance> allNodeDistances = new List<NodeDistance>();
        foreach (var n in model.allNodes)
        {
            allNodeDistances.Add(new NodeDistance
            {
                node = n,
                distance = Vector3.Distance(pos, n.transform.position)
            });
        }

        // 2. Ordenarlos por distancia (más cercano primero)
        var sortedNodes = allNodeDistances.OrderBy(nd => nd.distance);

        // 3. Tomar los primeros 'k' que sean visibles
        List<IA_P2_PathNode> results = new List<IA_P2_PathNode>();
        foreach (var nodeDist in sortedNodes)
        {
            // ¡Importante! Comprueba que haya línea de visión
            if (LineOfSight3D.Check(pos, nodeDist.node.transform.position, obstacleLayer))
            {
                results.Add(nodeDist.node);
                if (results.Count >= k) // Si ya tenemos 'k' nodos, paramos
                    break;
            }
        }

        return results;
    }

    /// <summary>
    /// [NUEVO] Calcula la longitud total de un camino (lista de Vector3), empezando desde el origen.
    /// </summary>
    static float CalculateTotalPathLength(List<Vector3> path, Vector3 origin)
    {
        if (path == null || path.Count == 0) return float.MaxValue;

        float totalLength = 0f;
        Vector3 lastPoint = origin;

        foreach (Vector3 point in path)
        {
            totalLength += Vector3.Distance(lastPoint, point);
            lastPoint = point;
        }

        return totalLength;
    }


    // -------------------------------------------------------------------
    // --- MÉTODOS A* (SIN CAMBIOS) ---
    // -------------------------------------------------------------------

    static List<IA_P2_PathNode> RunAStar(IA_P2_PathNode start, IA_P2_PathNode end, Vector3 targetPos)
    {
        LayerMask obstacle = IA_P2_PathfindingModel.Instance.obstacleLayer;

        var open = new List<IA_P2_PathNode>();
        var closed = new HashSet<IA_P2_PathNode>();

        var came = new Dictionary<IA_P2_PathNode, IA_P2_PathNode>();
        var g = new Dictionary<IA_P2_PathNode, float>();
        var f = new Dictionary<IA_P2_PathNode, float>();

        foreach (var n in IA_P2_PathfindingModel.Instance.allNodes)
        {
            g[n] = float.MaxValue;
            f[n] = float.MaxValue;
        }

        g[start] = 0f;
        f[start] = Vector3.Distance(start.transform.position, end.transform.position);

        open.Add(start);

        while (open.Count > 0)
        {
            // Node con menor F
            IA_P2_PathNode current = open[0];
            float bestF = f[current];

            for (int i = 1; i < open.Count; i++)
            {
                if (f[open[i]] < bestF)
                {
                    current = open[i];
                    bestF = f[current];
                }
            }

            // 🔥 Si desde este nodo veo el objetivo → recorto el camino
            if (LineOfSight3D.Check(current.transform.position, targetPos, obstacle))
                return ReconstructPartial(came, current);

            // Llegué al objetivo normal
            if (current == end)
                return Reconstruct(came, end);

            open.Remove(current);
            closed.Add(current);

            // Vecinos válidos
            foreach (var nb in current.neighbors)
            {
                if (nb == null || closed.Contains(nb)) continue;

                // 🔥 Chequeo de visibilidad entre nodos
                if (!LineOfSight3D.Check(current.transform.position, nb.transform.position, obstacle))
                    continue;

                float tentative = g[current] + nb.movementCost;

                if (!open.Contains(nb))
                    open.Add(nb);

                if (tentative >= g[nb]) continue;

                came[nb] = current;
                g[nb] = tentative;
                f[nb] = tentative + Vector3.Distance(nb.transform.position, end.transform.position);
            }
        }

        return null;
    }


    static List<IA_P2_PathNode> Reconstruct(Dictionary<IA_P2_PathNode, IA_P2_PathNode> came, IA_P2_PathNode end)
    {
        List<IA_P2_PathNode> path = new List<IA_P2_PathNode>();
        IA_P2_PathNode node = end;

        while (came.ContainsKey(node))
        {
            path.Add(node);
            node = came[node];
        }

        path.Add(node);
        path.Reverse();
        return path;
    }


    static List<IA_P2_PathNode> ReconstructPartial(Dictionary<IA_P2_PathNode, IA_P2_PathNode> came, IA_P2_PathNode end)
    {
        List<IA_P2_PathNode> path = new List<IA_P2_PathNode>();
        IA_P2_PathNode node = end;

        while (came.ContainsKey(node))
        {
            path.Add(node);
            node = came[node];
        }

        path.Add(node);
        path.Reverse();
        return path;
    }
}