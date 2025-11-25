using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSegunDeltaPosition : MonoBehaviour
{
    public Rigidbody rb;
    public float quietThreshold = 0.05f;
    public float tiempoQuietoParaAtacar = 0.5f;

    private float contadorQuieto = 0f;

    void Update()
    {
        float vel = rb.velocity.magnitude;

        if (vel < quietThreshold)
        {
            contadorQuieto += Time.deltaTime;

            if (contadorQuieto >= tiempoQuietoParaAtacar)
            {
                Atacar();
                contadorQuieto = 0f;
            }
        }
        else
        {
            contadorQuieto = 0f;
        }
    }

    void Atacar()
    {
        Debug.Log("ATAQUE!!");
    }

}
