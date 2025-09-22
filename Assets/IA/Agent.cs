using UnityEngine;

/// <summary>
/// Clase base abstracta para todos los agentes autónomos (Boids, Cazador).
/// Gestiona el movimiento cinemático básico sin usar Rigidbody.
/// </summary>
[RequireComponent(typeof(Collider))] // Útil para detección, no para física.
public abstract class Agent : MonoBehaviour
{
    [Header("Parámetros de Movimiento del Agente")]
    public Vector3 velocity;
    public float maxSpeed = 10f;
    public float maxForce = 10f; // Fuerza máxima de giro

    protected Vector3 acceleration;

    protected virtual void Update()
    {
        // Lógica de movimiento cinemático (sin físicas de Unity)
        velocity += acceleration * Time.deltaTime;

        // Limitar velocidad
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        transform.position += velocity * Time.deltaTime;

        // Orientar el agente en la dirección del movimiento
        if (velocity.magnitude > 0.1f)
        {
            transform.forward = velocity.normalized;
        }

        // Resetear la aceleración en cada frame
        acceleration = Vector3.zero;
    }

    /// <summary>
    /// Aplica una fuerza de dirección (steering force) a la aceleración del agente.
    /// </summary>
    protected void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }
}