using UnityEngine;

public static class LineOfSight3D
{
    public static bool Check(Vector3 from, Vector3 to,LayerMask obstacleLayer)
    {
        Vector3 dir = to - from;
        float dist = dir.magnitude;

        return !Physics.Raycast(from, dir.normalized, dist, obstacleLayer);
    }
}
