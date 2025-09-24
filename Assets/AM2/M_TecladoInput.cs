using UnityEngine;
using System;

public class M_TecladoInput : MonoBehaviour
{
    public event Action<Vector2> OnStickFloat;

    private void Update()
    {
        // lee teclas (Horizontal/Vertical → soporta flechas y WASD)
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        // invocar acciones
        OnStickFloat?.Invoke(input.normalized);
    }
}
