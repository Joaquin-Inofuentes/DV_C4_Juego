using UnityEngine;

public static class IA_P2_LineOfSight3D
{
    public static bool Check(Vector3 from, Vector3 to,LayerMask obstacleLayer)
    {
        from.y = 0;
        to.y = 0;
        Vector3 dir = to - from;
        float dist = dir.magnitude;

        return !Physics.Raycast(from, dir.normalized, dist, obstacleLayer);
    }
}
