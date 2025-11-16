using CustomInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IA_P2_AgentIA : MonoBehaviour
{
    [Button(nameof(GoToGameobject), true)]
    public GameObject targetObject;

    // --- NUEVO: Waypoints de patrulla ---
    public List<Transform> patrolWaypoints;

    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float nodeReachDistance = 0.5f;
    public float acceleration = 10f; // m/s²

    public List<Vector3> currentPath;
    public int currentIndex = 0;
    public bool isMoving = false;

    public float currentSpeed = 0f; // velocidad actual, aumenta al inicio

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
        Debug.Log("Se pide re calcular");
        Vector3 Origen = transform.position;
        Origen.y = 0;
        targetPosition.y = 0;
        currentPath = IA_P2_PathfindingManager.RequestPath(Origen, targetPosition);
        // 1) Línea roja solo la primera vez
        if (currentPath.Count > 1)
        {
            Debug.DrawLine(transform.position, currentPath[0], Color.red, 4f);
        }
        currentIndex = 0;
        isMoving = currentPath != null && currentPath.Count > 0;

        currentSpeed = 0f; // reset velocidad al iniciar

        if (currentPath == null || currentPath.Count < 2) return;

        // DEBUG
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

        bool isLastNode = currentIndex == currentPath.Count - 1;

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

        // --- Mover ---
        transform.position += toTarget.normalized * currentSpeed * Time.deltaTime;

        // --- Revisar llegada ---
        if (Vector3.Distance(transform.position, target) <= nodeReachDistance)
        {
            currentIndex++;
            if (currentIndex >= currentPath.Count)
                isMoving = false;
        }

        int lastIndex = currentPath.Count - 1;

        // --- DIBUJO DE LINEAS ---
        // 2) Tramos completados: amarillo
        for (int i = 0; i < currentIndex - 1; i++)
        {
            if (currentPath.Count > i+1)
                Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.yellow, 3f);
        }

        // 3) Tramos por recorrer: blanco
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
