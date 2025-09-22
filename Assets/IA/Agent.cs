using UnityEngine;

// Requiere que cualquier GameObject con este script también tenga un Collider.
// Es útil para la detección, aunque no usemos físicas.
[RequireComponent(typeof(Collider))]
public abstract class Agent : MonoBehaviour
{
    // --- VARIABLES PÚBLICAS ---

    // 'static' significa que esta variable es compartida por TODAS las instancias de Agent.
    // Es nuestro interruptor global para activar/desactivar el movimiento.
    public static bool movementEnabled = false;

    [Header("Parámetros de Movimiento del Agente")]
    // La velocidad actual del agente, representada como un vector (dirección y magnitud).
    public Vector3 velocity;
    // La velocidad máxima que el agente puede alcanzar.
    public float maxSpeed = 10f;
    // La fuerza máxima de giro que se puede aplicar en un solo frame. Limita la aceleración.
    public float maxForce = 10f;

    // --- VARIABLES PROTEGIDAS (Accesibles por esta clase y las que heredan de ella) ---

    // La aceleración acumulada en el frame actual. Se resetea en cada Update.
    protected Vector3 acceleration;

    // El texto que se mostrará sobre el agente para depuración.
    protected string debugStatusText = "Initializing...";
    // Referencia al componente Renderer para cambiar el color del objeto.
    private Renderer _renderer;

    // --- MÉTODOS DE UNITY ---

    // Se ejecuta una vez cuando el script se carga por primera vez.
    protected virtual void Awake()
    {
        // Busca y almacena el componente Renderer del GameObject.
        _renderer = GetComponent<Renderer>();
        // Si no se encuentra un Renderer, muestra un error en la consola.
        if (_renderer == null)
        {
            Debug.LogError($"El agente '{name}' no tiene un componente Renderer.");
        }
    }

    // Se ejecuta en cada frame.
    protected virtual void Update()
    {
        // Solo ejecuta la lógica de movimiento si nuestro interruptor global está activado.
        if (movementEnabled)
        {
            // --- FÍSICA CINEMÁTICA BÁSICA ---
            // 1. Actualiza la velocidad sumando la aceleración (multiplicada por el tiempo del frame).
            velocity += acceleration * Time.deltaTime;

            // --- NUEVA LÍNEA CRÍTICA ---
            // 2. RESTRINGIR AL PLANO XZ: Anulamos cualquier posible movimiento vertical.
            // No importa qué fuerzas se hayan calculado, la velocidad en el eje Y siempre será cero.
            velocity.y = 0;

            // 3. Limita la velocidad para que no exceda la velocidad máxima.
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            // 4. Mueve la posición del agente basándose en su nueva velocidad (que ahora es puramente 2D).
            transform.position += velocity * Time.deltaTime;

            // Si el agente se está moviendo significativamente...
            if (velocity.magnitude > 0.1f)
            {
                // ...haz que el modelo "mire" en la dirección del movimiento.
                transform.forward = velocity.normalized;
            }
        }
        // Resetea la aceleración a cero al final de cada frame, listo para el siguiente cálculo.
        acceleration = Vector3.zero;
    }

    // Se ejecuta cuando se dibujan los Gizmos en la escena del editor.
    protected virtual void OnDrawGizmos()
    {
        // Si no hay texto que mostrar, no hace nada.
        if (string.IsNullOrEmpty(debugStatusText)) return;
        // Si tenemos una referencia al Renderer...
        if (_renderer != null)
        {
            // ...dibuja la etiqueta en el centro visual del objeto.
            DebugHelper.DrawLabel(_renderer.bounds.center, debugStatusText, Color.white);
        }
        else
        {
            // ...si no, dibújala en la posición del pivote del transform.
            DebugHelper.DrawLabel(transform.position, debugStatusText, Color.white);
        }
    }

    // --- MÉTODOS PROTEGIDOS (Para ser usados por las clases hijas como Boid) ---

    // Permite que las clases hijas (Boid, Hunter) apliquen una fuerza de dirección.
    public virtual void ApplyForce(Vector3 force)
    {
        // Limita la magnitud de la fuerza para que no sea demasiado grande en un solo frame.
        force = Vector3.ClampMagnitude(force, maxForce);
        // Suma la fuerza a la aceleración acumulada de este frame.
        acceleration += force;
    }

    // Permite que las clases hijas cambien el color del material del agente.
    protected void SetDebugColor(Color color)
    {
        // Si la referencia al Renderer es válida...
        if (_renderer != null)
        {
            // ...cambia el color de su material.
            _renderer.material.color = color;
        }
    }
}