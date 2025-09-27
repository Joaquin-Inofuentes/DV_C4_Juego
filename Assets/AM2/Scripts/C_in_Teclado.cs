
using UnityEngine;
using System;

public class C_in_Teclado : MonoBehaviour
{
    [SerializeField] private C_InputManager Manager; // referencia al manager para enviar el input
    [SerializeField] private float sensibilidad = 1f; // opcional, multiplicador

    private Vector2 inputVector;

    void Update()
    {
        // obtenemos input horizontal y vertical
        float x = Input.GetAxisRaw("Horizontal"); // A/D o flechas
        float y = Input.GetAxisRaw("Vertical");   // W/S o flechas

        // normalizamos
        Vector2 rawInput = new Vector2(x, y);
        if (rawInput.magnitude > 1f) rawInput.Normalize();

        inputVector = rawInput * sensibilidad;

        // enviamos al manager
        Manager?.RecibirMove(inputVector);

        // debug opcional
        //Debug.Log($"Teclado InputVector: {inputVector}");
    }
}
