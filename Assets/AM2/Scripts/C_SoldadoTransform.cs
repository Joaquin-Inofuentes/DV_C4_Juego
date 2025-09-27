using UnityEngine;

public abstract class C_SoldadoTransform : MonoBehaviour
{
    [Header("Ajustes Comunes")]
    [SerializeField] protected float velocidad = 5f;
    [SerializeField] protected float sensibilidadRotacion = 200f;

    [Header("Límites verticales (solo si se usan en el hijo)")]
    [SerializeField] protected float maxPitchArriba = 80f;
    [SerializeField] protected float maxPitchAbajo = -80f;

    protected float pitch = 0f;

    // Métodos abstractos: cada hijo decide cómo obtiene su input
    protected abstract Vector2 GetMoveInput();
    protected abstract Vector2 GetPanInput();

    private void FixedUpdate()
    {
        Mover(GetMoveInput());
        Rotar(GetPanInput());
    }

    // Movimiento simple con transform
    protected virtual void Mover(Vector2 input)
    {
        if (input == Vector2.zero) return;

        Vector3 delta = new Vector3(input.x, 0, input.y) * velocidad * Time.deltaTime;
        transform.position += delta;
    }

    // Rotación básica usando transform
    protected virtual void Rotar(Vector2 input)
    {
        if (input == Vector2.zero) return;

        float yaw = input.x * sensibilidadRotacion * Time.fixedDeltaTime;
        transform.rotation *= Quaternion.Euler(0f, yaw, 0f);
    }
}
