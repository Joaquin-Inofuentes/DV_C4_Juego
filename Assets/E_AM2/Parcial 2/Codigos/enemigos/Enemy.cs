using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float vida = 50f;
    public float velocidad = 2f;
    public float distanciaAtaque = 1.5f;
    public float daño = 5f;
    public float cadenciaAtaque = 1f;

    [Header("Referencias")]
    public Animator animator;
    public Transform objetivo;

    private float proximoAtaque;

    void OnEnable()
    {
        vida = 50f;
        animator.SetBool("walk", true);
        animator.SetBool("Attack", false);
    }

    void Update()
    {
        if (objetivo == null) return;

        float dist = Vector3.Distance(transform.position, objetivo.position);

        if (dist > distanciaAtaque)
        {
            // Estado Caminar
            animator.SetBool("walk", true);
            animator.SetBool("Attack", false);

            // Moverse hacia el objetivo
            Vector3 dir = (objetivo.position - transform.position).normalized;
            transform.position += dir * velocidad * Time.deltaTime;

            // Rotar hacia el objetivo
            transform.forward = dir;
        }
        else
        {
            // Estado Atacar
            animator.SetBool("walk", false);
            animator.SetBool("Attack", true);

            if (Time.time >= proximoAtaque)
            {
                // Acción de Ataque
                Debug.Log("Cascarudo atacó!");

                proximoAtaque = Time.time + cadenciaAtaque;
            }
        }
    }

    public void RecibirDaño(float d)
    {
        vida -= d;

        if (vida <= 0)
            Morir();
    }

    void Morir()
    {
        EnemyPool.Instance.ReturnEnemy(this);
    }
}