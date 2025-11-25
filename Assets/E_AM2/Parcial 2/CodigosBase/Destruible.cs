using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruible : MonoBehaviour, I_ReceivesDamage
{
    [Header("Vida")]
    public int health = 100;
    public int healthMax = 100;

    [Header("Barra 3D")]
    public Transform barraVida;      // el objeto que escala en Z
    public float escalaMaxZ = 0.1f;  // 0.1 = vida completa

    public void ReceiveDamage(int damage)
    {
        health -= damage;
        //Debug.Log($"{gameObject.name} recibió {damage} de daño. Salud restante: {health}");

        if (health <= 0)
        {
            Destroy(transform.parent.gameObject);
            //Debug.Log($"{gameObject.name} ha sido destruido.");
        }
    }

    void Update()
    {
        ActualizarBarraVida();
    }

    void ActualizarBarraVida()
    {
        if (barraVida == null) return;

        // 0 → 1
        float n = Mathf.Clamp01((float)health / healthMax);

        // Escala Z desde el pivot
        barraVida.localScale = new Vector3(
            barraVida.localScale.x,
            barraVida.localScale.y,
            n * escalaMaxZ
        );
    }
}
