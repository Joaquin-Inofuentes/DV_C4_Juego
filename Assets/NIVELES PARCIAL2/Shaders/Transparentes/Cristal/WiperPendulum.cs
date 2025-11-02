using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiperPendulum : MonoBehaviour
{

    [Header("Movimiento")]
    [Tooltip("Ángulo máximo desde el centro (grados). El barrido será de -maxAngle a +maxAngle.")]
    public float maxAngle = 40f;
    [Tooltip("Velocidad angular (grados por segundo) durante el barrido.")]
    public float sweepSpeed = 80f;
    [Tooltip("Si true, el movimiento usará suavizado (Lerp) para un barrido más natural.")]
    public bool useSmoothing = true;
    [Tooltip("Si useSmoothing = true, mayor valor = más rígido (0..1).")]
    [Range(0f, 1f)]
    public float smoothing = 0.15f;

    [Header("Pausas")]
    [Tooltip("Duración de la pausa al pasar por el centro (en segundos).")]
    public float pauseAtCenter = 0.3f;
    [Tooltip("Duración de la pausa en los extremos (en segundos).")]
    public float pauseAtEnd = 0.15f;
    [Tooltip("Si true, hace pausa al centro.")]
    public bool doPauseCenter = true;
    [Tooltip("Si true, hace pausa en los extremos.")]
    public bool doPauseEnds = false;

    [Header("Opciones")]
    [Tooltip("Eje local en el que rota el brazo (por defecto Z).")]
    public Vector3 localRotationAxis = Vector3.forward;
    [Tooltip("Si true, el wiper empieza desde -maxAngle. Si false, empieza en 0.")]
    public bool startFromNegative = true;

    // estado interno
    float targetAngle;
    float currentAngle;
    int direction = 1; // 1 = moviendo hacia +maxAngle, -1 hacia -maxAngle
    bool isPaused = false;

    void Start()
    {
        // Inicializar ángulo actual según rotación local existente
        currentAngle = GetLocalRotationAngle();
        if (startFromNegative) currentAngle = -maxAngle;
        SetLocalRotation(currentAngle);
        // Empezar hacia +maxAngle si comenzamos desde -maxAngle
        direction = currentAngle < 0 ? 1 : -1;
        targetAngle = direction > 0 ? maxAngle : -maxAngle;
        StartCoroutine(MainLoop());
    }

    IEnumerator MainLoop()
    {
        while (true)
        {
            if (!isPaused)
            {
                // Actualizar targetAngle según dirección
                targetAngle = direction > 0 ? maxAngle : -maxAngle;

                // Mover hacia targetAngle
                float step = sweepSpeed * Time.deltaTime * direction;
                float newAngle = currentAngle + step;

                // Si smoothing activo, suavizar movimiento
                if (useSmoothing)
                {
                    newAngle = Mathf.Lerp(currentAngle, targetAngle, smoothing);
                }

                // Aplicar ángulo
                currentAngle = Mathf.Clamp(newAngle, -maxAngle, maxAngle);
                SetLocalRotation(currentAngle);

                // Comprobar paso por centro (de signo opuesto cruzando 0)
                if (doPauseCenter && direction == 1 && currentAngle >= 0f && Mathf.Abs(currentAngle) < 1f)
                {
                    // pequeña zona alrededor de 0 para detectar el centro
                    yield return StartCoroutine(PauseCoroutine(pauseAtCenter));
                    // seguir en misma dirección hasta extremo
                }
                else if (doPauseCenter && direction == -1 && currentAngle <= 0f && Mathf.Abs(currentAngle) < 1f)
                {
                    yield return StartCoroutine(PauseCoroutine(pauseAtCenter));
                }

                // Si alcanzamos el extremo, invertir dirección y (posible) pausa
                if (Mathf.Approximately(Mathf.Abs(currentAngle), maxAngle) || (direction > 0 && currentAngle >= maxAngle) || (direction < 0 && currentAngle <= -maxAngle))
                {
                    // asegurar exactitud
                    currentAngle = direction > 0 ? maxAngle : -maxAngle;
                    SetLocalRotation(currentAngle);

                    // invertir dirección
                    direction *= -1;

                    if (doPauseEnds)
                    {
                        yield return StartCoroutine(PauseCoroutine(pauseAtEnd));
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator PauseCoroutine(float time)
    {
        if (time <= 0f) yield break;
        isPaused = true;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            yield return null;
        }
        isPaused = false;
    }

    // Utilities: obtiene rotación local en torno al eje elegido (en grados)
    float GetLocalRotationAngle()
    {
        Quaternion q = transform.localRotation;
        // Convertimos la rotación local a ángulo en el eje especificado.
        Vector3 e = q.eulerAngles;
        // Ajuste para valores >180
        if (ApproximatelyAxis(localRotationAxis, Vector3.forward)) return NormalizeAngle(e.z);
        if (ApproximatelyAxis(localRotationAxis, Vector3.up)) return NormalizeAngle(e.y);
        if (ApproximatelyAxis(localRotationAxis, Vector3.right)) return NormalizeAngle(e.x);
        // si el eje no es exactamente axis cardinal, proyectamos.
        Vector3 axis = localRotationAxis.normalized;
        float angle;
        Vector3 rotated = q * axis;
        // cálculo aproximado: ángulo entre axis y rotated en grados (signo según cross)
        angle = Vector3.SignedAngle(axis, rotated, axis);
        return angle;
    }

    void SetLocalRotation(float angle)
    {
        Vector3 e = transform.localEulerAngles;
        if (ApproximatelyAxis(localRotationAxis, Vector3.forward))
        {
            e.z = angle;
        }
        else if (ApproximatelyAxis(localRotationAxis, Vector3.up))
        {
            e.y = angle;
        }
        else if (ApproximatelyAxis(localRotationAxis, Vector3.right))
        {
            e.x = angle;
        }
        else
        {
            // eje arbitrario: rotar usando quaternion a partir del eje y el ángulo
            Quaternion q = Quaternion.AngleAxis(angle, localRotationAxis.normalized);
            transform.localRotation = q;
            return;
        }
        transform.localEulerAngles = e;
    }

    static float NormalizeAngle(float a)    {
        if (a > 180f) a -= 360f;
        return a;
    }

    static bool ApproximatelyAxis(Vector3 a, Vector3 b)
    {
        return Vector3.Angle(a.normalized, b.normalized) < 5f;
    }
}