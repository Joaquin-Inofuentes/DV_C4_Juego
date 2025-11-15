using System.Collections.Generic;
using UnityEngine;

public static class PathfindingManager
{
    public static List<Vector3> RequestPath(Vector3 targetPos)
    {
        if (PathfindingModel.Instance == null)
            return null;

        var nodes = PathfindingModel.Instance.allNodes;
        if (nodes == null || nodes.Count == 0)
            return null;

        Vector3 agentPos = PlayerAgent.Instance.transform.position;

        // 1) Encontrar nodo más cercano al jugador
        PathNode rawStart = FindClosestNode(agentPos);

        // 2) Usar Dijkstra para encontrar primer nodo visible hacia el objetivo
        List<PathNode> dijkstraPath = RunSearch(rawStart, null, PathAlgo.Dijkstra, targetPos);
        PathNode startNode = (dijkstraPath != null && dijkstraPath.Count > 0) ? dijkstraPath[0] : rawStart;

        // 3) Nodo más cercano al objetivo
        PathNode endNode = FindClosestNode(targetPos);

        // 4) Usar A* desde startNode hasta endNode
        List<PathNode> nodePath = RunSearch(startNode, endNode, PathAlgo.AStar, targetPos);
        if (nodePath == null) return null;

        // 5) Construir lista de posiciones
        List<Vector3> finalPath = new List<Vector3>();
        foreach (var n in nodePath)
            finalPath.Add(n.transform.position);

        // 6) Agregar posición exacta del objetivo
        finalPath.Add(targetPos);

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



    static List<PathNode> RunSearch(PathNode start, PathNode end, PathAlgo algo, Vector3 targetPos)
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

        g[start] = 0;

        if (algo == PathAlgo.AStar)
            f[start] = Vector3.Distance(start.transform.position, end.transform.position);
        else
            f[start] = 0; // Dijkstra

        open.Add(start);

        while (open.Count > 0)
        {
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

            // 🔥 Si desde este nodo se ve el objetivo → cortar acá
            if (LineOfSight3D.Check(current.transform.position, targetPos, obstacle))
            {
                return ReconstructPartial(came, current);
            }

            if (algo == PathAlgo.AStar && current == end)
                return Reconstruct(came, end);

            open.Remove(current);
            closed.Add(current);

            foreach (var nb in current.neighbors)
            {
                if (nb == null || closed.Contains(nb)) continue;

                if (!LineOfSight3D.Check(current.transform.position, nb.transform.position, obstacle))
                    continue;

                float tentative = g[current] + nb.movementCost;

                if (!open.Contains(nb))
                    open.Add(nb);

                if (tentative >= g[nb])
                    continue;

                came[nb] = current;
                g[nb] = tentative;

                if (algo == PathAlgo.AStar)
                    f[nb] = tentative + Vector3.Distance(nb.transform.position, end.transform.position);
                else
                    f[nb] = tentative;
            }
        }

        return null;
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


    public enum PathAlgo
    {
        Dijkstra,
        AStar
    }




}
