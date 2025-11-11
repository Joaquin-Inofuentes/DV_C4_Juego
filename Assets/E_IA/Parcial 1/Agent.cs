using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Agent : MonoBehaviour
{
    public static bool movementEnabled = true;

    [Header("Parámetros de Movimiento del Agente")]
    public Vector3 velocity;
    public float maxSpeed = 10f;
    public float maxForce = 10f;

    protected Vector3 acceleration;
    protected string debugStatusText = "Initializing...";
    private Renderer _renderer;

    protected virtual void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError($"El agente '{name}' no tiene un componente Renderer.");
        }
    }

    protected virtual void Update()
    {
        if (movementEnabled)
        {
            velocity += acceleration * Time.deltaTime;
            velocity.y = 0;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            // --- RED DE SEGURIDAD FINAL ---
            // Antes de asignar la posición, comprueba si la velocidad es válida.
            // float.IsNaN() comprueba si un número es "Not a Number".
            if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
            {
                // Si la velocidad es inválida, la reseteamos a cero y evitamos el movimiento este frame.
                velocity = Vector3.zero;
                Debug.LogWarning($"Se detectó y corrigió una velocidad NaN en el agente {name}.");
            }
            else
            {
                // Si la velocidad es válida, aplicamos el movimiento.
                transform.position += velocity * Time.deltaTime;
            }

            if (velocity.magnitude > 0.1f)
            {
                transform.forward = velocity.normalized;
            }
        }
        acceleration = Vector3.zero;
    }

    public virtual void ApplyForce(Vector3 force)
    {
        force = Vector3.ClampMagnitude(force, maxForce);
        acceleration += force;
    }

    protected void SetDebugColor(Color color)
    {
        if (_renderer != null)
        {
            _renderer.material.color = color;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (string.IsNullOrEmpty(debugStatusText)) return;
        if (_renderer != null)
        {
            DebugHelper.DrawLabel(_renderer.bounds.center, debugStatusText, Color.white);
        }
        else
        {
            DebugHelper.DrawLabel(transform.position, debugStatusText, Color.white);
        }
    }
}