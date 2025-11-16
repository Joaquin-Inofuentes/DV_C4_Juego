using CustomInspector;
using System.Collections.Generic;
using UnityEngine;

public class IA_P2_AgentIA : MonoBehaviour
{
    [Button(nameof(GoToGameobject), true)]
    public GameObject targetObject;

    public List<Transform> patrolWaypoints;

    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float nodeReachDistance = 0.5f;

    public List<Vector3> currentPath;
    public int currentIndex = 0;
    public bool isMoving = false;

    public void GoToGameobject(GameObject target)
    {
        GoTo(target.transform.position);
    }

    public void GoTo(Vector3 targetPosition)
    {
        Debug.Log("Se pide re calcular");
        Vector3 Origen = transform.position;
        Origen.y = 0;
        targetPosition.y = 0;
        currentPath = IA_P2_PathfindingManager.RequestPath(Origen, targetPosition);

        if (currentPath.Count > 1)
            Debug.DrawLine(transform.position, currentPath[0], Color.red, 4f);

        currentIndex = 0;
        isMoving = currentPath != null && currentPath.Count > 0;

        if (currentPath == null || currentPath.Count < 2) return;

        // DEBUG: dibuja todo el path
        for (int i = 0; i < currentPath.Count - 1; i++)
            Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.cyan, 3f);
    }

    void Update()
    {
        if (!isMoving || currentPath == null || currentPath.Count == 0)
            return;

        Vector3 target = currentPath[currentIndex];
        Vector3 toTarget = target - transform.position;
        float distance = toTarget.magnitude;

        if (distance < 0.001f)
            return;

        // --- Mover a velocidad constante ---
        transform.position += toTarget.normalized * moveSpeed * Time.deltaTime;

        // --- Revisar llegada ---
        if (distance <= nodeReachDistance)
        {
            currentIndex++;
            if (currentIndex >= currentPath.Count)
                isMoving = false;
        }

        int lastIndex = currentPath.Count - 1;

        // --- DIBUJO DE LINEAS ---
        for (int i = 0; i < currentIndex - 1; i++)
            Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.yellow, 3f);
        for (int i = Mathf.Max(currentIndex - 1, 0); i < lastIndex; i++)
            Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.white, 0.1f);
    }

    public void StopAgent()
    {
        isMoving = false;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
