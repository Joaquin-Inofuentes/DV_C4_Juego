using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] puntosSpawn;
    public float tiempoEntreEnemigos = 0.5f;
    public int enemigosPorHorda = 10;

    void Start()
    {
        StartCoroutine(SpawnHorda());
    }

    IEnumerator SpawnHorda()
    {
        for (int i = 0; i < enemigosPorHorda; i++)
        {
            Transform spawn = puntosSpawn[Random.Range(0, puntosSpawn.Length)];

            EnemyPool.Instance.GetEnemy(
                spawn.position,
                spawn.rotation
            );

            yield return new WaitForSeconds(tiempoEntreEnemigos);
        }
    }
}