using System.Collections.Generic;
using UnityEngine;

public static class PathfindingManager
{
    public static List<Vector3> RequestPath(Vector3 Origen,Vector3 targetPos)
    {
        if (PathfindingModel.Instance == null) return null;

        var nodes = PathfindingModel.Instance.allNodes;
        if (nodes == null || nodes.Count == 0) return null;


        // 1) Nodo más cercano al jugador
        PathNode startNode = FindClosestNode(Origen);

        // 2) Nodo más cercano al objetivo
        PathNode endNode = FindClosestNode(targetPos);

        // 3) A*
        List<PathNode> nodePath = RunAStar(startNode, endNode, targetPos);
        if (nodePath == null) return null;

        // 4) Construir lista final
        List<Vector3> finalPath = new List<Vector3>();
        foreach (var n in nodePath)
            finalPath.Add(n.transform.position);

        finalPath.Add(targetPos); // posición exacta

        return finalPath;
    }


    static PathNode FindClosestNode(Vector3 pos)
    {
        float best = float.MaxValue;
        PathNode result = null;

        foreach (var n in PathfindingModel.Instance.allNodes)
        {
            float d = Vector3.Distance(pos, n.transform.position);
            if (d < best)
            {
                best = d;
                result = n;
            }
        }

        return result;
    }


    static List<PathNode> RunAStar(PathNode start, PathNode end, Vector3 targetPos)
    {
        LayerMask obstacle = PathfindingModel.Instance.obstacleLayer;

        var open = new List<PathNode>();
        var closed = new HashSet<PathNode>();

        var came = new Dictionary<PathNode, PathNode>();
        var g = new Dictionary<PathNode, float>();
        var f = new Dictionary<PathNode, float>();

        foreach (var n in PathfindingModel.Instance.allNodes)
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
            PathNode current = open[0];
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


    static List<PathNode> Reconstruct(Dictionary<PathNode, PathNode> came, PathNode end)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode node = end;

        while (came.ContainsKey(node))
        {
            path.Add(node);
            node = came[node];
        }

        path.Add(node);
        path.Reverse();
        return path;
    }


    static List<PathNode> ReconstructPartial(Dictionary<PathNode, PathNode> came, PathNode end)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode node = end;

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
