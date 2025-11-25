using UnityEngine;

public class RespawnEnemigos : MonoBehaviour
{
    [Header("Prefabs de Enemigos")]
    public GameObject[] enemigos;

    [Header("Puntos de Spawn (Transforms)")]
    public Transform[] puntosSpawn;

    [Header("Config")]
    public float intervaloSpawn = 3f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= intervaloSpawn)
        {
            timer = 0f;
            SpawnAleatorio();
        }
    }

    void SpawnAleatorio()
    {
        if (enemigos.Length == 0 || puntosSpawn.Length == 0) return;

        // Elegir enemigo aleatorio
        GameObject enemigo = enemigos[Random.Range(0, enemigos.Length)];

        // Elegir punto aleatorio
        Transform punto = puntosSpawn[Random.Range(0, puntosSpawn.Length)];

        // Instanciar
        Instantiate(enemigo, punto.position, punto.rotation, transform);

    }
}
