using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // Necesario para usar Handles

public class IA_P2_FOV : MonoBehaviour
{
    [Range(1f, 180f)]
    public float fovAngle = 90f;
    public float viewDistance = 10f;
    public LayerMask visionObstacles;
    public LayerMask targetLayer;

    [Header("Eventos de Detección")]
    [Tooltip("Se dispara UNA VEZ cuando un objetivo entra en visión.")]
    public Action<GameObject> OnTargetDetected;
    [Tooltip("Se dispara UNA VEZ cuando un objetivo sale de la visión.")]
    public Action<GameObject> OnTargetLost;

    [Header("Debug")]
    [Tooltip("Lista pública de enemigos que están DENTRO del BoxCollider (el trigger).")]
    public List<GameObject> enemiesInTrigger = new List<GameObject>();

    [Header("Visualización Debug")]
    public int resolution = 20;
    public Color fovColor = new Color(0, 1, 0, 0.2f);
    public Color detectionColor = Color.red;
    public Color lostTargetColor = Color.white;

    private List<GameObject> _visibleTargets = new List<GameObject>();
    private HashSet<GameObject> _currentlyVisibleTargets = new HashSet<GameObject>();


    // --- LÓGICA DE DETECCIÓN (NÚCLEO) ---

    private void Update()
    {
        _currentlyVisibleTargets.Clear();

        for (int i = enemiesInTrigger.Count - 1; i >= 0; i--)
        {
            GameObject enemy = enemiesInTrigger[i];

            if (enemy == null)
            {
                enemiesInTrigger.RemoveAt(i);
                continue;
            }
            if (enemy != null)
                ProcessTarget(enemy);
        }

        for (int i = _visibleTargets.Count - 1; i >= 0; i--)
        {
            GameObject target = _visibleTargets[i];

            if (target == null)
            {
                _visibleTargets.RemoveAt(i);
                continue;
            }

            if (!_currentlyVisibleTargets.Contains(target))
            {
                _visibleTargets.RemoveAt(i);
                OnTargetLost?.Invoke(target);
                //Debug.Log("Objetivo PERDIDO: " + target.name, target);
            }
        }
    }

    private void ProcessTarget(GameObject target)
    {
        Transform targetTransform = target.transform;

        // 1. Chequeo de Distancia
        float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
        if (distanceToTarget > viewDistance)
        {
            return;
        }

        // 2. Chequeo de Ángulo (FOV)
        if (!IsInFOV(targetTransform))
        {
            return;
        }

        // 3. Chequeo de Línea de Visión (Obstáculos)
        if (!HasLineOfSight(targetTransform))
        {
            //Debug.DrawLine(transform.position + Vector3.up * 0.5f, targetTransform.position + Vector3.up * 0.5f, lostTargetColor);
            return;
        }

        // --- ÉXITO TOTAL ---
        _currentlyVisibleTargets.Add(target);

        //Debug.DrawLine(transform.position + Vector3.up * 0.5f, targetTransform.position + Vector3.up * 0.5f, detectionColor);

        if (!_visibleTargets.Contains(target))
        {
            if (IA_P2_LineOfSight3D.Check(transform.parent.position, target.transform.position, visionObstacles))
            {
                bool sonEnemigos = gameObject.name.Contains("_Agente") != target.name.Contains("_Agente");
                if (sonEnemigos)
                {
                    _visibleTargets.Add(target);
                    OnTargetDetected?.Invoke(target);
                    //Debug.Log("Se encontro a " + target.name, target);
                    Debug.DrawLine(transform.parent.position, target.transform.position, detectionColor, 2f);
                }
                else
                {
                    //Debug.Log("Se ignoro a " + target.name + " vs " + gameObject.name, target);
                }

            }
            else
            {
                //Debug.DrawLine(transform.parent.position, target.transform.position, lostTargetColor, 2f);
            }
        }
    }

    // --- GESTIÓN DE TRIGGERS (MODIFICADA) ---

    private void OnTriggerEnter(Collider other)
    {
        // 1. Chequeo de Layer
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

        // 2. [CORREGIDO] Obtener el GameObject "Padre" (el que tiene el Rigidbody)
        GameObject enemyRoot = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;

        // 3. Ignorarse a sí mismo (si el "Padre" del otro es este mismo objeto)
        if (enemyRoot == this.gameObject) return;

        // 4. Añadir a la lista (solo si no está ya)
        if (!enemiesInTrigger.Contains(enemyRoot))
        {
            enemiesInTrigger.Add(enemyRoot);
            //Debug.Log(enemyRoot.name + " entró al trigger.", enemyRoot);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 1. Chequeo de Layer
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

        // 2. [CORREGIDO] Obtener el GameObject "Padre"
        GameObject enemyRoot = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;

        // 3. Quitar de la lista pública
        if (enemiesInTrigger.Remove(enemyRoot))
        {
            //Debug.Log(enemyRoot.name + " salió del trigger.", enemyRoot);

            // 4. Si lo quitamos, comprobar si además estaba en la lista de VISIBLES
            if (_visibleTargets.Remove(enemyRoot))
            {
                OnTargetLost?.Invoke(enemyRoot);
                Debug.Log("Objetivo PERDIDO (Salió del Trigger): " + enemyRoot.name, enemyRoot);
            }
        }
    }

    // --- MÉTODOS DE SOPORTE (Sin cambios) ---

    private bool IsInFOV(Transform target)
    {
        Vector3 dirToTarget = target.position - transform.position;
        dirToTarget.y = 0;

        if (dirToTarget.sqrMagnitude < 0.001f)
        {
            return false;
        }

        Vector3 agentForward = transform.forward;
        agentForward.y = 0;

        float angle = Vector3.Angle(agentForward.normalized, dirToTarget.normalized);
        return angle <= fovAngle * 0.5f;
    }

    private bool HasLineOfSight(Transform target)
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        Vector3 targetPos = target.position + Vector3.up * 0.5f;

        bool sinObstaculos = IA_P2_LineOfSight3D.Check(rayStart, targetPos, visionObstacles);

        if (!sinObstaculos)
        {
            Debug.DrawLine(rayStart, targetPos, Color.yellow);
        }

        return sinObstaculos;
    }


    // --- CONFIGURACIÓN DEL EDITOR (Sin cambios) ---

    private void Reset()
    {
        var col = GetComponent<BoxCollider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.isTrigger = true;
        col.size = new Vector3(viewDistance * 2, viewDistance * 2, viewDistance * 2);
    }

    private void OnValidate()
    {
        var col = GetComponent<BoxCollider>();
        if (col != null)
        {
            col.size = new Vector3(viewDistance * 2, viewDistance * 2, viewDistance * 2);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!enabled) return;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        BoxCollider col = GetComponent<BoxCollider>();
        if (col == null) return;

        // Dibuja la CAJA
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(col.center), transform.rotation, transform.lossyScale);
        Gizmos.matrix = oldMatrix;


        // Dibuja el CONO
        float halfFOV = fovAngle * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Vector3 fromDirection = leftRayRotation * forward;
        Handles.color = fovColor;
        Handles.DrawSolidArc(origin, Vector3.up, fromDirection, fovAngle, viewDistance);

        // Dibuja los BORDES del cono
        Color edgeColor = fovColor;
        edgeColor.a = 1f;
        Handles.color = edgeColor;
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = fromDirection;
        Vector3 rightRayDirection = rightRayRotation * forward;
        Handles.DrawLine(origin, origin + leftRayDirection * viewDistance);
        Handles.DrawLine(origin, origin + rightRayDirection * viewDistance);
    }
#endif
}