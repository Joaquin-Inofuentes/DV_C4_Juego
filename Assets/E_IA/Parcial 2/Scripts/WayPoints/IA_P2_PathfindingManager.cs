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
        if (model == null || model.allNodes == null || model.allNodes.Count == 0) return null;

        LayerMask obstacleLayer = model.obstacleLayer;
        if (stopOffset < 0f) stopOffset = 0f;

        // 1. [MODIFICADO] Línea de visión directa con detección de avoidance inmediata
        var directPath = CheckPathWithAvoidance(Origen, targetPos, obstacleLayer);
        if (directPath != null)
        {
            // Aplicar offset al último punto
            Vector3 last = directPath.Last();
            directPath[directPath.Count - 1] = GetOffsetTarget(Origen, last, stopOffset);

            // Debug del primer tramo
            Debug.DrawLine(Origen, directPath[0], Color.green, 2f);


            #region Dibujos de origen y destino
            // 1. ORIGEN: 1 círculo amarillo pequeño
            //DrawDebugCircle(Origen, 0.3f, Color.yellow, 2f);

            #endregion

            return directPath;
        }

        // 2. Estrategia KNN + Theta*
        const int K = 5;
        List<IA_P2_PathNode> startNodes = FindKClosestVisibleNodes(Origen, K, obstacleLayer);
        List<IA_P2_PathNode> endNodes = FindKClosestVisibleNodes(targetPos, K, obstacleLayer);

        if (startNodes.Count == 0 || endNodes.Count == 0) return null;

        FinalPathResult bestPath = null;

        foreach (var startNode in startNodes)
        {
            foreach (var endNode in endNodes)
            {
                // El RunThetaStar ahora ya trae los puntos de evasión integrados
                var nodePathResult = RunThetaStar(startNode, endNode, targetPos, obstacleLayer);
                if (nodePathResult == null) continue;

                Vector3 lastNodePos = nodePathResult.path.Last();
                Vector3 finalPosWithOffset = GetOffsetTarget(lastNodePos, targetPos, stopOffset);

                float costOriginToStart = Vector3.Distance(Origen, startNode.transform.position);
                float totalPathCost = costOriginToStart + nodePathResult.cost + Vector3.Distance(lastNodePos, finalPosWithOffset);

                if (bestPath == null || totalPathCost < bestPath.totalCost)
                {
                    List<Vector3> fullPath = new List<Vector3>(nodePathResult.path);
                    fullPath.Add(finalPosWithOffset);
                    bestPath = new FinalPathResult { path = fullPath, totalCost = totalPathCost };
                }
            }
        }

        if (bestPath != null)
        {
            Debug.DrawLine(Origen, bestPath.path[0], Color.green, 2f);
            for (int i = 0; i < bestPath.path.Count - 1; i++)
                Debug.DrawLine(bestPath.path[i], bestPath.path[i + 1], Color.cyan, 2f);
        }




        #region Dibujos de origen y destino
        // 1. ORIGEN: 1 círculo amarillo pequeño
        //DrawDebugCircle(Origen, 0.3f, Color.yellow, 2f);

        // 2. DESTINO: 2 círculos (uno grande, uno mediano) y una cruz
        List<Vector3> processedPath = bestPath?.path;
        // --- DEBUG DE CAMINO (Líneas verde y cian) ---
        if (processedPath != null && processedPath.Count > 0)
        {
            Debug.DrawLine(Origen, processedPath[0], Color.green, 2f);
            for (int i = 0; i < processedPath.Count - 1; i++)
            {
                Debug.DrawLine(processedPath[i], processedPath[i + 1], Color.cyan, 2f);
            }
        }
        #endregion









        return bestPath?.path;
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




    private class ThetaStarResult
    {
        public List<Vector3> path;
        public float cost;
    }

    static ThetaStarResult RunThetaStar(IA_P2_PathNode start, IA_P2_PathNode end, Vector3 targetPos, LayerMask obstacle)
    {
        var open = new List<IA_P2_PathNode>();
        var closed = new HashSet<IA_P2_PathNode>();
        var parent = new Dictionary<IA_P2_PathNode, IA_P2_PathNode>();
        var gScore = new Dictionary<IA_P2_PathNode, float>();
        var fScore = new Dictionary<IA_P2_PathNode, float>();

        // Diccionario para guardar puntos de evasión intermedios entre nodos
        var avoidancePoints = new Dictionary<IA_P2_PathNode, Vector3>();

        foreach (var n in IA_P2_PathfindingModel.Instance.allNodes)
        {
            gScore[n] = float.MaxValue;
            fScore[n] = float.MaxValue;
        }

        gScore[start] = 0f;
        fScore[start] = Vector3.Distance(start.transform.position, end.transform.position);
        parent[start] = start;
        open.Add(start);

        while (open.Count > 0)
        {
            IA_P2_PathNode current = open.OrderBy(n => fScore[n]).First();

            // Check hacia el objetivo final con avoidance
            var pathToTarget = CheckPathWithAvoidance(current.transform.position, targetPos, obstacle);
            if (pathToTarget != null)
            {
                var finalPath = ReconstructPathWithAvoidance(parent, avoidancePoints, current);
                finalPath.AddRange(pathToTarget);
                return new ThetaStarResult { path = finalPath, cost = gScore[current] + Vector3.Distance(current.transform.position, targetPos) };
            }

            if (current == end)
            {
                var finalPath = ReconstructPathWithAvoidance(parent, avoidancePoints, end);
                return new ThetaStarResult { path = finalPath, cost = gScore[end] };
            }

            open.Remove(current);
            closed.Add(current);

            foreach (var neighbor in current.Vecinos)
            {
                if (neighbor == null || closed.Contains(neighbor)) continue;

                IA_P2_PathNode p = parent[current];

                // Intentar visión directa desde el PADRE al VECINO (Theta*)
                var pathInfo = CheckPathWithAvoidance(p.transform.position, neighbor.transform.position, obstacle);

                if (pathInfo != null) // Si es visible (directo o esquivando)
                {
                    float dist = (pathInfo.Count > 1) ?
                        Vector3.Distance(p.transform.position, pathInfo[0]) + Vector3.Distance(pathInfo[0], neighbor.transform.position) :
                        Vector3.Distance(p.transform.position, neighbor.transform.position);

                    float tentativeG = gScore[p] + dist;
                    if (tentativeG < gScore[neighbor])
                    {
                        parent[neighbor] = p;
                        if (pathInfo.Count > 1) avoidancePoints[neighbor] = pathInfo[0];
                        else avoidancePoints.Remove(neighbor);

                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = gScore[neighbor] + Vector3.Distance(neighbor.transform.position, end.transform.position);
                        if (!open.Contains(neighbor)) open.Add(neighbor);
                    }
                }
                else // A* normal si no hay visión
                {
                    float tentativeG = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                    if (tentativeG < gScore[neighbor])
                    {
                        parent[neighbor] = current;
                        avoidancePoints.Remove(neighbor);
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = gScore[neighbor] + Vector3.Distance(neighbor.transform.position, end.transform.position);
                        if (!open.Contains(neighbor)) open.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    

    /// <summary>
    /// Comprueba si el objeto tiene una escala cercana a 0.7 (en cualquier eje)
    /// </summary>
    private static bool IsAvoidable(GameObject obj)
    {
        Vector3 scale = obj.transform.localScale;
        // Usamos un margen de error pequeño (0.05) para detectar el 0.7
        float target = 0.7f;
        float margin = 0.1f;

        bool xMatch = Mathf.Abs(scale.x - target) < margin;
        bool yMatch = Mathf.Abs(scale.y - target) < margin;
        bool zMatch = Mathf.Abs(scale.z - target) < margin;

        //Debug.Log($"Se tiene {yMatch} {xMatch} {zMatch} ");

        return xMatch || yMatch || zMatch;
    }

    /// <summary>
    /// Calcula un punto lateral para esquivar el objeto
    /// </summary>
    private static Vector3 CalculateAvoidancePoint(Vector3 from, Vector3 to, RaycastHit hit, LayerMask layer)
    {
        Vector3 direction = (to - from).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;

        float sideOffset = 1.3f; // Distancia para alejarse del centro del objeto
        Vector3 posA = hit.point + (right * sideOffset);
        Vector3 posB = hit.point - (right * sideOffset);

        // Elegimos el lado que no tenga obstáculos hacia el destino
        if (IA_P2_LineOfSight3D.Check(posA, to, layer)) return posA;
        return posB;
    }

    /// <summary>
    /// Dibuja un círculo de debug en el mundo
    /// </summary>
    public static void DrawDebugCircle(Vector3 center, float radius, Color color, float duration)
    {
        int segments = 12;
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            Vector3 start = center + Quaternion.Euler(0, i * angleStep, 0) * Vector3.forward * radius;
            Vector3 end = center + Quaternion.Euler(0, (i + 1) * angleStep, 0) * Vector3.forward * radius;
            Debug.DrawLine(start, end, color, duration);
        }
    }





    /// <summary>
    /// Comprueba si hay camino entre A y B. 
    /// Si hay un "Obstacule", devuelve una lista con [PuntoEvasion, Destino].
    /// Si hay un muro real, devuelve null.
    /// </summary>
    private static List<Vector3> CheckPathWithAvoidance(Vector3 start, Vector3 end, LayerMask layer)
    {
        Vector3 dir = (end - start).normalized;
        float dist = Vector3.Distance(start, end);
        RaycastHit hit;

        if (Physics.Raycast(start, dir, out hit, dist, layer))
        {
            GameObject obj = hit.collider.gameObject;

            // Filtro por nombre y escala
            if (obj.name.ToLower().Contains("obstacule") || IsAvoidable(obj))
            {
                //Debug.Log($"<color=yellow>Avoidance en tiempo real:</color> Esquivando {obj.name}");
                //DrawDebugCircle(obj.transform.position, 0.7f, Color.yellow, 1f);

                Vector3 avPoint = CalculateAvoidancePoint(start, end, hit, layer);

                // Verificamos que el punto de evasión sea seguro
                if (IA_P2_LineOfSight3D.Check(start, avPoint, layer) && IA_P2_LineOfSight3D.Check(avPoint, end, layer))
                {
                    return new List<Vector3> { avPoint, end };
                }
            }
            return null; // Bloqueado por algo no evadible
        }
        return new List<Vector3> { end }; // Camino despejado
    }

    private static List<Vector3> ReconstructPathWithAvoidance(
        Dictionary<IA_P2_PathNode, IA_P2_PathNode> parentMap,
        Dictionary<IA_P2_PathNode, Vector3> avoidanceMap,
        IA_P2_PathNode current)
    {
        List<Vector3> path = new List<Vector3>();
        while (parentMap.ContainsKey(current) && parentMap[current] != current)
        {
            path.Add(current.transform.position);
            if (avoidanceMap.ContainsKey(current)) path.Add(avoidanceMap[current]);
            current = parentMap[current];
        }
        path.Add(current.transform.position);
        path.Reverse();
        return path;
    }



    /// <summary>
    /// Dibuja una cruz (X) en el suelo en la posición indicada.
    /// </summary>
    private static void DrawDebugCross(Vector3 center, float size, Color color, float duration)
    {
        // Línea 1 de la cruz (Diagonal 1)
        Vector3 line1Start = center + new Vector3(-size, 0.05f, -size);
        Vector3 line1End = center + new Vector3(size, 0.05f, size);

        // Línea 2 de la cruz (Diagonal 2)
        Vector3 line2Start = center + new Vector3(size, 0.05f, -size);
        Vector3 line2End = center + new Vector3(-size, 0.05f, size);

        Debug.DrawLine(line1Start, line1End, color, duration);
        Debug.DrawLine(line2Start, line2End, color, duration);
    }
}