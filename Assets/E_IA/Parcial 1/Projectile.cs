using UnityEngine;

/// <summary>
/// Controla el comportamiento de un proyectil.
/// Se mueve en una dirección y destruye a los boids al impactar.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    // Referencia al Rigidbody para el movimiento físico.
    private Rigidbody rb;

    private void Awake()
    {
        // Obtenemos la referencia al Rigidbody al inicio.
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Inicializa el proyectil, dándole una velocidad inicial.
    /// </summary>
    /// <param name="direction">La dirección en la que debe moverse.</param>
    /// <param name="speed">La velocidad a la que debe moverse.</param>
    public void Launch(Vector3 direction, float speed)
    {
        // Establece la velocidad del Rigidbody para que se mueva en la dirección y velocidad deseadas.
        rb.velocity = direction.normalized * speed;
        // Destruye el proyectil después de 5 segundos si no ha chocado con nada.
        Destroy(gameObject, 5f);
    }

    // Se ejecuta cuando este collider choca con otro.
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Terrain") return; // Ignora colisiones con el terreno
        if (collision.gameObject.CompareTag("Hunter")) return; // Ignora colisiones con el cazador
        Debug.Log($"<color=orange>Proyectil impactó con:</color> {collision.gameObject.name}");
        
        // Comprueba si el objeto con el que chocamos tiene la etiqueta "Boid".
        if (collision.gameObject.CompareTag("Boid"))
        {
            // Si es un boid, lo destruye.
            Destroy(collision.gameObject);
        }

        // Destruye el proyectil después de cualquier colisión.
        Destroy(gameObject);
    }
}