using CustomInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IA_P2_AgentIA : MonoBehaviour
{
    [Button(nameof(GoToGameobject), true)]
    public GameObject targetObject;

    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float nodeReachDistance = 0.5f;
    public float acceleration = 10f;

    [Header("Debug")]
    public bool debug_BlockMovement = false;

    // --- SECCIÓN DE DETECCIÓN (ELIMINADA) ---
    // (Aquí estaban fieldOfView, viewDistance, etc.)

    public List<Vector3> currentPath;
    public int currentIndex = 0;
    public bool isMoving = false;
    public float currentSpeed = 0f;

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
    public void AsignarColor(Color color)
    {
        gameObject.GetComponent<Renderer>().material.color = color;
    }

    public void GoToGameobject(GameObject target)
    {
        GoTo(target.transform.position);
    }

    public void GoTo(Vector3 targetPosition)
    {
        Vector3 Origen = transform.position;
        Origen.y = 0;
        targetPosition.y = 0;
        currentPath = IA_P2_PathfindingManager.RequestPath(Origen, targetPosition);

        if (currentPath != null && currentPath.Count > 1)
        {
            Debug.DrawLine(transform.position, currentPath[0], Color.red, 4f);
        }

        currentIndex = 0;
        isMoving = currentPath != null && currentPath.Count > 0;
        currentSpeed = 0f;

        if (currentPath == null || currentPath.Count < 2) return;

        for (int i = 0; i < currentPath.Count - 1; i++)
            Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.cyan, 3f);
    }

    void Update()
    {
        if (!isMoving || currentPath == null || currentPath.Count == 0 || debug_BlockMovement)
            return;

        Vector3 target = currentPath[currentIndex];
        Vector3 toTarget = target - transform.position;
        float distance = toTarget.magnitude;

        if (distance < 0.001f)
            return;

        float step = moveSpeed * Time.deltaTime;
        if (step >= distance)
        {
            transform.position = target;
            currentIndex++;
            if (currentIndex >= currentPath.Count)
                isMoving = false;
        }
        else
        {
            transform.position += toTarget.normalized * step;
        }

        transform.position += toTarget.normalized * currentSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target) <= nodeReachDistance)
        {
            currentIndex++;
            if (currentIndex >= currentPath.Count)
                isMoving = false;
        }

        int lastIndex = currentPath.Count - 1;

        for (int i = 0; i < currentIndex - 1; i++)
        {
            if (currentPath.Count > i + 1)
                Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.yellow, 3f);
        }

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