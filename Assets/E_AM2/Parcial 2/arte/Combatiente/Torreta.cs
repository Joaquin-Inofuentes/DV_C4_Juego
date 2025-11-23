using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torreta : MonoBehaviour
{
    [Header("Stats")]
    public float rango = 10f;
    public float daño = 10f;
    public float cadencia = 1f;

    [Header("Referencias")]
    [SerializeField] public LayerMask layerEnemigos;
    public Transform puntoDisparo;
    public AudioSource sonidoDisparo;
    public Animator animator;
    
    private float tiempoDisparo;
    private Transform objetivo; 

    void Update()
    {
        BuscarObjetivo();

        if (objetivo != null)
        {
            Vector3 dir = objetivo.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);

            if (Time.time >= tiempoDisparo)
            {
                Disparar();
                tiempoDisparo = Time.time + (1f / cadencia);
            }

            animator.SetBool("Ataque", true);
        }
        else
        {
            animator.SetBool("Ataque", false);
        }
    }

    void BuscarObjetivo()
    {
        Collider[] encontrados = Physics.OverlapSphere(transform.position, rango, layerEnemigos);

        if (encontrados.Length > 0)
        {
            objetivo = encontrados[0].transform;
        }
        else
        {
            objetivo = null;
        }
    }

    void Disparar()
    {
        if (sonidoDisparo != null)
            sonidoDisparo.Play();

        animator.SetTrigger("Shoot");

       // var vida = objetivo.GetComponent<VidaEnemigo>();
       // if (vida != null)
        //    vida.RecibirDaño(daño);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}
