using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Necesario para OrderBy

public static class IA_P2_PathfindingManager
{
    // [Clases internas AStarResult, FinalPathResult, NodeDistance no cambian]
    // ... (Omitidas por brevedad, son iguales que en tu código)
    #region Clases Internas
    private class AStarResult
    {
        public List<IA_P2_PathNode> path;
        public float cost;
    }
    private class FinalPathResult
    {
        public List<Vector3> path;
        public float totalCost;
    }
    private class NodeDistance
    {
        public IA_P2_PathNode node;
        public float distance;
    }
    #endregion


    /// <summary>
    /// [MODIFICADO] Solicita un camino, probando múltiples nodos para encontrar el de menor COSTO total.
    /// Ahora incluye un 'stopOffset' opcional para detenerse antes del destino.
    /// </summary>
    /// <param name="Origen">Posición inicial</param>
    /// <param name="targetPos">Posición final</param>
    /// <param name="stopOffset">[NUEVO] Distancia a la que detenerse ANTES de 'targetPos'. 0 por defecto.</param>
    public static List<Vector3> RequestPath(Vector3 Origen, Vector3 targetPos, float stopOffset = 0f)
    {
        var model = IA_P2_PathfindingModel.Instance;
        if (model == null) return null;
        var nodes = model.allNodes;
        if (nodes == null || nodes.Count == 0) return null;

        LayerMask obstacleLayer = model.obstacleLayer;

        // [NUEVO] Asegurarse de que el offset no sea negativo
        if (stopOffset < 0f) stopOffset = 0f;

        // 🔹 Optimización 1: Línea de visión directa
        if (IA_P2_LineOfSight3D.Check(Origen, targetPos, obstacleLayer))
        {
            // [MODIFICADO] Calcular el punto final con el offset
            Vector3 finalPos = GetOffsetTarget(Origen, targetPos, stopOffset);
            return new List<Vector3>() { finalPos };
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

        List<FinalPathResult> allValidPaths = new List<FinalPathResult>();

        // 3) Probar todas las combinaciones (K*K)
        foreach (var startNode in startNodes)
        {
            foreach (var endNode in endNodes)
            {
                // 4) Ejecutar A*
                var g = g_costs; 
                AStarResult nodePathResult = RunAStar(startNode, endNode, targetPos, g);

                if (nodePathResult == null) continue; // No se encontró camino

                // --- [MODIFICADO] ---
                // 5) Calcular el COSTO TOTAL usando el offset
                
                // Obtenemos la posición del último nodo del camino A*
                Vector3 lastNodePos = nodePathResult.path.Last().transform.position;
                
                // Calculamos el punto final REAL (con offset)
                Vector3 finalPos = GetOffsetTarget(lastNodePos, targetPos, stopOffset);

                float costOriginToStart = Vector3.Distance(Origen, startNode.transform.position);
                float costAStarPath = nodePathResult.cost;
                // El costo final es hasta 'finalPos', no hasta 'targetPos'
                float costLastNodeToTarget = Vector3.Distance(lastNodePos, finalPos); 

                float currentTotalCost = costOriginToStart + costAStarPath + costLastNodeToTarget;

                // 6) Construir la lista final de Vector3
                List<Vector3> finalPathVectors = new List<Vector3>();
                foreach (var n in nodePathResult.path)
                    finalPathVectors.Add(n.transform.position);

                // Añadimos 'finalPos' (con offset) en lugar de 'targetPos'
                finalPathVectors.Add(finalPos); 
                // --- [FIN DE LA MODIFICACIÓN] ---


                // 7) Guardar este resultado
                allValidPaths.Add(new FinalPathResult { path = finalPathVectors, totalCost = currentTotalCost });
            }
        }

        // 8) Devolver el de MENOR COSTO
        if (allValidPaths.Count == 0) return null; 

        var bestPath = allValidPaths.OrderBy(p => p.totalCost).First();
        return bestPath.path;
    }

    /// <summary>
    /// [NUEVO] Calcula una posición 'offset' unidades *antes* de 'to',
    /// viniendo desde 'from'.
    /// </summary>
    private static Vector3 GetOffsetTarget(Vector3 from, Vector3 to, float offset)
    {
        // Si el offset es 0 (o casi 0), devolver el destino original
        if (offset <= 0.001f)
            return to;

        float distance = Vector3.Distance(from, to);

        // Si el offset es mayor que la distancia, no podemos retroceder más.
        // Devolvemos 'from' para evitar ir hacia atrás.
        if (offset >= distance)
            return from;
            
        // Calcular la dirección y retroceder 'offset' unidades desde 'to'
        Vector3 direction = (to - from).normalized;
        return to - direction * offset;
    }


    /// <summary>
    /// [NUEVO] Encuentra los K nodos más cercanos a una posición que tienen línea de visión directa.
    /// (Esta función no cambia)
    /// </summary>
    static List<IA_P2_PathNode> FindKClosestVisibleNodes(Vector3 pos, int k, LayerMask obstacleLayer)
    {
        var model = IA_P2_PathfindingModel.Instance;
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

        var sortedNodes = allNodeDistances.OrderBy(nd => nd.distance);
        List<IA_P2_PathNode> results = new List<IA_P2_PathNode>();

        foreach (var nodeDist in sortedNodes)
        {
            if (IA_P2_LineOfSight3D.Check(pos, nodeDist.node.transform.position, obstacleLayer))
            {
                results.Add(nodeDist.node);
                if (results.Count >= k) 
                    break;
            }
        }
        return results;
    }


    // -------------------------------------------------------------------
    // --- MÉTODOS A* (No cambian) ---
    // -------------------------------------------------------------------

    static Dictionary<IA_P2_PathNode, float> g_costs = new Dictionary<IA_P2_PathNode, float>();
    static Dictionary<IA_P2_PathNode, float> f_costs = new Dictionary<IA_P2_PathNode, float>();

    // [RunAStar no cambia]
    static AStarResult RunAStar(IA_P2_PathNode start, IA_P2_PathNode end, Vector3 targetPos, Dictionary<IA_P2_PathNode, float> g)
    {
        LayerMask obstacle = IA_P2_PathfindingModel.Instance.obstacleLayer;

        var open = new List<IA_P2_PathNode>();
        var closed = new HashSet<IA_P2_PathNode>();
        var came = new Dictionary<IA_P2_PathNode, IA_P2_PathNode>();

        var f = f_costs;
        g_costs.Clear(); 
        f.Clear();

        foreach (var n in IA_P2_PathfindingModel.Instance.allNodes)
        {
            if (n == null) continue; // Seguridad extra
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

            // [IMPORTANTE] El A* sigue usando 'targetPos' (el real) para 
            // la heurística y la comprobación de visión. El offset se aplica
            // *después* de que A* termine.
            if (IA_P2_LineOfSight3D.Check(current.transform.position, targetPos, obstacle))
                return new AStarResult { path = ReconstructPartial(came, current), cost = g[current] };

            if (current == end)
                return new AStarResult { path = Reconstruct(came, end), cost = g[end] };

            open.Remove(current);
            closed.Add(current);

            foreach (var nb in current.Vecinos)
            {
                if (nb == null || closed.Contains(nb)) continue;
                if (!IA_P2_LineOfSight3D.Check(current.transform.position, nb.transform.position, obstacle))
                    continue;
                

                // [CORRECCIÓN] Usamos el 'movementCost' de tu código original
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

    // [Reconstruct no cambia]
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

    // [ReconstructPartial no cambia]
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