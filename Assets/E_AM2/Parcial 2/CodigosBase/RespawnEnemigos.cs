using UnityEngine;
using System.Collections.Generic;

public class RespawnEnemigos : MonoBehaviour
{
    [Header("Prefabs de Enemigos")]
    public GameObject[] enemigos;

    [Header("Puntos de Spawn (Transforms)")]
    public Transform[] puntosSpawn;

    [Header("Config")]
    public float intervaloSpawn = 3f;
    public int maxEnemigos = 5;

    [Header("Debug")]
    public List<GameObject> enemigosVivos = new List<GameObject>();

    private float timer = 0f;

    void Update()
    {
        // Limpieza automática (por si alguno murió / fue destruido)
        enemigosVivos.RemoveAll(e => e == null);

        timer += Time.deltaTime;

        // No spawnear si está lleno
        if (enemigosVivos.Count >= maxEnemigos) return;

        if (timer >= intervaloSpawn)
        {
            timer = 0f;
            SpawnAleatorio();
        }
    }

    void SpawnAleatorio()
    {
        if (enemigos.Length == 0 || puntosSpawn.Length == 0) return;

        GameObject prefab = enemigos[Random.Range(0, enemigos.Length)];
        Transform punto = puntosSpawn[Random.Range(0, puntosSpawn.Length)];

        GameObject nuevo = Instantiate(prefab, punto.position, punto.rotation, transform);

        enemigosVivos.Add(nuevo);
    }

    void OnEnable()
    {
        string dificultad = PlayerPrefs.GetString("Dificultad", "");

        if (string.IsNullOrEmpty(dificultad))
        {
            Debug.LogWarning("No hay dificultad guardada en PlayerPrefs.");
            return;
        }

        if (dificultad == "Facil")
            intervaloSpawn = 3;
        else if (dificultad == "Medio")
            intervaloSpawn = 2;
        else if (dificultad == "Dificil")
            intervaloSpawn = 1;
    }
}
