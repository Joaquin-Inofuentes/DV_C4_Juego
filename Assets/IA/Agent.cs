using UnityEngine;

/// <summary>
/// Clase base abstracta para todos los agentes autónomos (Boids, Cazador).
/// Gestiona el movimiento cinemático básico sin usar Rigidbody.
/// También maneja la depuración visual básica como el color y el texto de estado.
/// </summary>
[RequireComponent(typeof(Collider))] // Útil para detección, no para física.
public abstract class Agent : MonoBehaviour
{
    [Header("Parámetros de Movimiento del Agente")]
    public Vector3 velocity;
    public float maxSpeed = 10f;
    public float maxForce = 10f; // Fuerza máxima de giro

    protected Vector3 acceleration;

    // --- Variables para depuración ---
    protected string debugStatusText = "Initializing...";
    private Renderer _renderer; // Usamos Renderer para que sea más general

    protected virtual void Awake()
    {
        // Obtenemos el componente Renderer para cambiar color y calcular el centro
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError($"El agente '{name}' no tiene un componente Renderer para cambiar de color o calcular su centro.");
        }
    }

    protected virtual void Update()
    {
        // --- LÓGICA DE MOVIMIENTO DESACTIVADA PARA LA FASE DE DEPURACIÓN VISUAL ---
        // Cuando actives el movimiento, descomenta estas líneas.
        /*
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
        */
    }

    /// <summary>
    /// Aplica una fuerza de dirección (steering force) a la aceleración del agente.
    /// </summary>
    protected void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }

    /// <summary>
    /// Cambia el color del material del agente para depuración.
    /// </summary>
    protected void SetDebugColor(Color color)
    {
        if (_renderer != null)
        {
            _renderer.material.color = color;
        }
    }

    /// <summary>
    /// Dibuja Gizmos en el editor, como la etiqueta de estado.
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        // Si no hay texto que mostrar, no hacemos nada.
        if (string.IsNullOrEmpty(debugStatusText)) return;

        // Usamos el centro del "bounding box" del renderer, que es el centro visual real del objeto.
        if (_renderer != null)
        {
            Vector3 centerPosition = _renderer.bounds.center;
            DebugHelper.DrawLabel(centerPosition, debugStatusText, Color.white);
        }
        else
        {
            // Si por alguna razón no hay renderer, usamos el transform.position como fallback.
            DebugHelper.DrawLabel(transform.position, debugStatusText, Color.white);
        }
    }
}