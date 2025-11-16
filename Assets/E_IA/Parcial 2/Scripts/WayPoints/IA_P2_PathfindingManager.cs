using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Necesario para OrderBy

public static class IA_P2_PathfindingManager
{
    // [NUEVO] Helper para que A* devuelva el camino Y su costo 'g'
    private class AStarResult
    {
        public List<IA_P2_PathNode> path;
        public float cost; // Este es el costo 'g' (Dijkstra) del camino
    }

    // [NUEVO] Helper para almacenar el camino final y su costo total
    private class FinalPathResult
    {
        public List<Vector3> path;
        public float totalCost;
    }

    // Helper interno para ordenar nodos por distancia
    private class NodeDistance
    {
        public IA_P2_PathNode node;
        public float distance;
    }

    /// <summary>
    /// [MODIFICADO] Solicita un camino, probando múltiples nodos para encontrar el de menor COSTO total.
    /// </summary>
    public static List<Vector3> RequestPath(Vector3 Origen, Vector3 targetPos)
    {
        var model = IA_P2_PathfindingModel.Instance;
        if (model == null) return null;
        var nodes = model.allNodes;
        if (nodes == null || nodes.Count == 0) return null;

        LayerMask obstacleLayer = model.obstacleLayer;

        // 🔹 Optimización 1: Línea de visión directa
        if (LineOfSight3D.Check(Origen, targetPos, obstacleLayer))
        {
            return new List<Vector3>() { targetPos };
        }

        // 🔹 Optimización 2: Estrategia KNN (K-Nearest Neighbors)
        const int K = 3;
        List<IA_P2_PathNode> startNodes = FindKClosestVisibleNodes(Origen, K, obstacleLayer);
        List<IA_P2_PathNode> endNodes = FindKClosestVisibleNodes(targetPos, K, obstacleLayer);

        if (startNodes.Count == 0 || endNodes.Count == 0)
        {
            Debug.LogWarning("PathfindingManager: No se encontraron nodos visibles.");
            return null;
        }

        // [MODIFICADO] Usamos una lista para almacenar todos los caminos válidos
        List<FinalPathResult> allValidPaths = new List<FinalPathResult>();

        // 3) Probar todas las combinaciones (K*K)
        foreach (var startNode in startNodes)
        {
            foreach (var endNode in endNodes)
            {
                // 4) Ejecutar A* para esta combinación
                // [MODIFICADO] A* ahora devuelve el camino de nodos Y su costo 'g'
                var g = g_costs; // ← Añadido: inicializa el diccionario 'g' para cada llamada
                AStarResult nodePathResult = RunAStar(startNode, endNode, targetPos, g);

                if (nodePathResult == null) continue; // No se encontró camino

                // 5) Calcular el COSTO TOTAL
                float costOriginToStart = Vector3.Distance(Origen, startNode.transform.position);
                float costAStarPath = nodePathResult.cost;
                float costLastNodeToTarget = Vector3.Distance(nodePathResult.path.Last().transform.position, targetPos);

                float currentTotalCost = costOriginToStart + costAStarPath + costLastNodeToTarget;

                // 6) Construir la lista final de Vector3
                List<Vector3> finalPathVectors = new List<Vector3>();
                foreach (var n in nodePathResult.path)
                    finalPathVectors.Add(n.transform.position);

                finalPathVectors.Add(targetPos);

                // 7) Guardar este resultado
                allValidPaths.Add(new FinalPathResult { path = finalPathVectors, totalCost = currentTotalCost });
            }
        }

        // 8) [MODIFICADO] De todos los caminos encontrados, devolver el de MENOR COSTO
        if (allValidPaths.Count == 0) return null; // No se encontró ningún camino

        // Ordena todos los caminos que encontramos por su 'totalCost' y devuelve el primero.
        var bestPath = allValidPaths.OrderBy(p => p.totalCost).First();
        return bestPath.path;
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
            if (n == null)
            {
                model.ReCalcularVecinos();
                continue;
            }
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
    /// [ELIMINADO] CalculateTotalPathLength() -> Esta función era incorrecta y se eliminó.
    /// </summary>


    // -------------------------------------------------------------------
    // --- MÉTODOS A* (MODIFICADOS PARA DEVOLVER COSTO) ---
    // -------------------------------------------------------------------

    // [MODIFICADO] El diccionario 'g' se crea UNA VEZ y se pasa como referencia.
    // Esto evita crear un diccionario nuevo en cada uno de los 9 bucles de A*
    static Dictionary<IA_P2_PathNode, float> g_costs = new Dictionary<IA_P2_PathNode, float>();
    static Dictionary<IA_P2_PathNode, float> f_costs = new Dictionary<IA_P2_PathNode, float>();


    static AStarResult RunAStar(IA_P2_PathNode start, IA_P2_PathNode end, Vector3 targetPos, Dictionary<IA_P2_PathNode, float> g)
    {
        LayerMask obstacle = IA_P2_PathfindingModel.Instance.obstacleLayer;

        var open = new List<IA_P2_PathNode>();
        var closed = new HashSet<IA_P2_PathNode>();
        var came = new Dictionary<IA_P2_PathNode, IA_P2_PathNode>();

        // [MODIFICADO] Reutilizamos los diccionarios para performance
        var f = f_costs;
        g_costs.Clear(); // Limpiamos el diccionario 'g'
        f.Clear();

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

            // [MODIFICADO] Devolvemos un AStarResult (camino + costo 'g')
            if (LineOfSight3D.Check(current.transform.position, targetPos, obstacle))
                return new AStarResult { path = ReconstructPartial(came, current), cost = g[current] };

            // [MODIFICADO] Devolvemos un AStarResult (camino + costo 'g')
            if (current == end)
                return new AStarResult { path = Reconstruct(came, end), cost = g[end] };

            open.Remove(current);
            closed.Add(current);

            foreach (var nb in current.neighbors)
            {
                if (nb == null || closed.Contains(nb)) continue;
                if (!LineOfSight3D.Check(current.transform.position, nb.transform.position, obstacle))
                    continue;

                // ESTA LÍNEA ES EL CORAZÓN DE DIJKSTRA / A*
                float tentative = g[current] + nb.movementCost; // <-- ¡AQUÍ SE USAN LOS PESOS!

                if (!open.Contains(nb))
                    open.Add(nb);

                if (tentative >= g[nb]) continue;

                came[nb] = current;
                g[nb] = tentative; // <-- Guardamos el costo Dijkstra
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