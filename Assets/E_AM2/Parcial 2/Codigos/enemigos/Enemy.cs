using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 2f;
    public float attackRange = 18;
    public float attackCooldown = 1f;
    public int damage = 10;

    private float attackTimer;
    private Animator anim;
    private Transform target;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        attackTimer = 0f;

        // Buscar objetivo automáticamente al instanciarse
        GameObject posibleTarget = GameObject.FindGameObjectWithTag("Objetivo");

        if (posibleTarget != null)
            target = posibleTarget.transform;
        else
            Debug.LogWarning("Enemy no encontró GameObject con tag 'Player'.");
    }

    void Update()
    {
        if (target == null)
            return;

        float distancia = Vector3.Distance(transform.position, target.position);

        if (distancia > attackRange)
        {
            // Caminar hacia el jugador
            anim.SetBool("walk", true);
            anim.SetBool("Attack", false);

            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;

            // Rotar hacia el objetivo
            transform.LookAt(target);
        }
        else
        {
            // Atacar
            anim.SetBool("walk", false);
            anim.SetBool("Attack", true);

            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
    }

    void Attack()
    {
        Debug.Log("Enemy golpeó al jugador por " + damage + " de daño.");
        // Acá aplicás daño real al jugador si tenés su script
    }
}